using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Genesis-008 — the proof. Multiple homogeneous state locations can be independently addressed,
    /// observed, modified, conflict-resolved, and compared, without sacrificing determinism — and
    /// state equality never depends on insertion order.
    /// </summary>
    public sealed class AddressableStateTests
    {
        private static TickRunner NewRunner()
        {
            return new TickRunner(new TransitionRunner());
        }

        [Test]
        public void Distinct_Addresses_Hold_Independent_Values()
        {
            SimulationState state = TestAddresses.StateWith(5, 8, 13);

            Assert.AreEqual(5L, state.CounterOf(TestAddresses.A));
            Assert.AreEqual(8L, state.CounterOf(TestAddresses.B));
            Assert.AreEqual(13L, state.CounterOf(TestAddresses.C));
        }

        [Test]
        public void Transition_Reads_Only_Declared_Address()
        {
            SimulationState initial = TestAddresses.StateWith(5, 20, 0);
            var transitions = new ITransition[]
            {
                new MirrorCounterTransition(TestAddresses.A, TestAddresses.C)
            };

            SimulationState result = NewRunner().Run(initial, transitions, 1);

            Assert.AreEqual(5L, result.CounterOf(TestAddresses.C)); // read A(5) through its declared scope
        }

        [Test]
        public void Transition_Cannot_Read_Undeclared_Address()
        {
            var transitions = new ITransition[] { new OutOfScopeTransition() }; // declares {A}, reads B

            Assert.Throws<ReadOutOfScopeException>(
                () => NewRunner().Run(TestAddresses.InitialState(), transitions, 1));
        }

        [Test]
        public void Contribution_Updates_Only_Its_Target_Address()
        {
            SimulationState initial = TestAddresses.StateWith(1, 2, 3);
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.B, 7)
            };

            SimulationState result = NewRunner().Run(initial, transitions, 1);

            Assert.AreEqual(1L, result.CounterOf(TestAddresses.A)); // untouched
            Assert.AreEqual(9L, result.CounterOf(TestAddresses.B)); // 2 + 7
            Assert.AreEqual(3L, result.CounterOf(TestAddresses.C)); // untouched
        }

        [Test]
        public void Contributions_To_The_Same_Address_Conflict()
        {
            var counting = new CountingResolver(new AdditionResolver());
            var resolvers = new Dictionary<CounterAddress, IConflictResolver>
            {
                { TestAddresses.B, counting }
            };
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.B, 2),
                new AddToCounterTransition(TestAddresses.B, 3)
            };

            SimulationState result = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(5L, result.CounterOf(TestAddresses.B)); // +2 and +3 resolve to +5
            Assert.AreEqual(1, counting.Calls);                     // resolver invoked exactly once
        }

        [Test]
        public void Contributions_To_Different_Addresses_Do_Not_Conflict()
        {
            // Same kind of value, different addresses — no resolver needed, no collision.
            var transitions = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.B, 2),
                new AddToCounterTransition(TestAddresses.C, 3)
            };

            SimulationState result = NewRunner().Run(TestAddresses.InitialState(), transitions, 1);

            Assert.AreEqual(2L, result.CounterOf(TestAddresses.B));
            Assert.AreEqual(3L, result.CounterOf(TestAddresses.C));
        }

        [Test]
        public void State_Equality_Is_Independent_Of_Insertion_Order()
        {
            var forward = new SimulationState(Tick.Zero, new Dictionary<CounterAddress, long>
            {
                { TestAddresses.A, 5 },
                { TestAddresses.B, 8 },
                { TestAddresses.C, 13 }
            });
            var reversed = new SimulationState(Tick.Zero, new Dictionary<CounterAddress, long>
            {
                { TestAddresses.C, 13 },
                { TestAddresses.A, 5 },
                { TestAddresses.B, 8 }
            });

            Assert.AreEqual(forward, reversed);
            Assert.AreEqual(forward.GetHashCode(), reversed.GetHashCode());
        }

        [Test]
        public void Enumeration_Order_Remains_Irrelevant()
        {
            var resolvers = new Dictionary<CounterAddress, IConflictResolver>
            {
                { TestAddresses.A, new AdditionResolver() }
            };
            var forward = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.A, 2),
                new AddToCounterTransition(TestAddresses.A, 3),
                new AddToCounterTransition(TestAddresses.B, 1)
            };
            var reversed = new ITransition[]
            {
                new AddToCounterTransition(TestAddresses.B, 1),
                new AddToCounterTransition(TestAddresses.A, 3),
                new AddToCounterTransition(TestAddresses.A, 2)
            };

            SimulationState first = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), forward, 100);
            SimulationState second = new TickRunner(new TransitionRunner(resolvers))
                .Run(TestAddresses.InitialState(), reversed, 100);

            Assert.AreEqual(first, second);
        }

        /// <summary>Declares scope {A} but tries to read B — a deliberately misbehaving transition.</summary>
        private sealed class OutOfScopeTransition : ITransition
        {
            public ReadScope ReadScope => new ReadScope(TestAddresses.A);

            public RelationScope RelationScope => RelationScope.Empty;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                return new[] { new Contribution(TestAddresses.C, view.Read(TestAddresses.B)) };
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
