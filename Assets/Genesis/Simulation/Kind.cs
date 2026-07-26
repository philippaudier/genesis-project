using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// The identity of a nature of value (RFC-0003). Opaque, closed, explicitly declared per world —
    /// a kind carries a causal role, not a value type (all values remain integers). Conflict
    /// semantics attach to kinds (D2): a kind's resolver is uniform wherever the kind occurs, because
    /// a kind <em>means</em> a shared causal role.
    /// </summary>
    public readonly struct Kind : IEquatable<Kind>, IComparable<Kind>
    {
        /// <summary>The underlying identity value. Two kinds are the same nature iff equal.</summary>
        public int Value { get; }

        public Kind(int value)
        {
            Value = value;
        }

        public bool Equals(Kind other) => Value == other.Value;

        public override bool Equals(object obj) => obj is Kind other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>Orders kinds by identity. Used only to make enumeration deterministic.</summary>
        public int CompareTo(Kind other) => Value.CompareTo(other.Value);

        public override string ToString() => $"Kind({Value})";

        public static bool operator ==(Kind left, Kind right) => left.Equals(right);
        public static bool operator !=(Kind left, Kind right) => !left.Equals(right);
    }
}
