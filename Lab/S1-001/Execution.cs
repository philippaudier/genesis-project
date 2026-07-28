using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// The execution driver, added under the execution authorisation (Campaign S1-001, post-seal
    /// filings). It runs the sealed parcels exactly as built, records the raw trace first (E2),
    /// and computes only declared-convention facts. It interprets nothing. Sealed instruments are
    /// used, never modified. The abort guard realises the sealed instruction to stay well before
    /// the 2^62 frontier.
    /// </summary>
    public static class Execution
    {
        private const long AbortThreshold = 1L << 40;

        public static int RunAll(string runsRoot)
        {
            Directory.CreateDirectory(runsRoot);
            foreach (Parcel parcel in Worlds.All())
            {
                RunOne(parcel, Path.Combine(runsRoot, parcel.Name));
            }

            Console.WriteLine($"Execution complete. Artifacts under {Path.GetFullPath(runsRoot)}.");
            return 0;
        }

        private static void RunOne(Parcel parcel, string directory)
        {
            Directory.CreateDirectory(directory);

            var tickRunner = new TickRunner(new TransitionRunner(parcel.Fixtures.Resolvers));
            var states = new List<SimulationState> { parcel.Initial };
            SimulationState state = parcel.Initial;
            bool aborted = false;
            for (long t = 0; t < parcel.PlannedTicks; t++)
            {
                state = tickRunner.Run(state, parcel.Relations, parcel.Fixtures.Transitions, parcel.Trace, 1);
                states.Add(state);
                if (MaxAbs(state) >= AbortThreshold)
                {
                    aborted = true;
                    break;
                }
            }

            IReadOnlyList<Kind> kinds = KindsOf(parcel.Initial);

            WriteStates(directory, states, parcel.Places, kinds);
            WriteLedger(directory, states, kinds);
            string audit = WriteAudit(directory, parcel, states);
            (long lockTick, List<long> changes) = WritePattern(directory, parcel, states);
            string negativity = WriteNegativity(directory, parcel, states);
            string partition = WritePartition(directory, parcel, states[states.Count - 1]);
            WriteSummary(directory, parcel, states, aborted, lockTick, changes, audit, negativity, partition);

            Console.WriteLine($"  {parcel.Name}: {states.Count - 1} tick(s) run{(aborted ? " (aborted at guard)" : "")}, artifacts written.");
        }

        // --- artifacts ---------------------------------------------------------------------

        private static void WriteStates(string directory, IReadOnlyList<SimulationState> states,
            IReadOnlyList<Place> places, IReadOnlyList<Kind> kinds)
        {
            var csv = new StringBuilder("tick,place,kind,value\n");
            for (int t = 0; t < states.Count; t++)
            {
                foreach (Place place in places)
                {
                    foreach (Kind kind in kinds)
                    {
                        long value = states[t].ValueAt(new Cell(place, kind));
                        csv.Append(t).Append(',').Append(place.Value).Append(',')
                           .Append(kind.Value).Append(',').Append(value).Append('\n');
                    }
                }
            }

            File.WriteAllText(Path.Combine(directory, "states.csv"), csv.ToString());
        }

        private static void WriteLedger(string directory, IReadOnlyList<SimulationState> states, IReadOnlyList<Kind> kinds)
        {
            IReadOnlyList<IReadOnlyDictionary<Kind, long>> ledger = PerKindLedger.Totals(states);
            var csv = new StringBuilder("tick");
            foreach (Kind kind in kinds)
            {
                csv.Append(",kind(").Append(kind.Value).Append(')');
            }

            csv.Append('\n');
            for (int t = 0; t < ledger.Count; t++)
            {
                csv.Append(t);
                foreach (Kind kind in kinds)
                {
                    ledger[t].TryGetValue(kind, out long total);
                    csv.Append(',').Append(total);
                }

                csv.Append('\n');
            }

            File.WriteAllText(Path.Combine(directory, "ledger.csv"), csv.ToString());
        }

        private static string WriteAudit(string directory, Parcel parcel, IReadOnlyList<SimulationState> states)
        {
            var report = new StringBuilder();
            IReadOnlyList<Kind> kinds = KindsOf(parcel.Initial);
            AppendAuditLine(report, "kind(2) net of declared crossings", states, new[] { K.Water }, parcel.Trace);
            if (Contains(kinds, K.Rock))
            {
                AppendAuditLine(report, "kind(3)+kind(4) summed", states, new[] { K.Rock, K.Sediment }, parcel.Trace);
            }

            if (Contains(kinds, K.Sacrificial))
            {
                AppendAuditLine(report, "kind(5)", states, new[] { K.Sacrificial }, parcel.Trace);
            }

            string text = report.ToString();
            File.WriteAllText(Path.Combine(directory, "audit.txt"), text);
            return text;
        }

        private static void AppendAuditLine(StringBuilder report, string label,
            IReadOnlyList<SimulationState> states, IReadOnlyCollection<Kind> kindSet, ExternalEventTrace trace)
        {
            IReadOnlyList<ConservationAudit.Violation> violations = ConservationAudit.Check(states, kindSet, trace);
            report.Append(label).Append(": ").Append(violations.Count).Append(" violation(s)").Append('\n');
            for (int i = 0; i < violations.Count && i < 10; i++)
            {
                report.Append("  ").Append(violations[i]).Append('\n');
            }

            if (violations.Count > 10)
            {
                report.Append("  ... (").Append(violations.Count - 10).Append(" more)").Append('\n');
            }
        }

        /// <summary>
        /// Declared convention: the descent pattern at a tick maps each place to its strictly-lower
        /// potential neighbours (potential = kind(1)+kind(2) values, from the record alone). The
        /// pattern's change ticks are recorded; the lock tick is the last change.
        /// </summary>
        private static (long lockTick, List<long> changes) WritePattern(string directory, Parcel parcel,
            IReadOnlyList<SimulationState> states)
        {
            var changes = new List<long>();
            string previous = null;
            for (int t = 0; t < states.Count; t++)
            {
                string pattern = DescentPattern(states[t], parcel);
                if (previous == null || pattern != previous)
                {
                    changes.Add(t);
                    previous = pattern;
                }
            }

            long lockTick = changes.Count > 0 ? changes[changes.Count - 1] : 0;
            var text = new StringBuilder();
            text.Append("descent-pattern change ticks (convention: potential = kind(1)+kind(2); strictly-lower neighbours): ");
            text.Append(string.Join(",", changes)).Append('\n');
            text.Append("pattern lock tick (last change): ").Append(lockTick).Append('\n');
            File.WriteAllText(Path.Combine(directory, "pattern.txt"), text.ToString());
            return (lockTick, changes);
        }

        private static string DescentPattern(SimulationState state, Parcel parcel)
        {
            var text = new StringBuilder();
            foreach (Place place in parcel.Places)
            {
                long here = Potential(state, place);
                text.Append(place.Value).Append(':');
                foreach (Relation relation in parcel.Relations.OutgoingFrom(place))
                {
                    if (Potential(state, relation.Target) < here)
                    {
                        text.Append(relation.Target.Value).Append(' ');
                    }
                }

                text.Append(';');
            }

            return text.ToString();
        }

        private static string WriteNegativity(string directory, Parcel parcel, IReadOnlyList<SimulationState> states)
        {
            for (int t = 0; t < states.Count; t++)
            {
                var negatives = new List<Place>();
                foreach (Place place in parcel.Places)
                {
                    if (states[t].ValueAt(new Cell(place, K.Water)) < 0)
                    {
                        negatives.Add(place);
                    }
                }

                if (negatives.Count > 0)
                {
                    var text = new StringBuilder();
                    text.Append("first tick with kind(2) < 0: ").Append(t).Append('\n');
                    foreach (Place place in negatives)
                    {
                        int lower = 0, total = 0;
                        long herePrev = Potential(states[t - 1], place);
                        foreach (Relation relation in parcel.Relations.OutgoingFrom(place))
                        {
                            total++;
                            if (Potential(states[t - 1], relation.Target) < herePrev)
                            {
                                lower++;
                            }
                        }

                        text.Append($"  place({place.Value}) value={states[t].ValueAt(new Cell(place, K.Water))}; at tick {t - 1} it stood strictly above {lower} of its {total} neighbours\n");
                    }

                    string result = text.ToString();
                    File.WriteAllText(Path.Combine(directory, "negativity.txt"), result);
                    return result;
                }
            }

            const string none = "no negative kind(2) value occurred in the run\n";
            File.WriteAllText(Path.Combine(directory, "negativity.txt"), none);
            return none;
        }

        private static string WritePartition(string directory, Parcel parcel, SimulationState final)
        {
            IReadOnlyDictionary<Place, Place> partition =
                PartitionReader.TerminalPartition(final, parcel.Relations, parcel.Places, K.Elevation, K.Water);
            string report = PartitionReader.Report(partition);
            IReadOnlyList<string> violations = PartitionReader.VocabularyViolations(report);
            if (violations.Count > 0)
            {
                report += $"VOCABULARY GATE TRIPPED: {string.Join(",", violations)}\n";
            }

            File.WriteAllText(Path.Combine(directory, "partition.txt"), report);
            return report;
        }

        private static void WriteSummary(string directory, Parcel parcel, IReadOnlyList<SimulationState> states,
            bool aborted, long lockTick, List<long> patternChanges, string audit, string negativity, string partition)
        {
            var text = new StringBuilder();
            text.Append("world: ").Append(parcel.Name).Append('\n');
            text.Append("ticks run: ").Append(states.Count - 1).Append(" of planned ").Append(parcel.PlannedTicks).Append('\n');
            text.Append("aborted at 2^40 guard: ").Append(aborted).Append('\n');

            long freezeTick = -1;
            for (int t = (int)Worlds.RainTicks; t + 1 < states.Count; t++)
            {
                if (ValuesEqual(states[t], states[t + 1], parcel))
                {
                    freezeTick = t;
                    break;
                }
            }

            bool endFrozen = states.Count >= 2 && ValuesEqual(states[states.Count - 2], states[states.Count - 1], parcel);
            text.Append("first unchanged tick pair after rain: ").Append(freezeTick >= 0 ? freezeTick.ToString(CultureInfo.InvariantCulture) : "none").Append('\n');
            text.Append("last two recorded states identical: ").Append(endFrozen).Append('\n');

            SimulationState final = states[states.Count - 1];
            text.Append("final max |kind(2)| value: ").Append(MaxAbsOfKind(final, parcel, K.Water)).Append('\n');
            text.Append("final max driving potential gradient across a relation: ").Append(MaxGradient(final, parcel)).Append('\n');
            text.Append("descent-pattern change count: ").Append(patternChanges.Count).Append("; lock tick: ").Append(lockTick).Append('\n');
            text.Append('\n').Append("audit:\n").Append(audit);
            text.Append('\n').Append("negativity:\n").Append(negativity);
            text.Append('\n').Append("partition (final state):\n").Append(partition);
            File.WriteAllText(Path.Combine(directory, "summary.txt"), text.ToString());
        }

        // --- readings (declared conventions, record only) ------------------------------------

        private static long Potential(SimulationState state, Place place)
        {
            return state.ValueAt(new Cell(place, K.Elevation)) + state.ValueAt(new Cell(place, K.Water));
        }

        private static IReadOnlyList<Kind> KindsOf(SimulationState state)
        {
            var kinds = new SortedSet<Kind>();
            foreach (Cell cell in state.Cells)
            {
                kinds.Add(cell.Kind);
            }

            return new List<Kind>(kinds);
        }

        private static bool Contains(IReadOnlyList<Kind> kinds, Kind kind)
        {
            for (int i = 0; i < kinds.Count; i++)
            {
                if (kinds[i] == kind)
                {
                    return true;
                }
            }

            return false;
        }

        private static long MaxAbs(SimulationState state)
        {
            long max = 0;
            foreach (Cell cell in state.Cells)
            {
                long value = Math.Abs(state.ValueAt(cell));
                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        private static long MaxAbsOfKind(SimulationState state, Parcel parcel, Kind kind)
        {
            long max = 0;
            foreach (Place place in parcel.Places)
            {
                long value = Math.Abs(state.ValueAt(new Cell(place, kind)));
                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        private static long MaxGradient(SimulationState state, Parcel parcel)
        {
            long max = 0;
            foreach (Place place in parcel.Places)
            {
                long here = Potential(state, place);
                foreach (Relation relation in parcel.Relations.OutgoingFrom(place))
                {
                    long diff = here - Potential(state, relation.Target);
                    if (diff > max)
                    {
                        max = diff;
                    }
                }
            }

            return max;
        }

        private static bool ValuesEqual(SimulationState a, SimulationState b, Parcel parcel)
        {
            foreach (Cell cell in a.Cells)
            {
                if (a.ValueAt(cell) != b.ValueAt(cell))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
