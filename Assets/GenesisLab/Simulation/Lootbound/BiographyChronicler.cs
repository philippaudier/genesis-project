using System.Collections.Generic;
using System.Text;

namespace Genesis.Simulation.Lootbound
{
    /// <summary>
    /// The laboratory's reader, second generation (L-003 — Richer Biographies). Replays a run —
    /// initial state + relations + laws + trace — and reports three layers, all facts, none
    /// intention: the <em>firsts</em> (the irreversible events of L-002), the <em>full movement
    /// history</em> of the subject object (every transition, not only the first — the layer whose
    /// absence hid Run-000002's most telling episode), and <em>structures</em> (counts: entries per
    /// place, visits before the first repair, how many times the object moved). Vocabulary is
    /// bounded by RD-L6's three documented specimens: no "refused" (intention), no "voluntary"
    /// (intention), no "superior" (a property the world does not define — swords differ only by
    /// identity, wear and repairs). Timestamps are ticks (invariant 5). The document remains a pure
    /// function of the record: two researchers reading the same run must produce the same text.
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
            var movements = new List<string>();
            var repairTicks = new List<long>();
            var entryLog = new List<(long tick, Place place)>();

            bool acquired = false, worn = false, repaired = false, seenBeside = false;
            bool leftCarrying = false, stowedOnce = false, retook = false;

            SimulationState previous = state;
            for (long t = 1; t <= ticks; t++)
            {
                state = runner.Run(previous, relations, laws, trace, 1);

                // --- movement history: EVERY transition of the subject object (the L-003 layer).
                long locBefore = previous.ValueAt(loc);
                long locNow = state.ValueAt(loc);
                if (locBefore != locNow)
                {
                    movements.Add($"t={t,-4} {PlaceName(locBefore)} -> {PlaceName(locNow)}");
                }

                // --- entries: the player arriving at each spatial place.
                foreach (Place place in LootboundWorld.Spatial)
                {
                    Cell here = new Cell(place, LootboundWorld.PlayerAt);
                    if (previous.ValueAt(here) == 0 && state.ValueAt(here) == 1)
                    {
                        entryLog.Add((t, place));
                    }
                }

                // --- repairs: all of them, not only the first.
                if (state.ValueAt(repairs) > previous.ValueAt(repairs))
                {
                    repairTicks.Add(t);
                }

                // --- the firsts (L-002's irreversible events, RD-L6-bounded vocabulary).
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
                        ? "Retrieved from the shelter again (the other sword still lay in the clearing)"
                        : "Retrieved from the shelter again"));
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
                    firsts.Add(Line(t, "Stored at the shelter"));
                }

                previous = state;
            }

            // --- structures: counts, computed from the logs. Facts; no interpretation.
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

            bool complete = acquired && worn && repaired && seenBeside && leftCarrying && stowedOnce && retook;

            var document = new StringBuilder();
            document.AppendLine("LB-Obs (draft, produced by a world)");
            document.AppendLine();
            document.AppendLine($"Subject: {runName}");
            document.AppendLine($"Object:  {swordName}");
            document.AppendLine();
            document.AppendLine("Firsts:");
            document.AppendLine();
            for (int i = 0; i < firsts.Count; i++)
            {
                document.AppendLine(firsts[i]);
            }

            document.AppendLine();
            document.AppendLine($"Movement history (every transition of the object — {movements.Count} in total):");
            document.AppendLine();
            for (int i = 0; i < movements.Count; i++)
            {
                document.AppendLine(movements[i]);
            }

            document.AppendLine();
            document.AppendLine("Structures (counts — facts, no interpretation):");
            document.AppendLine();
            document.AppendLine($"The object changed location {movements.Count} times.");
            document.AppendLine($"Repairs: {repairTicks.Count}{(repairTicks.Count > 0 ? " (t=" + string.Join(", t=", repairTicks) + ")" : "")}");
            var entryLine = new StringBuilder("Entries: ");
            foreach (Place place in LootboundWorld.Spatial)
            {
                entryLine.Append($"{SpatialName(place)} x{entriesPerPlace[place]}  ");
            }

            document.AppendLine(entryLine.ToString().TrimEnd());
            if (repairTicks.Count > 0)
            {
                document.AppendLine($"Entries at the repair station before the first repair: {stationEntriesBeforeRepair}");
            }

            document.AppendLine();
            document.AppendLine(complete
                ? "Status: All seven firsts observed."
                : "Status: Biography in progress.");
            return document.ToString();
        }

        private static string Line(long tick, string what)
        {
            return $"t={tick,-4} {what}";
        }

        private static string PlaceName(long locationValue)
        {
            if (locationValue == 0) return "in hand";
            if (locationValue == LootboundWorld.Shelter.Value) return "shelter chest";
            if (locationValue == LootboundWorld.Clearing.Value) return "clearing ground";
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
