using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.S1_004
{
    /// <summary>
    /// The strict pair of Campaign S1-004, built to the parameters sealed at `b114a34`:
    /// two identical three-place chains A ↔ B ↔ C differing by **exactly one fixture**.
    /// </summary>
    public sealed class Parcel4
    {
        public string Name { get; }
        public SimulationState Initial { get; }
        public RelationSet Relations { get; }
        public IReadOnlyList<IFixture> Fixtures { get; }
        public FixtureSet Set { get; }
        public ExternalEventTrace Crossings { get; }
        public IReadOnlyList<Place> Places { get; }
        public IReadOnlyList<SpyResolver> Spies { get; }
        public long Ticks { get; }

        public Parcel4(string name, SimulationState initial, RelationSet relations,
            IReadOnlyList<IFixture> fixtures, FixtureSet set, ExternalEventTrace crossings,
            IReadOnlyList<Place> places, IReadOnlyList<SpyResolver> spies, long ticks)
        {
            Name = name;
            Initial = initial;
            Relations = relations;
            Fixtures = fixtures;
            Set = set;
            Crossings = crossings;
            Places = places;
            Spies = spies;
            Ticks = ticks;
        }
    }

    public static class Worlds4
    {
        public const long Ticks = 6;
        public const long Threshold = 4;
        public const long RockAtEachPlace = 10;
        public const long CrossingAmount = 10;

        /// <summary>Constant divisor 2, for Water flow and for Sediment transport alike.</summary>
        public static readonly DivisorPolicy Constant2 = degree => 2;

        /// <summary>M0 — conversion only: Sediment may be made, but never moved.</summary>
        public static Parcel4 M0() => Build("M0", transport: false);

        /// <summary>M1 — conversion and transport: the same world, one fixture richer.</summary>
        public static Parcel4 M1() => Build("M1", transport: true);

        private static Parcel4 Build(string name, bool transport)
        {
            var places = new List<Place> { new Place(0), new Place(1), new Place(2) }; // A, B, C

            var cells = new Dictionary<Cell, long>();
            foreach (Place place in places)
            {
                cells[new Cell(place, K4.Base)] = 0;
                cells[new Cell(place, K4.Rock)] = RockAtEachPlace;
                cells[new Cell(place, K4.Sediment)] = 0;
                cells[new Cell(place, K4.Water)] = 0;
            }

            var initial = new SimulationState(Tick.Zero, cells);

            var relations = new RelationSet(initial,
                new Relation(places[0], places[1]), new Relation(places[1], places[0]),
                new Relation(places[1], places[2]), new Relation(places[2], places[1]));

            // One additive resolver, wrapped per kind so every invocation is witnessed.
            var additive = new AdditiveResolver();
            var spyWater = new SpyResolver(K4.Water, additive);
            var spyRock = new SpyResolver(K4.Rock, additive);
            var spySediment = new SpyResolver(K4.Sediment, additive);
            var spies = new List<SpyResolver> { spyWater, spyRock, spySediment };

            var fixtures = new List<IFixture>
            {
                new SurfaceFlowFixture(places, Constant2, spyWater),
                new SurfaceConversionFixture(places, Constant2, Threshold, spyRock, spySediment),
            };

            if (transport)
            {
                fixtures.Add(new SurfaceSedimentTransportFixture(places, Constant2, spySediment));
            }

            var set = new FixtureSet(fixtures.ToArray());

            // +10 Water at A on boundaries 0 and 1; silence on 2–5.
            var trace = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            trace.Append(new ExternalEvent(new Tick(0), new Cell(places[0], K4.Water), CrossingAmount));
            trace.Append(new ExternalEvent(new Tick(1), new Cell(places[0], K4.Water), CrossingAmount));

            return new Parcel4(name, initial, relations, fixtures, set, trace, places, spies, Ticks);
        }
    }
}
