using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// An explicit, directed connection from one place to another (RFC-0003 D3: relations connect
    /// places, not cells — topology is about location, not nature). It carries no spatial, semantic,
    /// weighted, or lifecycle meaning. Identity is structural over (source, target); direction
    /// matters, so A→B and B→A are distinct relations.
    /// </summary>
    public readonly struct Relation : IEquatable<Relation>
    {
        /// <summary>The place the relation points from.</summary>
        public Place Source { get; }

        /// <summary>The place the relation points to.</summary>
        public Place Target { get; }

        public Relation(Place source, Place target)
        {
            Source = source;
            Target = target;
        }

        public bool Equals(Relation other)
        {
            return Source == other.Source && Target == other.Target;
        }

        public override bool Equals(object obj)
        {
            return obj is Relation other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Asymmetric combination so A→B and B→A hash differently.
            return (Source.GetHashCode() * 397) ^ Target.GetHashCode();
        }

        public override string ToString()
        {
            return $"Relation({Source} -> {Target})";
        }

        public static bool operator ==(Relation left, Relation right) => left.Equals(right);
        public static bool operator !=(Relation left, Relation right) => !left.Equals(right);
    }
}
