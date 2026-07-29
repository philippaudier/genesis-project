using System;
using System.Collections.Generic;
using System.Text;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;

namespace Genesis.Lab.RD11_Probe
{
    /// <summary>
    /// An explicitly contaminated extension of S1-004/M1.
    /// This is reconnaissance for RD-11, never campaign evidence.
    /// </summary>
    public static class Program
    {
        private const int Horizon = 500;

        private sealed class Repeat
        {
            public int First;
            public int Again;
            public int Period => Again - First;
        }

        private sealed class ProbeParcel
        {
            public SimulationState Initial;
            public RelationSet Relations;
            public FixtureSet Set;
            public ExternalEventTrace Crossings;
            public IReadOnlyList<Place> Places;
        }

        public static int Main()
        {
            Parcel4 parcel = Worlds4.M1();
            var runner = new TickRunner(new TransitionRunner(parcel.Set.Resolvers));
            SimulationState state = parcel.Initial;

            var surfacesSeen = new Dictionary<string, int>();
            var statesSeen = new Dictionary<string, int>();
            var surfaceByTick = new List<string>();
            var fullByTick = new List<string>();
            var surfaceTail = new Queue<string>();
            Repeat surfaceRepeat = null;
            Repeat stateRepeat = null;
            long initialMatter = Matter(state, parcel.Places);
            long minimum = Minimum(state, parcel.Places);
            string initialSurface = SurfaceSignature(state, parcel.Places);
            int firstSurfaceChange = -1;

            Console.WriteLine("RD-11 PROBE — CONTAMINATED, NON-EVIDENTIAL");
            Console.WriteLine("Extending the already-known S1-004/M1 parcel.");
            Console.WriteLine();

            for (int tick = 0; tick <= Horizon; tick++)
            {
                string surface = SurfaceSignature(state, parcel.Places);
                string full = FullSignature(state, parcel.Places);
                surfaceByTick.Add(surface);
                fullByTick.Add(full);
                Remember(surfaceTail, $"t{tick} {surface}", 16);

                if (firstSurfaceChange < 0 && surface != initialSurface)
                {
                    firstSurfaceChange = tick;
                    surfacesSeen.Clear();
                }

                // Ignore the unchanged prefix. It is a repeated reading, but says nothing about
                // what happens after a relief has actually appeared.
                if (firstSurfaceChange >= 0 && surfaceRepeat == null &&
                    surfacesSeen.TryGetValue(surface, out int firstSurfaceTick))
                {
                    surfaceRepeat = new Repeat { First = firstSurfaceTick, Again = tick };
                }
                else if (firstSurfaceChange >= 0 && !surfacesSeen.ContainsKey(surface))
                {
                    surfacesSeen.Add(surface, tick);
                }

                if (stateRepeat == null)
                {
                    if (statesSeen.TryGetValue(full, out int first))
                    {
                        stateRepeat = new Repeat { First = first, Again = tick };
                    }
                    else
                    {
                        statesSeen.Add(full, tick);
                    }
                }

                minimum = Math.Min(minimum, Minimum(state, parcel.Places));
                if (tick < Horizon)
                {
                    state = runner.Run(state, parcel.Relations, parcel.Set.Transitions,
                        parcel.Crossings, 1);
                }
            }

            Console.WriteLine($"Horizon: {Horizon} ticks");
            Console.WriteLine($"First changed surface: tick {firstSurfaceChange}, {surfaceByTick[firstSurfaceChange]}");
            Console.WriteLine($"First repeated surface after change: {Describe(surfaceRepeat)}");
            Console.WriteLine($"First repeated full material state: {Describe(stateRepeat)}");
            if (stateRepeat != null)
            {
                Console.WriteLine("Material cycle surfaces:");
                for (int tick = stateRepeat.First; tick < stateRepeat.Again; tick++)
                {
                    Console.WriteLine($"  t{tick} {surfaceByTick[tick]} | {fullByTick[tick]}");
                }
            }
            Console.WriteLine($"Rock + Sediment: {initialMatter} -> {Matter(state, parcel.Places)}");
            Console.WriteLine($"Minimum value across all four kinds: {minimum}");
            Console.WriteLine();
            Console.WriteLine("Last 16 surface readings:");
            foreach (string line in surfaceTail)
            {
                Console.WriteLine($"  {line}");
            }

            Console.WriteLine();
            Console.WriteLine("This output asks where to look next. It proves nothing.");

            Console.WriteLine();
            Console.WriteLine("EXPLORATORY COMPETENCE SWEEP — SAME CONTAMINATED PARCEL");
            Console.WriteLine("threshold | first full-state recurrence | cycle surface(s) | matter | min | classification");
            for (long threshold = 0; threshold <= 6; threshold++)
            {
                Console.WriteLine(ExploreCompetence(threshold));
            }

            return 0;
        }

