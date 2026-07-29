using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.S1_004
{
    /// <summary>
    /// The kinds of Campaign S1-004 (sealed `b114a34`). Base is new; Water, Rock and Sediment
    /// keep the identifiers S1-001 gave them, because they mean the same causal roles. Names live
    /// here, on the observer's side: the kernel sees opaque integers.
    /// </summary>
    public static class K4
    {
        public static readonly Kind Water = new Kind(2);
        public static readonly Kind Rock = new Kind(3);
        public static readonly Kind Sediment = new Kind(4);
        public static readonly Kind Base = new Kind(6);

        /// <summary>SolidSurface(P) = Base + Rock + Sediment — a reading, never a Kind.</summary>
        public static long SolidSurface(SimulationState state, Place place)
        {
            return state.ValueAt(new Cell(place, Base))
                 + state.ValueAt(new Cell(place, Rock))
                 + state.ValueAt(new Cell(place, Sediment));
        }

        /// <summary>WaterPotential(P) = SolidSurface(P) + Water(P).</summary>
        public static long WaterPotential(SimulationState state, Place place)
        {
            return SolidSurface(state, place) + state.ValueAt(new Cell(place, Water));
        }
    }

    /// <summary>
    /// Reads the four sealed terms of the potential through a transition's declared view. The
    /// three fixtures below share it so that "the pair differs by exactly one law fixture" is
    /// true of the code and not only of the prose.
    /// </summary>
    internal static class Potential
    {
        internal static readonly Kind[] Terms = { K4.Base, K4.Rock, K4.Sediment, K4.Water };

        internal static long At(IRelationalStateView view, Place place)
        {
            long total = 0;
            for (int i = 0; i < Terms.Length; i++)
            {
                total += view.Read(new Cell(place, Terms[i]));
            }

            return total;
        }

        internal static ReadScope ScopeAt(Place place, params Kind[] extra)
        {
            var cells = new List<Cell>();
            foreach (Kind kind in Terms)
            {
                cells.Add(new Cell(place, kind));
            }

            foreach (Kind kind in extra)
            {
                cells.Add(new Cell(place, kind));
            }

            return new ReadScope(cells.ToArray());
        }

        internal static RelationScope RelationsAt(Place place)
        {
            return new RelationScope(new[] { place }, Terms);
        }
    }

    /// <summary>
    /// Water flow. Potential := Base + Rock + Sediment + Water (the sealed reading); transfer to
    /// each strictly-lower neighbour is floor(diff / divisor). Uncapped, exactly the studied
    /// family: negativity is a measured phenomenon, never an error.
    /// </summary>
    public sealed class SurfaceFlowFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        public SurfaceFlowFixture(IReadOnlyList<Place> places, DivisorPolicy divisor, IConflictResolver additive)
        {
            foreach (Place place in places)
            {
                _transitions.Add(new FlowTransition(place, divisor));
            }

            _resolvers = new Dictionary<Kind, IConflictResolver> { [K4.Water] = additive };
        }

        public IReadOnlyList<ITransition> Transitions => _transitions;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers => _resolvers;

        private sealed class FlowTransition : ITransition
        {
            private readonly Place _origin;
            private readonly DivisorPolicy _divisor;

            public FlowTransition(Place origin, DivisorPolicy divisor)
            {
                _origin = origin;
                _divisor = divisor;
                ReadScope = Potential.ScopeAt(origin);
                RelationScope = Potential.RelationsAt(origin);
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                long here = Potential.At(view, _origin);
                long divisor = _divisor(outgoing.Count);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count; i++)
                {
                    Place target = outgoing[i].Target;
                    long diff = here - Potential.At(view, target);
                    if (diff <= 0)
                    {
                        continue;
                    }

                    long transfer = diff / divisor;
                    if (transfer <= 0)
                    {
                        continue;
                    }

                    contributions.Add(new Contribution(new Cell(_origin, K4.Water), -transfer));
                    contributions.Add(new Contribution(new Cell(target, K4.Water), transfer));
                }

                return contributions;
            }
        }
    }

    /// <summary>
    /// Conversion. Where the prospective Water flux across an edge is **strictly greater** than
    /// the threshold, the origin emits the pair (−1 Rock, +1 Sediment) — one law, one whole
    /// emission (H1's shape). It recomputes the flux from the same snapshot; sharing a formula is
    /// law content, not coupling through state.
    /// </summary>
    public sealed class SurfaceConversionFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        /// <summary>
        /// Rock and Sediment take their resolvers separately so each kind can carry its own
        /// witness; both must be additive for D-G2's criterion to hold.
        /// </summary>
        public SurfaceConversionFixture(IReadOnlyList<Place> places, DivisorPolicy divisor,
            long threshold, IConflictResolver rockResolver, IConflictResolver sedimentResolver)
        {
            foreach (Place place in places)
            {
                _transitions.Add(new ConversionTransition(place, divisor, threshold));
            }

            _resolvers = new Dictionary<Kind, IConflictResolver>
            {
                [K4.Rock] = rockResolver,
                [K4.Sediment] = sedimentResolver,
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
                ReadScope = Potential.ScopeAt(origin);
                RelationScope = Potential.RelationsAt(origin);
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                long here = Potential.At(view, _origin);
                long divisor = _divisor(outgoing.Count);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count; i++)
                {
                    long diff = here - Potential.At(view, outgoing[i].Target);
                    if (diff <= 0)
                    {
                        continue;
                    }

                    if (diff / divisor > _threshold)
                    {
                        contributions.Add(new Contribution(new Cell(_origin, K4.Rock), -1));
                        contributions.Add(new Contribution(new Cell(_origin, K4.Sediment), 1));
                    }
                }

                return contributions;
            }
        }
    }

    /// <summary>
    /// Sediment transport — **the single fixture by which M1 differs from M0.** Sediment rides
    /// the same potential; the origin may hand over no more than it held in the tick's snapshot,
    /// which is what makes snapshot isolation load-bearing in the sealed derivation.
    /// </summary>
    public sealed class SurfaceSedimentTransportFixture : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers;

        public SurfaceSedimentTransportFixture(IReadOnlyList<Place> places, DivisorPolicy divisor,
            IConflictResolver additive)
        {
            foreach (Place place in places)
            {
                _transitions.Add(new TransportTransition(place, divisor));
            }

            _resolvers = new Dictionary<Kind, IConflictResolver> { [K4.Sediment] = additive };
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
                ReadScope = Potential.ScopeAt(origin);
                RelationScope = Potential.RelationsAt(origin);
            }

            public ReadScope ReadScope { get; }
            public RelationScope RelationScope { get; }

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                long here = Potential.At(view, _origin);
                long carried = view.Read(new Cell(_origin, K4.Sediment)); // the snapshot's holding
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

                    long transfer = Math.Min(carried, diff / divisor);
                    if (transfer <= 0)
                    {
                        continue;
                    }

                    carried -= transfer;
                    contributions.Add(new Contribution(new Cell(_origin, K4.Sediment), -transfer));
                    contributions.Add(new Contribution(new Cell(target, K4.Sediment), transfer));
                }

                return contributions;
            }
        }
    }

    /// <summary>
    /// The spy resolver (measurement 13, local witness). It records every invocation — the kind
    /// it stands for, the amounts it was handed, and the delta it committed — then delegates to
    /// the additive resolver it wraps. The kernel is untouched: a resolver is world content, and
    /// this one merely keeps a diary. It cannot see the cell (the interface hands it amounts
    /// only); provenance comes from the separate per-fixture collection, and the two witnesses
    /// must agree.
    /// </summary>
    public sealed class SpyResolver : IConflictResolver
    {
        public sealed class Invocation
        {
            public Kind Kind { get; }
            public long[] Amounts { get; }
            public long Committed { get; }

            public Invocation(Kind kind, long[] amounts, long committed)
            {
                Kind = kind;
                Amounts = amounts;
                Committed = committed;
            }

            public override string ToString() =>
                $"kind({Kind.Value}) [{string.Join(",", Amounts)}] -> {Committed}";
        }

        private readonly Kind _kind;
        private readonly IConflictResolver _inner;
        private readonly List<Invocation> _invocations = new List<Invocation>();

        public SpyResolver(Kind kind, IConflictResolver inner)
        {
            _kind = kind;
            _inner = inner;
        }

        public IReadOnlyList<Invocation> Invocations => _invocations;

        public long Resolve(IReadOnlyList<long> amounts)
        {
            long committed = _inner.Resolve(amounts);
            var copy = new long[amounts.Count];
            for (int i = 0; i < amounts.Count; i++)
            {
                copy[i] = amounts[i];
            }

            _invocations.Add(new Invocation(_kind, copy, committed));
            return committed;
        }
    }
}
