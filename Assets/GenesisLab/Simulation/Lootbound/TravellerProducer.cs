using System.Collections.Generic;

namespace Genesis.Simulation.Lootbound
{
    /// <summary>
    /// The Traveller (L-006). NOT world content and NOT a law: a <em>producer</em> in the sense of
    /// ADR-0005 — it stands outside the membrane, reads snapshots exactly as any observer may, and
    /// emits external events onto the trace. The world cannot know a traveller from a player from
    /// a replay (provenance-blindness); everything it does is recorded like everything else, so
    /// replay of a co-inhabited run is exact and free. Deterministic by construction: no
    /// randomness, a fixed itinerary, a pure function of the observed state and its own step
    /// index. Its policy is deliberately meagre — walk, pick up, put down, leave — because L-006
    /// observes exactly one thing: what a biography becomes when it is no longer alone.
    /// </summary>
    public sealed class TravellerProducer
    {
        private enum GoalKind { WalkTo, Toggle, Leave, Wait, Arrive, Done }

        private readonly struct Goal
        {
            public GoalKind Kind { get; }
            public Place Place { get; }
            public long Duration { get; }

            public Goal(GoalKind kind, Place place, long duration = 0)
            {
                Kind = kind;
                Place = place;
                Duration = duration;
            }
        }

        private readonly List<Goal> _itinerary;
        private int _current;
        private bool _emittedForCurrent;
        private long _waited;

        /// <summary>
        /// The itinerary, two passages (L-007): from the tree, walk to the clearing; pick up
        /// whatever sword lies there; carry it to the station; put it down; leave the world.
        /// Then — the same body, the same identity — stay away a while, come back, walk straight
        /// to the station where it left its object (it remembers its own world; it does not
        /// search), take it up, carry it back to the clearing, put it back where it found it,
        /// and leave again. A body passing through, twice — and a world never reset in between.
        /// </summary>
        public TravellerProducer()
        {
            _itinerary = new List<Goal>
            {
                // first passage
                new Goal(GoalKind.WalkTo, LootboundWorld.Field),
                new Goal(GoalKind.WalkTo, LootboundWorld.Clearing),
                new Goal(GoalKind.Toggle, LootboundWorld.Clearing),
                new Goal(GoalKind.WalkTo, LootboundWorld.Field),
                new Goal(GoalKind.WalkTo, LootboundWorld.Station),
                new Goal(GoalKind.Toggle, LootboundWorld.Station),
                new Goal(GoalKind.WalkTo, LootboundWorld.Field),
                new Goal(GoalKind.Leave, LootboundWorld.Field),
                // absence — the world keeps being the world
                new Goal(GoalKind.Wait, LootboundWorld.Field, 60),
                // second passage: the return
                new Goal(GoalKind.Arrive, LootboundWorld.Field),
                new Goal(GoalKind.WalkTo, LootboundWorld.Station),
                new Goal(GoalKind.Toggle, LootboundWorld.Station),
                new Goal(GoalKind.WalkTo, LootboundWorld.Field),
                new Goal(GoalKind.WalkTo, LootboundWorld.Clearing),
                new Goal(GoalKind.Toggle, LootboundWorld.Clearing),
                new Goal(GoalKind.WalkTo, LootboundWorld.Field),
                new Goal(GoalKind.Leave, LootboundWorld.Field)
            };
        }

        /// <summary>Whether the itinerary is finished (the body has left, or had nothing to do).</summary>
        public bool Done => _current >= _itinerary.Count;

        /// <summary>
        /// Called once per tick, before the tick runs. Reads the snapshot, emits at most one
        /// external event at the current boundary. Advances its goal only on observed completion —
        /// the producer trusts the world's record, never its own emission.
        /// </summary>
        public void Step(SimulationState state, ExternalEventTrace trace)
        {
            if (Done)
            {
                return;
            }

            Goal goal = _itinerary[_current];
            bool atGoalPlace = state.ValueAt(new Cell(goal.Place, LootboundWorld.BodyB)) == 1;

            switch (goal.Kind)
            {
                case GoalKind.WalkTo:
                    if (atGoalPlace)
                    {
                        Advance();
                        return;
                    }

                    EmitOnce(state, trace, new Cell(goal.Place, LootboundWorld.GoB));
                    return;

                case GoalKind.Toggle:
                    if (!atGoalPlace)
                    {
                        return; // not there yet; the previous WalkTo just completed this tick
                    }

                    if (_emittedForCurrent && state.ValueAt(new Cell(goal.Place, LootboundWorld.ActB)) == 0)
                    {
                        Advance(); // the intent was consumed: the gesture happened (or lawfully did nothing)
                        return;
                    }

                    EmitOnce(state, trace, new Cell(goal.Place, LootboundWorld.ActB));
                    return;

                case GoalKind.Wait:
                    _waited++;
                    if (_waited >= goal.Duration)
                    {
                        _waited = 0;
                        Advance();
                    }

                    return;

                case GoalKind.Arrive:
                    if (state.ValueAt(new Cell(LootboundWorld.Field, LootboundWorld.BodyB)) == 1)
                    {
                        Advance(); // back in the world, at the field
                        return;
                    }

                    EmitOnce(state, trace, new Cell(LootboundWorld.Field, LootboundWorld.ArriveB));
                    return;

                case GoalKind.Leave:
                    if (!atGoalPlace && state.ValueAt(new Cell(LootboundWorld.Field, LootboundWorld.BodyB)) == 0)
                    {
                        bool anywhere = false;
                        foreach (Place place in LootboundWorld.Spatial)
                        {
                            if (state.ValueAt(new Cell(place, LootboundWorld.BodyB)) == 1)
                            {
                                anywhere = true;
                            }
                        }

                        if (!anywhere)
                        {
                            Advance(); // gone
                            return;
                        }
                    }

                    EmitOnce(state, trace, new Cell(LootboundWorld.Field, LootboundWorld.LeaveB));
                    return;

                default:
                    return;
            }
        }

        private void EmitOnce(SimulationState state, ExternalEventTrace trace, Cell cell)
        {
            // One intent at a time: emit only when the previous one has been consumed.
            if (state.ValueAt(cell) == 0 && !_emittedForCurrent)
            {
                trace.Append(new ExternalEvent(state.CurrentTick, cell, 1));
                _emittedForCurrent = true;
            }
            else if (state.ValueAt(cell) == 0 && _emittedForCurrent)
            {
                // Intent consumed but goal not yet reached (e.g. mid-walk): allow re-emission.
                _emittedForCurrent = false;
            }
        }

        private void Advance()
        {
            _current++;
            _emittedForCurrent = false;
        }
    }
}
