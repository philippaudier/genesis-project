using System;
using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// One experimental parcel, fully assembled: everything a run needs, everything a replay needs.
    /// The harness drives a Parcel without knowing which world or which fixtures it holds.
    /// </summary>
    public sealed class Parcel
    {
        public string Name { get; }
        public SimulationState Initial { get; }
        public RelationSet Relations { get; }
        public FixtureSet Fixtures { get; }
        public ExternalEventTrace Trace { get; }
        public IReadOnlyList<Place> Places { get; }
        public long PlannedTicks { get; }

        public Parcel(string name, SimulationState initial, RelationSet relations, FixtureSet fixtures,
            ExternalEventTrace trace, IReadOnlyList<Place> places, long plannedTicks)
        {
            Name = name;
            Initial = initial;
            Relations = relations;
            Fixtures = fixtures;
            Trace = trace;
            Places = places;
            PlannedTicks = plannedTicks;
        }
    }

    /// <summary>
    /// The five sealed parcels of Campaign S1-001, parameters exactly as bound at seal `7a4814e`.
    /// Rain: uniform +1 Water at every place, boundaries 0–19 inclusive, then the world is closed.
    /// </summary>
    public static class Worlds
    {
        public const long RainTicks = 20;

        public static Parcel W0FlatControl()
        {
            return BuildGridParcel("W0", 9, 9, (r, c) => 0, Divisors.Naive, plannedTicks: 200);
        }

        public static Parcel W1NaiveBowl()
        {
            return BuildGridParcel("W1", 9, 9, BowlElevation, Divisors.Naive, plannedTicks: 120);
        }

        public static Parcel W2CorrectedBowl()
        {
            return BuildGridParcel("W2", 9, 9, BowlElevation, Divisors.DegreeAware, plannedTicks: 200);
        }

        public static Parcel W3TwinFloors()
        {
            return BuildGridParcel("W3", 15, 9, TwinFloorsElevation, Divisors.DegreeAware, plannedTicks: 300);
        }

        /// <summary>W4: 9×3 inclined channel; Rock 100 per place; conversion threshold 4.</summary>
        public static Parcel W4Conversion(bool withSacrificial = false)
        {
            const int width = 3;
            const int height = 9;
            IReadOnlyList<Place> places = GridPlaces(width, height);
            var cells = new Dictionary<Cell, long>();
            foreach (Place place in places)
            {
                int row = place.Value / width;
                cells[new Cell(place, K.Elevation)] = 8 - row;
                cells[new Cell(place, K.Water)] = 0;
                cells[new Cell(place, K.Rock)] = 100;
                cells[new Cell(place, K.Sediment)] = 0;
                if (withSacrificial)
                {
                    cells[new Cell(place, K.Sacrificial)] = 10;
                }
            }

            var initial = new SimulationState(Tick.Zero, cells);
            RelationSet relations = GridRelations(initial, width, height);

            var additive = new AdditiveResolver();
            var fixtures = new List<IFixture>
            {
                new FlowFixture(places, K.Water, Divisors.DegreeAware, additive),
                new ConversionFixture(places, Divisors.DegreeAware, threshold: 4, additive),
                new SedimentTransportFixture(places, Divisors.DegreeAware, additive),
            };
            if (withSacrificial)
            {
                fixtures.Add(new SacrificialFixture(places[0], places[1]));
            }

            var set = new FixtureSet(fixtures.ToArray());
            ExternalEventTrace trace = UniformRain(places);
            return new Parcel(withSacrificial ? "W4-B3" : "W4", initial, relations, set, trace, places, plannedTicks: 150);
        }

        public static IReadOnlyList<Parcel> All()
        {
            return new[] { W0FlatControl(), W1NaiveBowl(), W2CorrectedBowl(), W3TwinFloors(), W4Conversion(), W4Conversion(withSacrificial: true) };
        }

        private static long BowlElevation(int row, int col)
        {
            return Math.Max(Math.Abs(row - 4), Math.Abs(col - 4));
        }

        /// <summary>Two depressions (floors at (4,3) and (4,11)) split by a ridge of 5 on column 7.</summary>
        private static long TwinFloorsElevation(int row, int col)
        {
            if (col == 7)
            {
                return 5;
            }

            long left = Math.Max(Math.Abs(row - 4), Math.Abs(col - 3));
            long right = Math.Max(Math.Abs(row - 4), Math.Abs(col - 11));
            return Math.Min(4, Math.Min(left, right));
        }

        private static Parcel BuildGridParcel(string name, int width, int height, Func<int, int, long> elevation,
            DivisorPolicy divisor, long plannedTicks)
        {
            IReadOnlyList<Place> places = GridPlaces(width, height);
            var cells = new Dictionary<Cell, long>();
            foreach (Place place in places)
            {
                int row = place.Value / width;
                int col = place.Value % width;
                cells[new Cell(place, K.Elevation)] = elevation(row, col);
                cells[new Cell(place, K.Water)] = 0;
            }

            var initial = new SimulationState(Tick.Zero, cells);
            RelationSet relations = GridRelations(initial, width, height);
            var set = new FixtureSet(new FlowFixture(places, K.Water, divisor, new AdditiveResolver()));
            ExternalEventTrace trace = UniformRain(places);
            return new Parcel(name, initial, relations, set, trace, places, plannedTicks);
        }

        private static IReadOnlyList<Place> GridPlaces(int width, int height)
        {
            var places = new List<Place>(width * height);
            for (int i = 0; i < width * height; i++)
            {
                places.Add(new Place(i));
            }

            return places;
        }

        /// <summary>4-neighbour grid, both directions declared (relations are directed).</summary>
        private static RelationSet GridRelations(SimulationState state, int width, int height)
        {
            var relations = new List<Relation>();
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var here = new Place(row * width + col);
                    if (col + 1 < width)
                    {
                        var right = new Place(row * width + col + 1);
                        relations.Add(new Relation(here, right));
                        relations.Add(new Relation(right, here));
                    }

                    if (row + 1 < height)
                    {
                        var below = new Place((row + 1) * width + col);
                        relations.Add(new Relation(here, below));
                        relations.Add(new Relation(below, here));
                    }
                }
            }

            return new RelationSet(state, relations.ToArray());
        }

        private static ExternalEventTrace UniformRain(IReadOnlyList<Place> places)
        {
            var membrane = new Membrane(new[] { K.Water });
            var trace = new ExternalEventTrace(membrane);
            for (long t = 0; t < RainTicks; t++)
            {
                foreach (Place place in places)
                {
                    trace.Append(new ExternalEvent(new Tick(t), new Cell(place, K.Water), 1));
                }
            }

            return trace;
        }
    }
}
