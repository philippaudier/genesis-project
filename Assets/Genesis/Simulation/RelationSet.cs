using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// An immutable set of explicit relations between addresses that a given
    /// <see cref="SimulationState"/> defines (Genesis-009). Every relation is validated against that
    /// state's addresses at construction; a relation referencing an unknown address is rejected.
    ///
    /// It is a <em>set</em>: duplicates collapse, and equality and hashing depend only on which
    /// relations are present — never on insertion or enumeration order (the hash is a commutative
    /// aggregation over relation hashes). Constructing a relation set reads the state's addresses and
    /// modifies nothing.
    ///
    /// Deliberate placement (recorded as an open question): relations live <em>beside</em> the
    /// simulation state, not inside it. Whether topology ultimately belongs inside
    /// <see cref="SimulationState"/> — mutable by transitions like any other state — is not decided
    /// here.
    /// </summary>
    public sealed class RelationSet : IEquatable<RelationSet>
    {
        private readonly HashSet<Relation> _relations;

        public RelationSet(SimulationState state, params Relation[] relations)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (relations == null)
            {
                throw new ArgumentNullException(nameof(relations));
            }

            _relations = new HashSet<Relation>();
            foreach (Relation relation in relations)
            {
                if (!state.Defines(relation.Source))
                {
                    throw new ArgumentException(
                        $"Relation source {relation.Source} does not exist in the state.", nameof(relations));
                }

                if (!state.Defines(relation.Target))
                {
                    throw new ArgumentException(
                        $"Relation target {relation.Target} does not exist in the state.", nameof(relations));
                }

                _relations.Add(relation); // a duplicate collapses; the set is unchanged
            }
        }

        /// <summary>The number of distinct relations in the set.</summary>
        public int Count => _relations.Count;

        /// <summary>Whether the set contains exactly <paramref name="relation"/> (direction included).</summary>
        public bool Contains(Relation relation)
        {
            return _relations.Contains(relation);
        }

        public bool Equals(RelationSet other)
        {
            if (other is null)
            {
                return false;
            }

            return _relations.SetEquals(other._relations);
        }

        public override bool Equals(object obj)
        {
            return obj is RelationSet other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Commutative aggregation (sum) over relation hashes, so the hash — like equality —
            // cannot depend on enumeration order.
            long aggregate = 0;
            foreach (Relation relation in _relations)
            {
                aggregate += relation.GetHashCode();
            }

            return aggregate.GetHashCode();
        }

        public override string ToString()
        {
            return $"RelationSet(count={_relations.Count})";
        }
    }
}
