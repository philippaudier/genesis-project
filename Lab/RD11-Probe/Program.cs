using System;
using System.Collections.Generic;
using System.Text;
using Genesis.Simulation;
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
            return 0;
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
