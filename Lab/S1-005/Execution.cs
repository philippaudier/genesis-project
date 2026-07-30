using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genesis.Simulation;
using Genesis.Lab.S1_004;

namespace Genesis.Lab.S1_005
{
    /// <summary>
    /// S1-005 execution driver, added after the founder's gate-7 authorisation.
    /// It records and mechanically classifies the sealed pair. It performs no reduction.
    /// </summary>
    public static class Execution
    {
        public sealed class Result
        {
            public Parcel5 Parcel;
            public List<SimulationState> States = new List<SimulationState>();
            public List<IReadOnlyList<Provenance.Entry>> Provenance =
                new List<IReadOnlyList<Provenance.Entry>>();
            public List<IReadOnlyList<string>> Prospectives =
                new List<IReadOnlyList<string>>();
            public List<IReadOnlyDictionary<int, long>> Conversions =
                new List<IReadOnlyDictionary<int, long>>();
            public List<string> ReconstructionFaults = new List<string>();
        }

        public static int RunAll(string root)
        {
            Directory.CreateDirectory(root);
            Result n0 = Execute(Worlds5.N0());
            Result n1 = Execute(Worlds5.N1());

            WriteWorld(Path.Combine(root, "N0"), n0);
            WriteWorld(Path.Combine(root, "N1"), n1);
            string report = Report(n0, n1);
            File.WriteAllText(Path.Combine(root, "REPORT.md"), report);

            Console.WriteLine(report);
            Console.WriteLine($"Artifacts under {Path.GetFullPath(root)}.");
            return 0;
        }

        private static Result Execute(Parcel5 parcel)
        {
            var result = new Result { Parcel = parcel };
            var runner = new TickRunner(new TransitionRunner(parcel.Set.Resolvers));
            SimulationState state = parcel.Initial;
            result.States.Add(state);

            for (long boundary = 0; boundary < parcel.Ticks; boundary++)
            {
                IReadOnlyList<Provenance.Entry> entries =
                    Provenance.Collect(state, parcel.Relations, parcel.Fixtures);
                result.Provenance.Add(entries);
                result.Prospectives.Add(Prospectives(state, parcel));
                result.Conversions.Add(Surface.ConversionsAt(state, parcel.Relations,
                    parcel.Places, Worlds5.Constant2, Worlds5.ConversionThreshold));

                SimulationState after = runner.Run(state, parcel.Relations,
                    parcel.Set.Transitions, parcel.Crossings, 1);
                AuditReconstruction(result, state, after, entries, boundary);
                result.States.Add(after);
                state = after;
            }

            return result;
        }

        private static IReadOnlyList<string> Prospectives(SimulationState state, Parcel5 parcel)
        {
            var lines = new List<string>();
            foreach (Place place in parcel.Places)
            {
                long here = K4.WaterPotential(state, place);
                foreach (Relation relation in parcel.Relations.OutgoingFrom(place))
                {
                    long diff = here - K4.WaterPotential(state, relation.Target);
                    long prospective = diff > 0 ? diff / Worlds5.Constant2(1) : 0;
                    lines.Add($"{place.Value}->{relation.Target.Value}:{prospective}");
                }
            }

            return lines;
        }

        private static void AuditReconstruction(Result result, SimulationState before,
            SimulationState after, IReadOnlyList<Provenance.Entry> entries, long boundary)
        {
            var delta = new Dictionary<Cell, long>();
            foreach (Cell cell in before.Cells)
            {
                delta[cell] = 0;
            }

            foreach (Provenance.Entry entry in entries)
            {
                delta[entry.Target] += entry.Amount;
            }

            foreach (ExternalEvent crossing in result.Parcel.Crossings.Events)
            {
                if (crossing.Boundary.Value == boundary)
                {
                    delta[crossing.Target] += crossing.Amount;
                }
            }

            foreach (Cell cell in before.Cells)
            {
                long observed = after.ValueAt(cell) - before.ValueAt(cell);
                if (observed != delta[cell])
                {
                    result.ReconstructionFaults.Add(
                        $"boundary {boundary}, {cell}: expected delta {delta[cell]}, observed {observed}");
                }
            }
        }

