using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// The stable, explicit identity of one counter location (Genesis-008). Distinct addresses are
    /// distinct pieces of state of the same kind — the prerequisite of any future relation between
    /// them. An address says nothing about position, neighbourhood, or meaning; it only makes a
    /// location distinguishable and stable.
    /// </summary>
    public readonly struct CounterAddress : IEquatable<CounterAddress>, IComparable<CounterAddress>
    {
        /// <summary>The underlying identity value. Two addresses are the same location iff equal.</summary>
        public int Value { get; }

        public CounterAddress(int value)
        {
            Value = value;
        }

        public bool Equals(CounterAddress other) => Value == other.Value;

        public override bool Equals(object obj) => obj is CounterAddress other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>Orders addresses by identity. Used only to make runner iteration deterministic.</summary>
        public int CompareTo(CounterAddress other) => Value.CompareTo(other.Value);

        public override string ToString() => $"Address({Value})";

        public static bool operator ==(CounterAddress left, CounterAddress right) => left.Equals(right);
        public static bool operator !=(CounterAddress left, CounterAddress right) => !left.Equals(right);
    }
}
