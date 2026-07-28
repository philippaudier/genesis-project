using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.Demo001
{
    /// <summary>
    /// Demo-001's world. Belongs to no campaign; nothing here is evidence (see README).
    ///
    /// The terrain is built USING what S1-002 confirmed rather than hoping: an edge transports
    /// iff the potential difference reaches the local divisor. Every elevation step is kept at
    /// or below 4, strictly under the demo's constant divisor of 5 — so dry ground is inert by
    /// construction (Obs-011's stasis, on purpose), and water is the only thing that can move.
    /// </summary>
    public static class DemoWorld
    {
        public const int Width = 32;
        public const int Height = 32;
        public const int BasinRows = 8;
        public const long Divisor = 5;
        public const long MaxAllowedStep = 4;

        public const long RainTicks = 400;
        public const long TotalTicks = 550;

        /// <summary>Rain falls on every boundary — a slope of 4 carries one unit per tick.</summary>
        public const long RainPeriod = 1;

        /// <summary>Constant divisor: equals degree-aware on interior cells; removes the corner artifact.</summary>
        public static readonly DivisorPolicy Constant5 = degree => Divisor;

        public static long ElevationAt(int row, int col)
        {
            // A slope of 4 per row — the largest step the divisor still refuses on dry ground —
            // running into a flat basin, and three shallow troughs corrugating the whole world
            // at constant amplitude (a fading amplitude would add to the row step and break the
            // premise). The troughs continue into the basin, so the basin is not one floor but
            // three.
            int landRows = Height - BasinRows;
            double land = row < landRows ? 4.0 * (landRows - row) : 0.0;
            double wave = 6.0 * Math.Sin(2.0 * Math.PI * 3.0 * (col + 0.5) / Width);
            return (long)Math.Round(land + wave, MidpointRounding.AwayFromZero);
        }

        public static IReadOnlyList<Place> Places()
        {
            var places = new List<Place>(Width * Height);
            for (int i = 0; i < Width * Height; i++)
            {
                places.Add(new Place(i));
            }

            return places;
        }

        public static SimulationState InitialState()
        {
            var cells = new Dictionary<Cell, long>();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    var place = new Place(row * Width + col);
                    cells[new Cell(place, K.Elevation)] = ElevationAt(row, col);
                    cells[new Cell(place, K.Water)] = 0;
                }
            }

            return new SimulationState(Tick.Zero, cells);
        }

        public static RelationSet Relations(SimulationState state)
        {
            var relations = new List<Relation>();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    var here = new Place(row * Width + col);
                    if (col + 1 < Width)
                    {
                        var right = new Place(row * Width + col + 1);
                        relations.Add(new Relation(here, right));
                        relations.Add(new Relation(right, here));
                    }

                    if (row + 1 < Height)
                    {
                        var below = new Place((row + 1) * Width + col);
                        relations.Add(new Relation(here, below));
                        relations.Add(new Relation(below, here));
                    }
                }
            }

            return new RelationSet(state, relations.ToArray());
        }

        /// <summary>Rain on the top row only: +1 per cell per boundary, then the sky closes.</summary>
        public static ExternalEventTrace Rain(IReadOnlyList<Place> places)
        {
            var trace = new ExternalEventTrace(new Membrane(new[] { K.Water }));
            for (long t = 0; t < RainTicks; t += RainPeriod)
            {
                for (int col = 0; col < Width; col++)
                {
                    trace.Append(new ExternalEvent(new Tick(t), new Cell(places[col], K.Water), 1));
                }
            }

            return trace;
        }

        /// <summary>
        /// The construction guard: the largest elevation step between neighbours. If this ever
        /// reaches the divisor, dry ground would transport and the demo's premise would be false.
        /// </summary>
        public static long MaxElevationStep()
        {
            long max = 0;
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    long here = ElevationAt(row, col);
                    if (col + 1 < Width)
                    {
                        max = Math.Max(max, Math.Abs(here - ElevationAt(row, col + 1)));
                    }

                    if (row + 1 < Height)
                    {
                        max = Math.Max(max, Math.Abs(here - ElevationAt(row + 1, col)));
                    }
                }
            }

            return max;
        }
    }
}
