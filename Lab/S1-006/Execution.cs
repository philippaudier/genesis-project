using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;
using Genesis.Lab.S1_005;

namespace Genesis.Lab.S1_006
{
    /// <summary>
    /// Gate-7 execution driver. It records the sealed pair and classifies it without reduction.
    /// </summary>
    public static class Execution
    {
        public sealed class Result
        {
            public Parcel6 Parcel;
            public List<SimulationState> States = new List<SimulationState>();
            public List<IReadOnlyList<Provenance.Entry>> Provenance =
                new List<IReadOnlyList<Provenance.Entry>>();
            public List<IReadOnlyList<string>> Prospectives =
                new List<IReadOnlyList<string>>();
            public List<IReadOnlyDictionary<int, long>> Conversions =
                new List<IReadOnlyDictionary<int, long>>();
            public List<string> ReconstructionFaults = new List<string>();
            public List<string> AllocationSensitive = new List<string>();
        }

        public static int RunAll(string root)
        {
            Directory.CreateDirectory(root);
            Result p0 = Run(Worlds6.P0());
            Result p1 = Run(Worlds6.P1());
            WriteWorld(Path.Combine(root, "P0"), p0);
            WriteWorld(Path.Combine(root, "P1"), p1);
            string report = Report(p0, p1);
            File.WriteAllText(Path.Combine(root, "REPORT.md"), report);
            Console.WriteLine(report);
            Console.WriteLine($"Artifacts under {Path.GetFullPath(root)}.");
            return 0;
        }

        private static Result Run(Parcel6 parcel)
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
                result.Prospectives.Add(Prospectives(state, parcel, boundary, result));
                result.Conversions.Add(Surface.ConversionsAt(state, parcel.Relations,
                    parcel.Places, Divisors.DegreeAware, Worlds6.ConversionThreshold));

