using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;

namespace Genesis.Lab.S1_005
{
    /// <summary>
    /// Instrument obligations sealed for S1-005. Every dynamic check uses a foreign toy.
    /// N0 and N1 are constructed only by SealedPairBuilds and are never handed to a runner.
    /// </summary>
    public static class Calibration
    {
        private static readonly List<string> Failures = new List<string>();

        public static bool RunAll()
        {
            Failures.Clear();
            Check("1. equal surfaces with unequal hidden material remain distinct",
                EqualSurfaceDoesNotMeanEqualState);
            Check("2. a repeated surface with changing Water is not a fixed point",
                FalseSurfaceRestIsRejected);
            Check("3. a complete-state fixed point with zero contributions is accepted",
                TrueFixedPointIsAccepted);
            Check("4. a planted period-2 complete-state orbit is measured as period 2",
                PeriodTwoIsMeasured);
            Check("5. a dropped conversion half-pair breaks the matter audit",
                HalfPairIsCaught);
            Check("6. a planted negative value is found",
                NegativeValueIsCaught);
            Check("7. a branching toy distinguishes Constant2 from degree+1",
                BranchingToyDistinguishesDivisors);
            Check("8. conversion and transport share the snapshot holding",
                SnapshotHoldingIsLoadBearing);
            Check("9. threshold zero conforms, and the comparator catches a mismatch",
                ThresholdZeroConforms);
            Check("10. the A-G classifier obeys every outcome and sealed precedence",
                ClassifierObeysTheSeal);
            Check("sealed N0/N1 build exactly as approved (no tick run)",
                SealedPairBuilds);

            Console.WriteLine();
            if (Failures.Count == 0)
            {
                Console.WriteLine("CALIBRATION: all obligations discharged.");
                Console.WriteLine("N0 and N1 were constructed and inspected; neither was ticked.");
                return true;
            }

            Console.WriteLine($"CALIBRATION: {Failures.Count} failure(s):");
            foreach (string failure in Failures)
            {
                Console.WriteLine($"  FAIL {failure}");
            }

            return false;
        }

