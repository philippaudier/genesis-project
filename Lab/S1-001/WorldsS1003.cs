using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// The sealed but unexecuted S1-003 pair. Construction is inspectable; only a future,
    /// separately authorised execution driver may pass either parcel to ParcelRun.Execute.
    /// </summary>
    public static class WorldsS1003
    {
        public const int Width = 7;
        public const int Height = 7;
        public const long RainTicks = 60;
        public const long PlannedTicks = 120;
        public static readonly Place Source = new Place(3);
        public static readonly Place Perturbation = new Place(3 * Width + 2);

        private static readonly long[,] U0Elevation =
        {
            { 20, 18, 17, 20, 17, 18, 20 },
            { 17, 15, 14, 17, 14, 15, 17 },
            { 14, 12, 11, 14, 11, 12, 14 },
            { 11,  9,  8, 11,  8,  9, 11 },
            {  8,  6,  5,  8,  5,  6,  8 },
            {  5,  3,  2,  5,  2,  3,  5 },
            {  2,  0, -1,  2, -1,  0,  2 },
        };

        public static Parcel U0() => Build("U0", perturbed: false);
        public static Parcel U1() => Build("U1", perturbed: true);

        private static Parcel Build(string name, bool perturbed)
        {
            var places = new List<Place>(Width * Height);
            var cells = new Dictionary<Cell, long>();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    var place = new Place(row * Width + col);
                    places.Add(place);
                    long elevation = U0Elevation[row, col];
                    if (perturbed && row == 3 && col == 2)
                    {
                        elevation = 9;
                    }
                    cells[new Cell(place, K.Elevation)] = elevation;
                    cells[new Cell(place, K.Water)] = 0;
                }
            }

            var initial = new SimulationState(Tick.Zero, cells);
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

            var relationSet = new RelationSet(initial, relations.ToArray());
            DivisorPolicy constantFive = _ => 5;
            var fixtures = new FixtureSet(
                new FlowFixture(places, K.Water, constantFive, new AdditiveResolver()));
            var trace = new ExternalEventTrace(new Membrane(new[] { K.Water }));
            for (long tick = 0; tick < RainTicks; tick++)
            {
                trace.Append(new ExternalEvent(new Tick(tick), new Cell(Source, K.Water), 1));
            }

            return new Parcel(name, initial, relationSet, fixtures, trace, places, PlannedTicks);
        }
    }
}
