using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Genesis-010 — the proof. A transition can observe relation-discovered addresses and read their
    /// snapshot values through a deterministic, one-hop, explicitly declared relational view — and
    /// nothing outside that contract is observable.
    /// </summary>
    public sealed class RelationalViewTests
    {
        // Witness world: A = 5, B = 8, C = 13, with A -> B and B -> C.
        private static SimulationState WitnessState()
        {
            return TestAddresses.StateWith(5, 8, 13);
        }

        private static RelationSet WitnessRelations(SimulationState state)
        {
            return new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.B),
                new Relation(TestAddresses.B, TestAddresses.C));
        }

        private static RelationalStateView ViewFor(ReadScope readScope, RelationScope relationScope)
        {
            SimulationState state = WitnessState();
            return new RelationalStateView(state, WitnessRelations(state), readScope, relationScope);
        }

        [Test]
        public void Directly_Declared_Address_Remains_Readable()
        {
            RelationalStateView view = ViewFor(new ReadScope(TestAddresses.A), RelationScope.Empty);

            Assert.AreEqual(5L, view.Read(TestAddresses.A));
        }

        [Test]
        public void Declared_Outgoing_Relation_Is_Visible()
        {
            RelationalStateView view = ViewFor(ReadScope.Empty, new RelationScope(TestAddresses.A));

            IReadOnlyList<Relation> outgoing = view.OutgoingRelations(TestAddresses.A);

            Assert.AreEqual(1, outgoing.Count);
            Assert.AreEqual(new Relation(TestAddresses.A, TestAddresses.B), outgoing[0]);
        }

        [Test]
        public void Relation_Discovered_Target_Is_Readable()
        {
            // B is not in ReadScope; it becomes readable only through the declared origin A.
            RelationalStateView view = ViewFor(ReadScope.Empty, new RelationScope(TestAddresses.A));

            Assert.AreEqual(8L, view.Read(TestAddresses.B));
        }

        [Test]
        public void Relation_Discovered_Target_Does_Not_Grant_Transitive_Read()
        {
            // A -> B and B -> C exist, but observing A grants one hop only: C stays unreadable.
            RelationalStateView view = ViewFor(ReadScope.Empty, new RelationScope(TestAddresses.A));

            Assert.Throws<ReadOutOfScopeException>(() => view.Read(TestAddresses.C));
        }

        [Test]
        public void Discovered_Target_Does_Not_Become_An_Implicit_Origin()
        {
            // B was discovered through A, but B's own relations remain invisible.
            RelationalStateView view = ViewFor(ReadScope.Empty, new RelationScope(TestAddresses.A));

            Assert.Throws<RelationOutOfScopeException>(() => view.OutgoingRelations(TestAddresses.B));
        }

        [Test]
        public void Undeclared_Origin_Cannot_Expose_Its_Relations()
        {
            RelationalStateView view = ViewFor(new ReadScope(TestAddresses.B), RelationScope.Empty);

            Assert.Throws<RelationOutOfScopeException>(() => view.OutgoingRelations(TestAddresses.B));
        }

        [Test]
        public void No_Relations_Means_No_Discovered_Reads()
        {
            SimulationState state = WitnessState();
            var noRelations = new RelationSet(state); // origin declared, but nothing points anywhere

            var view = new RelationalStateView(
                state, noRelations, ReadScope.Empty, new RelationScope(TestAddresses.A));

            Assert.AreEqual(0, view.OutgoingRelations(TestAddresses.A).Count);
            Assert.Throws<ReadOutOfScopeException>(() => view.Read(TestAddresses.B));
        }

        [Test]
        public void Outgoing_Relations_Are_Enumerated_Canonically()
        {
            // A -> C inserted before A -> B; enumeration must still be target-ascending: B, then C.
            SimulationState state = WitnessState();
            var relations = new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.C),
                new Relation(TestAddresses.A, TestAddresses.B));

            var view = new RelationalStateView(
                state, relations, ReadScope.Empty, new RelationScope(TestAddresses.A));
            IReadOnlyList<Relation> outgoing = view.OutgoingRelations(TestAddresses.A);

            Assert.AreEqual(2, outgoing.Count);
            Assert.AreEqual(TestAddresses.B, outgoing[0].Target);
            Assert.AreEqual(TestAddresses.C, outgoing[1].Target);
        }

        [Test]
        public void Relation_Enumeration_Is_Independent_Of_Insertion_Order()
        {
            SimulationState state = WitnessState();
            var forward = new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.B),
                new Relation(TestAddresses.A, TestAddresses.C));
            var reversed = new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.C),
                new Relation(TestAddresses.A, TestAddresses.B));

            CollectionAssert.AreEqual(
                forward.OutgoingFrom(TestAddresses.A),
                reversed.OutgoingFrom(TestAddresses.A));
        }

        [Test]
        public void Relational_And_Direct_Reads_Use_The_Same_Snapshot()
        {
            // T1 writes +100 to B this tick; T2 reads B through A -> B and copies it to C.
            // T2 must see the snapshot's B (8), never the in-flight 108 — whatever the order.
            SimulationState initial = WitnessState();
            RelationSet relations = WitnessRelations(initial);
            ITransition addToB = new AddToCounterTransition(TestAddresses.B, 100);
            ITransition copyThrough = new CopyFirstOutgoingTargetTransition(TestAddresses.A, TestAddresses.C);
            var runner = new TickRunner(new TransitionRunner());

            SimulationState forward = runner.Run(initial, relations, new[] { addToB, copyThrough }, 1);
            SimulationState reverse = runner.Run(initial, relations, new[] { copyThrough, addToB }, 1);

            Assert.AreEqual(108L, forward.CounterOf(TestAddresses.B)); // 8 + 100
            Assert.AreEqual(21L, forward.CounterOf(TestAddresses.C));  // 13 + snapshot B (8), not 108
            Assert.AreEqual(forward, reverse);
        }

        [Test]
        public void Initial_State_Is_Never_Mutated()
        {
            SimulationState initial = WitnessState();
            RelationSet relations = WitnessRelations(initial);
            var transitions = new ITransition[]
            {
                new CopyFirstOutgoingTargetTransition(TestAddresses.A, TestAddresses.C)
            };

            new TickRunner(new TransitionRunner()).Run(initial, relations, transitions, 1000);

            Assert.AreEqual(WitnessState(), initial);
        }

        [Test]
        public void Two_Runs_Over_1000_Ticks_Remain_Strictly_Identical()
        {
            SimulationState initial = WitnessState();
            RelationSet relations = WitnessRelations(initial);
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.B, 1),
                new CopyFirstOutgoingTargetTransition(TestAddresses.A, TestAddresses.C)
            };
            var runner = new TickRunner(new TransitionRunner());

            SimulationState first = runner.Run(initial, relations, transitions, 1000);
            SimulationState second = runner.Run(initial, relations, transitions, 1000);

            Assert.AreEqual(first, second);
        }

        /// <summary>
        /// Test witness: declares one origin, follows its first outgoing relation (canonical order),
        /// reads the discovered target's snapshot value, and contributes it to a destination address.
        /// The first transition whose behaviour depends on a relation.
        /// </summary>
        private sealed class CopyFirstOutgoingTargetTransition : ITransition
        {
            private readonly CounterAddress _origin;
            private readonly CounterAddress _destination;
            private readonly RelationScope _relationScope;

            public CopyFirstOutgoingTargetTransition(CounterAddress origin, CounterAddress destination)
            {
                _origin = origin;
                _destination = destination;
                _relationScope = new RelationScope(origin);
            }

            public ReadScope ReadScope => ReadScope.Empty;

            public RelationScope RelationScope => _relationScope;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_origin);
                if (outgoing.Count == 0)
                {
                    return new Contribution[0];
                }

                long discovered = view.Read(outgoing[0].Target);
                return new[] { new Contribution(_destination, discovered) };
            }
        }
    }
}
