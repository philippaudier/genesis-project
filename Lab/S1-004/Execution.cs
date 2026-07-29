using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.S1_004
{
    /// <summary>
    /// The S1-004 execution driver, added under the founder's gate-8 authorisation (2026-07-29).
    /// It runs M0 and M1 exactly as sealed, records the thirteen sealed measurements, checks
    /// C0–C5, and classifies against outcomes A–F. It interprets nothing: the reduction is a
    /// separate authorisation and is not performed here.
    ///
    /// Indexing convention, as sealed: state[b] is the world *after* tick b; boundary b acts on
    /// state[b] and produces state[b+1].
    /// </summary>
    public static class Execution
    {
        public sealed class Result
        {
            public Parcel4 Parcel;
            public List<SimulationState> States = new List<SimulationState>();
            public List<IReadOnlyList<Provenance.Entry>> Provenance = new List<IReadOnlyList<Provenance.Entry>>();
            public List<IReadOnlyList<string>> WaterFlux = new List<IReadOnlyList<string>>();
            public List<IReadOnlyList<string>> SedimentFlux = new List<IReadOnlyList<string>>();
            public List<IReadOnlyDictionary<int, long>> Conversions = new List<IReadOnlyDictionary<int, long>>();
        }

        public static int RunAll(string root)
        {
            Directory.CreateDirectory(root);
            Result m0 = Execute(Worlds4.M0());
            Result m1 = Execute(Worlds4.M1());

            WriteWorld(Path.Combine(root, "M0"), m0);
            WriteWorld(Path.Combine(root, "M1"), m1);
            string report = Report(m0, m1);
            File.WriteAllText(Path.Combine(root, "REPORT.md"), report);

            Console.WriteLine(report);
            Console.WriteLine($"Artifacts under {Path.GetFullPath(root)}.");
            return 0;
        }

        private static Result Execute(Parcel4 parcel)
        {
            var result = new Result { Parcel = parcel };
            var runner = new TickRunner(new TransitionRunner(parcel.Set.Resolvers));
            SimulationState state = parcel.Initial;
            result.States.Add(state);

            for (long b = 0; b < parcel.Ticks; b++)
            {
                result.Provenance.Add(Provenance.Collect(state, parcel.Relations, parcel.Fixtures));
                result.WaterFlux.Add(Surface.Flux(state, parcel.Relations, parcel.Places,
                    K4.Water, Worlds4.Constant2, cappedByHolding: false));
                result.SedimentFlux.Add(Surface.Flux(state, parcel.Relations, parcel.Places,
                    K4.Sediment, Worlds4.Constant2, cappedByHolding: true));
                result.Conversions.Add(Surface.ConversionsAt(state, parcel.Relations, parcel.Places,
                    Worlds4.Constant2, Worlds4.Threshold));

                state = runner.Run(state, parcel.Relations, parcel.Set.Transitions, parcel.Crossings, 1);
                result.States.Add(state);
            }

            return result;
        }

        // --- artifacts ------------------------------------------------------------------------

        private static void WriteWorld(string directory, Result result)
        {
            Directory.CreateDirectory(directory);
            IReadOnlyList<Place> places = result.Parcel.Places;

            var states = new StringBuilder("tick,place,base,rock,sediment,water,solidsurface,localmatter\n");
            for (int t = 0; t < result.States.Count; t++)
            {
                foreach (Place place in places)
                {
                    states.Append(t).Append(',').Append(place.Value).Append(',')
                          .Append(result.States[t].ValueAt(new Cell(place, K4.Base))).Append(',')
                          .Append(result.States[t].ValueAt(new Cell(place, K4.Rock))).Append(',')
                          .Append(result.States[t].ValueAt(new Cell(place, K4.Sediment))).Append(',')
                          .Append(result.States[t].ValueAt(new Cell(place, K4.Water))).Append(',')
                          .Append(K4.SolidSurface(result.States[t], place)).Append(',')
                          .Append(Surface.LocalMatter(result.States[t], place)).Append('\n');
                }
            }

            File.WriteAllText(Path.Combine(directory, "states.csv"), states.ToString());

            var readings = new StringBuilder();
            for (int b = 0; b < result.Provenance.Count; b++)
            {
                readings.Append($"=== boundary {b} (acts on the state after tick {b}) ===\n");
                readings.Append("  water flux    : ").Append(Join(result.WaterFlux[b])).Append('\n');
                readings.Append("  sediment flux : ").Append(Join(result.SedimentFlux[b])).Append('\n');
                readings.Append("  conversions   : ").Append(ConversionLine(result.Conversions[b])).Append('\n');
                readings.Append("  contributions :\n");
                if (result.Provenance[b].Count == 0)
                {
                    readings.Append("      (none)\n");
                }

                foreach (Provenance.Entry entry in result.Provenance[b])
                {
                    readings.Append("      ").Append(entry).Append('\n');
                }

                IReadOnlyList<KeyValuePair<Cell, List<Provenance.Entry>>> collisions =
                    Provenance.Collisions(result.Provenance[b]);
                foreach (KeyValuePair<Cell, List<Provenance.Entry>> collision in collisions)
                {
                    readings.Append($"      COLLISION at place({collision.Key.Place.Value}) kind({collision.Key.Kind.Value}): ");
                    foreach (Provenance.Entry entry in collision.Value)
                    {
                        readings.Append(entry.Fixture).Append(' ').Append(entry.Amount >= 0 ? "+" : "").Append(entry.Amount).Append("  ");
                    }

                    readings.Append('\n');
                }
            }

            File.WriteAllText(Path.Combine(directory, "readings.txt"), readings.ToString());

            var spy = new StringBuilder("resolver invocations (the local witness)\n");
            foreach (SpyResolver resolver in result.Parcel.Spies)
            {
                foreach (SpyResolver.Invocation invocation in resolver.Invocations)
                {
                    spy.Append("  ").Append(invocation).Append('\n');
                }
            }

            if (spy.Length == "resolver invocations (the local witness)\n".Length)
            {
                spy.Append("  (none — no cell was contested)\n");
            }

            File.WriteAllText(Path.Combine(directory, "resolver.txt"), spy.ToString());
        }

        private static string Join(IReadOnlyList<string> items)
        {
            return items.Count == 0 ? "(none)" : string.Join(" ", items);
        }

        private static string ConversionLine(IReadOnlyDictionary<int, long> conversions)
        {
            var parts = new List<string>();
            var keys = new List<int>(conversions.Keys);
            keys.Sort();
            foreach (int key in keys)
            {
                if (conversions[key] > 0)
                {
                    parts.Add($"place({key}):{conversions[key]}");
                }
            }

            return parts.Count == 0 ? "(none)" : string.Join(" ", parts);
        }

        // --- readings of record ------------------------------------------------------------

        private static string Surfaces(Result result, int tick)
        {
            long[] reading = Surface.SolidSurfaces(result.States[tick], result.Parcel.Places);
            return "[" + string.Join(",", reading) + "]";
        }

        private static int FirstBoundaryWith(List<IReadOnlyList<string>> flux)
        {
            for (int b = 0; b < flux.Count; b++)
            {
                if (flux[b].Count > 0)
                {
                    return b;
                }
            }

            return -1;
        }

        private static int FirstConversionBoundary(Result result)
        {
            for (int b = 0; b < result.Conversions.Count; b++)
            {
                foreach (KeyValuePair<int, long> entry in result.Conversions[b])
                {
                    if (entry.Value > 0)
                    {
                        return b;
                    }
                }
            }

            return -1;
        }

        private static int FirstCollisionBoundary(Result result)
        {
            for (int b = 0; b < result.Provenance.Count; b++)
            {
                if (Provenance.Collisions(result.Provenance[b]).Count > 0)
                {
                    return b;
                }
            }

            return -1;
        }

        private static int FirstSurfaceChangeTick(Result result)
        {
            long[] initial = Surface.SolidSurfaces(result.States[0], result.Parcel.Places);
            for (int t = 1; t < result.States.Count; t++)
            {
                long[] now = Surface.SolidSurfaces(result.States[t], result.Parcel.Places);
                for (int i = 0; i < now.Length; i++)
                {
                    if (now[i] != initial[i])
                    {
                        return t;
                    }
                }
            }

            return -1;
        }

        private static int FirstDivergenceTick(Result a, Result b)
        {
            for (int t = 0; t < Math.Min(a.States.Count, b.States.Count); t++)
            {
                long[] sa = Surface.SolidSurfaces(a.States[t], a.Parcel.Places);
                long[] sb = Surface.SolidSurfaces(b.States[t], b.Parcel.Places);
                for (int i = 0; i < sa.Length; i++)
                {
                    if (sa[i] != sb[i])
                    {
                        return t;
                    }
                }
            }

            return -1;
        }

        private static long MinOf(Result result, Kind kind)
        {
            long min = long.MaxValue;
            foreach (SimulationState state in result.States)
            {
                foreach (Place place in result.Parcel.Places)
                {
                    long value = state.ValueAt(new Cell(place, kind));
                    if (value < min)
                    {
                        min = value;
                    }
                }
            }

            return min;
        }

        // --- claims ---------------------------------------------------------------------------

        private static bool C0(Result result) => result.Provenance[0].Count == 0;

        private static bool C1(Result result) =>
            Surface.MatterAudit(result.States, result.Parcel.Places).Count == 0;

        /// <summary>Every conversion fixture's contributions at a place sum to zero across kinds.</summary>
        private static bool C2(Result result)
        {
            for (int b = 0; b < result.Provenance.Count; b++)
            {
                var byPlace = new Dictionary<int, long>();
                foreach (Provenance.Entry entry in result.Provenance[b])
                {
                    if (!entry.Fixture.Contains("Conversion"))
                    {
                        continue;
                    }

                    byPlace.TryGetValue(entry.Target.Place.Value, out long sum);
                    byPlace[entry.Target.Place.Value] = sum + entry.Amount;
                }

                foreach (KeyValuePair<int, long> entry in byPlace)
                {
                    if (entry.Value != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool C3(Result m0) => FirstSurfaceChangeTick(m0) == -1;

        private static bool C4(Result result)
        {
            foreach (SimulationState state in result.States)
            {
                foreach (Place place in result.Parcel.Places)
                {
                    if (state.ValueAt(new Cell(place, K4.Base)) != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>C5: the two witnesses agree, and the contested cell committed 0.</summary>
        private static string C5(Result m1, out bool passed)
        {
            int boundary = FirstCollisionBoundary(m1);
            if (boundary < 0)
            {
                passed = false;
                return "no cell was contested — C5 not exercised";
            }

            IReadOnlyList<KeyValuePair<Cell, List<Provenance.Entry>>> collisions =
                Provenance.Collisions(m1.Provenance[boundary]);
            var lines = new List<string>();
            int sedimentCollisions = 0;
            foreach (KeyValuePair<Cell, List<Provenance.Entry>> collision in collisions)
            {
                if (collision.Key.Kind == K4.Sediment)
                {
                    sedimentCollisions++;
                }

                var amounts = new List<long>();
                foreach (Provenance.Entry entry in collision.Value)
                {
                    amounts.Add(entry.Amount);
                }

                lines.Add($"place({collision.Key.Place.Value}) kind({collision.Key.Kind.Value}) [{string.Join(",", amounts)}]");
            }

            long invocations = 0;
            long committed = 0;
            foreach (SpyResolver resolver in m1.Parcel.Spies)
            {
                foreach (SpyResolver.Invocation invocation in resolver.Invocations)
                {
                    invocations++;
                    committed += invocation.Committed;
                }
            }

            passed = sedimentCollisions == 1 && invocations == collisions.Count && committed == 0;
            return $"boundary {boundary}: {string.Join(" ", lines)}; resolver invocations {invocations}, committed sum {committed}";
        }

        // --- classification --------------------------------------------------------------------

        private static string Classify(Result m0, Result m1, out string letter)
        {
            bool instrumentsOk = C1(m0) && C1(m1) && C2(m0) && C2(m1) && C4(m0) && C4(m1);
            C5(m1, out bool c5);

            if (!instrumentsOk || !c5)
            {
                letter = "E";
                return "accounting or instrument failure";
            }

            if (!C3(m0))
            {
                letter = "D";
                return "attribution failure: M0's SolidSurface changed";
            }

            if (FirstConversionBoundary(m1) < 0)
            {
                letter = "C";
                return "the forcing never reached the conversion domain";
            }

            int transportBoundary = FirstBoundaryWith(m1.SedimentFlux);
            int surfaceTick = FirstSurfaceChangeTick(m1);
            if (transportBoundary < 0 || surfaceTick < 0)
            {
                letter = "B";
                return "conversion occurred, but no transport and/or no changed surface";
            }

            bool asDerived =
                FirstBoundaryWith(m1.WaterFlux) == 1 &&
                FirstConversionBoundary(m1) == 1 &&
                transportBoundary == 2 &&
                surfaceTick == 3 &&
                Surfaces(m1, 3) == "[9,11,10]";

            letter = asDerived ? "A" : "F";
            return asDerived
                ? "first changed surface, exactly as derived"
                : "the surface changed, but not as derived";
        }

        // --- the austere report -----------------------------------------------------------------

        private static string Report(Result m0, Result m1)
        {
            var text = new StringBuilder();
            text.Append("# Campaign S1-004 — Execution Report\n\n");
            text.Append("Sealed design: `b114a34`. Conformance: `76ccaf1` (CONFORMING). ");
            text.Append("M0 and M1 run six ticks exactly as sealed. Claims, statuses and record facts only — ");
            text.Append("no interpretation. The reduction is not authorised and is not here.\n\n");

            string classification = Classify(m0, m1, out string letter);

            text.Append("## Bound first events\n\n");
            text.Append("| Event | Derived | Record |\n|---|---|---|\n");
            text.Append($"| first Water flux | boundary 1, A→B, 5 | boundary {FirstBoundaryWith(m1.WaterFlux)}, {Join(m1.WaterFlux[Math.Max(FirstBoundaryWith(m1.WaterFlux), 0)])} |\n");
            text.Append($"| first conversion | boundary 1 at A | boundary {FirstConversionBoundary(m1)}, {ConversionLine(m1.Conversions[Math.Max(FirstConversionBoundary(m1), 0)])} |\n");
            int tb = FirstBoundaryWith(m1.SedimentFlux);
            text.Append($"| first Sediment transport (M1) | boundary 2, A→B, 1 | boundary {tb}, {(tb >= 0 ? Join(m1.SedimentFlux[tb]) : "(never)")} |\n");
            string c5line = C5(m1, out bool c5ok);
            text.Append($"| first cross-fixture conflict (M1) | boundary 2, (A,Sediment), (+1,−1) → 0 | {c5line} |\n");
            text.Append($"| first changed SolidSurface (M1) | after tick 3, [9,11,10] | after tick {FirstSurfaceChangeTick(m1)}, {Surfaces(m1, Math.Max(FirstSurfaceChangeTick(m1), 0))} |\n");
            text.Append($"| M0 SolidSurface | unchanged throughout | {(C3(m0) ? "unchanged throughout" : "CHANGED")} |\n");
            text.Append($"| first tick the two worlds' surfaces differ | after tick 3 | after tick {FirstDivergenceTick(m0, m1)} |\n\n");

            text.Append("## Claims\n\n");
            text.Append("| # | Claim | M0 | M1 |\n|---|---|---|---|\n");
            text.Append($"| C0 | dry stasis: no contribution before the crossings | {Verdict(C0(m0))} | {Verdict(C0(m1))} |\n");
            text.Append($"| C1 | Rock + Sediment invariant | {Verdict(C1(m0))} | {Verdict(C1(m1))} |\n");
            text.Append($"| C2 | each conversion pair is locally zero-sum | {Verdict(C2(m0))} | {Verdict(C2(m1))} |\n");
            text.Append($"| C3 | M0's SolidSurface never changes | {Verdict(C3(m0))} | — |\n");
            text.Append($"| C4 | Base never changes | {Verdict(C4(m0))} | {Verdict(C4(m1))} |\n");
            text.Append($"| C5 | the contested cell: two witnesses agree, committed 0 | — | {Verdict(c5ok)} |\n\n");

            text.Append("## Surfaces, tick by tick\n\n");
            text.Append("| after tick | M0 | M1 |\n|---|---|---|\n");
            for (int t = 0; t < m0.States.Count; t++)
            {
                text.Append($"| {t} | {Surfaces(m0, t)} | {Surfaces(m1, t)} |\n");
            }

            text.Append("\n## Matter and positivity\n\n");
            text.Append($"- Rock + Sediment, M0: {Surface.MatterTotal(m0.States[0], m0.Parcel.Places)} → {Surface.MatterTotal(m0.States[m0.States.Count - 1], m0.Parcel.Places)}; audit faults: {Surface.MatterAudit(m0.States, m0.Parcel.Places).Count}\n");
            text.Append($"- Rock + Sediment, M1: {Surface.MatterTotal(m1.States[0], m1.Parcel.Places)} → {Surface.MatterTotal(m1.States[m1.States.Count - 1], m1.Parcel.Places)}; audit faults: {Surface.MatterAudit(m1.States, m1.Parcel.Places).Count}\n");
            text.Append($"- minima M0 — Base {MinOf(m0, K4.Base)}, Rock {MinOf(m0, K4.Rock)}, Sediment {MinOf(m0, K4.Sediment)}, Water {MinOf(m0, K4.Water)}\n");
            text.Append($"- minima M1 — Base {MinOf(m1, K4.Base)}, Rock {MinOf(m1, K4.Rock)}, Sediment {MinOf(m1, K4.Sediment)}, Water {MinOf(m1, K4.Water)}\n");

            text.Append("\n## Classification\n\n");
            text.Append($"**Outcome {letter}** — {classification}.\n");

            text.Append("\n## Traces\n\n");
            text.Append("`Runs/M0/` and `Runs/M1/`: `states.csv` (every kind, every place, every tick, plus the ");
            text.Append("SolidSurface and local-matter readings), `readings.txt` (per boundary: water flux, sediment ");
            text.Append("flux, conversions, every fixture's contributions, and any contested cell), `resolver.txt` ");
            text.Append("(the resolver's own record of what it was handed and what it committed).\n");

            text.Append("\n## Decision\n\nWithheld. The reduction is a separate authorisation.\n");
            return text.ToString();
        }

        private static string Verdict(bool ok) => ok ? "held" : "**FAILED**";
    }
}
