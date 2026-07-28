using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// The S1-002 execution driver, added under the execution authorisation (2026-07-29). Runs
    /// the twelve sealed parcels exactly as built, records the full trace first (E2), computes
    /// the instruments — including the flux counter under each world's declared divisor
    /// convention — and interprets nothing. Per E4, no hand derivation is consulted or rewritten
    /// here: this driver only writes what the record says.
    /// </summary>
    public static class ExecutionS1002
    {
        public static int RunAll(string runsRoot)
        {
            Directory.CreateDirectory(runsRoot);
            foreach (Parcel parcel in WorldsS1002.All())
            {
                RunOne(parcel, Path.Combine(runsRoot, parcel.Name));
            }

            Console.WriteLine($"Execution complete. Artifacts under {Path.GetFullPath(runsRoot)}.");
            return 0;
        }

        /// <summary>The sealed declared convention per world: V2 stars use degree-aware; all others naive.</summary>
        private static DivisorPolicy ConventionFor(Parcel parcel)
        {
            return parcel.Name.StartsWith("V2", StringComparison.Ordinal) ? Divisors.DegreeAware : Divisors.Naive;
        }

        private static void RunOne(Parcel parcel, string directory)
        {
            Directory.CreateDirectory(directory);
            ParcelRun run = ParcelRun.Execute(parcel, parcel.PlannedTicks);
            DivisorPolicy convention = ConventionFor(parcel);

            WriteStates(directory, run);
            var fluxByTick = new List<IReadOnlyList<FluxCounter.EdgeFlux>>();
            for (int t = 0; t + 1 < run.States.Count; t++)
            {
                fluxByTick.Add(FluxCounter.FluxAt(run.States[t], parcel.Relations, parcel.Places,
                    K.Elevation, K.Water, convention));
            }

            WriteFlux(directory, fluxByTick);

            IReadOnlyList<FluxCounter.Mismatch> mismatches = FluxCounter.ConsistencyCheck(
                run.States, parcel.Relations, parcel.Places, parcel.Trace, K.Elevation, K.Water, convention);
            IReadOnlyList<ConservationAudit.Violation> violations = ConservationAudit.Check(
                run.States, new[] { K.Water }, parcel.Trace);

            WriteSummary(directory, parcel, run, fluxByTick, mismatches, violations);
            Console.WriteLine($"  {parcel.Name}: {run.States.Count - 1} tick(s), artifacts written.");
        }

        private static void WriteStates(string directory, ParcelRun run)
        {
            var csv = new StringBuilder("tick,place,kind,value\n");
            for (int t = 0; t < run.States.Count; t++)
            {
                foreach (Place place in run.Parcel.Places)
                {
                    csv.Append(t).Append(',').Append(place.Value).Append(",1,")
                       .Append(run.States[t].ValueAt(new Cell(place, K.Elevation))).Append('\n');
                    csv.Append(t).Append(',').Append(place.Value).Append(",2,")
                       .Append(run.States[t].ValueAt(new Cell(place, K.Water))).Append('\n');
                }
            }

            File.WriteAllText(Path.Combine(directory, "states.csv"), csv.ToString());
        }

        private static void WriteFlux(string directory, IReadOnlyList<IReadOnlyList<FluxCounter.EdgeFlux>> fluxByTick)
        {
            var csv = new StringBuilder("tick,source,target,amount\n");
            for (int t = 0; t < fluxByTick.Count; t++)
            {
                foreach (FluxCounter.EdgeFlux flux in fluxByTick[t])
                {
                    csv.Append(t).Append(',').Append(flux.Source.Value).Append(',')
                       .Append(flux.Target.Value).Append(',').Append(flux.Amount).Append('\n');
                }
            }

            File.WriteAllText(Path.Combine(directory, "flux.csv"), csv.ToString());
        }

        private static void WriteSummary(string directory, Parcel parcel, ParcelRun run,
            IReadOnlyList<IReadOnlyList<FluxCounter.EdgeFlux>> fluxByTick,
            IReadOnlyList<FluxCounter.Mismatch> mismatches,
            IReadOnlyList<ConservationAudit.Violation> violations)
        {
            var text = new StringBuilder();
            text.Append("world: ").Append(parcel.Name).Append('\n');
            text.Append("ticks run: ").Append(run.States.Count - 1).Append('\n');

            long totalFlux = 0;
            var firstTransfer = new SortedDictionary<string, long>(StringComparer.Ordinal);
            var firingTicks = new SortedDictionary<string, List<long>>(StringComparer.Ordinal);
            for (int t = 0; t < fluxByTick.Count; t++)
            {
                foreach (FluxCounter.EdgeFlux flux in fluxByTick[t])
                {
                    totalFlux += flux.Amount;
                    string edge = $"{flux.Source.Value}->{flux.Target.Value}";
                    if (!firstTransfer.ContainsKey(edge))
                    {
                        firstTransfer[edge] = t;
                        firingTicks[edge] = new List<long>();
                    }

                    if (firingTicks[edge].Count < 15)
                    {
                        firingTicks[edge].Add(t);
                    }
                }
            }

            text.Append("total counted flux over the run: ").Append(totalFlux).Append('\n');
            text.Append("first-transfer tick per edge (edges that ever fired):\n");
            if (firstTransfer.Count == 0)
            {
                text.Append("  (none - no edge ever fired)\n");
            }

            foreach (KeyValuePair<string, long> entry in firstTransfer)
            {
                text.Append("  ").Append(entry.Key).Append(": tick ").Append(entry.Value)
                    .Append("  (firing ticks: ").Append(string.Join(",", firingTicks[entry.Key])).Append(")\n");
            }

            long negativityTick = -1;
            int negativityPlace = -1;
            for (int t = 0; t < run.States.Count && negativityTick < 0; t++)
            {
                foreach (Place place in parcel.Places)
                {
                    if (run.States[t].ValueAt(new Cell(place, K.Water)) < 0)
                    {
                        negativityTick = t;
                        negativityPlace = place.Value;
                        break;
                    }
                }
            }

            text.Append("first negative kind(2) value: ")
                .Append(negativityTick >= 0 ? $"tick {negativityTick} at place({negativityPlace})" : "none")
                .Append('\n');

            text.Append("consistency check (C5 convention): ").Append(mismatches.Count).Append(" mismatch(es)\n");
            for (int i = 0; i < mismatches.Count && i < 10; i++)
            {
                text.Append("  ").Append(mismatches[i]).Append('\n');
            }

            text.Append("kind(2) conservation audit: ").Append(violations.Count).Append(" violation(s)\n");
            File.WriteAllText(Path.Combine(directory, "summary.txt"), text.ToString());
        }
    }
}
