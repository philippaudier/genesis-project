using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.S1_004
{
    /// <summary>
    /// One fixture's contributions on one snapshot, recovered by materialising the same declared
    /// views the runner materialises — the kernel is untouched, and provenance (which fixture
    /// wrote which cell) becomes visible, which the resolver interface alone cannot give.
    /// This is the second half of measurement 13; the SpyResolver is the first.
    /// </summary>
    public static class Provenance
    {
        public sealed class Entry
        {
            public string Fixture { get; }
            public Cell Target { get; }
            public long Amount { get; }

            public Entry(string fixture, Cell target, long amount)
            {
                Fixture = fixture;
                Target = target;
                Amount = amount;
            }

            public override string ToString() =>
                $"{Fixture}: place({Target.Place.Value}) kind({Target.Kind.Value}) {(Amount >= 0 ? "+" : "")}{Amount}";
        }

        /// <summary>Every contribution each fixture would make on this snapshot, labelled.</summary>
        public static IReadOnlyList<Entry> Collect(SimulationState snapshot, RelationSet relations,
            IReadOnlyList<IFixture> fixtures)
        {
            var entries = new List<Entry>();
            foreach (IFixture fixture in fixtures)
            {
                string label = fixture.GetType().Name;
                foreach (ITransition transition in fixture.Transitions)
                {
                    var view = new RelationalStateView(
                        snapshot, relations, transition.ReadScope, transition.RelationScope);
                    IReadOnlyList<Contribution> contributions = transition.Apply(view);
                    if (contributions == null)
                    {
                        continue;
                    }

                    foreach (Contribution contribution in contributions)
                    {
                        entries.Add(new Entry(label, contribution.Target, contribution.Amount));
                    }
                }
            }

            return entries;
        }

        /// <summary>Cells that more than one fixture wrote on this snapshot — the collisions.</summary>
        public static IReadOnlyList<KeyValuePair<Cell, List<Entry>>> Collisions(IReadOnlyList<Entry> entries)
        {
            var byCell = new Dictionary<Cell, List<Entry>>();
            foreach (Entry entry in entries)
            {
                if (!byCell.TryGetValue(entry.Target, out List<Entry> list))
                {
                    list = new List<Entry>();
                    byCell[entry.Target] = list;
                }

                list.Add(entry);
            }

            var collisions = new List<KeyValuePair<Cell, List<Entry>>>();
            var cells = new List<Cell>(byCell.Keys);
            cells.Sort();
            foreach (Cell cell in cells)
            {
                if (byCell[cell].Count > 1)
                {
                    collisions.Add(new KeyValuePair<Cell, List<Entry>>(cell, byCell[cell]));
                }
            }

            return collisions;
        }
    }

    /// <summary>
    /// The readings and audits of S1-004. Everything here is a pure function of the record plus
    /// a declared convention; no reader invents a formation, and none may name one.
    /// </summary>
    public static class Surface
    {
        public static readonly string[] ForbiddenWords =
            { "erosion", "deposition", "river", "landscape", "delta", "valley", "watershed" };

        public static long[] SolidSurfaces(SimulationState state, IReadOnlyList<Place> places)
        {
            var reading = new long[places.Count];
            for (int i = 0; i < places.Count; i++)
            {
                reading[i] = K4.SolidSurface(state, places[i]);
            }

            return reading;
        }

        public static long MatterTotal(SimulationState state, IReadOnlyList<Place> places)
        {
            long total = 0;
            foreach (Place place in places)
            {
                total += state.ValueAt(new Cell(place, K4.Rock)) + state.ValueAt(new Cell(place, K4.Sediment));
            }

            return total;
        }

        public static long LocalMatter(SimulationState state, Place place)
        {
            return state.ValueAt(new Cell(place, K4.Rock)) + state.ValueAt(new Cell(place, K4.Sediment));
        }

        /// <summary>
        /// The cross-kind audit (the global witness): Rock + Sediment must be invariant, since
        /// no crossing ever admits either kind. Reports every offending boundary.
        /// </summary>
        public static IReadOnlyList<string> MatterAudit(IReadOnlyList<SimulationState> states,
            IReadOnlyList<Place> places)
        {
            var faults = new List<string>();
            long expected = MatterTotal(states[0], places);
            for (int t = 1; t < states.Count; t++)
            {
                long found = MatterTotal(states[t], places);
                if (found != expected)
                {
                    faults.Add($"tick {t}: Rock+Sediment expected {expected}, found {found}");
                }
            }

            return faults;
        }

        /// <summary>
        /// Conversion pairs emitted per place on this snapshot (sealed measurement 5): one pair
        /// per outgoing edge whose prospective Water flux is strictly greater than the threshold.
        /// A Sediment cell's change is transport-in − transport-out **plus** these; forgetting
        /// them is exactly how an accounting reader lies without appearing to.
        /// </summary>
        public static IReadOnlyDictionary<int, long> ConversionsAt(SimulationState snapshot,
            RelationSet relations, IReadOnlyList<Place> places, DivisorPolicy divisor, long threshold)
        {
            var pairs = new Dictionary<int, long>();
            foreach (Place place in places)
            {
                IReadOnlyList<Relation> outgoing = relations.OutgoingFrom(place);
                long here = K4.WaterPotential(snapshot, place);
                long divisorValue = divisor(outgoing.Count);
                long count = 0;

                foreach (Relation relation in outgoing)
                {
                    long diff = here - K4.WaterPotential(snapshot, relation.Target);
                    if (diff > 0 && diff / divisorValue > threshold)
                    {
                        count++;
                    }
                }

                pairs[place.Value] = count;
            }

            return pairs;
        }

        /// <summary>Directed per-edge flux of a kind, reconstructed under the declared convention.</summary>
        public static IReadOnlyList<string> Flux(SimulationState snapshot, RelationSet relations,
            IReadOnlyList<Place> places, Kind kind, DivisorPolicy divisor, bool cappedByHolding)
        {
            var lines = new List<string>();
            foreach (Place place in places)
            {
                IReadOnlyList<Relation> outgoing = relations.OutgoingFrom(place);
                long here = K4.WaterPotential(snapshot, place);
                long carried = cappedByHolding ? snapshot.ValueAt(new Cell(place, kind)) : long.MaxValue;
                long divisorValue = divisor(outgoing.Count);

                foreach (Relation relation in outgoing)
                {
                    if (cappedByHolding && carried <= 0)
                    {
                        break;
                    }

                    long diff = here - K4.WaterPotential(snapshot, relation.Target);
                    if (diff <= 0)
                    {
                        continue;
                    }

                    long transfer = diff / divisorValue;
                    if (cappedByHolding && transfer > carried)
                    {
                        transfer = carried;
                    }

                    if (transfer <= 0)
                    {
                        continue;
                    }

                    if (cappedByHolding)
                    {
                        carried -= transfer;
                    }

                    lines.Add($"{place.Value}->{relation.Target.Value}:{transfer}");
                }
            }

            return lines;
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
    }
}