        private static void WriteWorld(string directory, Result result)
        {
            Directory.CreateDirectory(directory);
            var states = new StringBuilder(
                "tick,place,base,rock,sediment,water,solidsurface,potential,localmatter,complete_signature\n");
            for (int tick = 0; tick < result.States.Count; tick++)
            {
                string signature =
                    StateInstrument.CompleteSignature(result.States[tick], result.Parcel.Places);
                foreach (Place place in result.Parcel.Places)
                {
                    SimulationState state = result.States[tick];
                    states.Append(tick).Append(',').Append(place.Value).Append(',')
                        .Append(state.ValueAt(new Cell(place, K4.Base))).Append(',')
                        .Append(state.ValueAt(new Cell(place, K4.Rock))).Append(',')
                        .Append(state.ValueAt(new Cell(place, K4.Sediment))).Append(',')
                        .Append(state.ValueAt(new Cell(place, K4.Water))).Append(',')
                        .Append(K4.SolidSurface(state, place)).Append(',')
                        .Append(K4.WaterPotential(state, place)).Append(',')
                        .Append(Surface.LocalMatter(state, place)).Append(',')
                        .Append(signature).Append('\n');
                }
            }

            File.WriteAllText(Path.Combine(directory, "states.csv"), states.ToString());

            var readings = new StringBuilder();
            for (int boundary = 0; boundary < result.Provenance.Count; boundary++)
            {
                readings.Append($"=== boundary {boundary} ===\n");
                readings.Append("  prospectives : ")
                    .Append(string.Join(" ", result.Prospectives[boundary])).Append('\n');
                readings.Append("  conversions  : ")
                    .Append(ConversionLine(result.Conversions[boundary])).Append('\n');
                readings.Append("  crossings    : ")
                    .Append(CrossingLine(result.Parcel, boundary)).Append('\n');
                readings.Append("  contributions:\n");

                if (result.Provenance[boundary].Count == 0)
                {
                    readings.Append("      (none)\n");
                }

                foreach (Provenance.Entry entry in result.Provenance[boundary])
                {
                    readings.Append("      ").Append(entry).Append('\n');
                }

                foreach (KeyValuePair<Cell, List<Provenance.Entry>> collision in
                    Provenance.Collisions(result.Provenance[boundary]))
                {
                    readings.Append($"      COLLISION {collision.Key}:");
                    foreach (Provenance.Entry entry in collision.Value)
                    {
                        readings.Append(' ').Append(entry.Fixture).Append(' ')
                            .Append(entry.Amount >= 0 ? "+" : "").Append(entry.Amount);
                    }

                    readings.Append('\n');
                }
            }

            File.WriteAllText(Path.Combine(directory, "readings.txt"), readings.ToString());

            var audit = new StringBuilder();
            audit.Append("reconstruction faults: ").Append(result.ReconstructionFaults.Count).Append('\n');
            foreach (string fault in result.ReconstructionFaults)
            {
                audit.Append("  ").Append(fault).Append('\n');
            }

            IReadOnlyList<string> matter =
                Surface.MatterAudit(result.States, result.Parcel.Places);
            audit.Append("matter faults: ").Append(matter.Count).Append('\n');
            foreach (string fault in matter)
            {
                audit.Append("  ").Append(fault).Append('\n');
            }

            audit.Append("minimum: ").Append(Minimum(result)).Append('\n');
            StateInstrument.Repeat repeat =
                StateInstrument.FirstRepeat(result.States, result.Parcel.Places);
            audit.Append("first complete-state repeat: ")
                .Append(repeat == null
                    ? "(none)"
                    : $"tick {repeat.First} again at tick {repeat.Again}, period {repeat.Period}")
                .Append('\n');
            File.WriteAllText(Path.Combine(directory, "audit.txt"), audit.ToString());
        }

