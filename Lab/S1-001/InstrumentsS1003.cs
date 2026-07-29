using System;
using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>Pure readers required by S1-003. They know records and geometry, never campaigns.</summary>
    public static class S1003Instruments
    {
        public sealed class MirrorDifference
        {
            public long Tick { get; }
            public int Row { get; }
            public int Distance { get; }
            public long SignedDifference { get; }

            public MirrorDifference(long tick, int row, int distance, long signedDifference)
            {
                Tick = tick;
                Row = row;
                Distance = distance;
                SignedDifference = signedDifference;
            }
        }

        public sealed class WorldDifference
        {
            public long Tick { get; }
            public Place Place { get; }
            public long FirstValue { get; }
            public long SecondValue { get; }

            public WorldDifference(long tick, Place place, long firstValue, long secondValue)
            {
                Tick = tick;
                Place = place;
                FirstValue = firstValue;
                SecondValue = secondValue;
            }
        }

        public readonly struct RegionTotals
        {
            public long Left { get; }
            public long Centre { get; }
            public long Right { get; }
            public long Full { get; }

            public RegionTotals(long left, long centre, long right)
            {
                Left = left;
                Centre = centre;
                Right = right;
                Full = left + centre + right;
            }
        }

        public sealed class Extreme
        {
            public long Value { get; }
            public long Tick { get; }
            public Place Place { get; }

            public Extreme(long value, long tick, Place place)
            {
                Value = value;
                Tick = tick;
                Place = place;
            }
        }

        public static IReadOnlyList<MirrorDifference> MirrorDifferences(
            IReadOnlyList<SimulationState> states, int width, int height, Kind kind)
        {
            ValidateOddGrid(width, height);
            var result = new List<MirrorDifference>();
            int centre = width / 2;
            foreach (SimulationState state in states)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int distance = 1; distance <= centre; distance++)
                    {
                        var left = new Place(row * width + centre - distance);
                        var right = new Place(row * width + centre + distance);
                        long signed = state.ValueAt(new Cell(left, kind)) - state.ValueAt(new Cell(right, kind));
                        result.Add(new MirrorDifference(state.CurrentTick.Value, row, distance, signed));
                    }
                }
            }
            return result;
        }

        public static WorldDifference FirstWorldDifference(
            IReadOnlyList<SimulationState> first, IReadOnlyList<SimulationState> second, Kind kind)
        {
            if (first.Count != second.Count)
            {
                throw new ArgumentException("world records must contain the same number of states");
            }

            for (int tick = 0; tick < first.Count; tick++)
            {
                if (first[tick].CurrentTick != second[tick].CurrentTick)
                {
                    throw new ArgumentException("world records must align on tick");
                }

                var places = new SortedSet<int>();
                foreach (Cell cell in first[tick].Cells)
                {
                    if (cell.Kind == kind) places.Add(cell.Place.Value);
                }
                foreach (Cell cell in second[tick].Cells)
                {
                    if (cell.Kind == kind) places.Add(cell.Place.Value);
                }

                foreach (int id in places)
                {
                    var place = new Place(id);
                    long a = first[tick].ValueAt(new Cell(place, kind));
                    long b = second[tick].ValueAt(new Cell(place, kind));
                    if (a != b)
                    {
                        return new WorldDifference(first[tick].CurrentTick.Value, place, a, b);
                    }
                }
            }
            return null;
        }

        public static RegionTotals Totals(SimulationState state, int width, int height, Kind kind)
        {
            ValidateOddGrid(width, height);
            long left = 0, centre = 0, right = 0;
            int centreColumn = width / 2;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    long value = state.ValueAt(new Cell(new Place(row * width + col), kind));
                    if (col < centreColumn) left += value;
                    else if (col == centreColumn) centre += value;
                    else right += value;
                }
            }
            return new RegionTotals(left, centre, right);
        }

        public static IReadOnlyDictionary<Place, long> FirstWetTicks(
            IReadOnlyList<SimulationState> states, IReadOnlyList<Place> places, Kind kind)
        {
            var result = new Dictionary<Place, long>();
            foreach (SimulationState state in states)
            {
                foreach (Place place in places)
                {
                    if (!result.ContainsKey(place) && state.ValueAt(new Cell(place, kind)) > 0)
                    {
                        result[place] = state.CurrentTick.Value;
                    }
                }
            }
            return result;
        }

        public static (long magnitude, long tick) MaximumRegionImbalance(
            IReadOnlyList<SimulationState> states, int width, int height, Kind kind)
        {
            long maximum = -1;
            long atTick = -1;
            foreach (SimulationState state in states)
            {
                RegionTotals totals = Totals(state, width, height, kind);
                long magnitude = Math.Abs(totals.Left - totals.Right);
                if (magnitude > maximum)
                {
                    maximum = magnitude;
                    atTick = state.CurrentTick.Value;
                }
            }
            return (maximum, atTick);
        }

        public static Extreme MostNegative(
            IReadOnlyList<SimulationState> states, IReadOnlyList<Place> places, Kind kind)
        {
            Extreme result = null;
            foreach (SimulationState state in states)
            {
                foreach (Place place in places)
                {
                    long value = state.ValueAt(new Cell(place, kind));
                    if (result == null || value < result.Value)
                    {
                        result = new Extreme(value, state.CurrentTick.Value, place);
                    }
                }
            }
            return result;
        }

        private static void ValidateOddGrid(int width, int height)
        {
            if (width <= 0 || height <= 0 || width % 2 == 0)
            {
                throw new ArgumentException("mirror readings require a positive, odd-width grid");
            }
        }
    }
}
