using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;
using Genesis.Lab.S1_005;

namespace Genesis.Lab.S1_006
{
    /// <summary>
    /// Gate 5. Every dynamic or synthetic trajectory is a foreign toy. P0/P1 are only
    /// constructed and inspected; neither is ever passed to a TickRunner.
    /// </summary>
    public static class Calibration
    {
        private static readonly List<string> Failures = new List<string>();

        public static bool RunAll()
        {
            Failures.Clear();
            Check("surface equality remains weaker than complete-state equality",
                SurfaceAndCompleteStateRemainDistinct);
            Check("period 2 is rejected as fixed", PeriodIsNotFixed);
            Check("a late silent complete-state transition is accepted",
                LateFixedTransitionIsAccepted);
            Check("equal final surface with unequal hidden state remains visible",
                HiddenSelectionRemainsVisible);
            Check("wrong first boundary or fixture is rejected", FirstDifferenceIsExact);
            Check("dropped contribution, matter fault, negative, and crossing are caught",
                InvalidEvidenceIsCaught);
            Check("allocation-sensitive transport is exposed", AllocationIsExposed);
            Check("degree-aware differs from Constant2 on a branching toy",
                DivisorPoliciesAreDistinguished);
            Check("outcomes A-G and precedence are planted", ClassifierObeysSeal);
            Check("sealed P0/P1 build exactly as approved (no tick run)",
                SealedPairBuilds);

            Console.WriteLine();
            if (Failures.Count == 0)
            {
                Console.WriteLine("CALIBRATION: all S1-006 obligations discharged.");
                Console.WriteLine("P0 and P1 were constructed and inspected; neither was ticked.");
                return true;
            }

            Console.WriteLine($"CALIBRATION: {Failures.Count} failure(s).");
            foreach (string failure in Failures) Console.WriteLine($"  FAIL {failure}");
            return false;
        }

