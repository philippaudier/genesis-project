using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The conflict proof (Genesis-006, kept as a regression guard): conflicting contributions to the
    /// same address resolve exactly once, deterministically, and independently of enumeration order,
    /// via an explicit commutative resolver — and a conflict with no resolver is rejected.
    /// </summary>
    public sealed class ConflictResolutionTests
    {
        private static TickRunner RunnerWith(IReadOnlyDictionary<CounterAddress, IConflictResolver> resolvers)
        {
            return new TickRunner(new TransitionRunner(resolvers));
        }

        private static Dictionary<CounterAddress, IConflictResolver> SumOnA()
        {
            return new Dictionary<CounterAddress, IConflictResolver>
            {
                { TestAddresses.A, new AdditionResolver() }
            };
        }

        private static ITransition[] AddToA(long first, long second)
        {
            return new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.A, first),
                new AddToCounterTransition(TestAddresses.A, second)
            };
        }

        [Test]
        public void Conflicting_Writes_Are_Deterministic()
        {
            SimulationState first = RunnerWith(SumOnA()).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);
            SimulationState second = RunnerWith(SumOnA()).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);

            Assert.AreEqual(5L, first.CounterOf(TestAddresses.A)); // +2 and +3 resolve to +5
            Assert.AreEqual(first, second);
        }

        [Test]
        public void Enumeration_Order_Does_Not_Change_Conflict_Result()
        {
            SimulationState forward = RunnerWith(SumOnA()).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);
            SimulationState reverse = RunnerWith(SumOnA()).Run(TestAddresses.InitialState(), AddToA(3, 2), 1);

            Assert.AreEqual(5L, forward.CounterOf(TestAddresses.A));
            Assert.AreEqual(forward, reverse); // reversing the order still resolves to +5
        }

        [Test]
        public void Conflict_Policy_Is_Applied_Exactly_Once()
        {
            var counting = new AddressableStateTests.CountingResolver(new AdditionResolver());
            var resolvers = new Dictionary<CounterAddress, IConflictResolver>
            {
                { TestAddresses.A, counting }
            };

            RunnerWith(resolvers).Run(TestAddresses.InitialState(), AddToA(2, 3), 1);

            Assert.AreEqual(1, counting.Calls); // invoked once, not once per contribution
        }

        [Test]
        public void Conflict_Without_A_Resolver_Is_Rejected()
        {
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.B, 1),
                new AddToCounterTransition(TestAddresses.B, 1)
            };
            TickRunner runner = RunnerWith(new Dictionary<CounterAddress, IConflictResolver>()); // no resolver for B

            Assert.Throws<UnresolvedConflictException>(
                () => runner.Run(TestAddresses.InitialState(), transitions, 1));
        }

        [Test]
        public void A_Single_Contribution_Is_Not_A_Conflict_And_Needs_No_Resolver()
        {
            var transitions = new ITransition[] { new AddToCounterTransition(TestAddresses.B, 7) };
            TickRunner runner = RunnerWith(new Dictionary<CounterAddress, IConflictResolver>()); // no resolver for B

            SimulationState result = runner.Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(7L, result.CounterOf(TestAddresses.B));
        }
    }
}
