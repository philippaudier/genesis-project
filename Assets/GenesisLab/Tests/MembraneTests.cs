using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// L-001 — The First Crossing. Proves that an observable membrane exists between an external
    /// producer and a world (RFC-L001; ADR-0005; invariant 7): a declared external kind, an
    /// append-only trace, application at tick boundaries only, interpretation by laws alone through
    /// ADR-0001's one mechanism, exact replay, provenance-blindness, and a host whose hand cannot
    /// touch an observed run. No gameplay exists here: one cell, one law, one crossing.
    /// </summary>
    public sealed class MembraneTests
    {
        // Two natures of state at place A: E — the external kind (the membrane's declared entrance);
        // K — an ordinary counter the law writes. Producers write E through the trace; only laws
        // ever touch K.
        private static readonly Kind E = new Kind(9);
        private static readonly Cell ExternalCell = new Cell(TestAddresses.A, E);
        private static readonly Cell CounterCell = TestAddresses.CellA;

        private static SimulationState World(long external, long counter)
        {
            return new SimulationState(Tick.Zero, new Dictionary<Cell, long>
            {
                { ExternalCell, external },
                { CounterCell, counter }
            });
        }

        private static Membrane DeclaredMembrane() => new Membrane(new[] { E });

        private static TickRunner NewRunner()
        {
            var resolvers = new Dictionary<Kind, IConflictResolver>
            {
                { E, new AdditionResolver() },
                { TestAddresses.K, new AdditionResolver() }
            };
            return new TickRunner(new TransitionRunner(resolvers));
        }

        /// <summary>
        /// The one interpreting law of L-001: reads the external cell; if it holds anything, counts
        /// one interaction and consumes the value — both by contributions, nothing else. The meaning
        /// of the crossing lives here, not in the trace (the event/command guard).
        /// </summary>
        private sealed class InteractCountingTransition : ITransition
        {
            private readonly ReadScope _readScope = new ReadScope(ExternalCell);

            public ReadScope ReadScope => _readScope;
            public RelationScope RelationScope => RelationScope.Empty;

            public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
            {
                long pressed = view.Read(ExternalCell);
                if (pressed <= 0)
                {
                    return new Contribution[0];
                }

                return new[]
                {
                    new Contribution(CounterCell, 1),
                    new Contribution(ExternalCell, -pressed)
                };
            }
        }

        private static IReadOnlyList<ITransition> Laws() => new ITransition[] { new InteractCountingTransition() };

        // ------------------------------------------------------------------ DoD 1: declaration

        [Test]
        public void An_Undeclared_Kind_Cannot_Cross_The_Membrane()
        {
            var trace = new ExternalEventTrace(DeclaredMembrane());
            var undeclared = new ExternalEvent(Tick.Zero, CounterCell, 1); // K is not external

            Assert.Throws<UndeclaredExternalKindException>(() => trace.Append(undeclared));
            Assert.AreEqual(0, trace.Events.Count, "A refused event must never enter the record.");
        }

        // ------------------------------------------------------------------ DoD 2: append-only trace

        [Test]
        public void The_Trace_Only_Grows_And_Preserves_Order()
        {
            var trace = new ExternalEventTrace(DeclaredMembrane());
            trace.Append(new ExternalEvent(Tick.Zero, ExternalCell, 1));
            trace.Append(new ExternalEvent(new Tick(3), ExternalCell, 1));

            Assert.AreEqual(2, trace.Events.Count);
            Assert.AreEqual(Tick.Zero, trace.Events[0].Boundary);
            Assert.AreEqual(new Tick(3), trace.Events[1].Boundary);
            // No API exists to remove, reorder, or rewrite an entry — the enforcement is the surface
            // itself; this test documents the growth-only shape.
        }

        // ------------------------------------------------------------------ DoD 3: boundary timing

        [Test]
        public void An_Event_Applies_At_Its_Boundary_Never_Before_Never_After()
        {
            SimulationState state = World(external: 0, counter: 0);
            var relations = new RelationSet(state);
            var trace = new ExternalEventTrace(DeclaredMembrane());
            trace.Append(new ExternalEvent(new Tick(3), ExternalCell, 5));

            TickRunner runner = NewRunner();
            var noLaws = new ITransition[0];

            for (int tick = 1; tick <= 3; tick++)
            {
                state = runner.Run(state, relations, noLaws, trace, 1);
                Assert.AreEqual(0, state.ValueAt(ExternalCell), $"Nothing may appear before the boundary (t={tick}).");
            }

            state = runner.Run(state, relations, noLaws, trace, 1); // the tick that transforms t=3
            Assert.AreEqual(new Tick(4), state.CurrentTick);
            Assert.AreEqual(5, state.ValueAt(ExternalCell), "The crossing is first visible at Boundary + 1.");

            state = runner.Run(state, relations, noLaws, trace, 1);
            Assert.AreEqual(5, state.ValueAt(ExternalCell), "A boundary applies exactly once — never again after.");
        }

        // ------------------------------------------------------------------ DoD 4: a law interprets

        [Test]
        public void A_Law_Reads_The_Crossing_And_Acts_By_Contributions_Alone()
        {
            SimulationState state = World(external: 0, counter: 0);
            var relations = new RelationSet(state);
            var trace = new ExternalEventTrace(DeclaredMembrane());
            trace.Append(new ExternalEvent(Tick.Zero, ExternalCell, 1));

            TickRunner runner = NewRunner();

            state = runner.Run(state, relations, Laws(), trace, 1);
            Assert.AreEqual(1, state.ValueAt(ExternalCell), "t=1: the event landed; the law read the pre-event snapshot.");
            Assert.AreEqual(0, state.ValueAt(CounterCell), "t=1: no law may see a crossing mid-tick.");

            state = runner.Run(state, relations, Laws(), trace, 1);
            Assert.AreEqual(0, state.ValueAt(ExternalCell), "t=2: the law consumed the event — by a contribution.");
            Assert.AreEqual(1, state.ValueAt(CounterCell), "t=2: the law counted the interaction — by a contribution.");
        }

        // ------------------------------------------------------------------ DoD 5: exact replay

        [Test]
        public void Replay_Is_Exact_Same_State_Same_Trace_Same_Laws_Same_Result()
        {
            SimulationState initial = World(external: 0, counter: 0);
            var relations = new RelationSet(initial);
            var trace = new ExternalEventTrace(DeclaredMembrane());
            trace.Append(new ExternalEvent(Tick.Zero, ExternalCell, 1));
            trace.Append(new ExternalEvent(new Tick(2), ExternalCell, 1));
            trace.Append(new ExternalEvent(new Tick(7), ExternalCell, 1));

            TickRunner runner = NewRunner();

            SimulationState first = initial;
            SimulationState second = initial;
            for (int tick = 1; tick <= 10; tick++)
            {
                first = runner.Run(first, relations, Laws(), trace, 1);
                second = runner.Run(second, relations, Laws(), trace, 1);
                Assert.AreEqual(first, second, $"Replay must match tick-for-tick (t={tick}).");
            }

            Assert.AreEqual(3, first.ValueAt(CounterCell), "Every crossing was interpreted exactly once.");
        }

        // ------------------------------------------------------------------ DoD 6: provenance-blindness

        [Test]
        public void The_Player_And_The_Bot_Are_The_Same_Experiment()
        {
            // Two producers, two code paths, one content. The membrane records what crossed — never
            // who knocked. Laws cannot read the trace; the state carries no provenance.
            var playerTrace = new ExternalEventTrace(DeclaredMembrane());
            playerTrace.Append(new ExternalEvent(new Tick(1), ExternalCell, 1)); // "sampled input"
            playerTrace.Append(new ExternalEvent(new Tick(4), ExternalCell, 1));

            var botTrace = new ExternalEventTrace(DeclaredMembrane());
            foreach (long boundary in new long[] { 1, 4 })                       // "scripted schedule"
            {
                botTrace.Append(new ExternalEvent(new Tick(boundary), ExternalCell, 1));
            }

            SimulationState initial = World(external: 0, counter: 0);
            var relations = new RelationSet(initial);
            TickRunner runner = NewRunner();

            SimulationState playedWorld = runner.Run(initial, relations, Laws(), playerTrace, 8);
            SimulationState scriptedWorld = runner.Run(initial, relations, Laws(), botTrace, 8);

            Assert.AreEqual(playedWorld, scriptedWorld, "Identical crossings must produce an identical world. Not almost — identical.");
        }

        // ------------------------------------------------------------------ DoD 7: no hidden hand

        [Test]
        public void The_Host_Hand_Cannot_Touch_An_Observed_Run()
        {
            SimulationState initial = World(external: 0, counter: 0);
            var relations = new RelationSet(initial);
            var trace = new ExternalEventTrace(DeclaredMembrane());
            trace.Append(new ExternalEvent(Tick.Zero, ExternalCell, 1));

            TickRunner runner = NewRunner();
            SimulationState observed = runner.Run(initial, relations, Laws(), trace, 5);

            // The host "mutates" its reference — and produces only a detached copy. The initial
            // state is untouched; the observed run, re-run from it, is unchanged. There is no seam:
            // the loop exposes no intermediate state and accepts no callback (ADR-0005).
            SimulationState detached = initial.WithValue(CounterCell, 999);
            Assert.AreEqual(999, detached.ValueAt(CounterCell));
            Assert.AreEqual(0, initial.ValueAt(CounterCell), "Immutability: the world under observation was never touched.");

            SimulationState reRun = runner.Run(initial, relations, Laws(), trace, 5);
            Assert.AreEqual(observed, reRun, "The hidden hand failed by architecture, not by convention.");
        }

        // ------------------------------------------------------------------ DoD 8: the chronicle

        [Test]
        public void The_Laboratory_Can_Tell_The_Crossing()
        {
            // Observational (Genesis-011 category): the laboratory narrates one full crossing from
            // producer to verified replay, reading only public record — trace and states.
            SimulationState initial = World(external: 0, counter: 0);
            var relations = new RelationSet(initial);
            var trace = new ExternalEventTrace(DeclaredMembrane());
            trace.Append(new ExternalEvent(Tick.Zero, ExternalCell, 1));

            TickRunner runner = NewRunner();
            SimulationState afterLanding = runner.Run(initial, relations, Laws(), trace, 1);
            SimulationState afterInterpretation = runner.Run(afterLanding, relations, Laws(), trace, 1);
            SimulationState replayed = runner.Run(
                runner.Run(initial, relations, Laws(), trace, 1), relations, Laws(), trace, 1);

            var chronicle = new StringBuilder();
            chronicle.AppendLine($"external event        {trace.Events[0]}");
            chronicle.AppendLine($"trace append-only     {trace.Events.Count} crossing(s) on record");
            chronicle.AppendLine($"tick boundary         applied at {trace.Events[0].Boundary}, visible at {afterLanding.CurrentTick}");
            chronicle.AppendLine($"law                   counter {initial.ValueAt(CounterCell)} -> {afterInterpretation.ValueAt(CounterCell)}, event consumed");
            chronicle.AppendLine($"state delta           external {afterLanding.ValueAt(ExternalCell)} -> {afterInterpretation.ValueAt(ExternalCell)}");
            chronicle.AppendLine($"replay verified       {afterInterpretation.Equals(replayed)}");

            string story = chronicle.ToString();
            StringAssert.Contains("replay verified       True", story);
            StringAssert.Contains("counter 0 -> 1", story);
            Assert.AreEqual(1, afterInterpretation.ValueAt(CounterCell));
            Assert.AreEqual(afterInterpretation, replayed);

            // The story a developer can now SEE, not merely know:
            //   external event -> append-only trace -> tick boundary -> law -> state delta -> replay verified
            TestContext.WriteLine(story);
        }
    }
}
