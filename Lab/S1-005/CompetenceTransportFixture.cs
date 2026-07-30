using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;

namespace Genesis.Lab.S1_005
{
    /// <summary>
    /// The sole candidate distinction sealed for S1-005. This is the S1-004 Sediment
    /// transport with one scalar guard: prospective transfer must be strictly greater
    /// than competence. At competence zero it is extensionally the existing fixture.
    /// </summary>
    public sealed class CompetenceTransportFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        public long Competence { get; }

        public CompetenceTransportFixture(IReadOnlyList<Place> places, DivisorPolicy divisor,
            long competence, IConflictResolver additive)
        {
            Competence = competence;
            foreach (Place place in places)
            {
                _transitions.Add(new TransportTransition(place, divisor, competence));
            }

            _resolvers = new Dictionary<Kind, IConflictResolver> { [K4.Sediment] = additive };
        }

        public IReadOnlyList<ITransition> Transitions => _transitions;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers => _resolvers;

        private sealed class TransportTransition : ITransition
        {
            private readonly Place _origin;
            private readonly DivisorPolicy _divisor;
            private readonly long _competence;

            public TransportTransition(Place origin, DivisorPolicy divisor, long competence)
            {
                _origin = origin;
                _divisor = divisor;
                _competence = competence;
                ReadScope = Potential.ScopeAt(origin);
                RelationScope = Potential.RelationsAt(origin);
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                long here = Potential.At(view, _origin);
                long carried = view.Read(new Cell(_origin, K4.Sediment));
                long divisor = _divisor(outgoing.Count);
                var contributions = new List<Contribution>();

                for (int i = 0; i < outgoing.Count && carried > 0; i++)
                {
                    Place target = outgoing[i].Target;
                    long diff = here - Potential.At(view, target);
                    if (diff <= 0)
                    {
                        continue;
                    }

                    long prospective = diff / divisor;
                    if (prospective <= _competence)
                    {
                        continue;
                    }

                    long transfer = Math.Min(carried, prospective);
                    carried -= transfer;
                    contributions.Add(new Contribution(new Cell(_origin, K4.Sediment), -transfer));
                    contributions.Add(new Contribution(new Cell(target, K4.Sediment), transfer));
                }

                return contributions;
            }
        }
    }
}
