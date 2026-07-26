using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;
using Genesis.Simulation.Lootbound;

namespace Genesis.Tests
{
    /// <summary>
    /// L-002 — The First Living World. The smallest world able to produce a biography: five spatial
    /// places on a star graph, two swords, one resource, five interpreting laws behind the membrane.
    /// The final test is the milestone's Definition of Done: a full session driven purely through
    /// the trace produces a biography document — written by the world, not by a developer — whose
    /// seven moments are irreversible firsts.
    /// </summary>
    public sealed class LootboundWorldTests
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

        private static Cell P(Place place) => new Cell(place, LootboundWorld.PlayerAt);
        private static Cell Loc(Place sword) => new Cell(sword, LootboundWorld.Location);

        [Test]
        public void Walking_Follows_Relations_Only()
        {
            var (state, relations, laws, runner, trace) = NewWorld();

            // Shelter and Tree are not connected: the intent is consumed, the player does not move.
            trace.Append(new ExternalEvent(Tick.Zero, new Cell(LootboundWorld.Tree, LootboundWorld.Go), 1));
            state = runner.Run(state, relations, laws, trace, 3);
            Assert.AreEqual(1, state.ValueAt(P(LootboundWorld.Shelter)), "No teleporting: Tree is not adjacent to Shelter.");
            Assert.AreEqual(0, state.ValueAt(P(LootboundWorld.Tree)));

            // Shelter → Field → Tree works, one relation at a time.
            trace.Append(new ExternalEvent(new Tick(3), new Cell(LootboundWorld.Field, LootboundWorld.Go), 1));
            trace.Append(new ExternalEvent(new Tick(6), new Cell(LootboundWorld.Tree, LootboundWorld.Go), 1));
            state = runner.Run(state, relations, laws, trace, 6);
            Assert.AreEqual(1, state.ValueAt(P(LootboundWorld.Tree)));
            Assert.AreEqual(0, state.ValueAt(P(LootboundWorld.Shelter)));
        }

        [Test]
        public void The_First_Interaction_At_Home_Is_The_Acquisition()
        {
            var (state, relations, laws, runner, trace) = NewWorld();
            Assert.AreEqual(LootboundWorld.Shelter.Value, state.ValueAt(Loc(LootboundWorld.OldSword)), "The old sword begins in the chest.");

            trace.Append(new ExternalEvent(Tick.Zero, new Cell(LootboundWorld.Shelter, LootboundWorld.Act), 1));
            state = runner.Run(state, relations, laws, trace, 3);
            Assert.AreEqual(0, state.ValueAt(Loc(LootboundWorld.OldSword)), "Retrieved: in hand — the acquisition.");
        }

        [Test]
        public void Striking_The_Tree_Yields_Wood_And_Wears_The_Carried_Sword()
        {
            var (state, relations, laws, runner, trace) = NewWorld();
            trace.Append(new ExternalEvent(Tick.Zero, new Cell(LootboundWorld.Shelter, LootboundWorld.Act), 1));
            trace.Append(new ExternalEvent(new Tick(3), new Cell(LootboundWorld.Field, LootboundWorld.Go), 1));
            trace.Append(new ExternalEvent(new Tick(6), new Cell(LootboundWorld.Tree, LootboundWorld.Go), 1));
            trace.Append(new ExternalEvent(new Tick(9), new Cell(LootboundWorld.Tree, LootboundWorld.Attack), 1));
            state = runner.Run(state, relations, laws, trace, 12);

            Assert.AreEqual(1, state.ValueAt(new Cell(LootboundWorld.Pack, LootboundWorld.Wood)));
            Assert.AreEqual(1, state.ValueAt(new Cell(LootboundWorld.OldSword, LootboundWorld.Wear)));
        }

        [Test]
        public void The_World_Writes_The_First_Biography()
        {
            // The full scripted session — a bot producer, legitimate since L-001 (provenance-
            // blindness): acquire, harvest, repair, discover the better sword, refuse it, store the
            // old one, take it back. Every gesture crosses the membrane; nothing else touches the
            // world.
            var trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            void At(long t, Place place, Kind kind) => trace.Append(new ExternalEvent(new Tick(t), new Cell(place, kind), 1));

            At(0, LootboundWorld.Shelter, LootboundWorld.Act);      // acquisition
            At(3, LootboundWorld.Field, LootboundWorld.Go);
            At(6, LootboundWorld.Tree, LootboundWorld.Go);
            At(9, LootboundWorld.Tree, LootboundWorld.Attack);      // first wear, wood 1
            At(12, LootboundWorld.Tree, LootboundWorld.Attack);     // wood 2
            At(15, LootboundWorld.Field, LootboundWorld.Go);
            At(18, LootboundWorld.Station, LootboundWorld.Go);
            At(21, LootboundWorld.Station, LootboundWorld.Act);     // first repair
            At(24, LootboundWorld.Field, LootboundWorld.Go);
            At(27, LootboundWorld.Clearing, LootboundWorld.Go);     // discovery
            At(30, LootboundWorld.Field, LootboundWorld.Go);        // refusal: leaves, old sword in hand
            At(33, LootboundWorld.Shelter, LootboundWorld.Go);
            At(36, LootboundWorld.Shelter, LootboundWorld.Act);     // stored
            At(39, LootboundWorld.Shelter, LootboundWorld.Act);     // voluntary re-equipment

            string biography = BiographyChronicler.Chronicle(trace, 45, LootboundWorld.OldSword, "Run-000001", "Sword-1000");

            StringAssert.Contains("Acquisition", biography);
            StringAssert.Contains("First wear", biography);
            StringAssert.Contains("First repair", biography);
            StringAssert.Contains("First superior sword discovered", biography);
            StringAssert.Contains("Replacement refused", biography);
            StringAssert.Contains("Stored at the shelter", biography);
            StringAssert.Contains("Voluntary re-equipment", biography);
            StringAssert.Contains("Status: First complete biography observed.", biography);

            // Irreversibility: replaying the same run yields the same biography — and each first
            // occurred exactly once.
            string replayed = BiographyChronicler.Chronicle(trace, 45, LootboundWorld.OldSword, "Run-000001", "Sword-1000");
            Assert.AreEqual(biography, replayed, "Two researchers reading the same run must produce the same biography.");

            TestContext.WriteLine(biography);
        }
    }
}
