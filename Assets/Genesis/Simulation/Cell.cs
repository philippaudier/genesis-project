using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// One value-holding location: the pair (Place, Kind) — RFC-0003's addressable unit. The cell is
    /// the unit of writing and conflict (D1): contributions target cells, conflicts group by cell,
    /// and two contributions to the same place but different kinds never collide.
    /// </summary>
    public readonly struct Cell : IEquatable<Cell>, IComparable<Cell>
    {
        /// <summary>The location this cell belongs to.</summary>
        public Place Place { get; }

        /// <summary>The nature of the value this cell holds.</summary>
        public Kind Kind { get; }

        public Cell(Place place, Kind kind)
        {
            Place = place;
            Kind = kind;
        }

        public bool Equals(Cell other) => Place == other.Place && Kind == other.Kind;

        public override bool Equals(object obj) => obj is Cell other && Equals(other);

        public override int GetHashCode() => (Place.GetHashCode() * 397) ^ Kind.GetHashCode();

        /// <summary>Canonical order: place ascending, then kind ascending. Makes commits deterministic.</summary>
        public int CompareTo(Cell other)
        {
            int byPlace = Place.CompareTo(other.Place);
            return byPlace != 0 ? byPlace : Kind.CompareTo(other.Kind);
        }

        public override string ToString() => $"Cell({Place}, {Kind})";

        public static bool operator ==(Cell left, Cell right) => left.Equals(right);
        public static bool operator !=(Cell left, Cell right) => !left.Equals(right);
    }
}
