using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;
using Genesis.Lab.S1_005;

namespace Genesis.Lab.S1_006
{
    public sealed class Parcel6
    {
        public string Name { get; }
        public SimulationState Initial { get; }
        public RelationSet Relations { get; }
        public IReadOnlyList<IFixture> Fixtures { get; }
        public FixtureSet Set { get; }
        public ExternalEventTrace Crossings { get; }
        public IReadOnlyList<Place> Places { get; }
        public long Competence { get; }
        public long Ticks { get; }

        public Parcel6(string name, SimulationState initial, RelationSet relations,
            IReadOnlyList<IFixture> fixtures, FixtureSet set, ExternalEventTrace crossings,
            IReadOnlyList<Place> places, long competence, long ticks)
        {
            Name = name;
            Initial = initial;
            Relations = relations;
            Fixtures = fixtures;
            Set = set;
            Crossings = crossings;
            Places = places;
            Competence = competence;
            Ticks = ticks;
        }
    }

    /// <summary>
    /// The strict pair sealed at 0cc83b0. Construction and inspection are allowed.
    /// No method in this file advances either parcel by one tick.
    /// </summary>
    public static class Worlds6
    {
        public const long Ticks = 128;
        public const long RockAtEachPlace = 12;
        public const long ConversionThreshold = 5;
        public const long CrossingAmount = 12;

        public static Parcel6 P0() => Build("P0", 0);
        public static Parcel6 P1() => Build("P1", 1);

        private static Parcel6 Build(string name, long competence)
        {
            var places = new List<Place>
                { new Place(0), new Place(1), new Place(2), new Place(3) };
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
                new Relation(places[1], places[2]), new Relation(places[2], places[1]),
                new Relation(places[2], places[3]), new Relation(places[3], places[2]));

            var additive = new AdditiveResolver();
            var fixtures = new List<IFixture>
            {
                new SurfaceFlowFixture(places, Divisors.DegreeAware, additive),
                new SurfaceConversionFixture(places, Divisors.DegreeAware,
                    ConversionThreshold, additive, additive),
                new CompetenceTransportFixture(places, Divisors.DegreeAware,
                    competence, additive),
            };

            var crossings = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            foreach (long boundary in new long[] { 0, 1, 10, 11 })
            {
                crossings.Append(new ExternalEvent(new Tick(boundary),
                    new Cell(places[0], K4.Water), CrossingAmount));
            }

            return new Parcel6(name, initial, relations, fixtures,
                new FixtureSet(fixtures.ToArray()), crossings, places, competence, Ticks);
        }
    }
}
