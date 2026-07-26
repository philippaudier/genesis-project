using System.Collections.Generic;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Genesis-012 — the proof. The world can contain a cause of its own change: a second nature of
    /// state (a production rate) lives at the same places as the transported quantity, is read by a
    /// universal law that is inert wherever the rate is zero, and grows the total by exactly the
    /// declared production — accounting where conservation used to be. The cause is standing: it
    /// affects without ever being affected. Remove the rate (identical laws) and conservation
    /// returns; remove the relations and production continues but stays local.
    ///
    /// Genesis-012 demonstrates the kernel's new representation (RFC-0003); the standing-cause
    /// fixtures live here, in tests. No kernel type knows the pattern. Its name — the candidate word
    /// "Source" — is used nowhere in simulation code; per DN-003, the name is earned by the theorem
    /// these tests observe, and its Glossary admission is a separate editorial act.
    /// </summary>
    public sealed class StandingCauseTests
    {
        // Two natures of state at the same places: Q — the transported quantity; R — the production
        // rate. Same integer type, different causal roles (RFC-0003: kind is a role, not a type).
        private static readonly Kind Q = TestAddresses.K;
        private static readonly Kind R = new Kind(2);

        private static Cell QCell(Place place) => new Cell(place, Q);
        private static Cell RCell(Place place) => new Cell(place, R);

        private static readonly Place[] Places = { TestAddresses.A, TestAddresses.B, TestAddresses.C };

        private static SimulationState World(long qa, long qb, long qc, long ra, long rb, long rc)
        {
            return new SimulationState(Tick.Zero, new Dictionary<Cell, long>
            {
                { QCell(TestAddresses.A), qa }, { QCell(TestAddresses.B), qb }, { QCell(TestAddresses.C), qc },
                { RCell(TestAddresses.A), ra }, { RCell(TestAddresses.B), rb }, { RCell(TestAddresses.C), rc }
            });
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

        // The full law-set, identical in every scenario: the production law and the redistribution
        // law, instantiated at EVERY place. The laws are universal; the state localises them —
        // production is inert wherever the rate is zero.
        private static IReadOnlyList<ITransition> Laws()
        {
            var laws = new List<ITransition>();
            foreach (Place place in Places)
            {
                laws.Add(new ProductionTransition(place, Q, R));
                laws.Add(new RedistributionTests.RedistributionTransition(place, Q));
            }

            return laws;
        }

        private static TickRunner NewRunner()
        {
            // Q resolves additively (flows and production meet); R needs no resolver — nothing ever
            // writes it. The cause is structurally unaffected.
            var resolvers = new Dictionary<Kind, IConflictResolver>
            {
                { Q, new AdditionResolver() }
            };
            return new TickRunner(new TransitionRunner(resolvers));
        }

        private static long TotalQ(SimulationState state)
        {
            return state.ValueAt(QCell(TestAddresses.A))
                 + state.ValueAt(QCell(TestAddresses.B))
                 + state.ValueAt(QCell(TestAddresses.C));
        }

        [Test]
        public void Total_Grows_By_Exactly_The_Declared_Production()
        {
            // The accounting theorem: ΔTotal = ΣProduction. Conservation's domain is revealed, not
            // refuted — every new unit is causally chargeable to declared state.
            SimulationState state = World(0, 0, 0, 2, 0, 0);
            RelationSet relations = SymmetricChain(state);
            TickRunner runner = NewRunner();

            for (int tick = 1; tick <= 20; tick++)
            {
                state = runner.Run(state, relations, Laws(), 1);
                Assert.AreEqual(2L * tick, TotalQ(state), $"Accounting failed at tick {tick}.");
            }
        }

        [Test]
        public void The_Standing_Cause_Is_Never_Affected()
        {
            // Causal asymmetry: the rate affects the world every tick and is never consumed,
            // transported, or changed by what it drives.
            SimulationState initial = World(0, 0, 0, 2, 0, 0);
            RelationSet relations = SymmetricChain(initial);

            SimulationState result = NewRunner().Run(initial, relations, Laws(), 100);

            Assert.AreEqual(2L, result.ValueAt(RCell(TestAddresses.A)));
            Assert.AreEqual(0L, result.ValueAt(RCell(TestAddresses.B)));
            Assert.AreEqual(0L, result.ValueAt(RCell(TestAddresses.C)));
        }

        [Test]
        public void Produced_Quantity_Enters_The_Existing_Phenomenon()
        {
            // The cause does not replace Genesis-011; it feeds it. What the rate produces at A is
            // transported by the redistribution the kernel already proved — two hops from its origin.
            SimulationState initial = World(0, 0, 0, 2, 0, 0);
            RelationSet relations = SymmetricChain(initial);

            SimulationState result = NewRunner().Run(initial, relations, Laws(), 10);

            Assert.Greater(result.ValueAt(QCell(TestAddresses.B)), 0L);
            Assert.Greater(result.ValueAt(QCell(TestAddresses.C)), 0L);
        }

        [Test]
        public void Ablation_Zero_Rate_Under_Identical_Laws_Restores_Conservation()
        {
            // The double ablation, first blade. Same laws — every transition identical, production
            // instantiated everywhere — only the state differs: all rates zero. The growth vanishes
            // and the Genesis-011 world re-emerges exactly: (12,0,0) settles at (5,4,3), total 12
            // forever. The cause was in the world, not in the law.
            SimulationState initial = World(12, 0, 0, 0, 0, 0);
            RelationSet relations = SymmetricChain(initial);
            TickRunner runner = NewRunner();

            SimulationState result = runner.Run(initial, relations, Laws(), 100);

            Assert.AreEqual(12L, TotalQ(result));
            Assert.AreEqual(5L, result.ValueAt(QCell(TestAddresses.A)));
            Assert.AreEqual(4L, result.ValueAt(QCell(TestAddresses.B)));
            Assert.AreEqual(3L, result.ValueAt(QCell(TestAddresses.C)));
        }

        [Test]
        public void Ablation_No_Relations_Production_Continues_But_Stays_Local()
        {
            // The double ablation, second blade. Same laws, same productive state, no relations:
            // origin, transport, and mechanism are three separable roles.
            SimulationState initial = World(0, 0, 0, 2, 0, 0);
            var noRelations = new RelationSet(initial);

            SimulationState result = NewRunner().Run(initial, noRelations, Laws(), 10);

            Assert.AreEqual(20L, result.ValueAt(QCell(TestAddresses.A))); // 10 ticks × rate 2
            Assert.AreEqual(0L, result.ValueAt(QCell(TestAddresses.B)));
            Assert.AreEqual(0L, result.ValueAt(QCell(TestAddresses.C)));
        }

        [Test]
        public void The_Pattern_Is_Recognisable_From_State_Alone()
        {
            // DN-003 recognition-not-creation: the pattern is a deterministic predicate over declared
            // state. No object was created; no kernel type exists for it; the predicate simply became
            // true at one place and false at the others.
            SimulationState state = World(0, 0, 0, 2, 0, 0);

            bool IsStandingCause(Place place) => state.ValueAt(RCell(place)) > 0;

            Assert.IsTrue(IsStandingCause(TestAddresses.A));
            Assert.IsFalse(IsStandingCause(TestAddresses.B));
            Assert.IsFalse(IsStandingCause(TestAddresses.C));
        }

        [Test]
        public void Two_Runs_Over_500_Ticks_End_Strictly_Identical()
        {
            SimulationState initial = World(0, 0, 0, 2, 0, 0);
            RelationSet relations = SymmetricChain(initial);
            TickRunner runner = NewRunner();

            SimulationState first = runner.Run(initial, relations, Laws(), 500);
            SimulationState second = runner.Run(initial, relations, Laws(), 500);

            Assert.AreEqual(first, second);
        }

        /// <summary>
        /// The production law — universal, instantiated at every place, inert wherever the declared
        /// rate is zero. It reads its own rate cell and contributes that amount to its own quantity
        /// cell. It knows nothing of totals, growth, or the pattern's name: the law says "a declared
        /// production capacity produces"; the world says where, and how much.
        /// </summary>
        private sealed class ProductionTransition : ITransition
        {
            private readonly Cell _quantityCell;
            private readonly Cell _rateCell;
            private readonly ReadScope _readScope;

            public ProductionTransition(Place self, Kind quantity, Kind rate)
            {
                _quantityCell = new Cell(self, quantity);
                _rateCell = new Cell(self, rate);
                _readScope = new ReadScope(_rateCell);
            }

            public ReadScope ReadScope => _readScope;

            public RelationScope RelationScope => RelationScope.Empty;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                long rate = view.Read(_rateCell);
                if (rate <= 0)
                {
                    return new Contribution[0];
                }

                return new[] { new Contribution(_quantityCell, rate) };
            }
        }
    }
}
