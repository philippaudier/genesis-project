using System;
using System.Collections.Generic;
using System.Text;
using Genesis.Simulation;
using Genesis.Lab.S1_004;

namespace Genesis.Lab.S1_005
{
    public static class StateInstrument
    {
        public sealed class Repeat
        {
            public int First { get; }
            public int Again { get; }
            public int Period => Again - First;

            public Repeat(int first, int again)
            {
                First = first;
                Again = again;
            }
        }

        public static string CompleteSignature(SimulationState state,
            IReadOnlyList<Place> places)
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

        public static string SurfaceSignature(SimulationState state,
            IReadOnlyList<Place> places)
        {
            var text = new StringBuilder("[");
            for (int i = 0; i < places.Count; i++)
            {
                if (i > 0) text.Append(',');
                text.Append(K4.SolidSurface(state, places[i]));
            }

            return text.Append(']').ToString();
        }

        public static Repeat FirstRepeat(IReadOnlyList<SimulationState> states,
            IReadOnlyList<Place> places)
        {
            var seen = new Dictionary<string, int>();
            for (int tick = 0; tick < states.Count; tick++)
            {
                string signature = CompleteSignature(states[tick], places);
                if (seen.TryGetValue(signature, out int first))
                {
                    return new Repeat(first, tick);
                }

                seen.Add(signature, tick);
            }

            return null;
        }

        public static long Minimum(SimulationState state, IReadOnlyList<Place> places)
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

        private static void Append(StringBuilder text, SimulationState state,
            Place place, Kind kind)
        {
            text.Append(state.ValueAt(new Cell(place, kind))).Append(',');
        }
    }
}

