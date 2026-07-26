using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The scoped-reads proof (Genesis-007, kept as a regression guard): a transition's reads are
    /// explicit and structurally scoped — it receives only the cells it declared, and the bound does
    /// not disturb contribution or conflict semantics.
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
                new MirrorCounterTransition(TestAddresses.CellA, TestAddresses.CellC)
            };

            SimulationState result = NewRunner().Run(initial, transitions, 1);

            Assert.AreEqual(5L, result.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Read_Scope_Does_Not_Change_Contribution_Semantics()
        {
            var resolvers = new Dictionary<Kind, IConflictResolver>
            {
                { TestAddresses.K, new AdditionResolver() }
            };
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellA, 2),
                new AddToCounterTransition(TestAddresses.CellA, 3)
            };

            SimulationState result = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(5L, result.ValueAt(TestAddresses.CellA));
        }

        [Test]
        public void Every_Transition_Still_Reads_The_Same_Tick_Snapshot()
        {
            SimulationState initial = TestAddresses.StateWith(5, 20, 0);
            var transitions = new ITransition[]
            {
                new MirrorCounterTransition(TestAddresses.CellA, TestAddresses.CellB), // B += snapshot.A
                new MirrorCounterTransition(TestAddresses.CellB, TestAddresses.CellC)  // C += snapshot.B
            };

            SimulationState result = NewRunner().Run(initial, transitions, 1);

            Assert.AreEqual(25L, result.ValueAt(TestAddresses.CellB)); // 20 + snapshot.A(5)
            Assert.AreEqual(20L, result.ValueAt(TestAddresses.CellC)); // snapshot.B(20), NOT the updated 25
        }
    }
}
