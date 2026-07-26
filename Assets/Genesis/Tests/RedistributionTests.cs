using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The first-phenomenon proof (Genesis-011, kept as a regression guard, re-keyed under RFC-0003):
    /// a quantity redistributes across symmetrically related places — deterministically, locally,
    /// emergently, without being created or destroyed — and the phenomenon vanishes when the
    /// relations are removed. Conservation is named only here, in the tests that observe it.
    /// </summary>
    public sealed class RedistributionTests
    {
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
                new RedistributionTransition(TestAddresses.A, TestAddresses.K),
                new RedistributionTransition(TestAddresses.B, TestAddresses.K),
                new RedistributionTransition(TestAddresses.C, TestAddresses.K)
            };
        }

        private static TickRunner NewRunner()
        {
            var resolvers = new Dictionary<Kind, IConflictResolver>
            {
                { TestAddresses.K, new AdditionResolver() }
            };
            return new TickRunner(new TransitionRunner(resolvers));
        }

        [Test]
        public void Redistribution_Occurs_Across_Symmetric_Relations()
        {
            SimulationState initial = InitialState();

            SimulationState result = NewRunner().Run(initial, SymmetricChain(initial), RedistributionTransitions(), 10);

            Assert.Less(result.ValueAt(TestAddresses.CellA), InitialQuantity);
            Assert.Greater(result.ValueAt(TestAddresses.CellB), 0L);
            Assert.Greater(result.ValueAt(TestAddresses.CellC), 0L);
        }

        [Test]
        public void Ablation_Removing_The_Relations_Destroys_The_Phenomenon()
        {
            SimulationState initial = InitialState();
            var noRelations = new RelationSet(initial);

            SimulationState result = NewRunner().Run(initial, noRelations, RedistributionTransitions(), 10);

            // 12, 0, 0 — frozen forever. Not because the algorithm changed: because the world's
            // structure was removed (DN-002).
            Assert.AreEqual(InitialQuantity, result.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(0L, result.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(0L, result.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Total_Quantity_Is_Conserved_At_Every_Tick()
        {
            SimulationState state = InitialState();
            RelationSet relations = SymmetricChain(state);
            TickRunner runner = NewRunner();

            for (int tick = 0; tick < 50; tick++)
            {
                state = runner.Run(state, relations, RedistributionTransitions(), 1);

                long total = state.ValueAt(TestAddresses.CellA)
                           + state.ValueAt(TestAddresses.CellB)
                           + state.ValueAt(TestAddresses.CellC);
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
        public void Redistribution_Settles_At_A_Deterministic_Steady_State()
        {
            // From (12, 0, 0) the system reaches (5, 4, 3) and stays there — a quantised steady
            // state, an absorbing state of this discrete dynamic. Named only here.
            SimulationState initial = InitialState();
            RelationSet relations = SymmetricChain(initial);
            TickRunner runner = NewRunner();

            SimulationState settled = runner.Run(initial, relations, RedistributionTransitions(), 100);
            SimulationState later = runner.Run(initial, relations, RedistributionTransitions(), 101);

            Assert.AreEqual(5L, settled.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(4L, settled.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(3L, settled.ValueAt(TestAddresses.CellC));
            Assert.AreEqual(settled.ValueAt(TestAddresses.CellA), later.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(settled.ValueAt(TestAddresses.CellB), later.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(settled.ValueAt(TestAddresses.CellC), later.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Convergent_Flows_Meet_In_Conflict_And_Equalise()
        {
            // Two sources of flow, one shared neighbour: from (12, 0, 12) the system reaches perfect
            // equalisation (8, 8, 8), with B's conflicts resolved additively along the way.
            SimulationState initial = TestAddresses.StateWith(12, 0, 12);
            RelationSet relations = SymmetricChain(initial);
            TickRunner runner = NewRunner();

            SimulationState settled = runner.Run(initial, relations, RedistributionTransitions(), 100);

            Assert.AreEqual(8L, settled.ValueAt(TestAddresses.CellA));
            Assert.AreEqual(8L, settled.ValueAt(TestAddresses.CellB));
            Assert.AreEqual(8L, settled.ValueAt(TestAddresses.CellC));
        }

        [Test]
        public void Initial_State_Is_Never_Mutated()
        {
            SimulationState initial = InitialState();

            NewRunner().Run(initial, SymmetricChain(initial), RedistributionTransitions(), 1000);

            Assert.AreEqual(InitialState(), initial);
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
                    state.ValueAt(TestAddresses.CellA),
                    state.ValueAt(TestAddresses.CellB),
                    state.ValueAt(TestAddresses.CellC)
                };
            }

            return history;
        }

        /// <summary>
        /// The redistribution fixture — one per place, over one kind. Reads its own cell, observes
        /// its own outgoing relations, and for each neighbour holding strictly less, contributes the
        /// paired flux: −q to itself, +q to the neighbour, q = (self − target) / 2 in integers.
        /// </summary>
        internal sealed class RedistributionTransition : ITransition
        {
            private readonly Cell _selfCell;
            private readonly Kind _kind;
            private readonly ReadScope _readScope;
            private readonly RelationScope _relationScope;

            public RedistributionTransition(Place self, Kind kind)
            {
                Self = self;
                _kind = kind;
                _selfCell = new Cell(self, kind);
                _readScope = new ReadScope(_selfCell);
                _relationScope = new RelationScope(new[] { self }, new[] { kind });
            }

            public Place Self { get; }

            public ReadScope ReadScope => _readScope;

            public RelationScope RelationScope => _relationScope;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                long self = view.Read(_selfCell);
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(Self);

                var contributions = new List<Contribution>();
                for (int i = 0; i < outgoing.Count; i++)
                {
                    var targetCell = new Cell(outgoing[i].Target, _kind);
                    long neighbour = view.Read(targetCell);

                    if (self > neighbour)
                    {
                        long flux = (self - neighbour) / 2;
                        if (flux > 0)
                        {
                            contributions.Add(new Contribution(_selfCell, -flux));
                            contributions.Add(new Contribution(targetCell, flux));
                        }
                    }
                }

                return contributions;
            }
        }
    }
}
