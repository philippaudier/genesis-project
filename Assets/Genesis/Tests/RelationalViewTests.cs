using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The relational-views proof (Genesis-010, kept as a regression guard, re-keyed under RFC-0003
    /// D3/D4): a transition observes relation-discovered places and reads granted kinds' snapshot
    /// values through a deterministic, one-hop, explicitly declared view. The relation discovers
    /// places only; kind visibility is granted entirely by the observing transition.
    /// </summary>
    public sealed class RelationalViewTests
    {
        // Witness world: A = 5, B = 8, C = 13 (single kind K), with A -> B and B -> C.
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

        private static RelationScope ObservingK(params Place[] origins)
        {
            return new RelationScope(origins, new[] { TestAddresses.K });
        }

        private static RelationalStateView ViewFor(ReadScope readScope, RelationScope relationScope)
        {
            SimulationState state = WitnessState();
            return new RelationalStateView(state, WitnessRelations(state), readScope, relationScope);
        }

        [Test]
        public void Directly_Declared_Cell_Remains_Readable()
        {
            RelationalStateView view = ViewFor(new ReadScope(TestAddresses.CellA), RelationScope.Empty);

            Assert.AreEqual(5L, view.Read(TestAddresses.CellA));
        }

        [Test]
        public void Declared_Outgoing_Relation_Is_Visible()
        {
            RelationalStateView view = ViewFor(ReadScope.Empty, ObservingK(TestAddresses.A));

            IReadOnlyList<Relation> outgoing = view.OutgoingRelations(TestAddresses.A);

            Assert.AreEqual(1, outgoing.Count);
            Assert.AreEqual(new Relation(TestAddresses.A, TestAddresses.B), outgoing[0]);
        }

        [Test]
        public void Relation_Discovered_Target_Is_Readable_In_Granted_Kind()
        {
            RelationalStateView view = ViewFor(ReadScope.Empty, ObservingK(TestAddresses.A));

            Assert.AreEqual(8L, view.Read(TestAddresses.CellB)); // granted through A -> B
        }

        [Test]
        public void Relation_Discovered_Target_Does_Not_Grant_Transitive_Read()
        {
            // A -> B and B -> C exist, but observing A grants one hop only: C stays unreadable.
            RelationalStateView view = ViewFor(ReadScope.Empty, ObservingK(TestAddresses.A));

            Assert.Throws<ReadOutOfScopeException>(() => view.Read(TestAddresses.CellC));
        }

        [Test]
        public void Discovered_Target_Does_Not_Become_An_Implicit_Origin()
        {
            RelationalStateView view = ViewFor(ReadScope.Empty, ObservingK(TestAddresses.A));

            Assert.Throws<RelationOutOfScopeException>(() => view.OutgoingRelations(TestAddresses.B));
        }

        [Test]
        public void Undeclared_Origin_Cannot_Expose_Its_Relations()
        {
            RelationalStateView view = ViewFor(new ReadScope(TestAddresses.CellB), RelationScope.Empty);

            Assert.Throws<RelationOutOfScopeException>(() => view.OutgoingRelations(TestAddresses.B));
        }

        [Test]
        public void No_Relations_Means_No_Discovered_Reads()
        {
            SimulationState state = WitnessState();
            var noRelations = new RelationSet(state);

            var view = new RelationalStateView(state, noRelations, ReadScope.Empty, ObservingK(TestAddresses.A));

            Assert.AreEqual(0, view.OutgoingRelations(TestAddresses.A).Count);
            Assert.Throws<ReadOutOfScopeException>(() => view.Read(TestAddresses.CellB));
        }

        [Test]
        public void Outgoing_Relations_Are_Enumerated_Canonically()
        {
            SimulationState state = WitnessState();
            var relations = new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.C),
                new Relation(TestAddresses.A, TestAddresses.B));

            var view = new RelationalStateView(state, relations, ReadScope.Empty, ObservingK(TestAddresses.A));
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
            SimulationState initial = WitnessState();
            RelationSet relations = WitnessRelations(initial);
            ITransition addToB = new AddToCounterTransition(TestAddresses.CellB, 100);
            ITransition copyThrough = new CopyFirstOutgoingTargetTransition(
                TestAddresses.A, TestAddresses.CellC, TestAddresses.K);
            var runner = new TickRunner(new TransitionRunner());

            SimulationState forward = runner.Run(initial, relations, new[] { addToB, copyThrough }, 1);
            SimulationState reverse = runner.Run(initial, relations, new[] { copyThrough, addToB }, 1);

            Assert.AreEqual(108L, forward.ValueAt(TestAddresses.CellB)); // 8 + 100
            Assert.AreEqual(21L, forward.ValueAt(TestAddresses.CellC));  // 13 + snapshot B (8), not 108
            Assert.AreEqual(forward, reverse);
        }

        [Test]
        public void Initial_State_Is_Never_Mutated()
        {
            SimulationState initial = WitnessState();
            RelationSet relations = WitnessRelations(initial);
            var transitions = new ITransition[]
            {
                new CopyFirstOutgoingTargetTransition(TestAddresses.A, TestAddresses.CellC, TestAddresses.K)
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
                new AddToCounterTransition(TestAddresses.CellB, 1),
                new CopyFirstOutgoingTargetTransition(TestAddresses.A, TestAddresses.CellC, TestAddresses.K)
            };
            var runner = new TickRunner(new TransitionRunner());

            SimulationState first = runner.Run(initial, relations, transitions, 1000);
            SimulationState second = runner.Run(initial, relations, transitions, 1000);

            Assert.AreEqual(first, second);
        }

        /// <summary>
        /// Test witness: declares one origin place and one granted kind, follows the first outgoing
        /// relation (canonical order), reads the discovered target's granted cell, and contributes
        /// its value to a destination cell.
        /// </summary>
        private sealed class CopyFirstOutgoingTargetTransition : ITransition
        {
            private readonly Place _origin;
            private readonly Cell _destination;
            private readonly Kind _kind;
            private readonly RelationScope _relationScope;

            public CopyFirstOutgoingTargetTransition(Place origin, Cell destination, Kind kind)
            {
                _origin = origin;
                _destination = destination;
                _kind = kind;
                _relationScope = new RelationScope(new[] { origin }, new[] { kind });
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

                long discovered = view.Read(new Cell(outgoing[0].Target, _kind));
                return new[] { new Contribution(_destination, discovered) };
            }
        }
    }
}
