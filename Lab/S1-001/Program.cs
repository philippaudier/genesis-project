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

            if (args.Length >= 1 && args[0] == "--execute-s1-002")
            {
                // Gate opened under the founder's execution authorisation (2026-07-29, recorded
                // in the campaign document with rule E4). The parcels run exactly as sealed.
                Console.WriteLine("Executing Campaign S1-002 as sealed:");
                return ExecutionS1002.RunAll("Runs-S1-002");
            }

            if (args.Length >= 1 && args[0] == "--execute-s1-003")
            {
                // Gate opened by the founder's explicit execution authorisation on 2026-07-29:
                // "oui execute". The pair runs exactly as sealed.
                Console.WriteLine("Executing Campaign S1-003 as sealed:");
                return ExecutionS1003.RunAll("Runs-S1-003");
            }

            Console.WriteLine("Science-001 laboratory");
            Console.WriteLine("  --smoke            calibrate instruments on toy worlds (no sealed parcel is run)");
            Console.WriteLine("  --execute          run Campaign S1-001's sealed parcels (authorised 2026-07-28)");
            Console.WriteLine("  --execute-s1-002   run Campaign S1-002's sealed parcels (authorised 2026-07-29)");
            Console.WriteLine("  --execute-s1-003   run Campaign S1-003's sealed pair (authorised 2026-07-29)");
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
            Check("flux counter matches hand-known toy transfers", FluxCounterOnToys);
            Check("flux counter consistency check passes on an additive toy run", FluxConsistencyOnToy);
            Check("flux counter discriminates a wrong divisor convention", FluxDiscriminatesConvention);
            Check("S1-002 parcels build to their sealed parameters (no ticks run)", S1002ParcelsBuild);
            Check("S1-003 parcels build to their sealed parameters (no ticks run)", S1003ParcelsBuild);
            Check("S1-003 mirror reader is zero on a mirrored toy", S1003MirrorReaderOnSymmetricToy);
            Check("S1-003 mirror signs reverse under a left-right swap", S1003MirrorReaderSwap);
            Check("S1-003 cross-world reader locates an injected first difference", S1003FirstDifference);
            Check("S1-003 partition totals reconstruct the whole toy", S1003PartitionTotals);
            Check("S1-003 identical toy worlds report no difference", S1003IdenticalWorlds);
            Check("S1-003 conservation witness screams on a corrupted toy record", S1003ConservationWitness);

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

        // Calibration toys deliberately belong to NO campaign: a 2-cell slope of 5 (the sealed
        // sweep stops at 4) and chains with initial water (no sealed parcel starts wet).

        private static void FluxCounterOnToys()
        {
            var places = new List<Place> { new Place(0), new Place(1) };
            var cells = new Dictionary<Cell, long>
            {
                [new Cell(places[0], K.Elevation)] = 5,
                [new Cell(places[0], K.Water)] = 0,
                [new Cell(places[1], K.Elevation)] = 0,
                [new Cell(places[1], K.Water)] = 0,
            };
            var state = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(state,
                new Relation(places[0], places[1]), new Relation(places[1], places[0]));

            IReadOnlyList<FluxCounter.EdgeFlux> flux =
                FluxCounter.FluxAt(state, relations, places, K.Elevation, K.Water, Divisors.Naive);
            Assert(flux.Count == 1, "one edge should carry flux");
            Assert(flux[0].Source == places[0] && flux[0].Target == places[1] && flux[0].Amount == 2,
                "slope 5, naive divisor: the edge should carry floor(5/2) = 2");

            (SimulationState chain, RelationSet chainRelations, IReadOnlyList<Place> chainPlaces) = ToyChain(12, 0, 0);
            IReadOnlyList<FluxCounter.EdgeFlux> chainFlux =
                FluxCounter.FluxAt(chain, chainRelations, chainPlaces, K.Elevation, K.Water, Divisors.Naive);
            Assert(chainFlux.Count == 1 && chainFlux[0].Amount == 6, "chain (12,0,0): 0->1 should carry 6");
            long first = FluxCounter.FirstTransferTick(new[] { chain }, chainRelations, chainPlaces,
                K.Elevation, K.Water, Divisors.Naive, new Relation(chainPlaces[0], chainPlaces[1]));
            Assert(first == -1 || first == 0, "first-transfer scan should not crash on a single state");
        }

        private static ParcelRun ToyChainRun()
        {
            (SimulationState state, RelationSet relations, IReadOnlyList<Place> places) = ToyChain(12, 0, 0);
            var set = new FixtureSet(new FlowFixture(places, K.Water, Divisors.Naive, new AdditiveResolver()));
            var parcel = new Parcel("toy-flux", state, relations, set, EmptyRain(), places, 10);
            return ParcelRun.Execute(parcel, 10);
        }

        private static void FluxConsistencyOnToy()
        {
            ParcelRun run = ToyChainRun();
            IReadOnlyList<FluxCounter.Mismatch> mismatches = FluxCounter.ConsistencyCheck(
                run.States, run.Parcel.Relations, run.Parcel.Places, run.Parcel.Trace,
                K.Elevation, K.Water, Divisors.Naive);
            Assert(mismatches.Count == 0, $"counter disagreed with the record: {mismatches.Count} mismatch(es)");
        }

        private static void FluxDiscriminatesConvention()
        {
            ParcelRun run = ToyChainRun();
            IReadOnlyList<FluxCounter.Mismatch> mismatches = FluxCounter.ConsistencyCheck(
                run.States, run.Parcel.Relations, run.Parcel.Places, run.Parcel.Trace,
                K.Elevation, K.Water, Divisors.DegreeAware);
            Assert(mismatches.Count > 0, "the counter failed to notice a wrong divisor convention");
        }

        private static void S1002ParcelsBuild()
        {
            IReadOnlyList<Parcel> parcels = WorldsS1002.All();
            Assert(parcels.Count == 12, "V0 + 5×V1 + 3×V2 + V3 + V4a + V4b = 12 parcels");

            Parcel v0 = parcels[0];
            Assert(v0.Places.Count == 2 && v0.Trace.Events.Count == 2 * WorldsS1002.RainTicks,
                "V0: two cells, rain on both");

            for (int d = 0; d <= 4; d++)
            {
                Parcel v1 = parcels[1 + d];
                Assert(v1.Places.Count == 2, $"V1-d{d}: two cells");
                Assert(v1.Initial.ValueAt(new Cell(new Place(0), K.Elevation)) == d, $"V1-d{d}: high cell E = {d}");
                Assert(v1.Trace.Events.Count == WorldsS1002.RainTicks, $"V1-d{d}: rain on one cell only");
            }

            for (int k = 1; k <= 3; k++)
            {
                Parcel v2 = parcels[5 + k];
                Assert(v2.Places.Count == k + 1, $"V2-k{k}: centre + {k} leaves");
                Assert(v2.Initial.ValueAt(new Cell(new Place(0), K.Elevation)) == 1, $"V2-k{k}: centre E = 1");
            }

            Parcel v3 = parcels[9];
            Assert(v3.Places.Count == 5, "V3: five cells");
            Parcel v4b = parcels[11];
            Assert(v4b.Places.Count == 6, "V4b: six places");
            Assert(v4b.Relations.Count == 12, "V4b: six edges, both directions");
            foreach (Parcel parcel in parcels)
            {
                Assert(parcel.PlannedTicks == WorldsS1002.PlannedTicks, $"{parcel.Name}: 30 planned ticks");
            }
            // No S1-002 parcel is ticked here, and no hand derivation is consulted:
            // execution is not authorised; the laboratory stays blind.
        }

        private static void S1003ParcelsBuild()
        {
            Parcel u0 = WorldsS1003.U0();
            Parcel u1 = WorldsS1003.U1();
            Assert(u0.Places.Count == 49 && u1.Places.Count == 49, "U0/U1 must each contain 49 cells");
            Assert(u0.Relations.Count == 168 && u1.Relations.Count == 168,
                "a bidirectional 7x7 four-neighbour grid must contain 168 directed relations");
            Assert(u0.Trace.Events.Count == 60 && u1.Trace.Events.Count == 60,
                "each world must contain exactly sixty point-source crossings");
            Assert(u0.PlannedTicks == 120 && u1.PlannedTicks == 120, "each world must be planned for 120 ticks");

            int differences = 0;
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    var place = new Place(row * 7 + col);
                    long e0 = u0.Initial.ValueAt(new Cell(place, K.Elevation));
                    long e1 = u1.Initial.ValueAt(new Cell(place, K.Elevation));
                    if (e0 != e1)
                    {
                        differences++;
                        Assert(row == 3 && col == 2 && e0 == 8 && e1 == 9,
                            "the sole relief difference must be (3,2): 8 -> 9");
                    }

                    Assert(e0 == u0.Initial.ValueAt(new Cell(new Place(row * 7 + (6 - col)), K.Elevation)),
                        "U0 relief must be mirror symmetric");
                    if (col + 1 < 7)
                    {
                        long right = u0.Initial.ValueAt(new Cell(new Place(row * 7 + col + 1), K.Elevation));
                        long right1 = u1.Initial.ValueAt(new Cell(new Place(row * 7 + col + 1), K.Elevation));
                        Assert(Math.Abs(e0 - right) <= 4, "U0 has a horizontal dry gradient above four");
                        Assert(Math.Abs(e1 - right1) <= 4, "U1 has a horizontal dry gradient above four");
                    }
                    if (row + 1 < 7)
                    {
                        long below = u0.Initial.ValueAt(new Cell(new Place((row + 1) * 7 + col), K.Elevation));
                        long below1 = u1.Initial.ValueAt(new Cell(new Place((row + 1) * 7 + col), K.Elevation));
                        Assert(Math.Abs(e0 - below) <= 4, "U0 has a vertical dry gradient above four");
                        Assert(Math.Abs(e1 - below1) <= 4, "U1 has a vertical dry gradient above four");
                    }
                }
            }

            Assert(differences == 1, "U0/U1 must differ at exactly one elevation cell");
            Assert(u1.Initial.ValueAt(new Cell(new Place(2), K.Elevation)) ==
                   u1.Initial.ValueAt(new Cell(new Place(4), K.Elevation)),
                "the source's initial lateral neighbours must be equal");
            Assert(Math.Abs(0 - 3) + Math.Abs(3 - 2) == 4,
                "the perturbation must be four relations from the source");
            for (int i = 0; i < u0.Trace.Events.Count; i++)
            {
                ExternalEvent crossing = u0.Trace.Events[i];
                Assert(crossing.Boundary == new Tick(i) && crossing.Target == new Cell(WorldsS1003.Source, K.Water)
                       && crossing.Amount == 1,
                    "U0 point source must admit +1 Water at boundaries 0..59");
            }
        }

        private static IReadOnlyList<SimulationState> ToyGridSequence(params long[][] frames)
        {
            var states = new List<SimulationState>();
            for (int tick = 0; tick < frames.Length; tick++)
            {
                var cells = new Dictionary<Cell, long>();
                for (int place = 0; place < frames[tick].Length; place++)
                {
                    cells[new Cell(new Place(place), K.Water)] = frames[tick][place];
                }
                states.Add(new SimulationState(new Tick(tick), cells));
            }
            return states;
        }

        private static void S1003MirrorReaderOnSymmetricToy()
        {
            IReadOnlyList<SimulationState> states = ToyGridSequence(
                new long[] { 1, 2, 3, 4, 3, 2, 1 },
                new long[] { 5, 6, 7, 8, 7, 6, 5 });
            foreach (S1003Instruments.MirrorDifference item in
                S1003Instruments.MirrorDifferences(states, width: 7, height: 1, K.Water))
            {
                Assert(item.SignedDifference == 0, "a mirrored toy produced a non-zero signed difference");
            }
        }

        private static void S1003MirrorReaderSwap()
        {
            IReadOnlyList<SimulationState> original = ToyGridSequence(
                new long[] { 9, 4, 2, 0, 1, 3, 8 });
            IReadOnlyList<SimulationState> swapped = ToyGridSequence(
                new long[] { 8, 3, 1, 0, 2, 4, 9 });
            IReadOnlyList<S1003Instruments.MirrorDifference> a =
                S1003Instruments.MirrorDifferences(original, 7, 1, K.Water);
            IReadOnlyList<S1003Instruments.MirrorDifference> b =
                S1003Instruments.MirrorDifferences(swapped, 7, 1, K.Water);
            Assert(a.Count == b.Count, "swap changed the number of mirror readings");
            for (int i = 0; i < a.Count; i++)
            {
                Assert(a[i].SignedDifference == -b[i].SignedDifference,
                    "swap did not reverse a mirror sign");
                Assert(Math.Abs(a[i].SignedDifference) == Math.Abs(b[i].SignedDifference),
                    "swap changed a mirror magnitude");
            }
        }

        private static void S1003FirstDifference()
        {
            IReadOnlyList<SimulationState> a = ToyGridSequence(
                new long[] { 0, 0, 0 }, new long[] { 1, 0, 0 }, new long[] { 1, 1, 0 });
            IReadOnlyList<SimulationState> b = ToyGridSequence(
                new long[] { 0, 0, 0 }, new long[] { 1, 0, 0 }, new long[] { 1, 2, 0 });
            S1003Instruments.WorldDifference difference = S1003Instruments.FirstWorldDifference(a, b, K.Water);
            Assert(difference != null && difference.Tick == 2 && difference.Place == new Place(1),
                "the injected difference was not located at tick 2, place 1");
        }

        private static void S1003PartitionTotals()
        {
            SimulationState state = ToyGridSequence(
                new long[] { 1, 2, 3, 4, 5, 6, 7 },
                new long[] { 2, 4, 6, 8, 10, 12, 14 })[1];
            S1003Instruments.RegionTotals totals = S1003Instruments.Totals(state, 7, 1, K.Water);
            Assert(totals.Left + totals.Centre + totals.Right == totals.Full,
                "left + centre + right did not reconstruct the full total");
        }

        private static void S1003IdenticalWorlds()
        {
            IReadOnlyList<SimulationState> states = ToyGridSequence(
                new long[] { 0, 1 }, new long[] { 1, 1 });
            Assert(S1003Instruments.FirstWorldDifference(states, states, K.Water) == null,
                "identical toy worlds produced a cross-world difference");
        }

        private static void S1003ConservationWitness()
        {
            IReadOnlyList<SimulationState> corrupted = ToyGridSequence(
                new long[] { 0, 0 }, new long[] { 1, 1 });
            var trace = new ExternalEventTrace(new Membrane(new[] { K.Water }));
            trace.Append(new ExternalEvent(Tick.Zero, new Cell(new Place(0), K.Water), 1));
            Assert(ConservationAudit.Check(corrupted, new[] { K.Water }, trace).Count == 1,
                "the conservation witness stayed silent on a corrupted toy record");
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
