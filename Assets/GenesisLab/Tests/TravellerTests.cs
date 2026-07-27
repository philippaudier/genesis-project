using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;
using Genesis.Simulation.Lootbound;

namespace Genesis.Tests
{
    /// <summary>
    /// L-006 — The Traveller. Era II's first phenomenon: the world becomes capable of producing
    /// several simultaneous biographies. The Traveller is a producer, not a law (ADR-0005): it
    /// stands outside the membrane and emits external events like any player, bot or replay. The
    /// final test is the milestone's question made mechanical: two biographies interacting without
    /// having been written for each other — and the object's biography telling the meeting.
    /// </summary>
    public sealed class TravellerTests
    {
        private static (SimulationState state, RelationSet relations, IReadOnlyList<ITransition> laws, TickRunner runner, ExternalEventTrace trace) NewWorld()
        {
            SimulationState state = LootboundWorld.BuildInitialState();
            return (
                state,
                LootboundWorld.BuildRelations(state),
                LootboundWorld.BuildLaws(),
                LootboundWorld.BuildRunner(),
                new ExternalEventTrace(LootboundWorld.BuildMembrane()));
        }

        private static Cell B(Place place) => new Cell(place, LootboundWorld.BodyB);
        private static Cell Loc(Place sword) => new Cell(sword, LootboundWorld.Location);

        [Test]
        public void The_Second_Body_Walks_The_Same_Graph_Under_The_Same_Law()
        {
            var (state, relations, laws, runner, trace) = NewWorld();
            Assert.AreEqual(1, state.ValueAt(B(LootboundWorld.Tree)), "Body B begins at the tree.");

            // Tree -> Clearing is not a relation: the intent is consumed, the body stays.
            trace.Append(new ExternalEvent(Tick.Zero, new Cell(LootboundWorld.Clearing, LootboundWorld.GoB), 1));
            state = runner.Run(state, relations, laws, trace, 3);
            Assert.AreEqual(1, state.ValueAt(B(LootboundWorld.Tree)), "No teleporting for anyone.");

            trace.Append(new ExternalEvent(new Tick(3), new Cell(LootboundWorld.Field, LootboundWorld.GoB), 1));
            trace.Append(new ExternalEvent(new Tick(6), new Cell(LootboundWorld.Clearing, LootboundWorld.GoB), 1));
            state = runner.Run(state, relations, laws, trace, 6);
            Assert.AreEqual(1, state.ValueAt(B(LootboundWorld.Clearing)));
            Assert.AreEqual(1, state.ValueAt(new Cell(LootboundWorld.Shelter, LootboundWorld.PlayerAt)),
                "Body A never moved: two markers, two biographies, one world.");
        }

        [Test]
        public void Body_B_Picks_Up_Carries_And_Puts_Down()
        {
            var (state, relations, laws, runner, trace) = NewWorld();
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            At(0, LootboundWorld.Field, LootboundWorld.GoB);
            At(3, LootboundWorld.Clearing, LootboundWorld.GoB);
            At(6, LootboundWorld.Clearing, LootboundWorld.ActB);      // pick up the sword lying there
            At(9, LootboundWorld.Field, LootboundWorld.GoB);
            At(12, LootboundWorld.Station, LootboundWorld.GoB);
            At(15, LootboundWorld.Station, LootboundWorld.ActB);      // put it down

            state = runner.Run(state, relations, laws, trace, 18);
            Assert.AreEqual(LootboundWorld.Station.Value, state.ValueAt(Loc(LootboundWorld.NewSword)),
                "The clearing's sword now lies at the station: an absence was manufactured lawfully.");
        }

        [Test]
        public void Departure_Drops_What_Is_Held_Objects_Are_Conserved()
        {
            var (state, relations, laws, runner, trace) = NewWorld();
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            At(0, LootboundWorld.Field, LootboundWorld.GoB);
            At(3, LootboundWorld.Clearing, LootboundWorld.GoB);
            At(6, LootboundWorld.Clearing, LootboundWorld.ActB);      // pick up
            At(9, LootboundWorld.Field, LootboundWorld.GoB);
            At(12, LootboundWorld.Field, LootboundWorld.LeaveB);      // leave while holding

            state = runner.Run(state, relations, laws, trace, 15);
            foreach (Place place in LootboundWorld.Spatial)
            {
                Assert.AreEqual(0, state.ValueAt(B(place)), "The body is gone from everywhere.");
            }

            Assert.AreEqual(LootboundWorld.Field.Value, state.ValueAt(Loc(LootboundWorld.NewSword)),
                "Departure may not destroy: the held sword lies where the body last stood.");
        }

