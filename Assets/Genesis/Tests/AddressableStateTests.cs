using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The addressability proof (Genesis-008, kept as a regression guard, re-keyed under RFC-0003):
    /// distinct cells hold independent values, contributions target cells, conflicts group by cell —
    /// and, new under heterogeneity, same place + different kinds never conflict (D1). Equality never
    /// depends on insertion order.
    /// </summary>
    public sealed class AddressableStateTests
    {
        private static TickRunner NewRunner()
        {
            return new TickRunner(new TransitionRunner());
        }

        [Test]
        public void Distinct_Cells_Hold_Independent_Values()
        {
            SimulationState state = TestAddresses.StateWith(5, 8, 13);

            Assert.AreEqual(5L, state.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(8L, state.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(13L, state.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Transition_Cannot_Read_Undeclared_Cell()
        {
            var transitions = new ITransition[] { new OutOfScopeTransition() }; // declares {CellA}, reads CellB

            Assert.Throws<ReadOutOfScopeException>(
                () => NewRunner().Run(TestAddresses.InitialState(), transitions, 1));
        }

        [Test]
        public void Contribution_Updates_Only_Its_Target_Cell()
        {
            SimulationState initial = TestAddresses.StateWith(1, 2, 3);
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellB, 7)
            };

            SimulationState result = NewRunner().Run(initial, transitions, 1);

            Assert.AreEqual(1L, result.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(9L, result.ValueAt(TestAddresses.CellB)); // 2 + 7
            Assert.AreEqual(3L, result.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Contributions_To_The_Same_Cell_Conflict()
        {
            var counting = new CountingResolver(new AdditionResolver());
            var resolvers = new Dictionary<Kind, IConflictResolver> { { TestAddresses.K, counting } };
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellB, 2),
                new AddToCounterTransition(TestAddresses.CellB, 3)
            };

            SimulationState result = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(5L, result.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(1, counting.Calls);
        }

        [Test]
        public void Contributions_To_Different_Places_Do_Not_Conflict()
        {
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellB, 2),
                new AddToCounterTransition(TestAddresses.CellC, 3)
            };

            SimulationState result = NewRunner().Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(2L, result.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(3L, result.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Contributions_To_Same_Place_Different_Kinds_Do_Not_Conflict()
        {
            // RFC-0003 D1 made testable: the cell — not the place — is the unit of conflict.
            var otherKind = new Kind(2);
            var cellAK = TestAddresses.CellA;
            var cellA2 = new Cell(TestAddresses.A, otherKind);
            var state = new SimulationState(Tick.Zero, new Dictionary<Cell, long>
            {
                { cellAK, 0 },
                { cellA2, 0 }
            });
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(cellAK, 2),
                new AddToCounterTransition(cellA2, 3)
            };

            // No resolver anywhere — if these conflicted, the run would throw.
            SimulationState result = NewRunner().Run(state, transitions, 1);

            Assert.AreEqual(2L, result.ValueAt(cellAK));
            Assert.AreEqual(3L, result.ValueAt(cellA2));
        }

        [Test]
        public void State_Equality_Is_Independent_Of_Insertion_Order()
        {
            var forward = new SimulationState(Tick.Zero, new Dictionary<Cell, long>
            {
                { TestAddresses.CellA, 5 },
                { TestAddresses.CellB, 8 },
                { TestAddresses.CellC, 13 }
            });
            var reversed = new SimulationState(Tick.Zero, new Dictionary<Cell, long>
            {
                { TestAddresses.CellC, 13 },
                { TestAddresses.CellA, 5 },
                { TestAddresses.CellB, 8 }
            });

            Assert.AreEqual(forward, reversed);
            Assert.AreEqual(forward.GetHashCode(), reversed.GetHashCode());
        }

        [Test]
        public void Enumeration_Order_Remains_Irrelevant()
        {
            var resolvers = new Dictionary<Kind, IConflictResolver>
            {
                { TestAddresses.K, new AdditionResolver() }
            };
            var forward = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellA, 2),
                new AddToCounterTransition(TestAddresses.CellA, 3),
                new AddToCounterTransition(TestAddresses.CellB, 1)
            };
            var reversed = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.CellB, 1),
                new AddToCounterTransition(TestAddresses.CellA, 3),
                new AddToCounterTransition(TestAddresses.CellA, 2)
            };

            SimulationState first = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), forward, 100);
            SimulationState second = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), reversed, 100);

            Assert.AreEqual(first, second);
        }

        /// <summary>Declares {CellA} but tries to read CellB — a deliberately misbehaving transition.</summary>
        private sealed class OutOfScopeTransition : ITransition
        {
            public ReadScope ReadScope => new ReadScope(TestAddresses.CellA);

            public RelationScope RelationScope => RelationScope.Empty;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                return new[] { new Contribution(TestAddresses.CellC, view.Read(TestAddresses.CellB)) };
            }
        }

        /// <summary>Test double: wraps a resolver and counts how many times it is invoked.</summary>
        internal sealed class CountingResolver : IConflictResolver
        {
            private readonly IConflictResolver _inner;

            public CountingResolver(IConflictResolver inner)
            {
                _inner = inner;
            }

            public int Calls { get; private set; }

            public long Resolve(IReadOnlyList<long> amounts)
            {
                Calls++;
                return _inner.Resolve(amounts);
            }
        }
    }
}