                SimulationState after = runner.Run(state, parcel.Relations,
                    parcel.Set.Transitions, parcel.Crossings, 1);
                AuditReconstruction(result, state, after, entries, boundary);
                result.States.Add(after);
                state = after;
            }
            return result;
        }

        private static IReadOnlyList<string> Prospectives(SimulationState state,
            Parcel6 parcel, long boundary, Result result)
        {
            var lines = new List<string>();
            foreach (Place place in parcel.Places)
            {
                long here = K4.WaterPotential(state, place);
                long holding = state.ValueAt(new Cell(place, K4.Sediment));
                var eligible = new List<long>();
                foreach (Relation relation in parcel.Relations.OutgoingFrom(place))
                {
                    long diff = here - K4.WaterPotential(state, relation.Target);
                    long prospective = diff > 0
                        ? diff / Divisors.DegreeAware(parcel.Relations.OutgoingFrom(place).Count)
                        : 0;
                    bool passes = prospective > parcel.Competence;
                    if (passes) eligible.Add(prospective);
                    lines.Add($"{place.Value}->{relation.Target.Value}:water={prospective}," +
                        $"sediment={(passes ? Math.Min(holding, prospective) : 0)}," +
                        $"guard={(passes ? "pass" : "block")}");
                }
                if (Instrument6.AllocationSensitive(holding, eligible.ToArray()))
                {
                    result.AllocationSensitive.Add(
                        $"boundary {boundary}, place {place.Value}, holding {holding}, " +
                        $"eligible [{string.Join(",", eligible)}]");
                }
            }
            return lines;
        }

        private static void AuditReconstruction(Result result, SimulationState before,
            SimulationState after, IReadOnlyList<Provenance.Entry> entries, long boundary)
        {
            var delta = new Dictionary<Cell, long>();
            foreach (Cell cell in before.Cells) delta[cell] = 0;
            foreach (Provenance.Entry entry in entries) delta[entry.Target] += entry.Amount;
            foreach (ExternalEvent crossing in result.Parcel.Crossings.Events)
            {
                if (crossing.Boundary.Value == boundary) delta[crossing.Target] += crossing.Amount;
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
                SimulationState state = result.States[tick];
                string signature = StateInstrument.CompleteSignature(state, result.Parcel.Places);
                foreach (Place place in result.Parcel.Places)
                {
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
                readings.Append($"=== boundary {boundary} ===\n")
                    .Append("  prospectives : ")
                    .Append(string.Join(" | ", result.Prospectives[boundary])).Append('\n')
                    .Append("  conversions  : ")
                    .Append(ConversionLine(result.Conversions[boundary])).Append('\n')
                    .Append("  crossings    : ")
                    .Append(CrossingLine(result.Parcel, boundary)).Append('\n')
                    .Append("  contributions:\n");
                if (result.Provenance[boundary].Count == 0) readings.Append("      (none)\n");
                foreach (Provenance.Entry entry in result.Provenance[boundary])
                    readings.Append("      ").Append(entry).Append('\n');
            }
            File.WriteAllText(Path.Combine(directory, "readings.txt"), readings.ToString());

            var audit = new StringBuilder()
                .Append("reconstruction faults: ").Append(result.ReconstructionFaults.Count).Append('\n');
            foreach (string fault in result.ReconstructionFaults)
                audit.Append("  ").Append(fault).Append('\n');
            IReadOnlyList<string> matter = Surface.MatterAudit(result.States, result.Parcel.Places);
            audit.Append("matter faults: ").Append(matter.Count).Append('\n');
            foreach (string fault in matter) audit.Append("  ").Append(fault).Append('\n');
            audit.Append("minimum: ").Append(Minimum(result)).Append('\n')
                .Append("allocation-sensitive boundaries: ")
                .Append(result.AllocationSensitive.Count).Append('\n');
            foreach (string item in result.AllocationSensitive)
                audit.Append("  ").Append(item).Append('\n');
            Instrument6.FixedWitness fixedPoint = Fixed(result);
            audit.Append("first fixed transition: ")
                .Append(fixedPoint == null
                    ? "(none)"
                    : $"boundary {fixedPoint.Boundary}, quiet suffix {fixedPoint.QuietSuffix}")
                .Append('\n');
            File.WriteAllText(Path.Combine(directory, "audit.txt"), audit.ToString());
        }

        private static string Report(Result p0, Result p1)
        {
            bool c0 = PrefixExact(p0) && PrefixExact(p1) &&
                FirstDivergence(p0, p1) >= 7;
            bool c1 = FirstDivergence(p0, p1) == 7 &&
                BoundarySixGuardDifference(p0, p1);
            bool c2 = ValidAccounting(p0) && ValidAccounting(p1);
            Instrument6.FixedWitness fixed0 = Fixed(p0);
            Instrument6.FixedWitness fixed1 = Fixed(p1);
            bool c3 = fixed0 != null && fixed1 != null;
            string finalSurface0 = SurfaceAt(p0, p0.States.Count - 1);
            string finalSurface1 = SurfaceAt(p1, p1.States.Count - 1);
            bool c4 = finalSurface0 != finalSurface1;
            bool c5 = StrictPair(p0.Parcel, p1.Parcel) && c1 &&
                p0.ReconstructionFaults.Count == 0 && p1.ReconstructionFaults.Count == 0;
            bool c6 = CrossingsExact(p0.Parcel) && CrossingsExact(p1.Parcel);
            bool valid = c0 && c2 && c6;
            bool completeEqual = Signature(p0, 128) == Signature(p1, 128);
            string outcome = Instrument6.Classify(valid, c5, c1, c3,
                !c4, completeEqual);

            var report = new StringBuilder();
            report.AppendLine("# S1-006 execution report").AppendLine();
            report.AppendLine("Seal: `0cc83b0`; instrument: `c98f6a5`; conformance: `3b5dea4`; execution authorised 2026-07-31.")
                .AppendLine();
            report.AppendLine("## Mechanical record").AppendLine();
            report.AppendLine("| Claim | Held | P0 | P1 |")
                .AppendLine("|---|---|---|---|")
                .AppendLine($"| C0 common prefix | {Verdict(c0)} | sealed states through t6 | sealed states through t6 |")
                .AppendLine($"| C1 first discrimination | {Verdict(c1)} | first complete difference t{FirstDivergence(p0, p1)}; boundary 6 C→D unit moves | boundary 6 C→D unit blocked |")
                .AppendLine($"| C2 accounting domain | {Verdict(c2)} | matter {MatterRange(p0)}, min {Minimum(p0)}, reconstruction faults {p0.ReconstructionFaults.Count} | matter {MatterRange(p1)}, min {Minimum(p1)}, reconstruction faults {p1.ReconstructionFaults.Count} |")
                .AppendLine($"| C3 common stability | {Verdict(c3)} | {FixedLine(fixed0)} | {FixedLine(fixed1)} |")
                .AppendLine($"| C4 selected form | {Verdict(c4)} | final {finalSurface0} | final {finalSurface1} |")
                .AppendLine($"| C5 attribution | {Verdict(c5)} | strict pair and reconstructed suffix | strict pair and reconstructed suffix |")
                .AppendLine($"| C6 repeated history | {Verdict(c6)} | boundaries 0,1,10,11 | boundaries 0,1,10,11 |")
                .AppendLine();
            report.AppendLine("## Selected surface checkpoints").AppendLine()
                .AppendLine("| tick | P0 | P1 |").AppendLine("|---:|---|---|");
            foreach (int tick in new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 16, 32, 64, 96, 128 })
                report.AppendLine($"| {tick} | {SurfaceAt(p0, tick)} | {SurfaceAt(p1, tick)} |");
            report.AppendLine().AppendLine("## Allocation sensitivity").AppendLine()
                .AppendLine($"P0: {p0.AllocationSensitive.Count} witnessed boundary/place records.")
                .AppendLine($"P1: {p1.AllocationSensitive.Count} witnessed boundary/place records.")
                .AppendLine();
            report.AppendLine("## Classification").AppendLine()
                .AppendLine($"**Outcome {outcome} — {OutcomeName(outcome)}.**")
                .AppendLine()
                .AppendLine("Classification precedence applied as sealed: G → F → E → D → B → C → A.")
                .AppendLine()
                .AppendLine("## Decision").AppendLine()
                .AppendLine("Withheld. Reduction was not authorised.");
            return report.ToString();
        }

        private static Instrument6.FixedWitness Fixed(Result result)
        {
            var contributions = new List<int>();
            var crossings = new List<int>();
            for (int boundary = 0; boundary < result.Provenance.Count; boundary++)
            {
                contributions.Add(result.Provenance[boundary].Count);
                crossings.Add(CrossingCount(result.Parcel, boundary));
            }
            return Instrument6.FirstFixed(result.States, result.Parcel.Places,
                contributions, crossings);
        }

        private static bool PrefixExact(Result result)
        {
            long[][] rock =
            {
                new long[]{12,12,12,12}, new long[]{12,12,12,12},
                new long[]{11,12,12,12}, new long[]{10,12,12,12},
                new long[]{10,12,12,12}, new long[]{10,12,12,12},
                new long[]{10,12,12,12}
            };
            long[][] sediment =
            {
                new long[]{0,0,0,0}, new long[]{0,0,0,0}, new long[]{1,0,0,0},
                new long[]{1,1,0,0}, new long[]{1,0,1,0}, new long[]{0,1,0,1},
                new long[]{0,0,1,1}
            };
            long[][] water =
            {
                new long[]{0,0,0,0}, new long[]{12,0,0,0}, new long[]{18,6,0,0},
                new long[]{12,10,2,0}, new long[]{12,7,5,0}, new long[]{10,9,3,2},
                new long[]{10,7,5,2}
            };
            for (int tick = 0; tick <= 6; tick++)
                for (int i = 0; i < 4; i++)
                {
                    Place place = result.Parcel.Places[i];
                    SimulationState state = result.States[tick];
                    if (state.ValueAt(new Cell(place, K4.Rock)) != rock[tick][i] ||
                        state.ValueAt(new Cell(place, K4.Sediment)) != sediment[tick][i] ||
                        state.ValueAt(new Cell(place, K4.Water)) != water[tick][i]) return false;
                }
            return true;
        }

        private static bool BoundarySixGuardDifference(Result p0, Result p1)
        {
            List<string> a = ContributionLines(p0.Provenance[6]);
            List<string> b = ContributionLines(p1.Provenance[6]);
            foreach (string common in b) a.Remove(common);
            a.Sort(StringComparer.Ordinal);
            return string.Join("|", a) ==
                "CompetenceTransportFixture:2:4:-1|CompetenceTransportFixture:3:4:1";
        }

        private static List<string> ContributionLines(IReadOnlyList<Provenance.Entry> entries)
        {
            var lines = new List<string>();
            foreach (Provenance.Entry entry in entries)
                lines.Add($"{entry.Fixture}:{entry.Target.Place.Value}:{entry.Target.Kind.Value}:{entry.Amount}");
            return lines;
        }

        private static bool StrictPair(Parcel6 p0, Parcel6 p1)
        {
            if (p0.Competence != 0 || p1.Competence != 1 ||
                p0.Relations.Count != p1.Relations.Count ||
                p0.Fixtures.Count != p1.Fixtures.Count) return false;
            for (int i = 0; i < p0.Fixtures.Count; i++)
                if (p0.Fixtures[i].GetType() != p1.Fixtures[i].GetType()) return false;
            return StateInstrument.CompleteSignature(p0.Initial, p0.Places) ==
                StateInstrument.CompleteSignature(p1.Initial, p1.Places) &&
                CrossingsExact(p0) && CrossingsExact(p1);
        }

        private static bool ValidAccounting(Result result) =>
            result.ReconstructionFaults.Count == 0 &&
            Surface.MatterAudit(result.States, result.Parcel.Places).Count == 0 &&
            Surface.MatterTotal(result.States[0], result.Parcel.Places) == 48 &&
            Minimum(result) >= 0;

        private static bool CrossingsExact(Parcel6 parcel) =>
            Instrument6.CrossingsMatch(parcel.Crossings.Events,
                new long[] { 0, 1, 10, 11 },
                new Cell(parcel.Places[0], K4.Water), 12);

        private static int FirstDivergence(Result a, Result b)
        {
            for (int tick = 0; tick < a.States.Count; tick++)
                if (Signature(a, tick) != Signature(b, tick)) return tick;
            return -1;
        }

        private static string Signature(Result result, int tick) =>
            StateInstrument.CompleteSignature(result.States[tick], result.Parcel.Places);
        private static string SurfaceAt(Result result, int tick) =>
            StateInstrument.SurfaceSignature(result.States[tick], result.Parcel.Places);

        private static long Minimum(Result result)
        {
            long minimum = long.MaxValue;
            foreach (SimulationState state in result.States)
                minimum = Math.Min(minimum, StateInstrument.Minimum(state, result.Parcel.Places));
            return minimum;
        }

        private static string MatterRange(Result result) =>
            $"{Surface.MatterTotal(result.States[0], result.Parcel.Places)}→" +
            $"{Surface.MatterTotal(result.States[result.States.Count - 1], result.Parcel.Places)}";

        private static int CrossingCount(Parcel6 parcel, int boundary)
        {
            int count = 0;
            foreach (ExternalEvent crossing in parcel.Crossings.Events)
                if (crossing.Boundary.Value == boundary) count++;
            return count;
        }

        private static string CrossingLine(Parcel6 parcel, int boundary)
        {
            var lines = new List<string>();
            foreach (ExternalEvent crossing in parcel.Crossings.Events)
                if (crossing.Boundary.Value == boundary)
                    lines.Add($"{crossing.Target.Place.Value}:{crossing.Target.Kind.Value}:{crossing.Amount}");
            return lines.Count == 0 ? "(none)" : string.Join(" ", lines);
        }

        private static string ConversionLine(IReadOnlyDictionary<int, long> conversions)
        {
            var lines = new List<string>();
            foreach (KeyValuePair<int, long> pair in conversions)
                if (pair.Value > 0) lines.Add($"{pair.Key}:{pair.Value}");
            lines.Sort(StringComparer.Ordinal);
            return lines.Count == 0 ? "(none)" : string.Join(" ", lines);
        }

        private static string FixedLine(Instrument6.FixedWitness witness) =>
            witness == null ? "none by boundary 127" :
            $"boundary {witness.Boundary}, quiet suffix {witness.QuietSuffix}";
        private static string Verdict(bool held) => held ? "held" : "**FAILED**";

        private static string OutcomeName(string outcome)
        {
            switch (outcome)
            {
                case "A": return "form selected";
                case "B": return "hidden selection only";
                case "C": return "selection erased";
                case "D": return "stability is not common within the window";
                case "E": return "mechanism incomplete";
                case "F": return "strict pair broken";
                default: return "invalid evidence";
            }
        }
    }
}
