using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Presentation
{
    /// <summary>
    /// The demonstration world the laboratory observes: the Genesis-012 witness — three places in a
    /// symmetric chain, a transported quantity, and a production rate that beats at one place. This
    /// is world *content*, defined presentation-side by the bootstrapper (per
    /// docs/Engineering/unity.md): the bootstrapper constructs the world and decides when it ticks;
    /// it never decides what a tick does.
    ///
    /// The two law fixtures below are duplicated from the Genesis-011/012 test fixtures. A shared
    /// home for world content is deliberately deferred — per the Discovery-era motto, it will be
    /// added when a world makes it inevitable, not because it seems useful.
    /// </summary>
    public sealed class DemoWorldDefinition
    {
        public SimulationState InitialState;
        public RelationSet Relations;
        public IReadOnlyList<ITransition> Laws;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers;
        public IReadOnlyList<Place> Places;
        public Kind QuantityKind;
        public Kind RateKind;

        /// <summary>Demo-world kind labels — world metadata, not engine vocabulary.</summary>
        public string KindName(Kind kind)
        {
            if (kind == QuantityKind)
            {
                return "Quantity";
            }

            if (kind == RateKind)
            {
                return "Rate";
            }

            return kind.ToString();
        }
    }

    public static class DemoWorld
    {
        public static DemoWorldDefinition Build()
        {
            var a = new Place(1);
            var b = new Place(2);
            var c = new Place(3);
            var quantity = new Kind(1);
            var rate = new Kind(2);
            var places = new[] { a, b, c };

            var cells = new Dictionary<Cell, long>();
            foreach (Place place in places)
            {
                cells[new Cell(place, quantity)] = 0;
                cells[new Cell(place, rate)] = 0;
            }

            cells[new Cell(a, rate)] = 2; // the standing cause beats here

            var state = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(
                state,
                new Relation(a, b), new Relation(b, a),
                new Relation(b, c), new Relation(c, b));

            var laws = new List<ITransition>();
            foreach (Place place in places)
            {
                laws.Add(new ProductionTransition(place, quantity, rate));
                laws.Add(new RedistributionTransition(place, quantity));
            }

            return new DemoWorldDefinition
            {
                InitialState = state,
                Relations = relations,
                Laws = laws,
                Resolvers = new Dictionary<Kind, IConflictResolver>
                {
                    { quantity, new AdditionResolver() }
                },
                Places = places,
                QuantityKind = quantity,
                RateKind = rate
            };
        }

        /// <summary>Universal production law: inert wherever the declared rate is zero.</summary>
        private sealed class ProductionTransition : ITransition
        {
            private readonly Cell _quantityCell;
            private readonly Cell _rateCell;
            private readonly ReadScope _readScope;

            public ProductionTransition(Place self, Kind quantity, Kind rate)
            {
                _quantityCell = new Cell(self, quantity);
                _rateCell = new Cell(self, rate);
                _readScope = new ReadScope(_rateCell);
            }

            public ReadScope ReadScope => _readScope;

            public RelationScope RelationScope => RelationScope.Empty;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                long rate = view.Read(_rateCell);
                if (rate <= 0)
                {
                    return new Contribution[0];
                }

                return new[] { new Contribution(_quantityCell, rate) };
            }
        }

        /// <summary>Redistribution law: paired downhill flux, q = (self − neighbour) / 2.</summary>
        private sealed class RedistributionTransition : ITransition
        {
            private readonly Place _self;
            private readonly Kind _kind;
            private readonly Cell _selfCell;
            private readonly ReadScope _readScope;
            private readonly RelationScope _relationScope;

            public RedistributionTransition(Place self, Kind kind)
            {
                _self = self;
                _kind = kind;
                _selfCell = new Cell(self, kind);
                _readScope = new ReadScope(_selfCell);
                _relationScope = new RelationScope(new[] { self }, new[] { kind });
            }

            public ReadScope ReadScope => _readScope;

            public RelationScope RelationScope => _relationScope;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                long self = view.Read(_selfCell);
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_self);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count; i++)
                {
                    var targetCell = new Cell(outgoing[i].Target, _kind);
                    long neighbour = view.Read(targetCell);

                    if (self > neighbour)
                    {
                        long flux = (self - neighbour) / 2;
                        if (flux > 0)
                        {
                            contributions.Add(new Contribution(_selfCell, -flux));
                            contributions.Add(new Contribution(targetCell, flux));
                        }
                    }
                }

                return contributions;
            }
        }
    }
}
