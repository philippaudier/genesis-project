using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;
using Genesis.Simulation.Lootbound;

namespace Genesis.Tests
{
    /// <summary>
    /// L-009 — Persistent History (RFC-L003). The world no longer starts over: a session is an
    /// observation window on a continuing world, and persistence is achieved by replay — the
    /// world's file is its trace. These tests are the RFC's kill criterion made mechanical, and
    /// the standing invariant-6 audit: a world reconstructible from initial + trace provably has
    /// no hidden state.
    /// </summary>
    public sealed class PersistenceTests
    {
        private static ExternalEventTrace SessionTrace()
        {
            var trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            // A life across what will become a session boundary at t=60.
            At(0, LootboundWorld.Shelter, LootboundWorld.Act);      // acquisition
            At(3, LootboundWorld.Field, LootboundWorld.Go);
            At(6, LootboundWorld.Tree, LootboundWorld.Go);
            At(9, LootboundWorld.Tree, LootboundWorld.Attack);
            At(12, LootboundWorld.Tree, LootboundWorld.Attack);
            At(15, LootboundWorld.Field, LootboundWorld.Go);
            At(18, LootboundWorld.Station, LootboundWorld.Go);
            At(21, LootboundWorld.Station, LootboundWorld.Act);     // repair
            At(70, LootboundWorld.Field, LootboundWorld.Go);        // "the next evening"
            At(73, LootboundWorld.Shelter, LootboundWorld.Go);
            At(76, LootboundWorld.Shelter, LootboundWorld.Act);     // stored, in another session
            return trace;
        }

        [Test]
        public void A_Continued_World_Equals_An_Uninterrupted_One()
        {
            ExternalEventTrace trace = SessionTrace();
            SimulationState initial = LootboundWorld.BuildInitialState();
            RelationSet relations = LootboundWorld.BuildRelations(initial);
            IReadOnlyList<ITransition> laws = LootboundWorld.BuildLaws();
            TickRunner runner = LootboundWorld.BuildRunner();

            // One life, never interrupted.
            SimulationState uninterrupted = runner.Run(initial, relations, laws, trace, 100);

            // The same life, lived as two sessions: the window closes at t=60 and DISCARDS its
            // state; reopening replays initial + trace — the world's file is its trace — then
            // the world simply continues.
            SimulationState firstSession = runner.Run(initial, relations, laws, trace, 60);
            Assert.IsNotNull(firstSession); // the window may forget it; the record does not.

            SimulationState reopened = runner.Run(initial, relations, laws, trace, 60);
            SimulationState continued = runner.Run(reopened, relations, laws, trace, 40);

            Assert.AreEqual(uninterrupted, continued,
                "A continued world must equal an uninterrupted one — else the kernel holds hidden state (invariant 6).");
        }

        [Test]
        public void Opening_A_New_World_Is_Replaying_An_Empty_History()
        {
            // The practice article — "a session is never allowed to know whether it is the
            // first" — holds because opening is ONE operation: replay what the trace holds.
            // A new world is the special case of nothing to replay, not a different code path.
            var empty = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            SimulationState initial = LootboundWorld.BuildInitialState();
            RelationSet relations = LootboundWorld.BuildRelations(initial);
            SimulationState opened = LootboundWorld.BuildRunner()
                .Run(initial, relations, LootboundWorld.BuildLaws(), empty, 0);

            Assert.AreEqual(initial, opened, "Zero history replays to the initial state, exactly.");
        }

        [Test]
        public void A_Fresh_Producer_On_A_Departed_World_Retires_Silently()
        {
            // Persistence's one honest guard: a producer whose body already left the world —
            // a continued world where the visit happened in an earlier session — must not emit
            // dead intents forever. It discovers the absence and retires. (The being does NOT
            // return in L-009: producer policy across sessions stays deferred, per RFC-L003.)
            SimulationState state = LootboundWorld.BuildInitialState();
            RelationSet relations = LootboundWorld.BuildRelations(state);
            IReadOnlyList<ITransition> laws = LootboundWorld.BuildLaws();
            TickRunner runner = LootboundWorld.BuildRunner();
            var trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());

            var firstVisit = new TravellerProducer();
            for (long t = 1; t <= 170; t++)
            {
                firstVisit.Step(state, trace);
                state = runner.Run(state, relations, laws, trace, 1);
            }

            Assert.IsTrue(firstVisit.Done, "The visit completed and the body left.");

            int crossings = trace.Events.Count;
            var nextEvening = new TravellerProducer();
            for (long t = 171; t <= 180; t++)
            {
                nextEvening.Step(state, trace);
                state = runner.Run(state, relations, laws, trace, 1);
            }

            Assert.IsTrue(nextEvening.Done, "A fresh producer finds no body and retires.");
            Assert.AreEqual(crossings, trace.Events.Count, "It emits nothing — no dead intents pollute the world's record.");
        }
    }
}