        private static void Check(string name, Action check)
        {
            try
            {
                check();
                Console.WriteLine($"  ok   {name}");
            }
            catch (Exception exception)
            {
                Failures.Add($"{name}: {exception.Message}");
                Console.WriteLine($"  FAIL {name}: {exception.Message}");
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static SimulationState State(Tick tick, IReadOnlyList<Place> places,
            long[] rock, long[] sediment, long[] water)
        {
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i < places.Count; i++)
            {
                cells[new Cell(places[i], K4.Base)] = 0;
                cells[new Cell(places[i], K4.Rock)] = rock[i];
                cells[new Cell(places[i], K4.Sediment)] = sediment[i];
                cells[new Cell(places[i], K4.Water)] = water[i];
            }

            return new SimulationState(tick, cells);
        }

        private static void EqualSurfaceDoesNotMeanEqualState()
        {
            var places = new List<Place> { new Place(20) };
            SimulationState rock = State(Tick.Zero, places,
                new long[] { 5 }, new long[] { 0 }, new long[] { 2 });
            SimulationState sediment = State(new Tick(1), places,
                new long[] { 4 }, new long[] { 1 }, new long[] { 2 });

            Assert(StateInstrument.SurfaceSignature(rock, places) ==
                StateInstrument.SurfaceSignature(sediment, places),
                "the planted surfaces must agree");
            Assert(StateInstrument.CompleteSignature(rock, places) !=
                StateInstrument.CompleteSignature(sediment, places),
                "the complete-state reader collapsed Rock and Sediment");
        }

        private static void FalseSurfaceRestIsRejected()
        {
            var places = new List<Place> { new Place(21) };
            SimulationState before = State(Tick.Zero, places,
                new long[] { 5 }, new long[] { 0 }, new long[] { 2 });
            SimulationState after = State(new Tick(1), places,
                new long[] { 5 }, new long[] { 0 }, new long[] { 3 });

            Assert(StateInstrument.SurfaceSignature(before, places) ==
                StateInstrument.SurfaceSignature(after, places),
                "the planted surface must repeat");
            Assert(StateInstrument.CompleteSignature(before, places) !=
                StateInstrument.CompleteSignature(after, places),
                "changing Water was mistaken for complete-state rest");
        }

        private static void TrueFixedPointIsAccepted()
        {
            var places = new List<Place> { new Place(22), new Place(23) };
            SimulationState initial = State(Tick.Zero, places,
                new long[] { 5, 5 }, new long[] { 0, 0 }, new long[] { 0, 0 });
            var relations = new RelationSet(initial,
                new Relation(places[0], places[1]), new Relation(places[1], places[0]));
            var additive = new AdditiveResolver();
            var fixtures = new List<IFixture>
            {
                new SurfaceFlowFixture(places, Worlds5.Constant2, additive),
                new SurfaceConversionFixture(places, Worlds5.Constant2,
                    Worlds5.ConversionThreshold, additive, additive),
                new CompetenceTransportFixture(places, Worlds5.Constant2, 1, additive),
            };

            IReadOnlyList<Provenance.Entry> entries =
                Provenance.Collect(initial, relations, fixtures);
            Assert(entries.Count == 0, "the planted fixed point emitted a contribution");

            var trace = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            var set = new FixtureSet(fixtures.ToArray());
            var runner = new TickRunner(new TransitionRunner(set.Resolvers));
            SimulationState after = runner.Run(initial, relations, set.Transitions, trace, 1);
            Assert(StateInstrument.CompleteSignature(initial, places) ==
                StateInstrument.CompleteSignature(after, places),
                "a zero-contribution tick changed the complete state");
        }

        private static void PeriodTwoIsMeasured()
        {
            var places = new List<Place> { new Place(24), new Place(25) };
            SimulationState a0 = State(Tick.Zero, places,
                new long[] { 4, 6 }, new long[] { 1, 0 }, new long[] { 3, 2 });
            SimulationState b = State(new Tick(1), places,
                new long[] { 4, 6 }, new long[] { 0, 1 }, new long[] { 2, 3 });
            SimulationState a2 = State(new Tick(2), places,
                new long[] { 4, 6 }, new long[] { 1, 0 }, new long[] { 3, 2 });

            StateInstrument.Repeat repeat =
                StateInstrument.FirstRepeat(new[] { a0, b, a2 }, places);
            Assert(repeat != null && repeat.First == 0 && repeat.Again == 2 &&
                repeat.Period == 2, "the planted orbit was not measured as period 2");
        }

        private static void HalfPairIsCaught()
        {
            var places = new List<Place> { new Place(26) };
            SimulationState before = State(Tick.Zero, places,
                new long[] { 5 }, new long[] { 0 }, new long[] { 0 });
            SimulationState broken = State(new Tick(1), places,
                new long[] { 4 }, new long[] { 0 }, new long[] { 0 });

            Assert(Surface.MatterAudit(new[] { before, broken }, places).Count == 1,
                "the dropped +Sediment half-pair passed the matter audit");
        }

        private static void NegativeValueIsCaught()
        {
            var places = new List<Place> { new Place(27) };
            SimulationState negative = State(Tick.Zero, places,
                new long[] { 5 }, new long[] { 0 }, new long[] { -1 });
            Assert(StateInstrument.Minimum(negative, places) == -1,
                "the planted negative Water value was not found");
        }

        private static void BranchingToyDistinguishesDivisors()
        {
            Assert(Worlds5.Constant2(2) == 2, "Constant2 did not return 2");
            Assert(Divisors.DegreeAware(2) == 3,
                "the corpus degree-aware policy did not return degree+1");

            var places = new List<Place>
                { new Place(30), new Place(31), new Place(32) };
            SimulationState state = State(Tick.Zero, places,
                new long[] { 5, 5, 5 }, new long[] { 0, 0, 0 },
                new long[] { 12, 0, 0 });
            var relations = new RelationSet(state,
                new Relation(places[0], places[1]),
                new Relation(places[0], places[2]));

            var constant = new SurfaceFlowFixture(places, Worlds5.Constant2,
                new AdditiveResolver());
            var degree = new SurfaceFlowFixture(places, Divisors.DegreeAware,
                new AdditiveResolver());

            string constantRecord = Contributions(state, relations, constant);
            string degreeRecord = Contributions(state, relations, degree);
            Assert(constantRecord != degreeRecord,
                "the branching toy could not distinguish the divisor policies");
            Assert(constantRecord.Contains("-6") && degreeRecord.Contains("-4"),
                "the branching toy did not expose the expected 6-versus-4 transfer");
        }

        private static void SnapshotHoldingIsLoadBearing()
        {
            var places = new List<Place> { new Place(40), new Place(41) };
            SimulationState state = State(Tick.Zero, places,
                new long[] { 5, 5 }, new long[] { 1, 0 }, new long[] { 20, 0 });
            var relations = new RelationSet(state,
                new Relation(places[0], places[1]), new Relation(places[1], places[0]));

            var additive = new AdditiveResolver();
            var spyRock = new SpyResolver(K4.Rock, additive);
            var spySediment = new SpyResolver(K4.Sediment, additive);
            var fixtures = new List<IFixture>
            {
                new SurfaceConversionFixture(places, Worlds5.Constant2, 4,
                    spyRock, spySediment),
                new CompetenceTransportFixture(places, Worlds5.Constant2, 0,
                    spySediment),
            };

            IReadOnlyList<Provenance.Entry> entries =
                Provenance.Collect(state, relations, fixtures);
            int plusOne = 0;
            int minusOne = 0;
            foreach (Provenance.Entry entry in entries)
            {
                if (entry.Target == new Cell(places[0], K4.Sediment) && entry.Amount == 1)
                    plusOne++;
                if (entry.Target == new Cell(places[0], K4.Sediment) && entry.Amount == -1)
                    minusOne++;
            }

            Assert(plusOne == 1 && minusOne == 1,
                "the collision did not carry the snapshot's (+1,-1) witness");

            var set = new FixtureSet(fixtures.ToArray());
            var trace = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            var runner = new TickRunner(new TransitionRunner(set.Resolvers));
            SimulationState after = runner.Run(state, relations, set.Transitions, trace, 1);

            Assert(after.ValueAt(new Cell(places[0], K4.Sediment)) == 1,
                "transport consumed the within-tick conversion emission");
            Assert(after.ValueAt(new Cell(places[1], K4.Sediment)) == 1,
                "transport did not move exactly the one snapshot unit");
            Assert(spySediment.Invocations.Count == 1 &&
                spySediment.Invocations[0].Committed == 0,
                "the contested Sediment cell was not resolved once to zero");
        }

        private static void ThresholdZeroConforms()
        {
            var cases = new List<Tuple<SimulationState, RelationSet, IReadOnlyList<Place>>>();

            var two = new List<Place> { new Place(50), new Place(51) };
            SimulationState moving = State(Tick.Zero, two,
                new long[] { 5, 5 }, new long[] { 3, 0 }, new long[] { 8, 0 });
            cases.Add(Tuple.Create(moving,
                new RelationSet(moving, new Relation(two[0], two[1]),
                    new Relation(two[1], two[0])),
                (IReadOnlyList<Place>)two));

            var three = new List<Place>
                { new Place(52), new Place(53), new Place(54) };
            SimulationState branching = State(Tick.Zero, three,
                new long[] { 5, 5, 5 }, new long[] { 3, 0, 0 },
                new long[] { 10, 0, 2 });
            cases.Add(Tuple.Create(branching,
                new RelationSet(branching, new Relation(three[0], three[1]),
                    new Relation(three[0], three[2])),
                (IReadOnlyList<Place>)three));

            foreach (Tuple<SimulationState, RelationSet, IReadOnlyList<Place>> item in cases)
            {
                var existing = new SurfaceSedimentTransportFixture(item.Item3,
                    Worlds5.Constant2, new AdditiveResolver());
                var candidate = new CompetenceTransportFixture(item.Item3,
                    Worlds5.Constant2, 0, new AdditiveResolver());
                Assert(Contributions(item.Item1, item.Item2, existing) ==
                    Contributions(item.Item1, item.Item2, candidate),
                    "competence zero diverged from the existing transport fixture");
            }

            var unitPlaces = new List<Place> { new Place(55), new Place(56) };
            SimulationState unit = State(Tick.Zero, unitPlaces,
                new long[] { 5, 5 }, new long[] { 1, 0 }, new long[] { 2, 0 });
            var unitRelations = new RelationSet(unit,
                new Relation(unitPlaces[0], unitPlaces[1]));
            var threshold0 = new CompetenceTransportFixture(unitPlaces,
                Worlds5.Constant2, 0, new AdditiveResolver());
            var plantedMismatch = new CompetenceTransportFixture(unitPlaces,
                Worlds5.Constant2, 1, new AdditiveResolver());
            Assert(Contributions(unit, unitRelations, threshold0) !=
                Contributions(unit, unitRelations, plantedMismatch),
                "the comparator accepted a planted unit-threshold mismatch");
        }

        private static string Contributions(SimulationState state,
            RelationSet relations, IFixture fixture)
        {
            IReadOnlyList<Provenance.Entry> entries =
                Provenance.Collect(state, relations, new[] { fixture });
            var lines = new List<string>();
            foreach (Provenance.Entry entry in entries)
            {
                lines.Add($"{entry.Target.Place.Value}:{entry.Target.Kind.Value}:{entry.Amount}");
            }

            lines.Sort(StringComparer.Ordinal);
            return string.Join("|", lines);
        }

        private static void ClassifierObeysTheSeal()
        {
            Assert(Execution.Classify(c0: false, c1: false, c2: false, c3: false,
                durable: false, c4: false) == "G", "G did not take first precedence");
            Assert(Execution.Classify(c0: true, c1: true, c2: false, c3: true,
                durable: true, c4: true) == "F", "F was not selected");
            Assert(Execution.Classify(c0: true, c1: true, c2: true, c3: false,
                durable: true, c4: true) == "C", "C was not selected");
            Assert(Execution.Classify(c0: true, c1: false, c2: true, c3: true,
                durable: true, c4: true) == "B", "B was not selected");
            Assert(Execution.Classify(c0: true, c1: true, c2: true, c3: true,
                durable: false, c4: false) == "D", "D was not selected");
            Assert(Execution.Classify(c0: true, c1: true, c2: true, c3: true,
                durable: true, c4: false) == "E", "E was not selected");
            Assert(Execution.Classify(c0: true, c1: true, c2: true, c3: true,
                durable: true, c4: true) == "A", "A was not selected");
        }

        private static void SealedPairBuilds()
        {
            Parcel5 n0 = Worlds5.N0();
            Parcel5 n1 = Worlds5.N1();

            Assert(n0.Places.Count == 2 && n1.Places.Count == 2,
                "the sealed pair requires two places");
            Assert(n0.Relations.Count == 2 && n1.Relations.Count == 2,
                "the sealed pair requires A<->B");
            Assert(n0.Fixtures.Count == 3 && n1.Fixtures.Count == 3,
                "each world requires exactly three fixtures");
            Assert(n0.Competence == 0 && n1.Competence == 1,
                "the competence pair is not 0/1");
            Assert(n0.Ticks == 8 && n1.Ticks == 8, "the sealed run is eight ticks");
            Assert(StateInstrument.CompleteSignature(n0.Initial, n0.Places) ==
                StateInstrument.CompleteSignature(n1.Initial, n1.Places),
                "the initial states differ");

            foreach (Place place in n0.Places)
            {
                Assert(n0.Initial.ValueAt(new Cell(place, K4.Base)) == 0, "Base must be 0");
                Assert(n0.Initial.ValueAt(new Cell(place, K4.Rock)) == 8, "Rock must be 8");
                Assert(n0.Initial.ValueAt(new Cell(place, K4.Sediment)) == 0,
                    "Sediment must be 0");
                Assert(n0.Initial.ValueAt(new Cell(place, K4.Water)) == 0, "Water must be 0");
            }

            Assert(n0.Crossings.Events.Count == 2 && n1.Crossings.Events.Count == 2,
                "each world requires two crossings");
            for (int i = 0; i < n0.Crossings.Events.Count; i++)
            {
                ExternalEvent a = n0.Crossings.Events[i];
                ExternalEvent b = n1.Crossings.Events[i];
                Assert(a.Boundary == b.Boundary && a.Target == b.Target &&
                    a.Amount == b.Amount, "the crossing traces differ");
                Assert(a.Boundary.Value == i && a.Amount == 8,
                    "crossings must be +8 at boundaries 0 and 1");
            }

            var c0 = (CompetenceTransportFixture)n0.Fixtures[2];
            var c1 = (CompetenceTransportFixture)n1.Fixtures[2];
            Assert(c0.Competence == 0 && c1.Competence == 1,
                "the final fixture is not the sealed competence pair");
            // Deliberately no TickRunner here.
        }
    }
}
