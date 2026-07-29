using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.Demo001
{
    /// <summary>
    /// A non-evidential A/B probe for the question Demo-001 exposed: can a sub-threshold lateral
    /// relief affect an already non-uniform flow? This pair is prior exposure, never a campaign
    /// specimen. Its only purpose is to decide whether an independent sealed experiment is owed.
    /// </summary>
    public static class RoutingProbe
    {
        private const int Width = 7;
        private const int Height = 7;
        private const long Ticks = 40;
        private static readonly long[] Corrugation = { 4, 2, 0, -2, 0, 2, 4 };

        public static int Run()
        {
            ProbeWorld flat = Build(corrugated: false);
            ProbeWorld shaped = Build(corrugated: true);
            long firstDivergence = -1;
            int firstDivergenceCell = -1;

            for (long tick = 1; tick <= Ticks; tick++)
            {
                flat.State = flat.Runner.Run(flat.State, flat.Relations, flat.Fixtures.Transitions, flat.Rain, 1);
                shaped.State = shaped.Runner.Run(
                    shaped.State, shaped.Relations, shaped.Fixtures.Transitions, shaped.Rain, 1);

                if (firstDivergence < 0)
                {
                    for (int i = 0; i < flat.Places.Count; i++)
                    {
                        long a = flat.State.ValueAt(new Cell(flat.Places[i], K.Water));
                        long b = shaped.State.ValueAt(new Cell(shaped.Places[i], K.Water));
                        if (a != b)
                        {
                            firstDivergence = tick;
                            firstDivergenceCell = i;
                            break;
                        }
                    }
                }
            }

            Console.WriteLine("Routing probe — NON-EVIDENTIAL (this exact pair is now contaminated)");
            Console.WriteLine("  A: laterally flat slope · B: same slope + sub-threshold corrugation");
            Console.WriteLine("  forcing: identical point rain at the top-centre cell");
            Console.WriteLine(firstDivergence < 0
                ? $"  verdict: no water-state divergence in {Ticks} ticks"
                : $"  verdict: first water-state divergence at tick {firstDivergence},"
                    + $" row {firstDivergenceCell / Width}, column {firstDivergenceCell % Width}");
            return 0;
        }

        private static ProbeWorld Build(bool corrugated)
        {
            var places = new List<Place>(Width * Height);
            var cells = new Dictionary<Cell, long>();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    var place = new Place(row * Width + col);
                    places.Add(place);
                    long elevation = 4L * (Height - 1 - row);
                    if (corrugated)
                    {
                        elevation += Corrugation[col];
                    }

                    cells[new Cell(place, K.Elevation)] = elevation;
                    cells[new Cell(place, K.Water)] = 0;
                }
            }

            var state = new SimulationState(Tick.Zero, cells);
            var relations = new List<Relation>();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    var here = places[row * Width + col];
                    if (col + 1 < Width)
                    {
                        AddBoth(relations, here, places[row * Width + col + 1]);
                    }

                    if (row + 1 < Height)
                    {
                        AddBoth(relations, here, places[(row + 1) * Width + col]);
                    }
                }
            }

            var rain = new ExternalEventTrace(new Membrane(new[] { K.Water }));
            Place source = places[Width / 2];
            for (long tick = 0; tick < Ticks; tick++)
            {
                rain.Append(new ExternalEvent(new Tick(tick), new Cell(source, K.Water), 1));
            }

            var fixtures = new FixtureSet(
                new FlowFixture(places, K.Water, DemoWorld.Constant5, new AdditiveResolver()));
            return new ProbeWorld(
                state,
                new RelationSet(state, relations.ToArray()),
                places,
                rain,
                fixtures,
                new TickRunner(new TransitionRunner(fixtures.Resolvers)));
        }

        private static void AddBoth(List<Relation> relations, Place a, Place b)
        {
            relations.Add(new Relation(a, b));
            relations.Add(new Relation(b, a));
        }

        private sealed class ProbeWorld
        {
            public ProbeWorld(
                SimulationState state,
                RelationSet relations,
                IReadOnlyList<Place> places,
                ExternalEventTrace rain,
                FixtureSet fixtures,
                TickRunner runner)
            {
                State = state;
                Relations = relations;
                Places = places;
                Rain = rain;
                Fixtures = fixtures;
                Runner = runner;
            }

            public SimulationState State { get; set; }
            public RelationSet Relations { get; }
            public IReadOnlyList<Place> Places { get; }
            public ExternalEventTrace Rain { get; }
            public FixtureSet Fixtures { get; }
            public TickRunner Runner { get; }
        }
    }
}