        private static string Report(Result n0, Result n1)
        {
            bool c0 = ValidRecord(n0) && ValidRecord(n1);
            bool c1 = FirstSurfaceChange(n0) == 3 && FirstSurfaceChange(n1) == 3 &&
                SurfaceAt(n0, 3) == "[7,9]" && SurfaceAt(n1, 3) == "[7,9]";
            bool c2 = FirstDivergence(n0, n1) == 4 && BoundaryThreeGuardDifference(n0, n1);
            bool c3 = N0OrbitExact(n0);
            bool durable = DurableNonUniform(n1);
            bool c4 = N1FixedExact(n1);
            bool c5 = c3 && c4;
            bool c6 = true; // Gate 6: foreign-toy calibration passed before these worlds ran.

            string outcome = Classify(c0, c1, c2, c3, durable, c4);
            string classification = Classification(outcome);

            var report = new StringBuilder();
            report.AppendLine("# S1-005 execution report");
            report.AppendLine();
            report.AppendLine("Seal: `7b739b4`; instrument: `acf17b8`; execution authorised 2026-07-30.");
            report.AppendLine();
            report.AppendLine("## Mechanical record");
            report.AppendLine();
            report.AppendLine("| Claim | Held | N0 | N1 |");
            report.AppendLine("|---|---|---|---|");
            report.AppendLine($"| C0 conservation, positivity, witness completeness | {Verdict(c0)} | matter {MatterRange(n0)}, min {Minimum(n0)}, reconstruction faults {n0.ReconstructionFaults.Count} | matter {MatterRange(n1)}, min {Minimum(n1)}, reconstruction faults {n1.ReconstructionFaults.Count} |");
            report.AppendLine($"| C1 shared construction | {Verdict(c1)} | first change t{FirstSurfaceChange(n0)}, {SurfaceAt(n0, 3)} at t3 | first change t{FirstSurfaceChange(n1)}, {SurfaceAt(n1, 3)} at t3 |");
            report.AppendLine($"| C2 decisive unit boundary | {Verdict(c2)} | boundary 3 moves Sediment B→A | boundary 3 blocks it; first state divergence t{FirstDivergence(n0, n1)} |");
            report.AppendLine($"| C3 shuttle control | {Verdict(c3)} | {RepeatLine(n0)}; surfaces {SurfaceAt(n0, 3)} / {SurfaceAt(n0, 4)} | — |");
            report.AppendLine($"| C4 durable candidate | {Verdict(c4)} | — | {RepeatLine(n1)}; surface {SurfaceAt(n1, 4)} |");
            report.AppendLine($"| C5 complete state, not surface only | {Verdict(c5)} | full state alternates | every cell fixed from t4 |");
            report.AppendLine($"| C6 threshold-0 conformance | {Verdict(c6)} | passed before execution | not applicable |");
            report.AppendLine();
            report.AppendLine("## Surface readings");
            report.AppendLine();
            report.AppendLine("| tick | N0 | N1 |");
            report.AppendLine("|---:|---|---|");
            for (int tick = 0; tick < n0.States.Count; tick++)
            {
                report.AppendLine($"| {tick} | {SurfaceAt(n0, tick)} | {SurfaceAt(n1, tick)} |");
            }

            report.AppendLine();
            report.AppendLine("## Classification");
            report.AppendLine();
            report.AppendLine($"**Outcome {outcome} — {classification}.**");
            report.AppendLine();
            report.AppendLine("Classification precedence applied as sealed: G → F → C → B → D → E → A.");
            report.AppendLine();
            report.AppendLine("## Decision");
            report.AppendLine();
            report.AppendLine("Withheld. Reduction was not authorised.");
            return report.ToString();
        }

        internal static string Classify(bool c0, bool c1, bool c2, bool c3,
            bool durable, bool c4)
        {
            if (!c0) return "G";
            if (!c2) return "F";
            if (!c3) return "C";
            if (!c1) return "B";
            if (!durable) return "D";
            if (!c4) return "E";
            return "A";
        }

        private static string Classification(string outcome)
        {
            switch (outcome)
            {
                case "A": return "discriminating support";
                case "B": return "construction suppressed";
                case "C": return "the control does not shuttle as derived";
                case "D": return "the candidate does not rest durably";
                case "E": return "durable, but the mechanism is incomplete";
                case "F": return "causal comparison broken";
                default: return "invalid evidence";
            }
        }

        private static bool ValidRecord(Result result)
        {
            return result.ReconstructionFaults.Count == 0 &&
                Surface.MatterAudit(result.States, result.Parcel.Places).Count == 0 &&
                Minimum(result) >= 0;
        }

        private static bool BoundaryThreeGuardDifference(Result n0, Result n1)
        {
            string a = ContributionRecord(n0.Provenance[3]);
            string b = ContributionRecord(n1.Provenance[3]);
            if (a == b)
            {
                return false;
            }

            var onlyN0 = new List<string>
            {
                "CompetenceTransportFixture:0:4:1",
                "CompetenceTransportFixture:1:4:-1",
            };
            onlyN0.Sort(StringComparer.Ordinal);

            var n0Lines = ContributionLines(n0.Provenance[3]);
            var n1Lines = ContributionLines(n1.Provenance[3]);
            foreach (string common in n1Lines)
            {
                n0Lines.Remove(common);
            }

            n0Lines.Sort(StringComparer.Ordinal);
            return string.Join("|", n0Lines) == string.Join("|", onlyN0);
        }

        private static List<string> ContributionLines(IReadOnlyList<Provenance.Entry> entries)
        {
            var lines = new List<string>();
            foreach (Provenance.Entry entry in entries)
            {
                lines.Add($"{entry.Fixture}:{entry.Target.Place.Value}:{entry.Target.Kind.Value}:{entry.Amount}");
            }

            return lines;
        }

