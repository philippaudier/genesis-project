using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.Demo001
{
    /// <summary>
    /// Runs Demo-001 headless and writes a record for the presentation layer to replay.
    /// The simulation never knows it is being watched; the watcher never knows a law.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            long maxStep = DemoWorld.MaxElevationStep();
            Console.WriteLine($"Demo-001 — a world to look at (no campaign, no evidence)");
            Console.WriteLine($"  grid            : {DemoWorld.Width}x{DemoWorld.Height}");
            Console.WriteLine($"  divisor         : {DemoWorld.Divisor} (constant, demo convention)");
            Console.WriteLine($"  max terrain step: {maxStep}  (must stay <= {DemoWorld.MaxAllowedStep})");

            if (maxStep >= DemoWorld.Divisor)
            {
                Console.WriteLine("REFUSED: dry ground would transport — the demo's premise is false.");
                return 1;
            }

            Console.WriteLine("  dry ground is inert by construction; only water can move.");

            SimulationState state = DemoWorld.InitialState();
            IReadOnlyList<Place> places = DemoWorld.Places();
            RelationSet relations = DemoWorld.Relations(state);
            ExternalEventTrace rain = DemoWorld.Rain(places);
            var fixtures = new FixtureSet(
                new FlowFixture(places, K.Water, DemoWorld.Constant5, new AdditiveResolver()));
            var runner = new TickRunner(new TransitionRunner(fixtures.Resolvers));

            Directory.CreateDirectory("Record");
            string path = Path.Combine("Record", "demo-001.record");

            var record = new StringBuilder();
            record.Append("genesis-record 1\n");
            record.Append($"grid {DemoWorld.Width} {DemoWorld.Height}\n");
            record.Append($"ticks {DemoWorld.TotalTicks}\n");
            record.Append("elevation");
            foreach (Place place in places)
            {
                record.Append(' ').Append(state.ValueAt(new Cell(place, K.Elevation)));
            }

            record.Append('\n');
            AppendWaterRow(record, state, places, 0);

            long totalRain = 0;
            long mostNegative = 0;
            long deepest = 0;
            DateTime started = DateTime.UtcNow;

            for (long t = 0; t < DemoWorld.TotalTicks; t++)
            {
                state = runner.Run(state, relations, fixtures.Transitions, rain, 1);
                AppendWaterRow(record, state, places, t + 1);

                foreach (Place place in places)
                {
                    long water = state.ValueAt(new Cell(place, K.Water));
                    if (water < mostNegative) mostNegative = water;
                    if (water > deepest) deepest = water;
                }

                if ((t + 1) % 50 == 0)
                {
                    Console.WriteLine($"  tick {t + 1,4}/{DemoWorld.TotalTicks}  deepest={deepest}  mostNegative={mostNegative}");
                }
            }

            foreach (ExternalEvent crossing in rain.Events)
            {
                totalRain += crossing.Amount;
            }

            long finalTotal = 0;
            foreach (Place place in places)
            {
                finalTotal += state.ValueAt(new Cell(place, K.Water));
            }

            File.WriteAllText(path, record.ToString());

            Console.WriteLine();
            Console.WriteLine($"  water that crossed the membrane : {totalRain}");
            Console.WriteLine($"  water present at the end        : {finalTotal}   (equal ⇒ nothing appeared or vanished)");
            Console.WriteLine($"  deepest                         : {deepest}");
            Console.WriteLine($"  most negative                   : {mostNegative}");
            Console.WriteLine($"  elapsed                         : {(DateTime.UtcNow - started).TotalSeconds:F1}s");
            Console.WriteLine($"  record                          : {Path.GetFullPath(path)} ({new FileInfo(path).Length / 1024} KB)");
            return 0;
        }

        private static void AppendWaterRow(StringBuilder record, SimulationState state,
            IReadOnlyList<Place> places, long tick)
        {
            record.Append("w ").Append(tick);
            foreach (Place place in places)
            {
                record.Append(' ').Append(state.ValueAt(new Cell(place, K.Water)));
            }

            record.Append('\n');
        }
    }
}
