using System.Collections.Generic;
using System.Text;

namespace Genesis.Simulation.Lootbound
{
    /// <summary>
    /// The laboratory's reader, third generation (L-004 — Regularities). Replays a run and reports
    /// four layers, all facts, none intention, none concept: the <em>firsts</em> (L-002), the
    /// <em>full movement history</em> of the subject object (L-003), the <em>structures</em>
    /// (counts, L-003), and the <em>regularities</em> (L-004): frequencies, distributions, raw
    /// sequences, repetitions and durations. The reader computes; it never concludes. It may say
    /// "this length-3 subsequence occurred 4 times"; it may never say "loop", "habit", "routine"
    /// or "alternation" — naming a shape is the laboratory's later act, through observations.
    /// Vocabulary is bounded by RD-L6's four documented specimens: no "refused" (intention), no
    /// "voluntary" (intention), no "superior" (a property the world does not define), no "chest"
    /// (an object the world does not contain — swords at the shelter have Location = Shelter,
    /// nothing more). Conventions are declared in the output itself. Timestamps are ticks
    /// (invariant 5). The document remains a pure function of the record.
    /// </summary>
    public static class BiographyChronicler
    {
        public static string Chronicle(ExternalEventTrace trace, long ticks, Place sword, string runName, string swordName)
        {
            SimulationState state = LootboundWorld.BuildInitialState();
            RelationSet relations = LootboundWorld.BuildRelations(state);
            IReadOnlyList<ITransition> laws = LootboundWorld.BuildLaws();
            TickRunner runner = LootboundWorld.BuildRunner();

            Place other = sword == LootboundWorld.OldSword ? LootboundWorld.NewSword : LootboundWorld.OldSword;
            Cell loc = new Cell(sword, LootboundWorld.Location);
            Cell wear = new Cell(sword, LootboundWorld.Wear);
            Cell repairs = new Cell(sword, LootboundWorld.Repairs);
            Cell otherLoc = new Cell(other, LootboundWorld.Location);
            Cell atClearing = new Cell(LootboundWorld.Clearing, LootboundWorld.PlayerAt);

            var firsts = new List<string>();
            var moves = new List<(long tick, long from, long to)>();
            var repairTicks = new List<long>();
            var entryLog = new List<(long tick, Place place)>();
            var occupancy = new Dictionary<Place, long>();
            Place startPlace = LootboundWorld.Field;
            foreach (Place place in LootboundWorld.Spatial)
            {
                occupancy[place] = 0;
                if (state.ValueAt(new Cell(place, LootboundWorld.PlayerAt)) == 1)
                {
                    startPlace = place;
                }
            }

            bool acquired = false, worn = false, repaired = false, seenBeside = false;
            bool leftCarrying = false, stowedOnce = false, retook = false;

            SimulationState previous = state;
            for (long t = 1; t <= ticks; t++)
            {
                state = runner.Run(previous, relations, laws, trace, 1);

                long locBefore = previous.ValueAt(loc);
                long locNow = state.ValueAt(loc);
                if (locBefore != locNow)
                {
                    moves.Add((t, locBefore, locNow));
                }

                foreach (Place place in LootboundWorld.Spatial)
                {
                    Cell here = new Cell(place, LootboundWorld.PlayerAt);
                    if (state.ValueAt(here) == 1)
                    {
                        occupancy[place]++;
                    }

                    if (previous.ValueAt(here) == 0 && state.ValueAt(here) == 1)
                    {
                        entryLog.Add((t, place));
                    }
                }

                if (state.ValueAt(repairs) > previous.ValueAt(repairs))
                {
                    repairTicks.Add(t);
                }

                if (!acquired && locBefore != 0 && locNow == 0)
                {
                    acquired = true;
                    firsts.Add(Line(t, "Acquisition (first taken in hand)"));
                }
                else if (acquired && stowedOnce && !retook && locBefore == LootboundWorld.Shelter.Value && locNow == 0)
                {
                    retook = true;
                    bool otherStillThere = state.ValueAt(otherLoc) == LootboundWorld.Clearing.Value;
                    firsts.Add(Line(t, otherStillThere
                        ? "Taken in hand at the shelter again (the other sword still lay in the clearing)"
                        : "Taken in hand at the shelter again"));
                }

                if (!worn && previous.ValueAt(wear) == 0 && state.ValueAt(wear) > 0)
                {
                    worn = true;
                    firsts.Add(Line(t, "First wear"));
                }

                if (!repaired && previous.ValueAt(repairs) == 0 && state.ValueAt(repairs) > 0)
                {
                    repaired = true;
                    firsts.Add(Line(t, "First repair"));
                }

                if (!seenBeside && previous.ValueAt(atClearing) == 0 && state.ValueAt(atClearing) == 1
                    && state.ValueAt(otherLoc) == LootboundWorld.Clearing.Value)
                {
                    seenBeside = true;
                    firsts.Add(Line(t, "First presence at the clearing while the other sword lay there"));
                }

                if (seenBeside && !leftCarrying
                    && previous.ValueAt(atClearing) == 1 && state.ValueAt(atClearing) == 0
                    && state.ValueAt(otherLoc) == LootboundWorld.Clearing.Value
                    && state.ValueAt(loc) == 0)
                {
                    leftCarrying = true;
                    firsts.Add(Line(t, "Left the clearing carrying this sword while the other lay there"));
                }

                if (!stowedOnce && locBefore == 0 && locNow == LootboundWorld.Shelter.Value)
                {
                    stowedOnce = true;
                    firsts.Add(Line(t, "First time out of hand at the shelter"));
                }

                previous = state;
            }

            // ---------------- structures (L-003): counts from the logs.
            var entriesPerPlace = new Dictionary<Place, int>();
            foreach (Place place in LootboundWorld.Spatial)
            {
                entriesPerPlace[place] = 0;
            }

            long firstRepairTick = repairTicks.Count > 0 ? repairTicks[0] : long.MaxValue;
            int stationEntriesBeforeRepair = 0;
            for (int i = 0; i < entryLog.Count; i++)
            {
                entriesPerPlace[entryLog[i].place]++;
                if (entryLog[i].place == LootboundWorld.Station && entryLog[i].tick < firstRepairTick)
                {
                    stationEntriesBeforeRepair++;
                }
            }

            // ---------------- regularities (L-004): frequencies, sequences, repetitions, durations.
            var pathSequence = new List<Place> { startPlace };
            for (int i = 0; i < entryLog.Count; i++)
            {
                pathSequence.Add(entryLog[i].place);
            }

            var transitionCounts = new Dictionary<string, int>();
            for (int i = 1; i < pathSequence.Count; i++)
            {
                string key = $"{SpatialName(pathSequence[i - 1])} -> {SpatialName(pathSequence[i])}";
                transitionCounts[key] = transitionCounts.TryGetValue(key, out int c) ? c + 1 : 1;
            }

            var tripleCounts = new Dictionary<string, int>();
            for (int i = 2; i < pathSequence.Count; i++)
            {
                string key = $"{SpatialName(pathSequence[i - 2])} -> {SpatialName(pathSequence[i - 1])} -> {SpatialName(pathSequence[i])}";
                tripleCounts[key] = tripleCounts.TryGetValue(key, out int c) ? c + 1 : 1;
            }

            var inHandIntervals = new List<string>();
            long heldSince = -1;
            long heldTotal = 0;
            long initialLoc = LootboundWorld.BuildInitialState().ValueAt(loc);
            if (initialLoc == 0)
            {
                heldSince = 0;
            }

            for (int i = 0; i < moves.Count; i++)
            {
                if (moves[i].to == 0)
                {
                    heldSince = moves[i].tick;
                }
                else if (moves[i].from == 0 && heldSince >= 0)
                {
                    inHandIntervals.Add($"t={heldSince}..{moves[i].tick} ({moves[i].tick - heldSince} ticks)");
                    heldTotal += moves[i].tick - heldSince;
                    heldSince = -1;
                }
            }

            if (heldSince >= 0)
            {
                inHandIntervals.Add($"t={heldSince}..end of reading ({ticks - heldSince} ticks)");
                heldTotal += ticks - heldSince;
            }

            bool complete = acquired && worn && repaired && seenBeside && leftCarrying && stowedOnce && retook;

            // ---------------- the document.
            var d = new StringBuilder();
            d.AppendLine("LB-Obs (draft, produced by a world)");
            d.AppendLine();
            d.AppendLine($"Subject: {runName}");
            d.AppendLine($"Object:  {swordName}");
            d.AppendLine();
            d.AppendLine("Firsts:");
            d.AppendLine();
            foreach (string first in firsts)
            {
                d.AppendLine(first);
            }

            d.AppendLine();
            d.AppendLine($"Movement history (every transition of the object — {moves.Count} in total):");
            d.AppendLine();
            foreach ((long t, long from, long to) in moves)
            {
                d.AppendLine($"t={t,-4} {PlaceName(from)} -> {PlaceName(to)}");
            }

            d.AppendLine();
            d.AppendLine("Structures (counts — facts, no interpretation):");
            d.AppendLine();
            d.AppendLine($"The object changed location {moves.Count} times.");
            d.AppendLine($"Repairs: {repairTicks.Count}{(repairTicks.Count > 0 ? " (t=" + string.Join(", t=", repairTicks) + ")" : "")}");
            var entryLine = new StringBuilder("Entries: ");
            foreach (Place place in LootboundWorld.Spatial)
            {
                entryLine.Append($"{SpatialName(place)} x{entriesPerPlace[place]}  ");
            }

            d.AppendLine(entryLine.ToString().TrimEnd());
            if (repairTicks.Count > 0)
            {
                d.AppendLine($"Entries at the repair station before the first repair: {stationEntriesBeforeRepair}");
            }

            d.AppendLine();
            d.AppendLine("Regularities (computed — frequencies, sequences, durations; the reader names no shape):");
            d.AppendLine("(conventions: the sequence is the places entered, in order, starting from the initial place;");
            d.AppendLine(" repeated subsequences are reported at length 3, count >= 2)");
            d.AppendLine();
            var seq = new StringBuilder("Sequence: ");
            foreach (Place place in pathSequence)
            {
                seq.Append(SpatialName(place)).Append(' ');
            }

            d.AppendLine(seq.ToString().TrimEnd());
            d.AppendLine();
            d.AppendLine("Transition frequencies:");
            foreach (KeyValuePair<string, int> pair in Sorted(transitionCounts))
            {
                d.AppendLine($"  {pair.Key}: {pair.Value}");
            }

            d.AppendLine();
            d.AppendLine("Repeated length-3 subsequences:");
            bool anyTriple = false;
            foreach (KeyValuePair<string, int> pair in Sorted(tripleCounts))
            {
                if (pair.Value >= 2)
                {
                    anyTriple = true;
                    d.AppendLine($"  {pair.Key}: {pair.Value}");
                }
            }

            if (!anyTriple)
            {
                d.AppendLine("  (none occurred more than once)");
            }

            d.AppendLine();
            var occLine = new StringBuilder("Ticks present: ");
            foreach (Place place in LootboundWorld.Spatial)
            {
                occLine.Append($"{SpatialName(place)} {occupancy[place]}  ");
            }

            d.AppendLine(occLine.ToString().TrimEnd());
            d.AppendLine();
            d.AppendLine($"In hand ({inHandIntervals.Count} interval(s), {heldTotal} ticks in total):");
            foreach (string interval in inHandIntervals)
            {
                d.AppendLine($"  {interval}");
            }

            d.AppendLine();
            d.AppendLine(complete
                ? "Status: All seven firsts observed."
                : "Status: Biography in progress.");
            return d.ToString();
        }

