using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Genesis-011 — the first phenomenon. Redistribution: a quantity spreads across symmetrically
    /// related addresses — deterministically, locally, and emergently, without being created or
    /// destroyed — and the phenomenon vanishes when the relations are removed.
    ///
    /// Genesis-011 demonstrates the kernel; it does not extend it. The entire implementation lives in
    /// this test assembly: the transition below is an experimental fixture, not a kernel primitive,
    /// and the kernel's own folder is untouched by this milestone.
    ///
    /// Conservation — the kernel's first theorem — is stated nowhere in simulation code. It is only
    /// observed here, in the tests, because paired contributions and additive resolution imply it.
    /// </summary>
    public sealed class RedistributionTests
    {
        // The witness world: A holds everything; B and C hold nothing. A ↔ B ↔ C, expressed purely
        // as ordinary directed relations — the kernel has no notion of "undirected".
        private const long InitialQuantity = 12;

        private static SimulationState InitialState()
        {
            return TestAddresses.StateWith(InitialQuantity, 0, 0);
        }

        private static RelationSet SymmetricChain(SimulationState state)
        {
            return new RelationSet(
                state,
                new Relation(TestAddresses.A, TestAddresses.B),
                new Relation(TestAddresses.B, TestAddresses.A),
                new Relation(TestAddresses.B, TestAddresses.C),
                new Relation(TestAddresses.C, TestAddresses.B));
        }

        private static IReadOnlyList<ITransition> RedistributionTransitions()
        {
            return new ITransition[]
            {
                new RedistributionTransition(TestAddresses.A),
                new RedistributionTransition(TestAddresses.B),
                new RedistributionTransition(TestAddresses.C)
            };
        }

        private static TickRunner NewRunner()
        {
            // Every address needs additive resolution: each tick, an address may receive quantity
            // from a neighbour while giving quantity away itself. Genesis-006's machinery becomes
            // load-bearing for the first time.
            var resolvers = new Dictionary<CounterAddress, IConflictResolver>
            {
                { TestAddresses.A, new AdditionResolver() },
                { TestAddresses.B, new AdditionResolver() },
                { TestAddresses.C, new AdditionResolver() }
            };
            return new TickRunner(new TransitionRunner(resolvers));
        }

        [Test]
        public void Redistribution_Occurs_Across_Symmetric_Relations()
        {
            SimulationState initial = InitialState();

            SimulationState result = NewRunner().Run(initial, SymmetricChain(initial), RedistributionTransitions(), 10);

            // The quantity spread: the source lost, and the far end — two hops away, never directly
            // related to A — received. No transition knows the chain; each knows one neighbourhood.
            Assert.Less(result.CounterOf(TestAddresses.A), InitialQuantity);
            Assert.Greater(result.CounterOf(TestAddresses.B), 0L);
            Assert.Greater(result.CounterOf(TestAddresses.C), 0L);
        }

        [Test]
        public void Ablation_Removing_The_Relations_Destroys_The_Phenomenon()
        {
            // Identical transitions. Identical initial state. Empty RelationSet.
            SimulationState initial = InitialState();
            var noRelations = new RelationSet(initial);

            SimulationState result = NewRunner().Run(initial, noRelations, RedistributionTransitions(), 10);

            // 12, 0, 0 — frozen forever. Not because the algorithm changed: because the world's
            // structure was removed. This is what makes redistribution a phenomenon (DN-002).
            Assert.AreEqual(InitialQuantity, result.CounterOf(TestAddresses.A));
            Assert.AreEqual(0L, result.CounterOf(TestAddresses.B));
            Assert.AreEqual(0L, result.CounterOf(TestAddresses.C));
        }

        [Test]
        public void Total_Quantity_Is_Conserved_At_Every_Tick()
        {
            // The kernel's first theorem, observed tick by tick. No type states it; no API enforces
            // it; it holds because paired −q/+q contributions meet additive resolution.
            SimulationState state = InitialState();
            RelationSet relations = SymmetricChain(state);
            TickRunner runner = NewRunner();

            for (int tick = 0; tick < 50; tick++)
            {
                state = runner.Run(state, relations, RedistributionTransitions(), 1);

                long total = state.CounterOf(TestAddresses.A)
                           + state.CounterOf(TestAddresses.B)
                           + state.CounterOf(TestAddresses.C);
                Assert.AreEqual(InitialQuantity, total, $"Quantity was created or destroyed at tick {tick + 1}.");
            }
        }

        [Test]
        public void Repeated_Executions_Produce_Identical_Histories()
        {
            long[][] first = RecordHistory(50);
            long[][] second = RecordHistory(50);

            for (int tick = 0; tick < first.Length; tick++)
            {
                CollectionAssert.AreEqual(first[tick], second[tick], $"Histories diverged at tick {tick + 1}.");
            }
        }

        [Test]
        public void Transitions_Declare_Only_Local_Scopes()
        {
            // Locality by declaration (DN-002): each transition observes itself and its own outgoing
            // relations — nothing global. The kernel already enforces that nothing beyond the
            // declaration is observable; this asserts the declarations themselves are local.
            foreach (ITransition transition in RedistributionTransitions())
            {
                var redistribution = (RedistributionTransition)transition;

                Assert.AreEqual(1, redistribution.ReadScope.Addresses.Count);
                Assert.IsTrue(redistribution.ReadScope.Includes(redistribution.Self));
                Assert.AreEqual(1, redistribution.RelationScope.Origins.Count);
                Assert.IsTrue(redistribution.RelationScope.Includes(redistribution.Self));
            }
        }

        [Test]
        public void Redistribution_Settles_At_A_Deterministic_Steady_State()
        {
            // Emergent and exactly reproducible: from (12, 0, 0) the system reaches (5, 4, 3) and
            // stays there — a quantised steady state, an absorbing state of this discrete dynamic
            // (not an equilibrium in the physical sense: the values are not equal). No transition
            // describes it; "steady state" is named only here, in the test that observes it.
            SimulationState initial = InitialState();
            RelationSet relations = SymmetricChain(initial);
            TickRunner runner = NewRunner();

            SimulationState settled = runner.Run(initial, relations, RedistributionTransitions(), 100);
            SimulationState later = runner.Run(initial, relations, RedistributionTransitions(), 101);

            Assert.AreEqual(5L, settled.CounterOf(TestAddresses.A));
            Assert.AreEqual(4L, settled.CounterOf(TestAddresses.B));
            Assert.AreEqual(3L, settled.CounterOf(TestAddresses.C));
            Assert.AreEqual(settled.CounterOf(TestAddresses.A), later.CounterOf(TestAddresses.A));
            Assert.AreEqual(settled.CounterOf(TestAddresses.B), later.CounterOf(TestAddresses.B));
            Assert.AreEqual(settled.CounterOf(TestAddresses.C), later.CounterOf(TestAddresses.C));
        }

        [Test]
        public void Convergent_Flows_Meet_In_Conflict_And_Equalise()
        {
            // Two sources, one shared neighbour: A and C both push into B in the same tick, so B
            // receives multiple contributions — the first time conflict resolution is load-bearing
            // rather than witnessed. From (12, 0, 12) the system reaches perfect equalisation
            // (8, 8, 8) and holds it, conserving the total (24) throughout.
            SimulationState initial = TestAddresses.StateWith(12, 0, 12);
            RelationSet relations = SymmetricChain(initial);
            TickRunner runner = NewRunner();

            SimulationState settled = runner.Run(initial, relations, RedistributionTransitions(), 100);

            Assert.AreEqual(8L, settled.CounterOf(TestAddresses.A));
            Assert.AreEqual(8L, settled.CounterOf(TestAddresses.B));
            Assert.AreEqual(8L, settled.CounterOf(TestAddresses.C));
        }

        private static long[][] RecordHistory(int ticks)
        {
            SimulationState state = InitialState();
            RelationSet relations = SymmetricChain(state);
            TickRunner runner = NewRunner();
            var history = new long[ticks][];

            for (int i = 0; i < ticks; i++)
            {
                state = runner.Run(state, relations, RedistributionTransitions(), 1);
                history[i] = new[]
                {
                    state.CounterOf(TestAddresses.A),
                    state.CounterOf(TestAddresses.B),
                    state.CounterOf(TestAddresses.C)
                };
            }

            return history;
        }

        /// <summary>
        /// The redistribution fixture — one per address. It reads its own value, observes its own
        /// outgoing relations, and for each neighbour holding strictly less, contributes the paired
        /// flux: −q to itself, +q to the neighbour, with q = (self − target) / 2 in integers.
        /// Pushing only downhill avoids truncation bias (no negative flux exists); with at most two
        /// neighbours (the witness chain), the total outflow never exceeds the value held. It states
        /// nothing about totals, chains, or steady states — it knows one neighbourhood, nothing more.
        /// </summary>
        private sealed class RedistributionTransition : ITransition
        {
            private readonly ReadScope _readScope;
            private readonly RelationScope _relationScope;

            public RedistributionTransition(CounterAddress self)
            {
                Self = self;
                _readScope = new ReadScope(self);
                _relationScope = new RelationScope(self);
            }

            public CounterAddress Self { get; }

            public ReadScope ReadScope => _readScope;

            public RelationScope RelationScope => _relationScope;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                long self = view.Read(Self);
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(Self);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count; i++)
                {
                    CounterAddress target = outgoing[i].Target;
                    long neighbour = view.Read(target);

                    if (self > neighbour)
                    {
                        long flux = (self - neighbour) / 2;
                        if (flux > 0)
                        {
                            contributions.Add(new Contribution(Self, -flux));
                            contributions.Add(new Contribution(target, flux));
                        }
                    }
                }

                return contributions;
            }
        }
    }
}
