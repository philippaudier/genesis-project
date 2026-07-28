using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;
using Genesis.Simulation.Lootbound;

namespace Genesis.Tests
{
    /// <summary>
    /// L-008 — Interference. When does a biography become a cause in another biography? The
    /// minimum was built: the station's dead case was lifted (no new law, no new kind, no new
    /// cell) so the player can take what the Traveller cached — and the Traveller's brain was not
    /// touched: it returns, finds nothing, and its gestures lawfully do nothing. The "…" stays
    /// empty by design.
    /// </summary>
    public sealed class InterferenceTests
    {
        private static Cell Loc(Place sword) => new Cell(sword, LootboundWorld.Location);

        [Test]
        public void Repair_Behaviour_Is_Unchanged_Where_Repair_Is_Possible()
        {
            // Regression: the exact L-002 scenario — acquire, harvest, repair — must behave
            // identically. Only the previously-dead case gained a meaning.
            var trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            At(0, LootboundWorld.Shelter, LootboundWorld.Act);
            At(3, LootboundWorld.Field, LootboundWorld.Go);
            At(6, LootboundWorld.Tree, LootboundWorld.Go);
            At(9, LootboundWorld.Tree, LootboundWorld.Attack);
            At(12, LootboundWorld.Field, LootboundWorld.Go);
            At(15, LootboundWorld.Station, LootboundWorld.Go);
            At(18, LootboundWorld.Station, LootboundWorld.Act);

            SimulationState state = LootboundWorld.BuildInitialState();
            var relations = LootboundWorld.BuildRelations(state);
            var laws = LootboundWorld.BuildLaws();
            var runner = LootboundWorld.BuildRunner();
            state = runner.Run(state, relations, laws, trace, 22);

            Assert.AreEqual(0, state.ValueAt(new Cell(LootboundWorld.OldSword, LootboundWorld.Wear)), "Repaired.");
            Assert.AreEqual(1, state.ValueAt(new Cell(LootboundWorld.OldSword, LootboundWorld.Repairs)));
            Assert.AreEqual(0, state.ValueAt(new Cell(LootboundWorld.Pack, LootboundWorld.Wood)), "Wood spent on the repair, not a swap.");
            Assert.AreEqual(0, state.ValueAt(Loc(LootboundWorld.OldSword)), "Still in hand: repair won over exchange.");
        }

        [Test]
        public void The_Lifted_Case_Takes_What_Lies_At_The_Station()
        {
            // The traveller caches the clearing's sword at the station (first passage), and the
            // player — empty-handed, no wood, nothing to repair — takes it.
            var trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            At(30, LootboundWorld.Field, LootboundWorld.Go);
            At(33, LootboundWorld.Station, LootboundWorld.Go);
            At(36, LootboundWorld.Station, LootboundWorld.Act);

            SimulationState state = LootboundWorld.BuildInitialState();
            var relations = LootboundWorld.BuildRelations(state);
            var laws = LootboundWorld.BuildLaws();
            var runner = LootboundWorld.BuildRunner();
            var traveller = new TravellerProducer();

            for (long t = 1; t <= 40; t++)
            {
                traveller.Step(state, trace);
                state = runner.Run(state, relations, laws, trace, 1);
            }

            Assert.AreEqual(0, state.ValueAt(Loc(LootboundWorld.NewSword)),
                "The cached sword is in the player's hand: the interference happened.");
        }

        [Test]
        public void The_Traveller_Returns_Finds_Nothing_And_Its_Gestures_Lawfully_Do_Nothing()
        {
            // The full interference: the player empties the cache during the absence; the
            // traveller returns unchanged, its station toggle finds nothing, its clearing toggle
            // finds nothing, and it leaves. The "…" is empty — and the record shows it.
            var trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            At(30, LootboundWorld.Field, LootboundWorld.Go);
            At(33, LootboundWorld.Station, LootboundWorld.Go);
            At(36, LootboundWorld.Station, LootboundWorld.Act);   // take the cache
            At(40, LootboundWorld.Field, LootboundWorld.Go);
            At(43, LootboundWorld.Shelter, LootboundWorld.Go);    // go home with it

            SimulationState state = LootboundWorld.BuildInitialState();
            var relations = LootboundWorld.BuildRelations(state);
            var laws = LootboundWorld.BuildLaws();
            var runner = LootboundWorld.BuildRunner();
            var traveller = new TravellerProducer();

            for (long t = 1; t <= 170; t++)
            {
                traveller.Step(state, trace);
                state = runner.Run(state, relations, laws, trace, 1);
            }

            Assert.IsTrue(traveller.Done, "The itinerary completed — interrupted in effect, not in motion.");
            Assert.AreEqual(0, state.ValueAt(Loc(LootboundWorld.NewSword)),
                "The sword stayed with the player: return without recovery.");
            foreach (Place place in LootboundWorld.Spatial)
            {
                Assert.AreEqual(0, state.ValueAt(new Cell(place, LootboundWorld.BodyB)), "Gone, empty-handed.");
            }

            // The object's biography now holds both hands — the interference, told by the world:
            string biography = BiographyChronicler.Chronicle(trace, 170, LootboundWorld.NewSword, "interference", "Sword-2000");
            StringAssert.Contains("held (body B)", biography);
            StringAssert.Contains("in hand", biography);
        }
    }
}
