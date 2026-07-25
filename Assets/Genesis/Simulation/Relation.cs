using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// The smallest possible relation (Genesis-009): an explicit, directed connection from one
    /// address to another. It carries no spatial, semantic, weighted, or lifecycle meaning — it only
    /// states that a connection exists, and in which direction. Identity is structural over
    /// (source, target); direction matters, so A→B and B→A are distinct relations.
    /// </summary>
    public readonly struct Relation : IEquatable<Relation>
    {
        /// <summary>The address the relation points from.</summary>
        public CounterAddress Source { get; }

        /// <summary>The address the relation points to.</summary>
        public CounterAddress Target { get; }

        public Relation(CounterAddress source, CounterAddress target)
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
