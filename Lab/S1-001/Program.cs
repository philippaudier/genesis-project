using System;
using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// The S1-001 laboratory entry point. Two modes:
    ///   --smoke    instrument calibration on TOY worlds only (never a sealed parcel)
    ///   --execute  refused: campaign execution requires separate authorisation
    /// The gate is deliberately real — opening it is a one-line change under an authorisation
    /// commit, so the design → seal → implementation → execution chain stays provable by ancestry.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--smoke")
            {
                return Smoke.RunAll() ? 0 : 1;
            }

            if (args.Length >= 1 && args[0] == "--execute")
            {
                // Gate opened under the founder's execution authorisation (Campaign S1-001,
                // post-seal filings, 2026-07-28). The parcels run exactly as sealed.
                Console.WriteLine("Executing Campaign S1-001 as sealed:");
                return Execution.RunAll("Runs");
            }

            Console.WriteLine("S1-001 laboratory (Campaign S1-001 - The First Watershed)");
            Console.WriteLine("  --smoke    calibrate instruments on toy worlds (no sealed parcel is run)");
            Console.WriteLine("  --execute  run the sealed parcels (authorised 2026-07-28)");
            return 0;
        }
    }

    /// <summary>
    /// Instrument calibration. Every world here is a toy built for the check at hand — the sealed
    /// parcels W0–W4 are constructed (structure verified) but never ticked: that would be execution.
    /// </summary>
    public static class Smoke
    {
        private static readonly List<string> Failures = new List<string>();

        public static bool RunAll()
        {
            Check("fixture-set rejects double-binding a kind", FixtureSetRejectsDoubleBind);
            Check("membrane refuses undeclared crossings", MembraneRefusesUndeclared);
            Check("toy flow moves water downhill as the family dictates", ToyFlowMatchesHandComputation);
            Check("toy runs are deterministic (two runs, identical states)", ToyRunsDeterministic);
            Check("ledger + audit: additive toy conserves, rain accounted", AuditSilentOnAdditiveToy);
            Check("audit screams on the non-additive toy (B3 rehearsal)", AuditScreamsOnMaxResolverToy);
            Check("conversion emission is one whole pair (H1 shape)", ConversionEmitsWholePairs);
            Check("partition reader: toy slope terminates at the low end", PartitionReaderOnToySlope);
            Check("partition report passes the vocabulary gate; the gate itself catches a plant", VocabularyGateWorks);
            Check("sealed parcels build to their sealed parameters (no ticks run)", SealedParcelsBuild);

            Console.WriteLine();
            if (Failures.Count == 0)
            {
                Console.WriteLine("SMOKE: all checks passed. No sealed parcel was executed.");
                return true;
            }

            Console.WriteLine($"SMOKE: {Failures.Count} failure(s):");
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
                Console.WriteLine($"  FAIL {name}");
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        // --- toys ---------------------------------------------------------------------------

        private static (SimulationState state, RelationSet relations, IReadOnlyList<Place> places) ToyChain(params long[] water)
        {
            var places = new List<Place>();
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i < water.Length; i++)
            {
                var place = new Place(i);
                places.Add(place);
                cells[new Cell(place, K.Elevation)] = 0;
                cells[new Cell(place, K.Water)] = water[i];
            }

            var state = new SimulationState(Tick.Zero, cells);
            var relations = new List<Relation>();
            for (int i = 0; i + 1 < water.Length; i++)
            {
                relations.Add(new Relation(places[i], places[i + 1]));
                relations.Add(new Relation(places[i + 1], places[i]));
            }

            return (state, new RelationSet(state, relations.ToArray()), places);
        }

        private static ExternalEventTrace EmptyRain()
        {
            return new ExternalEventTrace(new Membrane(new[] { K.Water }));
        }

        // --- checks -------------------------------------------------------------------------

        private static void FixtureSetRejectsDoubleBind()
        {
            (SimulationState state, RelationSet _, IReadOnlyList<Place> places) = ToyChain(0, 0);
            var one = new FlowFixture(places, K.Water, Divisors.Naive, new AdditiveResolver());
            var two = new FlowFixture(places, K.Water, Divisors.Naive, new AdditiveResolver());
            try
            {
                var _ = new FixtureSet(one, two);
            }
            catch (InvalidOperationException)
            {
                return;
            }

            throw new InvalidOperationException("two resolvers for one kind were accepted");
        }

        private static void MembraneRefusesUndeclared()
        {
            ExternalEventTrace trace = EmptyRain();
            try
            {
                trace.Append(new ExternalEvent(Tick.Zero, new Cell(new Place(0), K.Rock), 1));
            }
            catch (UndeclaredExternalKindException)
            {
                return;
            }

            throw new InvalidOperationException("an undeclared kind crossed the membrane");
        }

        private static void ToyFlowMatchesHandComputation()
        {
            // Chain (12,0,0), flat ground, naive divisor 2: place0 sends floor(12/2)=6 to place1;
            // nothing else moves from the same snapshot. Expected after one tick: (6,6,0).
            (SimulationState state, RelationSet relations, IReadOnlyList<Place> places) = ToyChain(12, 0, 0);
            var set = new FixtureSet(new FlowFixture(places, K.Water, Divisors.Naive, new AdditiveResolver()));
            var runner = new TickRunner(new TransitionRunner(set.Resolvers));
            SimulationState next = runner.Run(state, relations, set.Transitions, EmptyRain(), 1);
            Assert(next.ValueAt(new Cell(places[0], K.Water)) == 6, "place0 should hold 6");
            Assert(next.ValueAt(new Cell(places[1], K.Water)) == 6, "place1 should hold 6");
            Assert(next.ValueAt(new Cell(places[2], K.Water)) == 0, "place2 should hold 0");
        }

        private static void ToyRunsDeterministic()
        {
            (SimulationState state, RelationSet relations, IReadOnlyList<Place> places) = ToyChain(12, 0, 0);
            var set = new FixtureSet(new FlowFixture(places, K.Water, Divisors.Naive, new AdditiveResolver()));
            var runner = new TickRunner(new TransitionRunner(set.Resolvers));
            SimulationState a = runner.Run(state, relations, set.Transitions, EmptyRain(), 25);
            SimulationState b = runner.Run(state, relations, set.Transitions, EmptyRain(), 25);
            Assert(a.Equals(b), "two identical runs diverged");
        }

        private static void AuditSilentOnAdditiveToy()
        {
            (SimulationState state, RelationSet relations, IReadOnlyList<Place> places) = ToyChain(12, 0, 0);
            var membrane = new Membrane(new[] { K.Water });
            var trace = new ExternalEventTrace(membrane);
            trace.Append(new ExternalEvent(new Tick(2), new Cell(places[1], K.Water), 5));

            var set = new FixtureSet(new FlowFixture(places, K.Water, Divisors.Naive, new AdditiveResolver()));
            var parcel = new Parcel("toy", state, relations, set, trace, places, 10);
            ParcelRun run = ParcelRun.Execute(parcel, 10);
            IReadOnlyList<ConservationAudit.Violation> violations =
                ConservationAudit.Check(run.States, new[] { K.Water }, trace);
            Assert(violations.Count == 0, $"additive toy leaked: {violations.Count} violation(s)");

            var ledger = PerKindLedger.Totals(run.States);
            Assert(ledger[10][K.Water] == 17, "final total should be 12 + 5");
        }

        private static void AuditScreamsOnMaxResolverToy()
        {
            (SimulationState state0, RelationSet _, IReadOnlyList<Place> _) = ToyChain(0, 0);
            var places = new List<Place> { new Place(0), new Place(1) };
            var cells = new Dictionary<Cell, long>
            {
                [new Cell(places[0], K.Sacrificial)] = 10,
                [new Cell(places[1], K.Sacrificial)] = 10,
            };
            var state = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(state);
            var set = new FixtureSet(new SacrificialFixture(places[0], places[1]));
            var trace = new ExternalEventTrace(new Membrane(new Kind[0]));
            var parcel = new Parcel("toy-b3", state, relations, set, trace, places, 3);
            ParcelRun run = ParcelRun.Execute(parcel, 3);
            IReadOnlyList<ConservationAudit.Violation> violations =
                ConservationAudit.Check(run.States, new[] { K.Sacrificial }, trace);
            Assert(violations.Count > 0, "the audit stayed silent on a non-additive resolver");
        }

        private static void ConversionEmitsWholePairs()
        {
            // Steep 2-place slope: diff 20, one neighbour, naive divisor -> transfer 10 > threshold 4.
            var places = new List<Place> { new Place(0), new Place(1) };
            var cells = new Dictionary<Cell, long>
            {
                [new Cell(places[0], K.Elevation)] = 20,
                [new Cell(places[0], K.Water)] = 0,
                [new Cell(places[1], K.Elevation)] = 0,
                [new Cell(places[1], K.Water)] = 0,
            };
            var state = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(state, new Relation(places[0], places[1]));
            var fixture = new ConversionFixture(places, Divisors.Naive, threshold: 4, new AdditiveResolver());

            ITransition transition = null;
            foreach (ITransition candidate in fixture.Transitions)
            {
                if (candidate.RelationScope.IncludesOrigin(places[0]))
                {
                    transition = candidate;
                }
            }

            IReadOnlyList<Contribution> emitted =
                transition.Apply(new RelationalStateView(state, relations, transition.ReadScope, transition.RelationScope));
            long rock = 0, sediment = 0;
            foreach (Contribution contribution in emitted)
            {
                if (contribution.Target.Kind == K.Rock) rock += contribution.Amount;
                if (contribution.Target.Kind == K.Sediment) sediment += contribution.Amount;
            }

            Assert(emitted.Count == 2, "expected exactly one whole pair");
            Assert(rock == -1 && sediment == 1, "the pair should be (-1 rock, +1 sediment)");
            Assert(rock + sediment == 0, "the pair should be zero-sum across kinds");
        }

        private static void PartitionReaderOnToySlope()
        {
            var places = new List<Place> { new Place(0), new Place(1), new Place(2) };
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i < 3; i++)
            {
                cells[new Cell(places[i], K.Elevation)] = 2 - i;
                cells[new Cell(places[i], K.Water)] = 0;
            }

            var state = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(state,
                new Relation(places[0], places[1]), new Relation(places[1], places[0]),
                new Relation(places[1], places[2]), new Relation(places[2], places[1]));
            IReadOnlyDictionary<Place, Place> partition =
                PartitionReader.TerminalPartition(state, relations, places, K.Elevation, K.Water);
            foreach (Place place in places)
            {
                Assert(partition[place] == places[2], $"place {place.Value} should terminate at place 2");
            }
        }

        private static void VocabularyGateWorks()
        {
            var places = new List<Place> { new Place(0) };
            var cells = new Dictionary<Cell, long>
            {
                [new Cell(places[0], K.Elevation)] = 0,
                [new Cell(places[0], K.Water)] = 0,
            };
            var state = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(state);
            IReadOnlyDictionary<Place, Place> partition =
                PartitionReader.TerminalPartition(state, relations, places, K.Elevation, K.Water);
            string report = PartitionReader.Report(partition);
            Assert(PartitionReader.VocabularyViolations(report).Count == 0,
                "the reader's own report used forbidden vocabulary");
            Assert(PartitionReader.VocabularyViolations("a basin appeared").Count == 1,
                "the gate failed to catch a planted forbidden word");
        }

        private static void SealedParcelsBuild()
        {
            Parcel w0 = Worlds.W0FlatControl();
            Parcel w1 = Worlds.W1NaiveBowl();
            Parcel w2 = Worlds.W2CorrectedBowl();
            Parcel w3 = Worlds.W3TwinFloors();
            Parcel w4 = Worlds.W4Conversion();
            Parcel b3 = Worlds.W4Conversion(withSacrificial: true);

            Assert(w0.Places.Count == 81 && w1.Places.Count == 81 && w2.Places.Count == 81, "9x9 parcels should hold 81 places");
            Assert(w3.Places.Count == 135, "W3 should hold 135 places");
            Assert(w4.Places.Count == 27, "W4 should hold 27 places");
            Assert(w0.Initial.ValueAt(new Cell(new Place(0), K.Elevation)) == 0, "W0 is flat");
            Assert(w1.Initial.ValueAt(new Cell(new Place(4 * 9 + 4), K.Elevation)) == 0, "W1 centre is 0");
            Assert(w1.Initial.ValueAt(new Cell(new Place(0), K.Elevation)) == 4, "W1 rim is 4");
            Assert(w3.Initial.ValueAt(new Cell(new Place(4 * 15 + 7), K.Elevation)) == 5, "W3 ridge column is 5");
            Assert(w4.Initial.ValueAt(new Cell(new Place(0), K.Rock)) == 100, "W4 rock starts at 100");
            Assert(w0.Trace.Events.Count == 81 * Worlds.RainTicks, "W0 rain: one crossing per place per rain tick");
            Assert(b3.Fixtures.Resolvers.ContainsKey(K.Sacrificial), "B3 variant binds the sacrificial kind");
            // No parcel is ticked here: execution is not authorised.
        }
    }
}
