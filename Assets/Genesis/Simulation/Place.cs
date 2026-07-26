using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// The stable, explicit identity of one location (RFC-0003). A place says nothing about
    /// position, neighbourhood, or meaning; it only makes a location distinguishable and stable.
    /// Relations connect places (D3); what a place holds is a matter of its cells.
    /// </summary>
    public readonly struct Place : IEquatable<Place>, IComparable<Place>
    {
        /// <summary>The underlying identity value. Two places are the same location iff equal.</summary>
        public int Value { get; }

        public Place(int value)
        {
            Value = value;
        }

        public bool Equals(Place other) => Value == other.Value;

        public override bool Equals(object obj) => obj is Place other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>Orders places by identity. Used only to make enumeration deterministic.</summary>
        public int CompareTo(Place other) => Value.CompareTo(other.Value);

        public override string ToString() => $"Place({Value})";

        public static bool operator ==(Place left, Place right) => left.Equals(right);
        public static bool operator !=(Place left, Place right) => !left.Equals(right);
    }
}
