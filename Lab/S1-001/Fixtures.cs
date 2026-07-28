using System;
using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// The laboratory's kind identifiers. Names live here, on the observer's side of the membrane —
    /// the kernel sees opaque integers only.
    /// </summary>
    public static class K
    {
        public static readonly Kind Elevation = new Kind(1);
        public static readonly Kind Water = new Kind(2);
        public static readonly Kind Rock = new Kind(3);
        public static readonly Kind Sediment = new Kind(4);
        public static readonly Kind Sacrificial = new Kind(5);
    }

    public sealed class AdditiveResolver : IConflictResolver
    {
        public long Resolve(IReadOnlyList<long> amounts)
        {
            long sum = 0;
            for (int i = 0; i < amounts.Count; i++)
            {
                sum += amounts[i];
            }

            return sum;
        }
    }

    /// <summary>B3's deliberately broken algebra: keeps the maximum, discards the rest.</summary>
    public sealed class MaxResolver : IConflictResolver
    {
        public long Resolve(IReadOnlyList<long> amounts)
        {
            long max = amounts[0];
            for (int i = 1; i < amounts.Count; i++)
            {
                if (amounts[i] > max)
                {
                    max = amounts[i];
                }
            }

            return max;
        }
    }

    /// <summary>
    /// The divisor policy is the only difference between the naive family (divisor 2, the corpus
    /// law) and the degree-aware cure (divisor = degree + 1). Sealed as W1-vs-W2's subject.
    /// </summary>
    public delegate long DivisorPolicy(int degree);

    public static class Divisors
    {
        public static readonly DivisorPolicy Naive = degree => 2;
        public static readonly DivisorPolicy DegreeAware = degree => degree + 1;
    }

    /// <summary>
    /// The flow fixture: one transition per place. Potential := Elevation + moved kind's carrier
    /// (sealed identification, Blind Spot Audit item 1). For each downhill neighbour, transfer
    /// floor(diff / divisor) of the moved kind — uncapped, exactly the corpus family: negativity is
    /// a phenomenon under test, not an error.
    /// </summary>
    public sealed class FlowFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        public FlowFixture(IReadOnlyList<Place> places, Kind movedKind, DivisorPolicy divisor, IConflictResolver movedResolver)
        {
            foreach (Place place in places)
            {
                _transitions.Add(new FlowTransition(place, movedKind, divisor));
            }

            _resolvers = new Dictionary<Kind, IConflictResolver> { [movedKind] = movedResolver };
        }

        public IReadOnlyList<ITransition> Transitions => _transitions;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers => _resolvers;

        private sealed class FlowTransition : ITransition
        {
            private readonly Place _origin;
            private readonly Kind _moved;
            private readonly DivisorPolicy _divisor;

            public FlowTransition(Place origin, Kind moved, DivisorPolicy divisor)
            {
                _origin = origin;
                _moved = moved;
                _divisor = divisor;
                ReadScope = new ReadScope(new Cell(origin, K.Elevation), new Cell(origin, moved));
                RelationScope = new RelationScope(new[] { origin }, new[] { K.Elevation, moved });
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                long here = view.Read(new Cell(_origin, K.Elevation)) + view.Read(new Cell(_origin, _moved));
                long divisor = _divisor(outgoing.Count);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count; i++)
                {
                    Place target = outgoing[i].Target;
                    long there = view.Read(new Cell(target, K.Elevation)) + view.Read(new Cell(target, _moved));
                    long diff = here - there;
                    if (diff <= 0)
                    {
                        continue;
                    }

                    long transfer = diff / divisor;
                    if (transfer <= 0)
                    {
                        continue;
                    }

                    contributions.Add(new Contribution(new Cell(_origin, _moved), -transfer));
                    contributions.Add(new Contribution(new Cell(target, _moved), transfer));
                }

                return contributions;
            }
        }
    }

    /// <summary>
    /// The conversion fixture (W4): where this tick's water outflow across an edge would exceed the
    /// threshold, emit the pair (−1 Rock, +1 Sediment) at the source place — one law, one atomic
    /// emission per qualifying edge (H1 is exactly this list being returned whole). It recomputes
    /// the corrected flow formula from the same snapshot; sharing a formula is law content, not
    /// coupling through state.
    /// </summary>
    public sealed class ConversionFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        public ConversionFixture(IReadOnlyList<Place> places, DivisorPolicy divisor, long threshold, IConflictResolver additive)
        {
            foreach (Place place in places)
            {
                _transitions.Add(new ConversionTransition(place, divisor, threshold));
            }

            _resolvers = new Dictionary<Kind, IConflictResolver>
            {
                [K.Rock] = additive,
                [K.Sediment] = additive,
            };
        }

        public IReadOnlyList<ITransition> Transitions => _transitions;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers => _resolvers;

        private sealed class ConversionTransition : ITransition
        {
            private readonly Place _origin;
            private readonly DivisorPolicy _divisor;
            private readonly long _threshold;

            public ConversionTransition(Place origin, DivisorPolicy divisor, long threshold)
            {
                _origin = origin;
                _divisor = divisor;
                _threshold = threshold;
                ReadScope = new ReadScope(new Cell(origin, K.Elevation), new Cell(origin, K.Water));
                RelationScope = new RelationScope(new[] { origin }, new[] { K.Elevation, K.Water });
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                long here = view.Read(new Cell(_origin, K.Elevation)) + view.Read(new Cell(_origin, K.Water));
                long divisor = _divisor(outgoing.Count);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count; i++)
                {
                    Place target = outgoing[i].Target;
                    long there = view.Read(new Cell(target, K.Elevation)) + view.Read(new Cell(target, K.Water));
                    long diff = here - there;
                    if (diff <= 0)
                    {
                        continue;
                    }

                    long transfer = diff / divisor;
                    if (transfer > _threshold)
                    {
                        contributions.Add(new Contribution(new Cell(_origin, K.Rock), -1));
                        contributions.Add(new Contribution(new Cell(_origin, K.Sediment), 1));
                    }
                }

                return contributions;
            }
        }
    }

    /// <summary>
    /// The sediment transport fixture (W4): Sediment rides the corrected flow driven by the same
    /// potential (Elevation + Water), guarded by what is actually present — a passenger, not the
    /// phenomenon under stability test.
    /// </summary>
    public sealed class SedimentTransportFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        public SedimentTransportFixture(IReadOnlyList<Place> places, DivisorPolicy divisor, IConflictResolver additive)
        {
            foreach (Place place in places)
            {
                _transitions.Add(new TransportTransition(place, divisor));
            }

            _resolvers = new Dictionary<Kind, IConflictResolver> { [K.Sediment] = additive };
        }

        public IReadOnlyList<ITransition> Transitions => _transitions;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers => _resolvers;

        private sealed class TransportTransition : ITransition
        {
            private readonly Place _origin;
            private readonly DivisorPolicy _divisor;

            public TransportTransition(Place origin, DivisorPolicy divisor)
            {
                _origin = origin;
                _divisor = divisor;
                ReadScope = new ReadScope(
                    new Cell(origin, K.Elevation), new Cell(origin, K.Water), new Cell(origin, K.Sediment));
                RelationScope = new RelationScope(new[] { origin }, new[] { K.Elevation, K.Water });
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                long here = view.Read(new Cell(_origin, K.Elevation)) + view.Read(new Cell(_origin, K.Water));
                long carried = view.Read(new Cell(_origin, K.Sediment));
                long divisor = _divisor(outgoing.Count);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count && carried > 0; i++)
                {
                    Place target = outgoing[i].Target;
                    long there = view.Read(new Cell(target, K.Elevation)) + view.Read(new Cell(target, K.Water));
                    long diff = here - there;
                    if (diff <= 0)
                    {
                        continue;
                    }

                    long transfer = Math.Min(carried, diff / divisor);
                    if (transfer <= 0)
                    {
                        continue;
                    }

                    carried -= transfer;
                    contributions.Add(new Contribution(new Cell(_origin, K.Sediment), -transfer));
                    contributions.Add(new Contribution(new Cell(target, K.Sediment), transfer));
                }

                return contributions;
            }
        }
    }

    /// <summary>
    /// B3's negative control: two transitions emit zero-sum pairs on the Sacrificial kind between
    /// two places, but the kind's resolver is MaxResolver — additive algebra would conserve; Max
    /// must leak. If the conservation audit stays silent on this world, the instrument is refuted.
    /// </summary>
    public sealed class SacrificialFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        public SacrificialFixture(Place a, Place b)
        {
            _transitions.Add(new PairEmitter(a, b, 2));
            _transitions.Add(new PairEmitter(a, b, 3));
            _resolvers = new Dictionary<Kind, IConflictResolver> { [K.Sacrificial] = new MaxResolver() };
        }

        public IReadOnlyList<ITransition> Transitions => _transitions;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers => _resolvers;

        private sealed class PairEmitter : ITransition
        {
            private readonly Place _from;
            private readonly Place _to;
            private readonly long _amount;

            public PairEmitter(Place from, Place to, long amount)
            {
                _from = from;
                _to = to;
                _amount = amount;
                ReadScope = new ReadScope(new Cell(from, K.Sacrificial));
                RelationScope = RelationScope.Empty;
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                return new[]
                {
                    new Contribution(new Cell(_from, K.Sacrificial), -_amount),
                    new Contribution(new Cell(_to, K.Sacrificial), _amount),
                };
            }
        }
    }
}
