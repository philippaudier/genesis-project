using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The composition proof (Genesis-005, kept as a regression guard): independent transitions
    /// compose deterministically and order-independently, and each reads the same start-of-tick
    /// snapshot through its own scoped view.
    /// </summary>
    public sealed class TransitionCompositionTests
    {
        private static readonly ITransition AddA = new AddToCounterTransition(TestAddresses.CellA, 1);
        private static readonly ITransition AddB = new AddToCounterTransition(TestAddresses.CellB, 1);
        private static readonly ITransition AddC = new AddToCounterTransition(TestAddresses.CellC, 1);

        private static TickRunner NewRunner()
        {
            return new TickRunner(new TransitionRunner());
        }

        [Test]
        public void Independent_Transitions_Do_Not_Interact()
        {
            SimulationState result = NewRunner().Run(TestAddresses.InitialState(), new[] { AddA, AddB, AddC }, 1);

            Assert.AreEqual(1L, result.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(1L, result.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(1L, result.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Non_Conflicting_Writes_Are_Unaffected()
        {
            SimulationState abc = NewRunner().Run(TestAddresses.InitialState(), new[] { AddA, AddB, AddC }, 100);
            SimulationState cab = NewRunner().Run(TestAddresses.InitialState(), new[] { AddC, AddA, AddB }, 100);

            Assert.AreEqual(abc, cab);
            Assert.AreEqual(100L, abc.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(100L, abc.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(100L, abc.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Every_Transition_Reads_The_Same_Snapshot()
        {
            SimulationState initial = TestAddresses.StateWith(5, 0, 0);
            ITransition mirror = new MirrorCounterTransition(TestAddresses.CellA, TestAddresses.CellC);

            SimulationState forward = NewRunner().Run(initial, new[] { AddA, mirror }, 1);
            SimulationState reverse = NewRunner().Run(initial, new[] { mirror, AddA }, 1);

            // C receives A read from the snapshot (5), never the value AddA produced (6),
            // and the result is identical whichever order the two ran in.
            Assert.AreEqual(5L, forward.ValueAt(TestAddresses.CellC));
            Assert.AreEqual(6L, forward.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(forward, reverse);
        }

        [Test]
        public void Initial_State_Is_Never_Mutated()
        {
            SimulationState initial = TestAddresses.InitialState();

            NewRunner().Run(initial, new[] { AddA, AddB, AddC }, 1000);

            Assert.AreEqual(TestAddresses.InitialState(), initial);
        }
    }
}
