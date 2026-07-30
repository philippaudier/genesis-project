using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;

namespace Genesis.Lab.S1_005
{
    public sealed class Parcel5
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

        public Parcel5(string name, SimulationState initial, RelationSet relations,
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
    /// The strict pair sealed at 7b739b4. Construction is allowed; execution is not.
    /// </summary>
    public static class Worlds5
    {
        public const long Ticks = 8;
        public const long RockAtEachPlace = 8;
        public const long ConversionThreshold = 3;
        public const long CrossingAmount = 8;
        public static readonly DivisorPolicy Constant2 = degree => 2;

        public static Parcel5 N0() => Build("N0", competence: 0);
        public static Parcel5 N1() => Build("N1", competence: 1);

        private static Parcel5 Build(string name, long competence)
        {
            var places = new List<Place> { new Place(0), new Place(1) };
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
                new Relation(places[0], places[1]),
                new Relation(places[1], places[0]));

            var additive = new AdditiveResolver();
            var fixtures = new List<IFixture>
            {
                new SurfaceFlowFixture(places, Constant2, additive),
                new SurfaceConversionFixture(places, Constant2, ConversionThreshold,
                    additive, additive),
                new CompetenceTransportFixture(places, Constant2, competence, additive),
            };

            var crossings = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            crossings.Append(new ExternalEvent(new Tick(0),
                new Cell(places[0], K4.Water), CrossingAmount));
            crossings.Append(new ExternalEvent(new Tick(1),
                new Cell(places[0], K4.Water), CrossingAmount));

            return new Parcel5(name, initial, relations, fixtures,
                new FixtureSet(fixtures.ToArray()), crossings, places, competence, Ticks);
        }
    }
}

