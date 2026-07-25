using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Tests for the <see cref="Tick"/> value type: its zero, its successor, its equality and
    /// ordering, and its refusal of negative indices.
    /// </summary>
    public sealed class TickTests
    {
        [Test]
        public void Zero_Has_Value_Zero()
        {
            Assert.AreEqual(0L, Tick.Zero.Value);
        }

        [Test]
        public void Next_Returns_The_Following_Tick()
        {
            Assert.AreEqual(new Tick(1), Tick.Zero.Next());
            Assert.AreEqual(new Tick(43), new Tick(42).Next());
        }

        [Test]
        public void Ticks_Have_Value_Equality()
        {
            Assert.AreEqual(new Tick(7), new Tick(7));
            Assert.AreNotEqual(new Tick(7), new Tick(8));
        }

        [Test]
        public void Ticks_Are_Ordered_By_Value()
        {
            Assert.IsTrue(new Tick(3) < new Tick(4));
            Assert.IsTrue(new Tick(5) > new Tick(4));
            Assert.IsTrue(new Tick(4) <= new Tick(4));
            Assert.IsTrue(new Tick(4) >= new Tick(4));
        }

        [Test]
        public void Negative_Tick_Is_Rejected()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Tick(-1));
        }
    }
}
