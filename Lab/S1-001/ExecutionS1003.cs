using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// S1-003's authorised recorder. It writes both complete records before producing comparative
    /// measurements. Outcome classification is mechanical and uses only the six sealed outcomes.
    /// </summary>
    public static class ExecutionS1003
    {
        private static readonly DivisorPolicy ConstantFive = _ => 5;

        public static int RunAll(string runsRoot)
        {
            Directory.CreateDirectory(runsRoot);
            ParcelRun u0 = ParcelRun.Execute(WorldsS1003.U0(), WorldsS1003.PlannedTicks);
            ParcelRun u1 = ParcelRun.Execute(WorldsS1003.U1(), WorldsS1003.PlannedTicks);

            WriteWorld(Path.Combine(runsRoot, "U0"), u0);
            WriteWorld(Path.Combine(runsRoot, "U1"), u1);
            string verdict = WriteComparison(runsRoot, u0, u1);

            Console.WriteLine($"  U0: {u0.States.Count - 1} ticks recorded.");
            Console.WriteLine($"  U1: {u1.States.Count - 1} ticks recorded.");
            Console.WriteLine($"  Bound outcome: {verdict}");
            Console.WriteLine($"Artifacts written under {Path.GetFullPath(runsRoot)}.");
            return verdict.StartsWith("E", StringComparison.Ordinal) ||
                   verdict.StartsWith("F", StringComparison.Ordinal) ? 1 : 0;
        }

        private static void WriteWorld(string directory, ParcelRun run)
        {
            Directory.CreateDirectory(directory);
            WriteStates(Path.Combine(directory, "states.csv"), run);
            WriteFlux(Path.Combine(directory, "flux.csv"), run);
            WriteMirror(Path.Combine(directory, "mirror.csv"), run);
            WriteFirstWet(Path.Combine(directory, "first-wet.csv"), run);
            WriteRegions(Path.Combine(directory, "regions.csv"), run);

            IReadOnlyList<FluxCounter.Mismatch> reconstruction = FluxCounter.ConsistencyCheck(
                run.States, run.Parcel.Relations, run.Parcel.Places, run.Parcel.Trace,
                K.Elevation, K.Water, ConstantFive);
            IReadOnlyList<ConservationAudit.Violation> conservation = ConservationAudit.Check(
                run.States, new[] { K.Water }, run.Parcel.Trace);
            S1003Instruments.Extreme minimum =
                S1003Instruments.MostNegative(run.States, run.Parcel.Places, K.Water);
            (long magnitude, long tick) maximum = S1003Instruments.MaximumRegionImbalance(
                run.States, WorldsS1003.Width, WorldsS1003.Height, K.Water);
            S1003Instruments.RegionTotals at60 =
                S1003Instruments.Totals(run.States[60], WorldsS1003.Width, WorldsS1003.Height, K.Water);
            S1003Instruments.RegionTotals at120 =
                S1003Instruments.Totals(run.States[120], WorldsS1003.Width, WorldsS1003.Height, K.Water);

            var text = new StringBuilder();
            text.Append("world: ").Append(run.Parcel.Name).Append('\n');
            text.Append("ticks: 120\n");
            text.Append("flux reconstruction mismatches: ").Append(reconstruction.Count).Append('\n');
            text.Append("conservation violations: ").Append(conservation.Count).Append('\n');
            text.Append("most negative water: ").Append(minimum.Value).Append(" at tick ")
                .Append(minimum.Tick).Append(", place ").Append(minimum.Place.Value).Append('\n');
            text.Append("maximum |left-right|: ").Append(maximum.magnitude)
                .Append(" at tick ").Append(maximum.tick).Append('\n');
            text.Append("left-right at tick 60: ").Append(at60.Left - at60.Right).Append('\n');
            text.Append("left-right at tick 120: ").Append(at120.Left - at120.Right).Append('\n');
            File.WriteAllText(Path.Combine(directory, "witnesses.txt"), text.ToString());
        }

        private static string WriteComparison(string root, ParcelRun u0, ParcelRun u1)
        {
            S1003Instruments.WorldDifference first =
                S1003Instruments.FirstWorldDifference(u0.States, u1.States, K.Water);
            IReadOnlyList<S1003Instruments.MirrorDifference> control =
                S1003Instruments.MirrorDifferences(u0.States, 7, 7, K.Water);
            IReadOnlyList<S1003Instruments.MirrorDifference> subject =
                S1003Instruments.MirrorDifferences(u1.States, 7, 7, K.Water);

            bool controlFailure = AnyNonZero(control);
            bool instrumentFailure =
                FluxCounter.ConsistencyCheck(u0.States, u0.Parcel.Relations, u0.Parcel.Places,
                    u0.Parcel.Trace, K.Elevation, K.Water, ConstantFive).Count != 0 ||
                FluxCounter.ConsistencyCheck(u1.States, u1.Parcel.Relations, u1.Parcel.Places,
                    u1.Parcel.Trace, K.Elevation, K.Water, ConstantFive).Count != 0 ||
                ConservationAudit.Check(u0.States, new[] { K.Water }, u0.Parcel.Trace).Count != 0 ||
                ConservationAudit.Check(u1.States, new[] { K.Water }, u1.Parcel.Trace).Count != 0;

            bool outside = false;
            bool atPerturbation = false;
            for (int tick = 0; tick < u0.States.Count; tick++)
            {
                foreach (Place place in u0.Parcel.Places)
                {
                    if (u0.States[tick].ValueAt(new Cell(place, K.Water)) ==
                        u1.States[tick].ValueAt(new Cell(place, K.Water))) continue;
                    if (place == WorldsS1003.Perturbation) atPerturbation = true;
                    else outside = true;
                }
            }

            bool persists = false;
            foreach (S1003Instruments.MirrorDifference item in subject)
            {
                if (item.Tick == 120 && item.SignedDifference != 0) persists = true;
            }

            string verdict;
            if (instrumentFailure) verdict = "F - instrument failure";
            else if (controlFailure) verdict = "E - control failure";
            else if (outside) verdict = persists
                ? "A - confirmed, persistent"
                : "B - confirmed, transient";
            else if (atPerturbation) verdict = "C - refuted, confined";
            else verdict = "D - refuted, silent";

            var text = new StringBuilder();
            text.Append("first U0/U1 water difference: ");
            if (first == null) text.Append("none\n");
            else text.Append("tick ").Append(first.Tick).Append(", place ").Append(first.Place.Value)
                .Append(" (U0=").Append(first.FirstValue).Append(", U1=").Append(first.SecondValue).Append(")\n");
            text.Append("difference outside perturbed cell: ").Append(outside).Append('\n');
            text.Append("difference at perturbed cell: ").Append(atPerturbation).Append('\n');
            text.Append("U0 mirror violation: ").Append(controlFailure).Append('\n');
            text.Append("U1 mirror difference at tick 120: ").Append(persists).Append('\n');
            text.Append("instrument failure: ").Append(instrumentFailure).Append('\n');
            text.Append("bound outcome: ").Append(verdict).Append('\n');
            File.WriteAllText(Path.Combine(root, "COMPARISON.txt"), text.ToString());
            return verdict;
        }

        private static bool AnyNonZero(IReadOnlyList<S1003Instruments.MirrorDifference> values)
        {
            foreach (S1003Instruments.MirrorDifference item in values)
                if (item.SignedDifference != 0) return true;
            return false;
        }

        private static void WriteStates(string path, ParcelRun run)
        {
            var csv = new StringBuilder("tick,row,col,elevation,water\n");
            foreach (SimulationState state in run.States)
                foreach (Place place in run.Parcel.Places)
                    csv.Append(state.CurrentTick.Value).Append(',').Append(place.Value / 7).Append(',')
                        .Append(place.Value % 7).Append(',')
                        .Append(state.ValueAt(new Cell(place, K.Elevation))).Append(',')
                        .Append(state.ValueAt(new Cell(place, K.Water))).Append('\n');
            File.WriteAllText(path, csv.ToString());
        }

        private static void WriteFlux(string path, ParcelRun run)
        {
            var csv = new StringBuilder("tick,source,target,amount\n");
            for (int tick = 0; tick + 1 < run.States.Count; tick++)
                foreach (FluxCounter.EdgeFlux flux in FluxCounter.FluxAt(run.States[tick],
                    run.Parcel.Relations, run.Parcel.Places, K.Elevation, K.Water, ConstantFive))
                    csv.Append(tick).Append(',').Append(flux.Source.Value).Append(',')
                        .Append(flux.Target.Value).Append(',').Append(flux.Amount).Append('\n');
            File.WriteAllText(path, csv.ToString());
        }

        private static void WriteMirror(string path, ParcelRun run)
        {
            var csv = new StringBuilder("tick,row,distance,left_minus_right\n");
            foreach (S1003Instruments.MirrorDifference item in S1003Instruments.MirrorDifferences(
                run.States, 7, 7, K.Water))
                csv.Append(item.Tick).Append(',').Append(item.Row).Append(',').Append(item.Distance)
                    .Append(',').Append(item.SignedDifference).Append('\n');
            File.WriteAllText(path, csv.ToString());
        }

        private static void WriteFirstWet(string path, ParcelRun run)
        {
            IReadOnlyDictionary<Place, long> first =
                S1003Instruments.FirstWetTicks(run.States, run.Parcel.Places, K.Water);
            var csv = new StringBuilder("row,col,first_wet_tick\n");
            foreach (Place place in run.Parcel.Places)
                csv.Append(place.Value / 7).Append(',').Append(place.Value % 7).Append(',')
                    .Append(first.TryGetValue(place, out long tick) ? tick.ToString() : "never").Append('\n');
            File.WriteAllText(path, csv.ToString());
        }

        private static void WriteRegions(string path, ParcelRun run)
        {
            var csv = new StringBuilder("tick,left,centre,right,left_minus_right,total\n");
            foreach (SimulationState state in run.States)
            {
                S1003Instruments.RegionTotals totals = S1003Instruments.Totals(state, 7, 7, K.Water);
                csv.Append(state.CurrentTick.Value).Append(',').Append(totals.Left).Append(',')
                    .Append(totals.Centre).Append(',').Append(totals.Right).Append(',')
                    .Append(totals.Left - totals.Right).Append(',').Append(totals.Full).Append('\n');
            }
            File.WriteAllText(path, csv.ToString());
        }
    }
}
