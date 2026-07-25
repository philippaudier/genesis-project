using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Tests for <see cref="SimulationState"/> as an immutable value: addressing counters, per-address
    /// derivation, refusal of unknown addresses, and value equality (which the determinism proofs
    /// rely on).
    /// </summary>
    public sealed class SimulationStateTests
    {
        [Test]
        public void Initial_State_Defines_Its_Addresses_At_Zero()
        {
            SimulationState state = TestAddresses.InitialState();

            Assert.AreEqual(Tick.Zero, state.CurrentTick);
            Assert.AreEqual(0L, state.CounterOf(TestAddresses.A));
            Assert.AreEqual(0L, state.CounterOf(TestAddresses.B));
            Assert.AreEqual(0L, state.CounterOf(TestAddresses.C));
            Assert.IsTrue(state.Defines(TestAddresses.A));
        }

        [Test]
        public void WithCounter_Sets_Only_The_Targeted_Address()
        {
            SimulationState state = TestAddresses.StateWith(1, 2, 3);

            SimulationState next = state.WithCounter(TestAddresses.B, 9);

            Assert.AreEqual(1L, next.CounterOf(TestAddresses.A));
            Assert.AreEqual(9L, next.CounterOf(TestAddresses.B));
            Assert.AreEqual(3L, next.CounterOf(TestAddresses.C));
            Assert.AreNotSame(state, next);
            Assert.AreEqual(2L, state.CounterOf(TestAddresses.B)); // the original is immutable
        }

        [Test]
        public void Reading_Or_Writing_An_Unknown_Address_Is_Rejected()
        {
            SimulationState state = TestAddresses.InitialState();
            var unknown = new CounterAddress(999);

            Assert.IsFalse(state.Defines(unknown));
            Assert.Throws<System.ArgumentException>(() => state.CounterOf(unknown));
            Assert.Throws<System.ArgumentException>(() => state.WithCounter(unknown, 1));
        }

        [Test]
        public void States_Have_Value_Equality()
        {
            Assert.AreEqual(TestAddresses.StateWith(1, 2, 3), TestAddresses.StateWith(1, 2, 3));
            Assert.AreNotEqual(TestAddresses.StateWith(1, 2, 3), TestAddresses.StateWith(1, 2, 9));
        }
    }
}
