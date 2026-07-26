using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// An immutable set of explicit relations between places that a given
    /// <see cref="SimulationState"/> defines. Every relation is validated against the state's places
    /// at construction (place existence is derived from cells — RFC-0003 D5); a relation referencing
    /// an unknown place is rejected. It is a <em>set</em>: duplicates collapse, and equality and
    /// hashing depend only on which relations are present — never on insertion or enumeration order.
    /// Relations live beside the simulation state; whether topology ultimately belongs inside it
    /// remains deliberately open.
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
                if (!state.DefinesPlace(relation.Source))
                {
                    throw new ArgumentException(
                        $"Relation source {relation.Source} does not exist in the state.", nameof(relations));
                }

                if (!state.DefinesPlace(relation.Target))
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

        /// <summary>
        /// The relations whose source is <paramref name="origin"/>, in canonical order (target place
        /// ascending). The backing set's iteration order is never exposed: the projection is sorted,
        /// so the result is identical however the set was built.
        /// </summary>
        public IReadOnlyList<Relation> OutgoingFrom(Place origin)
        {
            var outgoing = new List<Relation>();
            foreach (Relation relation in _relations)
            {
                if (relation.Source == origin)
                {
                    outgoing.Add(relation);
                }
            }

            outgoing.Sort((left, right) => left.Target.CompareTo(right.Target));
            return outgoing;
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
            // Commutative aggregation (sum) over relation hashes — order-blind, like equality.
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
