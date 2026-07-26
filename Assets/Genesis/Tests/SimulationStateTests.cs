using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Tests for <see cref="SimulationState"/> as an immutable value over cells: addressing,
    /// per-cell derivation, refusal of unknown cells, derived place existence (RFC-0003 D5), and
    /// value equality (which the determinism proofs rely on).
    /// </summary>
    public sealed class SimulationStateTests
    {
        [Test]
        public void Initial_State_Defines_Its_Cells_At_Zero()
        {
            SimulationState state = TestAddresses.InitialState();

            Assert.AreEqual(Tick.Zero, state.CurrentTick);
            Assert.AreEqual(0L, state.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(0L, state.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(0L, state.ValueAt(TestAddresses.CellC));
            Assert.IsTrue(state.Defines(TestAddresses.CellA));
        }

        [Test]
        public void Place_Existence_Is_Derived_From_Cells()
        {
            SimulationState state = TestAddresses.InitialState();

            Assert.IsTrue(state.DefinesPlace(TestAddresses.A));
            Assert.IsFalse(state.DefinesPlace(new Place(999)));
        }

        [Test]
        public void WithValue_Sets_Only_The_Targeted_Cell()
        {
            SimulationState state = TestAddresses.StateWith(1, 2, 3);

            SimulationState next = state.WithValue(TestAddresses.CellB, 9);

            Assert.AreEqual(1L, next.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(9L, next.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(3L, next.ValueAt(TestAddresses.CellC));
            Assert.AreNotSame(state, next);
            Assert.AreEqual(2L, state.ValueAt(TestAddresses.CellB)); // the original is immutable
        }

        [Test]
        public void Reading_Or_Writing_An_Unknown_Cell_Is_Rejected()
        {
            SimulationState state = TestAddresses.InitialState();
            var unknown = new Cell(new Place(999), TestAddresses.K);

            Assert.IsFalse(state.Defines(unknown));
            Assert.Throws<System.ArgumentException>(() => state.ValueAt(unknown));
            Assert.Throws<System.ArgumentException>(() => state.WithValue(unknown, 1));
        }

        [Test]
        public void States_Have_Value_Equality()
        {
            Assert.AreEqual(TestAddresses.StateWith(1, 2, 3), TestAddresses.StateWith(1, 2, 3));
            Assert.AreNotEqual(TestAddresses.StateWith(1, 2, 3), TestAddresses.StateWith(1, 2, 9));
        }
    }
}
