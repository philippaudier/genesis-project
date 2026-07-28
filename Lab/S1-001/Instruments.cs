using System;
using System.Collections.Generic;
using System.Text;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// Drives a parcel tick by tick and keeps every intermediate state — the record the instruments
    /// read. The harness knows FixtureSet, never a fixture; it mutates nothing (each tick is one
    /// observed-loop call into the kernel's runner).
    /// </summary>
    public sealed class ParcelRun
    {
        public Parcel Parcel { get; }
        public IReadOnlyList<SimulationState> States => _states;

        private readonly List<SimulationState> _states = new List<SimulationState>();

        private ParcelRun(Parcel parcel)
        {
            Parcel = parcel;
        }

        public static ParcelRun Execute(Parcel parcel, long ticks)
        {
            var run = new ParcelRun(parcel);
            var tickRunner = new TickRunner(new TransitionRunner(parcel.Fixtures.Resolvers));
            SimulationState state = parcel.Initial;
            run._states.Add(state);
            for (long t = 0; t < ticks; t++)
            {
                state = tickRunner.Run(state, parcel.Relations, parcel.Fixtures.Transitions, parcel.Trace, 1);
                run._states.Add(state);
            }

            return run;
        }
    }

    /// <summary>Per-kind totals per tick, computed from the record alone.</summary>
    public static class PerKindLedger
    {
        public static IReadOnlyList<IReadOnlyDictionary<Kind, long>> Totals(IReadOnlyList<SimulationState> states)
        {
            var result = new List<IReadOnlyDictionary<Kind, long>>();
            foreach (SimulationState state in states)
            {
                var totals = new Dictionary<Kind, long>();
                foreach (Cell cell in state.Cells)
                {
                    totals.TryGetValue(cell.Kind, out long sum);
                    totals[cell.Kind] = sum + state.ValueAt(cell);
                }

                result.Add(totals);
            }

            return result;
        }
    }

    /// <summary>
    /// The conservation audit: checks that the summed total of a declared kind-set is invariant
    /// tick over tick, net of declared external crossings. Reports every violating boundary.
    /// A silent audit on the B3 world refutes the instrument, not the world.
    /// </summary>
    public static class ConservationAudit
    {
        public sealed class Violation
        {
            public long FromTick { get; }
            public long Expected { get; }
            public long Found { get; }

            public Violation(long fromTick, long expected, long found)
            {
                FromTick = fromTick;
                Expected = expected;
                Found = found;
            }

            public override string ToString() => $"tick {FromTick}->{FromTick + 1}: expected {Expected}, found {Found}";
        }

        public static IReadOnlyList<Violation> Check(
            IReadOnlyList<SimulationState> states,
            IReadOnlyCollection<Kind> kindSet,
            ExternalEventTrace trace)
        {
            var ledger = PerKindLedger.Totals(states);
            var violations = new List<Violation>();
            for (int t = 0; t + 1 < ledger.Count; t++)
            {
                long before = SetTotal(ledger[t], kindSet);
                long crossings = CrossingsInto(trace, kindSet, states[t].CurrentTick);
                long after = SetTotal(ledger[t + 1], kindSet);
                if (after != before + crossings)
                {
                    violations.Add(new Violation(states[t].CurrentTick.Value, before + crossings, after));
                }
            }

            return violations;
        }

        private static long SetTotal(IReadOnlyDictionary<Kind, long> totals, IReadOnlyCollection<Kind> kindSet)
        {
            long sum = 0;
            foreach (Kind kind in kindSet)
            {
                if (totals.TryGetValue(kind, out long value))
                {
                    sum += value;
                }
            }

            return sum;
        }

        private static long CrossingsInto(ExternalEventTrace trace, IReadOnlyCollection<Kind> kindSet, Tick boundary)
        {
            long sum = 0;
            foreach (ExternalEvent crossing in trace.Events)
            {
                if (crossing.Boundary == boundary && ((ICollection<Kind>)new List<Kind>(kindSet)).Contains(crossing.Target.Kind))
                {
                    sum += crossing.Amount;
                }
            }

            return sum;
        }
    }

    /// <summary>
    /// The partition reader: for each place, the terminal place its descent path reaches on the
    /// final potential field. Declared conventions (part of the output): potential = kind(1) +
    /// kind(2) values; step = neighbour with the lowest strictly-lower potential; tie broken by
    /// lowest place id; no lower neighbour = terminal. Output is facts under these conventions —
    /// vocabulary is test-enforced against the forbidden list.
    /// </summary>
    public static class PartitionReader
    {
        public static readonly string[] ForbiddenWords =
            { "river", "valley", "watershed", "basin", "erosion", "delta" };

        public static IReadOnlyDictionary<Place, Place> TerminalPartition(
            SimulationState state, RelationSet relations, IReadOnlyList<Place> places, Kind ground, Kind carried)
        {
            var terminal = new Dictionary<Place, Place>();
            foreach (Place start in places)
            {
                Place current = start;
                for (int guard = 0; guard <= places.Count; guard++)
                {
                    // OutgoingFrom is canonically ordered (target ascending), so keeping the first
                    // strict minimum realises the declared tie-break (lowest place id) for free.
                    Place next = current;
                    long best = Potential(state, current, ground, carried);
                    foreach (Relation relation in relations.OutgoingFrom(current))
                    {
                        long candidate = Potential(state, relation.Target, ground, carried);
                        if (candidate < best)
                        {
                            best = candidate;
                            next = relation.Target;
                        }
                    }

                    if (next == current)
                    {
                        break;
                    }

                    current = next;
                }

                terminal[start] = current;
            }

            return terminal;
        }

        public static string Report(IReadOnlyDictionary<Place, Place> partition)
        {
            var classes = new SortedDictionary<int, List<int>>();
            foreach (KeyValuePair<Place, Place> entry in partition)
            {
                if (!classes.TryGetValue(entry.Value.Value, out List<int> members))
                {
                    members = new List<int>();
                    classes[entry.Value.Value] = members;
                }

                members.Add(entry.Key.Value);
            }

            var report = new StringBuilder();
            report.AppendLine("terminal-minimum partition (convention: potential = kind(1)+kind(2); steepest descent; tie -> lowest place id)");
            foreach (KeyValuePair<int, List<int>> entry in classes)
            {
                entry.Value.Sort();
                report.AppendLine($"class terminal=place({entry.Key}) members={entry.Value.Count}: [{string.Join(",", entry.Value)}]");
            }

            return report.ToString();
        }

        public static IReadOnlyList<string> VocabularyViolations(string output)
        {
            var found = new List<string>();
            string lower = output.ToLowerInvariant();
            foreach (string word in ForbiddenWords)
            {
                if (lower.Contains(word))
                {
                    found.Add(word);
                }
            }

            return found;
        }

        private static long Potential(SimulationState state, Place place, Kind ground, Kind carried)
        {
            return state.ValueAt(new Cell(place, ground)) + state.ValueAt(new Cell(place, carried));
        }
    }
}
