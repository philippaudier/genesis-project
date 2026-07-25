using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The scoped-reads proof (Genesis-007, kept as a regression guard): a transition's reads are
    /// explicit and structurally scoped — it receives only the addresses it declared, cannot observe
    /// anything else, and the bound does not disturb contribution or conflict semantics.
    /// </summary>
    public sealed class ScopedReadTests
    {
        private static TickRunner NewRunner()
        {
            return new TickRunner(new TransitionRunner());
        }

        [Test]
        public void Transition_Can_Read_Declared_State()
        {
            SimulationState initial = TestAddresses.StateWith(5, 0, 0);
            var transitions = new ITransition[]
            {
                new MirrorCounterTransition(TestAddresses.A, TestAddresses.C)
            };

            SimulationState result = NewRunner().Run(initial, transitions, 1);

            Assert.AreEqual(5L, result.CounterOf(TestAddresses.C));
        }

        [Test]
        public void Read_Scope_Does_Not_Change_Contribution_Semantics()
        {
            var resolvers = new Dictionary<CounterAddress, IConflictResolver>
            {
                { TestAddresses.A, new AdditionResolver() }
            };
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.A, 2),
                new AddToCounterTransition(TestAddresses.A, 3)
            };

            SimulationState result = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(5L, result.CounterOf(TestAddresses.A));
        }

        [Test]
        public void Every_Transition_Still_Reads_The_Same_Tick_Snapshot()
        {
            SimulationState initial = TestAddresses.StateWith(5, 20, 0);
            var transitions = new ITransition[]
            {
                new MirrorCounterTransition(TestAddresses.A, TestAddresses.B), // scope {A}: B += snapshot.A
                new MirrorCounterTransition(TestAddresses.B, TestAddresses.C)  // scope {B}: C += snapshot.B
            };

            SimulationState result = NewRunner().Run(initial, transitions, 1);

            Assert.AreEqual(25L, result.CounterOf(TestAddresses.B)); // 20 + snapshot.A(5)
            Assert.AreEqual(20L, result.CounterOf(TestAddresses.C)); // snapshot.B(20), NOT the updated 25
        }
    }
}
