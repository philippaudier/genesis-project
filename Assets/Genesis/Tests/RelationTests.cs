using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The explicit-relations proof (Genesis-009, kept as a regression guard): places can be
    /// connected through explicit, directed relations — structural identity, direction distinctness,
    /// set semantics independent of insertion order, rejection of unknown places (existence derived
    /// from cells, RFC-0003 D5) — and none of it touches the simulation state.
    /// </summary>
    public sealed class RelationTests
    {
        [Test]
        public void Relation_Connects_Two_Known_Places()
        {
            SimulationState state = TestAddresses.InitialState();

            var relations = new RelationSet(state, new Relation(TestAddresses.A, TestAddresses.B));

            Assert.AreEqual(1, relations.Count);
            Assert.IsTrue(relations.Contains(new Relation(TestAddresses.A, TestAddresses.B)));
        }

        [Test]
        public void Reverse_Relation_Is_Distinct()
        {
            SimulationState state = TestAddresses.InitialState();

            var relations = new RelationSet(state, new Relation(TestAddresses.A, TestAddresses.B));

            Assert.IsTrue(relations.Contains(new Relation(TestAddresses.A, TestAddresses.B)));
            Assert.IsFalse(relations.Contains(new Relation(TestAddresses.B, TestAddresses.A)));
        }

        [Test]
        public void Duplicate_Relations_Do_Not_Change_The_Set()
        {
            SimulationState state = TestAddresses.InitialState();

            var relations = new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.B),
                new Relation(TestAddresses.A, TestAddresses.B));

            Assert.AreEqual(1, relations.Count);
        }

        [Test]
        public void Relation_Equality_Is_Independent_Of_Insertion_Order()
        {
            SimulationState state = TestAddresses.InitialState();
            var ab = new Relation(TestAddresses.A, TestAddresses.B);
            var bc = new Relation(TestAddresses.B, TestAddresses.C);

            var forward = new RelationSet(state, ab, bc);
            var reversed = new RelationSet(state, bc, ab);

            Assert.AreEqual(forward, reversed);
            Assert.AreEqual(forward.GetHashCode(), reversed.GetHashCode());
        }

        [Test]
        public void Relation_To_Unknown_Place_Is_Rejected()
        {
            SimulationState state = TestAddresses.InitialState();
            var unknown = new Place(999);

            Assert.Throws<System.ArgumentException>(
                () => new RelationSet(state, new Relation(unknown, TestAddresses.B)));
            Assert.Throws<System.ArgumentException>(
                () => new RelationSet(state, new Relation(TestAddresses.A, unknown)));
        }

        [Test]
        public void Relations_Do_Not_Modify_Simulation_State()
        {
            SimulationState state = TestAddresses.StateWith(1, 2, 3);

            var _ = new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.B),
                new Relation(TestAddresses.B, TestAddresses.C));

            Assert.AreEqual(TestAddresses.StateWith(1, 2, 3), state);
        }

        [Test]
        public void Existing_Computational_Kernel_Remains_Deterministic()
        {
            SimulationState initial = TestAddresses.InitialState();
            var relations = new RelationSet(initial, new Relation(TestAddresses.A, TestAddresses.B));
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellA, 1),
                new MirrorCounterTransition(TestAddresses.CellA, TestAddresses.CellC)
            };
            var runner = new TickRunner(new TransitionRunner());

            SimulationState first = runner.Run(initial, transitions, 1000);
            SimulationState second = runner.Run(initial, transitions, 1000);

            Assert.AreEqual(first, second);
            Assert.AreEqual(new RelationSet(initial, new Relation(TestAddresses.A, TestAddresses.B)), relations);
        }
    }
}