        private static IEnumerable<KeyValuePair<string, int>> Sorted(Dictionary<string, int> counts)
        {
            var list = new List<KeyValuePair<string, int>>(counts);
            list.Sort((a, b) => b.Value != a.Value ? b.Value.CompareTo(a.Value) : string.CompareOrdinal(a.Key, b.Key));
            return list;
        }

        private static string Line(long tick, string what)
        {
            return $"t={tick,-4} {what}";
        }

        private static string PlaceName(long locationValue)
        {
            // Declared conventions (L-006): 0 = held by body A ("in hand", the historical name);
            // 1 = held by body B. Neither is a place id (place ids are >= 100).
            if (locationValue == LootboundWorld.HeldByA) return "in hand";
            if (locationValue == LootboundWorld.HeldByB) return "held (body B)";
            if (locationValue == LootboundWorld.Shelter.Value) return "shelter";
            if (locationValue == LootboundWorld.Clearing.Value) return "clearing ground";
            if (locationValue == LootboundWorld.Station.Value) return "station ground";
            if (locationValue == LootboundWorld.Field.Value) return "field ground";
            if (locationValue == LootboundWorld.Tree.Value) return "tree ground";
            return $"place {locationValue}";
        }

        private static string SpatialName(Place place)
        {
            if (place == LootboundWorld.Shelter) return "Shelter";
            if (place == LootboundWorld.Tree) return "Tree";
            if (place == LootboundWorld.Station) return "Station";
            if (place == LootboundWorld.Clearing) return "Clearing";
            return "Field";
        }
    }
}
