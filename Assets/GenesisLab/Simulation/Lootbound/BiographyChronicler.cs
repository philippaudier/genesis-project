using System.Collections.Generic;
using System.Text;

namespace Genesis.Simulation.Lootbound
{
    /// <summary>
    /// The laboratory's reader (L-002 DoD): turns a run — initial state + relations + laws + trace
    /// — into a sword's biography. It replays the run tick by tick and derives the irreversible
    /// firsts by comparing successive states; no law writes a biography, no developer writes one —
    /// the world produces it. Timestamps are ticks: the world may not know the wall clock
    /// (invariant 5); real-time decoration belongs to a presentation journal, someday. The refusal
    /// is a derived non-event: the player saw the better sword and left without it — no law
    /// recorded that; this reader deduces it. The document is a function of the record alone, so
    /// two researchers reading the same run must produce the same biography.
    /// </summary>
    public static class BiographyChronicler
    {
        public static string Chronicle(ExternalEventTrace trace, long ticks, Place sword, string runName, string swordName)
        {
            SimulationState state = LootboundWorld.BuildInitialState();
            RelationSet relations = LootboundWorld.BuildRelations(state);
            IReadOnlyList<ITransition> laws = LootboundWorld.BuildLaws();
            TickRunner runner = LootboundWorld.BuildRunner();

            Cell loc = new Cell(sword, LootboundWorld.Location);
            Cell wear = new Cell(sword, LootboundWorld.Wear);
            Cell repairs = new Cell(sword, LootboundWorld.Repairs);
            Cell betterLoc = new Cell(LootboundWorld.NewSword, LootboundWorld.Location);
            Cell atClearing = new Cell(LootboundWorld.Clearing, LootboundWorld.PlayerAt);

            var events = new List<string>();
            bool acquired = false, worn = false, repaired = false, discovered = false;
            bool refused = false, stowedOnce = false, reequipped = false;
            long retrievals = 0;

            SimulationState previous = state;
            for (long t = 1; t <= ticks; t++)
            {
                state = runner.Run(previous, relations, laws, trace, 1);

                long locBefore = previous.ValueAt(loc);
                long locNow = state.ValueAt(loc);

                if (locBefore != 0 && locNow == 0)
                {
                    retrievals++;
                    if (!acquired)
                    {
                        acquired = true;
                        events.Add(Line(t, "Acquisition"));
                    }
                    else if (stowedOnce && !reequipped)
                    {
                        reequipped = true;
                        bool betterStillThere = state.ValueAt(betterLoc) == LootboundWorld.Clearing.Value;
                        events.Add(Line(t, betterStillThere
                            ? "Voluntary re-equipment (a better sword still lay in the clearing)"
                            : "Voluntary re-equipment"));
                    }
                }

                if (!worn && previous.ValueAt(wear) == 0 && state.ValueAt(wear) > 0)
                {
                    worn = true;
                    events.Add(Line(t, "First wear"));
                }

                if (!repaired && previous.ValueAt(repairs) == 0 && state.ValueAt(repairs) > 0)
                {
                    repaired = true;
                    events.Add(Line(t, "First repair"));
                }

                if (!discovered && previous.ValueAt(atClearing) == 0 && state.ValueAt(atClearing) == 1
                    && state.ValueAt(betterLoc) == LootboundWorld.Clearing.Value)
                {
                    discovered = true;
                    events.Add(Line(t, "First superior sword discovered"));
                }

                if (discovered && !refused
                    && previous.ValueAt(atClearing) == 1 && state.ValueAt(atClearing) == 0
                    && state.ValueAt(betterLoc) == LootboundWorld.Clearing.Value
                    && state.ValueAt(loc) == 0)
                {
                    refused = true;
                    events.Add(Line(t, "Replacement refused (left the clearing carrying the old sword)"));
                }

                if (locBefore == 0 && locNow == LootboundWorld.Shelter.Value)
                {
                    if (!stowedOnce)
                    {
                        stowedOnce = true;
                        events.Add(Line(t, "Stored at the shelter"));
                    }
                }

                previous = state;
            }

            bool complete = acquired && worn && repaired && discovered && refused && stowedOnce && reequipped;

            var document = new StringBuilder();
            document.AppendLine("LB-Obs (draft, produced by a world)");
            document.AppendLine();
            document.AppendLine($"Subject: {runName}");
            document.AppendLine($"Object:  {swordName}");
            document.AppendLine();
            document.AppendLine("Biography:");
            document.AppendLine();
            for (int i = 0; i < events.Count; i++)
            {
                document.AppendLine(events[i]);
            }

            document.AppendLine();
            document.AppendLine(complete
                ? "Status: First complete biography observed."
                : "Status: Biography in progress.");
            return document.ToString();
        }

        private static string Line(long tick, string what)
        {
            return $"t={tick,-4} {what}";
        }
    }
}
