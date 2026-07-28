using System;
using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// The sealed parcels of Campaign S1-002 — The First Flow (seal `4885d04`), built from the
    /// sealed text alone. Fixtures are S1-001's, unchanged (Fixture Transparency exercised for
    /// real). Point rain = +1 into kind Water at the declared place(s), boundaries 0–29; run
    /// length 30 ticks each. Built blind: no builder is ever adjusted because a calculation
    /// "seems strange" — comparison belongs to execution, never before.
    /// </summary>
    public static class WorldsS1002
    {
        public const long RainTicks = 30;
        public const long PlannedTicks = 30;

        /// <summary>V0 — stasis control: 2 cells, flat, rain on BOTH cells.</summary>
        public static Parcel V0()
        {
            return LineParcel("V0", cellCount: 2, elevation: _ => 0, Divisors.Naive, rainPlaces: new[] { 0, 1 });
        }

        /// <summary>V1 — threshold sweep: 2 cells, naive divisor, E = (d, 0), rain on the high cell.</summary>
        public static Parcel V1(int d)
        {
            return LineParcel($"V1-d{d}", cellCount: 2, elevation: i => i == 0 ? d : 0, Divisors.Naive, rainPlaces: new[] { 0 });
        }

        /// <summary>V2 — local-structure sweep: star, degree-aware divisor, centre E = 1, k leaves at 0, rain on the centre.</summary>
        public static Parcel V2(int k)
        {
            var places = new List<Place>();
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i <= k; i++)
            {
                var place = new Place(i);
                places.Add(place);
                cells[new Cell(place, K.Elevation)] = i == 0 ? 1 : 0;
                cells[new Cell(place, K.Water)] = 0;
            }

            var initial = new SimulationState(Tick.Zero, cells);
            var relations = new List<Relation>();
            for (int leaf = 1; leaf <= k; leaf++)
            {
                relations.Add(new Relation(places[0], places[leaf]));
                relations.Add(new Relation(places[leaf], places[0]));
            }

            var relationSet = new RelationSet(initial, relations.ToArray());
            var set = new FixtureSet(new FlowFixture(places, K.Water, Divisors.DegreeAware, new AdditiveResolver()));
            return new Parcel($"V2-k{k}", initial, relationSet, set, PointRain(places, new[] { 0 }), places, PlannedTicks);
        }

        /// <summary>V3 — the cascade: 5-cell chain, flat, naive divisor, rain on one end.</summary>
        public static Parcel V3()
        {
            return LineParcel("V3", cellCount: 5, elevation: _ => 0, Divisors.Naive, rainPlaces: new[] { 0 });
        }

        /// <summary>V4a — single path R–A1–M–X–Y: flat, naive, rain at R. (V3 relabelled — internal replication.)</summary>
        public static Parcel V4a()
        {
            return LineParcel("V4a", cellCount: 5, elevation: _ => 0, Divisors.Naive, rainPlaces: new[] { 0 });
        }

        /// <summary>
        /// V4b — forked path: R–A1–M and R–A2–M (diamond upstream), then M–X–Y; flat, naive,
        /// rain at R. Places: R=0, A1=1, A2=2, M=3, X=4, Y=5. Watched edge: X→Y (4→5).
        /// </summary>
        public static Parcel V4b()
        {
            var places = new List<Place>();
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i < 6; i++)
            {
                var place = new Place(i);
                places.Add(place);
                cells[new Cell(place, K.Elevation)] = 0;
                cells[new Cell(place, K.Water)] = 0;
            }

            var initial = new SimulationState(Tick.Zero, cells);
            var edges = new[] { (0, 1), (0, 2), (1, 3), (2, 3), (3, 4), (4, 5) };
            var relations = new List<Relation>();
            foreach ((int a, int b) in edges)
            {
                relations.Add(new Relation(places[a], places[b]));
                relations.Add(new Relation(places[b], places[a]));
            }

            var relationSet = new RelationSet(initial, relations.ToArray());
            var set = new FixtureSet(new FlowFixture(places, K.Water, Divisors.Naive, new AdditiveResolver()));
            return new Parcel("V4b", initial, relationSet, set, PointRain(places, new[] { 0 }), places, PlannedTicks);
        }

        public static IReadOnlyList<Parcel> All()
        {
            var parcels = new List<Parcel> { V0() };
            for (int d = 0; d <= 4; d++)
            {
                parcels.Add(V1(d));
            }

            for (int k = 1; k <= 3; k++)
            {
                parcels.Add(V2(k));
            }

            parcels.Add(V3());
            parcels.Add(V4a());
            parcels.Add(V4b());
            return parcels;
        }

        private static Parcel LineParcel(string name, int cellCount, Func<int, long> elevation,
            DivisorPolicy divisor, int[] rainPlaces)
        {
            var places = new List<Place>();
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i < cellCount; i++)
            {
                var place = new Place(i);
                places.Add(place);
                cells[new Cell(place, K.Elevation)] = elevation(i);
                cells[new Cell(place, K.Water)] = 0;
            }

            var initial = new SimulationState(Tick.Zero, cells);
            var relations = new List<Relation>();
            for (int i = 0; i + 1 < cellCount; i++)
            {
                relations.Add(new Relation(places[i], places[i + 1]));
                relations.Add(new Relation(places[i + 1], places[i]));
            }

            var relationSet = new RelationSet(initial, relations.ToArray());
            var set = new FixtureSet(new FlowFixture(places, K.Water, divisor, new AdditiveResolver()));
            return new Parcel(name, initial, relationSet, set, PointRain(places, rainPlaces), places, PlannedTicks);
        }

        private static ExternalEventTrace PointRain(IReadOnlyList<Place> places, int[] rainPlaces)
        {
            var membrane = new Membrane(new[] { K.Water });
            var trace = new ExternalEventTrace(membrane);
            for (long t = 0; t < RainTicks; t++)
            {
                foreach (int index in rainPlaces)
                {
                    trace.Append(new ExternalEvent(new Tick(t), new Cell(places[index], K.Water), 1));
                }
            }

            return trace;
        }
    }
}