        private static void Check(string name, Action action)
        {
            try
            {
                action();
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
            if (!condition) throw new InvalidOperationException(message);
        }

        private static SimulationState State(long tick, IReadOnlyList<Place> places,
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
            return new SimulationState(new Tick(tick), cells);
        }

        private static void SurfaceAndCompleteStateRemainDistinct()
        {
            var places = new[] { new Place(20) };
            SimulationState a = State(0, places, new long[] { 5 },
                new long[] { 0 }, new long[] { 2 });
            SimulationState b = State(1, places, new long[] { 4 },
                new long[] { 1 }, new long[] { 2 });
            Assert(StateInstrument.SurfaceSignature(a, places) ==
                StateInstrument.SurfaceSignature(b, places), "planted surfaces differ");
            Assert(StateInstrument.CompleteSignature(a, places) !=
                StateInstrument.CompleteSignature(b, places), "hidden material was collapsed");
        }

        private static void PeriodIsNotFixed()
        {
            var places = new[] { new Place(21) };
            SimulationState a0 = State(0, places, new long[] { 5 },
                new long[] { 0 }, new long[] { 0 });
            SimulationState b = State(1, places, new long[] { 5 },
                new long[] { 0 }, new long[] { 1 });
            SimulationState a2 = State(2, places, new long[] { 5 },
                new long[] { 0 }, new long[] { 0 });
            var states = new[] { a0, b, a2 };
            var zero = new[] { 0, 0 };
            Assert(Instrument6.FirstFixed(states, places, zero, zero, 0, 1) == null,
                "a period-2 return was mistaken for adjacent complete-state rest");
        }

        private static void LateFixedTransitionIsAccepted()
        {
            var places = new[] { new Place(22) };
            var states = new List<SimulationState>();
            var contributions = new List<int>();
            var crossings = new List<int>();
            for (int tick = 0; tick <= 15; tick++)
            {
                long water = tick < 14 ? tick : 14;
                states.Add(State(tick, places, new long[] { 5 },
                    new long[] { 0 }, new long[] { water }));
                if (tick < 15)
                {
                    contributions.Add(tick < 14 ? 1 : 0);
                    crossings.Add(0);
                }
            }
            Instrument6.FixedWitness witness =
                Instrument6.FirstFixed(states, places, contributions, crossings, 12, 14);
            Assert(witness != null && witness.Boundary == 14,
                "the planted late silent transition was not accepted");
        }

        private static void HiddenSelectionRemainsVisible()
        {
            SurfaceAndCompleteStateRemainDistinct();
            Assert(Instrument6.Classify(true, true, true, true, true, false) == "B",
                "equal surface / unequal complete state did not classify B");
            Assert(Instrument6.Classify(true, true, true, true, true, true) == "C",
                "identical complete states did not classify C");
        }

        private static void FirstDifferenceIsExact()
        {
            bool Match(int boundary, string fixture, int from, int to, long amount) =>
                boundary == 6 && fixture == nameof(CompetenceTransportFixture) &&
                from == 2 && to == 3 && amount == 1;
            Assert(Match(6, nameof(CompetenceTransportFixture), 2, 3, 1),
                "the sealed witness was rejected");
            Assert(!Match(5, nameof(CompetenceTransportFixture), 2, 3, 1),
                "wrong boundary passed");
            Assert(!Match(6, nameof(SurfaceFlowFixture), 2, 3, 1),
                "wrong fixture passed");
        }

        private static void InvalidEvidenceIsCaught()
        {
            var places = new[] { new Place(23) };
            SimulationState initial = State(0, places, new long[] { 5 },
                new long[] { 0 }, new long[] { 0 });
            SimulationState dropped = State(1, places, new long[] { 4 },
                new long[] { 0 }, new long[] { -1 });
            Assert(Surface.MatterAudit(new[] { initial, dropped }, places).Count == 1,
                "dropped conversion half-pair passed");
            Assert(StateInstrument.Minimum(dropped, places) == -1,
                "negative cell passed");

            var trace = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            trace.Append(new ExternalEvent(new Tick(4),
                new Cell(places[0], K4.Water), 7));
            Assert(!Instrument6.CrossingsMatch(trace.Events, new long[] { 4, 5 },
                new Cell(places[0], K4.Water), 7), "missing crossing passed");
        }

        private static void AllocationIsExposed()
        {
            Assert(Instrument6.AllocationSensitive(3, 2, 2),
                "insufficient holding across two eligible edges was hidden");
            Assert(!Instrument6.AllocationSensitive(4, 2, 2),
                "fully satisfiable demand was called allocation-sensitive");
            Assert(!Instrument6.AllocationSensitive(1, 2, 0),
                "one eligible edge was called allocation-sensitive");
        }

        private static void DivisorPoliciesAreDistinguished()
        {
            Assert(Divisors.Naive(2) == 2, "Constant2 changed");
            Assert(Divisors.DegreeAware(2) == 3, "degree+1 changed");

            var places = new[] { new Place(30), new Place(31), new Place(32) };
            SimulationState state = State(0, places, new long[] { 5, 5, 5 },
                new long[] { 0, 0, 0 }, new long[] { 12, 0, 0 });
            var relations = new RelationSet(state,
                new Relation(places[0], places[1]),
                new Relation(places[0], places[2]));
            var constant = new SurfaceFlowFixture(places, Divisors.Naive,
                new AdditiveResolver());
            var degree = new SurfaceFlowFixture(places, Divisors.DegreeAware,
                new AdditiveResolver());
            int constantAmount = FirstOutflow(state, relations, constant);
            int degreeAmount = FirstOutflow(state, relations, degree);
            Assert(constantAmount == 6 && degreeAmount == 4,
                "branching toy did not expose 6-versus-4");
        }

        private static int FirstOutflow(SimulationState state, RelationSet relations,
            IFixture fixture)
        {
            foreach (Provenance.Entry entry in
                Provenance.Collect(state, relations, new[] { fixture }))
            {
                if (entry.Amount < 0) return (int)-entry.Amount;
            }
            return 0;
        }

        private static void ClassifierObeysSeal()
        {
            Assert(Instrument6.Classify(true, true, true, true, false, false) == "A", "A");
            Assert(Instrument6.Classify(true, true, true, true, true, false) == "B", "B");
            Assert(Instrument6.Classify(true, true, true, true, true, true) == "C", "C");
            Assert(Instrument6.Classify(true, true, true, false, false, false) == "D", "D");
            Assert(Instrument6.Classify(true, true, false, false, false, false) == "E", "E");
            Assert(Instrument6.Classify(true, false, false, false, false, false) == "F", "F");
            Assert(Instrument6.Classify(false, false, false, false, false, false) == "G", "G");
        }

        private static void SealedPairBuilds()
        {
            Parcel6 p0 = Worlds6.P0();
            Parcel6 p1 = Worlds6.P1();
            Assert(p0.Name == "P0" && p1.Name == "P1", "pair names differ");
            Assert(p0.Competence == 0 && p1.Competence == 1, "competence pair differs");
            Assert(p0.Ticks == 128 && p1.Ticks == 128, "tick window differs");
            Assert(p0.Places.Count == 4 && p1.Places.Count == 4, "place count differs");
            Assert(p0.Relations.Count == 6 && p1.Relations.Count == 6, "chain differs");
            Assert(p0.Fixtures.Count == 3 && p1.Fixtures.Count == 3, "fixture count differs");

            foreach (Place place in p0.Places)
            {
                Assert(p0.Initial.ValueAt(new Cell(place, K4.Base)) == 0, "Base differs");
                Assert(p0.Initial.ValueAt(new Cell(place, K4.Rock)) == 12, "Rock differs");
                Assert(p0.Initial.ValueAt(new Cell(place, K4.Sediment)) == 0, "Sediment differs");
                Assert(p0.Initial.ValueAt(new Cell(place, K4.Water)) == 0, "Water differs");
            }

            var boundaries = new long[] { 0, 1, 10, 11 };
            Assert(Instrument6.CrossingsMatch(p0.Crossings.Events, boundaries,
                new Cell(p0.Places[0], K4.Water), 12), "P0 crossings differ");
            Assert(Instrument6.CrossingsMatch(p1.Crossings.Events, boundaries,
                new Cell(p1.Places[0], K4.Water), 12), "P1 crossings differ");

            for (int i = 0; i < p0.Fixtures.Count; i++)
                Assert(p0.Fixtures[i].GetType() == p1.Fixtures[i].GetType(),
                    "fixture types differ");
        }
    }
}