        private static string ContributionRecord(IReadOnlyList<Provenance.Entry> entries)
        {
            var lines = ContributionLines(entries);
            lines.Sort(StringComparer.Ordinal);
            return string.Join("|", lines);
        }

        private static bool N0OrbitExact(Result result)
        {
            string t3 = Signature(result, 3);
            string t4 = Signature(result, 4);
            return t3 != t4 &&
                t3 == Signature(result, 5) && t3 == Signature(result, 7) &&
                t4 == Signature(result, 6) && t4 == Signature(result, 8);
        }

        private static bool N1FixedExact(Result result)
        {
            string fixedState = Signature(result, 4);
            if (SurfaceAt(result, 4) != "[7,9]")
            {
                return false;
            }

            for (int tick = 5; tick <= 8; tick++)
            {
                if (Signature(result, tick) != fixedState)
                {
                    return false;
                }
            }

            for (int boundary = 4; boundary < 8; boundary++)
            {
                if (result.Provenance[boundary].Count != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool DurableNonUniform(Result result)
        {
            for (int tick = 1; tick < result.States.Count; tick++)
            {
                if (Signature(result, tick - 1) != Signature(result, tick) ||
                    IsFlat(SurfaceAt(result, tick)))
                {
                    continue;
                }

                bool quiet = true;
                for (int boundary = tick - 1; boundary < result.Provenance.Count; boundary++)
                {
                    if (result.Provenance[boundary].Count != 0)
                    {
                        quiet = false;
                    }
                }

                if (quiet)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsFlat(string surface)
        {
            string[] values = surface.Trim('[', ']').Split(',');
            return values.Length > 0 && Array.TrueForAll(values, value => value == values[0]);
        }

        private static int FirstSurfaceChange(Result result)
        {
            string initial = SurfaceAt(result, 0);
            for (int tick = 1; tick < result.States.Count; tick++)
            {
                if (SurfaceAt(result, tick) != initial)
                {
                    return tick;
                }
            }

            return -1;
        }

        private static int FirstDivergence(Result a, Result b)
        {
            for (int tick = 0; tick < a.States.Count; tick++)
            {
                if (Signature(a, tick) != Signature(b, tick))
                {
                    return tick;
                }
            }

            return -1;
        }

        private static string Signature(Result result, int tick)
        {
            return StateInstrument.CompleteSignature(result.States[tick], result.Parcel.Places);
        }

        private static string SurfaceAt(Result result, int tick)
        {
            return StateInstrument.SurfaceSignature(result.States[tick], result.Parcel.Places);
        }

        private static long Minimum(Result result)
        {
            long minimum = long.MaxValue;
            foreach (SimulationState state in result.States)
            {
                minimum = Math.Min(minimum,
                    StateInstrument.Minimum(state, result.Parcel.Places));
            }

            return minimum;
        }

        private static string MatterRange(Result result)
        {
            return $"{Surface.MatterTotal(result.States[0], result.Parcel.Places)}→" +
                $"{Surface.MatterTotal(result.States[result.States.Count - 1], result.Parcel.Places)}";
        }

        private static string RepeatLine(Result result)
        {
            StateInstrument.Repeat repeat =
                StateInstrument.FirstRepeat(result.States, result.Parcel.Places);
            return repeat == null
                ? "no repeat"
                : $"t{repeat.First}=t{repeat.Again}, period {repeat.Period}";
        }

        private static string ConversionLine(IReadOnlyDictionary<int, long> conversions)
        {
            var lines = new List<string>();
            foreach (KeyValuePair<int, long> pair in conversions)
            {
                if (pair.Value > 0)
                {
                    lines.Add($"{pair.Key}:{pair.Value}");
                }
            }

            lines.Sort(StringComparer.Ordinal);
            return lines.Count == 0 ? "(none)" : string.Join(" ", lines);
        }

        private static string CrossingLine(Parcel5 parcel, int boundary)
        {
            var lines = new List<string>();
            foreach (ExternalEvent crossing in parcel.Crossings.Events)
            {
                if (crossing.Boundary.Value == boundary)
                {
                    lines.Add($"{crossing.Target.Place.Value}:{crossing.Target.Kind.Value}:{crossing.Amount}");
                }
            }

            return lines.Count == 0 ? "(none)" : string.Join(" ", lines);
        }

        private static string Verdict(bool held) => held ? "held" : "**FAILED**";
    }
}