        [Test]
        public void Two_Biographies_Interact_Without_Having_Been_Written_For_Each_Other()
        {
            // The milestone's question, mechanical. The player's script is Run-000001's, verbatim
            // — written in L-002, long before any Traveller existed. The Traveller is the default
            // producer, written without knowledge of that script. Same world, same laws, one trace.
            var trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            At(0, LootboundWorld.Shelter, LootboundWorld.Act);
            At(3, LootboundWorld.Field, LootboundWorld.Go);
            At(6, LootboundWorld.Tree, LootboundWorld.Go);
            At(9, LootboundWorld.Tree, LootboundWorld.Attack);
            At(12, LootboundWorld.Tree, LootboundWorld.Attack);
            At(15, LootboundWorld.Field, LootboundWorld.Go);
            At(18, LootboundWorld.Station, LootboundWorld.Go);
            At(21, LootboundWorld.Station, LootboundWorld.Act);
            At(24, LootboundWorld.Field, LootboundWorld.Go);
            At(27, LootboundWorld.Clearing, LootboundWorld.Go);
            At(30, LootboundWorld.Field, LootboundWorld.Go);
            At(33, LootboundWorld.Shelter, LootboundWorld.Go);
            At(36, LootboundWorld.Shelter, LootboundWorld.Act);
            At(39, LootboundWorld.Shelter, LootboundWorld.Act);

            SimulationState state = LootboundWorld.BuildInitialState();
            RelationSet relations = LootboundWorld.BuildRelations(state);
            IReadOnlyList<ITransition> laws = LootboundWorld.BuildLaws();
            TickRunner runner = LootboundWorld.BuildRunner();
            var traveller = new TravellerProducer();

            for (long t = 1; t <= 45; t++)
            {
                traveller.Step(state, trace);
                state = runner.Run(state, relations, laws, trace, 1);
            }

            Assert.IsFalse(traveller.Done, "L-007: the first passage is over, but the traveller is only away — not gone.");
            Assert.AreEqual(LootboundWorld.Station.Value, state.ValueAt(Loc(LootboundWorld.NewSword)),
                "The clearing's sword was moved by a biography the player never saw.");

            // L-007 — the return: the same body comes back, walks straight to where it left its
            // object, takes it up, and puts it back where it found it. The world was never reset.
            for (long t = 46; t <= 170; t++)
            {
                traveller.Step(state, trace);
                state = runner.Run(state, relations, laws, trace, 1);
            }

            Assert.IsTrue(traveller.Done, "Two passages, then gone for good.");
            Assert.AreEqual(LootboundWorld.Clearing.Value, state.ValueAt(Loc(LootboundWorld.NewSword)),
                "The sword lies in the clearing again: continuity restored what passage displaced.");
            foreach (Place place in LootboundWorld.Spatial)
            {
                Assert.AreEqual(0, state.ValueAt(B(place)), "The body is gone from everywhere, again.");
            }

            // The absence, as the world's record states it: when the player entered the clearing
            // (their script, t=27..30), what did the biography of the OTHER sword hold?
            string otherBiography = ChronicleWithTraveller(trace, 45, LootboundWorld.NewSword);
            StringAssert.Contains("held (body B)", otherBiography);
            StringAssert.Contains("station ground", otherBiography);

            // Replay stays exact and free: one trace, several biographies, same world twice.
            SimulationState replayA = LootboundWorld.BuildInitialState();
            SimulationState replayB = LootboundWorld.BuildInitialState();
            for (long t = 1; t <= 170; t++)
            {
                replayA = runner.Run(replayA, relations, laws, trace, 1);
                replayB = runner.Run(replayB, relations, laws, trace, 1);
            }

            Assert.AreEqual(replayA, replayB, "Co-inhabited replay is exact.");
            Assert.AreEqual(state, replayA, "The traveller's decisions live in the trace: replay needs no traveller.");
        }

        private static string ChronicleWithTraveller(ExternalEventTrace trace, long ticks, Place sword)
        {
            return BiographyChronicler.Chronicle(trace, ticks, sword, "co-run", sword.ToString());
        }
    }
}
