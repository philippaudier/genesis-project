using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The conflict proof (Genesis-006, kept as a regression guard): conflicting contributions to the
    /// same cell resolve exactly once, deterministically, and independently of enumeration order, via
    /// an explicit commutative resolver — now attached to the cell's <em>kind</em> (RFC-0003 D2) —
    /// and a conflict on a kind with no resolver is rejected.
    /// </summary>
    public sealed class ConflictResolutionTests
    {
        private static TickRunner RunnerWith(IReadOnlyDictionary<Kind, IConflictResolver> resolvers)
        {
            return new TickRunner(new TransitionRunner(resolvers));
        }

        private static Dictionary<Kind, IConflictResolver> SumOnK()
        {
            return new Dictionary<Kind, IConflictResolver>
            {
                { TestAddresses.K, new AdditionResolver() }
            };
        }

        private static ITransition[] AddToA(long first, long second)
        {
            return new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellA, first),
                new AddToCounterTransition(TestAddresses.CellA, second)
            };
        }

        [Test]
        public void Conflicting_Writes_Are_Deterministic()
        {
            SimulationState first = RunnerWith(SumOnK()).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);
            SimulationState second = RunnerWith(SumOnK()).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);

            Assert.AreEqual(5L, first.ValueAt(TestAddresses.CellA)); // +2 and +3 resolve to +5
            Assert.AreEqual(first, second);
        }

        [Test]
        public void Enumeration_Order_Does_Not_Change_Conflict_Result()
        {
            SimulationState forward = RunnerWith(SumOnK()).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);
            SimulationState reverse = RunnerWith(SumOnK()).Run(TestAddresses.InitialState(), AddToA(3, 2), 1);

            Assert.AreEqual(5L, forward.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(forward, reverse);
        }

        [Test]
        public void Conflict_Policy_Is_Applied_Exactly_Once()
        {
            var counting = new AddressableStateTests.CountingResolver(new AdditionResolver());
            var resolvers = new Dictionary<Kind, IConflictResolver>
            {
                { TestAddresses.K, counting }
            };

            RunnerWith(resolvers).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);

            Assert.AreEqual(1, counting.Calls); // invoked once, not once per contribution
        }

        [Test]
        public void Conflict_Without_A_Resolver_Is_Rejected()
        {
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellB, 1),
                new AddToCounterTransition(TestAddresses.CellB, 1)
            };
            TickRunner runner = RunnerWith(new Dictionary<Kind, IConflictResolver>()); // no resolver for K

            Assert.Throws<UnresolvedConflictException>(
                () => runner.Run(TestAddresses.InitialState(), transitions, 1));
        }

        [Test]
        public void A_Single_Contribution_Is_Not_A_Conflict_And_Needs_No_Resolver()
        {
            var transitions = new ITransition[] { new AddToCounterTransition(TestAddresses.CellB, 7) };
            TickRunner runner = RunnerWith(new Dictionary<Kind, IConflictResolver>());

            SimulationState result = runner.Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(7L, result.ValueAt(TestAddresses.CellB));
        }
    }
}