        private static string ExploreCompetence(long threshold)
        {
            ProbeParcel parcel = BuildCompetenceParcel(threshold);
            var runner = new TickRunner(new TransitionRunner(parcel.Set.Resolvers));
            SimulationState state = parcel.Initial;
            long initialMatter = Matter(state, parcel.Places);
            long minimum = Minimum(state, parcel.Places);
            var seen = new Dictionary<string, int>();
            var surfaces = new List<string>();
            Repeat repeat = null;

            for (int tick = 0; tick <= Horizon; tick++)
            {
                string full = FullSignature(state, parcel.Places);
                surfaces.Add(SurfaceSignature(state, parcel.Places));
                minimum = Math.Min(minimum, Minimum(state, parcel.Places));
                if (seen.TryGetValue(full, out int first))
                {
                    repeat = new Repeat { First = first, Again = tick };
                    break;
                }

                seen.Add(full, tick);
                if (tick < Horizon)
                {
                    state = runner.Run(state, parcel.Relations, parcel.Set.Transitions,
                        parcel.Crossings, 1);
                }
            }

            if (repeat == null)
            {
                return $"{threshold,9} | none through {Horizon,-9} | {surfaces[surfaces.Count - 1],-20} | " +
                    $"{initialMatter}->{Matter(state, parcel.Places),-2} | {minimum,3} | unresolved";
            }

            var cycle = new List<string>();
            for (int tick = repeat.First; tick < repeat.Again; tick++)
            {
                if (!cycle.Contains(surfaces[tick]))
                {
                    cycle.Add(surfaces[tick]);
                }
            }

            string classification;
            if (repeat.Period != 1)
            {
                classification = $"material orbit p{repeat.Period}";
            }
            else
            {
                classification = IsFlat(cycle[0]) ? "flat fixed point" : "non-uniform fixed point";
            }

            string recurrence = $"t{repeat.First}->t{repeat.Again} (p{repeat.Period})";
            string cycleSurfaces = string.Join(" / ", cycle);
            string matter = $"{initialMatter}->{Matter(state, parcel.Places)}";
            return $"{threshold,9} | {recurrence,-27} | {cycleSurfaces,-23} | {matter,-6} | {minimum,3} | {classification}";
        }

        private static ProbeParcel BuildCompetenceParcel(long threshold)
        {
            var places = new List<Place> { new Place(0), new Place(1), new Place(2) };
            var cells = new Dictionary<Cell, long>();
            foreach (Place place in places)
            {
                cells[new Cell(place, K4.Base)] = 0;
                cells[new Cell(place, K4.Rock)] = Worlds4.RockAtEachPlace;
                cells[new Cell(place, K4.Sediment)] = 0;
                cells[new Cell(place, K4.Water)] = 0;
            }

            var initial = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(initial,
                new Relation(places[0], places[1]), new Relation(places[1], places[0]),
                new Relation(places[1], places[2]), new Relation(places[2], places[1]));

            var additive = new AdditiveResolver();
            var fixtures = new List<IFixture>
            {
                new SurfaceFlowFixture(places, Worlds4.Constant2, additive),
                new SurfaceConversionFixture(places, Worlds4.Constant2, Worlds4.Threshold,
                    additive, additive),
                new CompetenceTransportFixture(places, Worlds4.Constant2, threshold, additive),
            };

            var crossings = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            crossings.Append(new ExternalEvent(new Tick(0), new Cell(places[0], K4.Water),
                Worlds4.CrossingAmount));
            crossings.Append(new ExternalEvent(new Tick(1), new Cell(places[0], K4.Water),
                Worlds4.CrossingAmount));

            return new ProbeParcel
            {
                Initial = initial,
                Relations = relations,
                Set = new FixtureSet(fixtures.ToArray()),
                Crossings = crossings,
                Places = places,
            };
        }

        private static bool IsFlat(string surface)
        {
            string[] values = surface.Trim('[', ']').Split(',');
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] != values[0])
                {
                    return false;
                }
            }

            return true;
        }

        private static string Describe(Repeat repeat)
        {
            return repeat == null
                ? $"none through tick {Horizon}"
                : $"tick {repeat.First} again at tick {repeat.Again} (first-return distance {repeat.Period})";
        }

        private static void Remember(Queue<string> queue, string value, int capacity)
        {
            queue.Enqueue(value);
            while (queue.Count > capacity)
            {
                queue.Dequeue();
            }
        }

        private static string SurfaceSignature(SimulationState state, IReadOnlyList<Place> places)
        {
            var text = new StringBuilder("[");
            for (int i = 0; i < places.Count; i++)
            {
                if (i > 0) text.Append(',');
                text.Append(K4.SolidSurface(state, places[i]));
            }

            return text.Append(']').ToString();
        }

        private static string FullSignature(SimulationState state, IReadOnlyList<Place> places)
        {
            var text = new StringBuilder();
            foreach (Place place in places)
            {
                Append(text, state, place, K4.Base);
                Append(text, state, place, K4.Rock);
                Append(text, state, place, K4.Sediment);
                Append(text, state, place, K4.Water);
            }

            return text.ToString();
        }

        private static void Append(StringBuilder text, SimulationState state, Place place, Kind kind)
        {
            text.Append(state.ValueAt(new Cell(place, kind))).Append(',');
        }

        private static long Matter(SimulationState state, IReadOnlyList<Place> places)
        {
            long total = 0;
            foreach (Place place in places)
            {
                total += state.ValueAt(new Cell(place, K4.Rock));
                total += state.ValueAt(new Cell(place, K4.Sediment));
            }

            return total;
        }

        private static long Minimum(SimulationState state, IReadOnlyList<Place> places)
        {
            long minimum = long.MaxValue;
            foreach (Place place in places)
            {
                minimum = Math.Min(minimum, state.ValueAt(new Cell(place, K4.Base)));
                minimum = Math.Min(minimum, state.ValueAt(new Cell(place, K4.Rock)));
                minimum = Math.Min(minimum, state.ValueAt(new Cell(place, K4.Sediment)));
                minimum = Math.Min(minimum, state.ValueAt(new Cell(place, K4.Water)));
            }

            return minimum;
        }
    }
}
