using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;

namespace Genesis.Lab.S1_004
{
    /// <summary>
    /// The S1-004 laboratory. Two modes:
    ///   --calibrate  discharge the eight sealed instrument obligations, on FOREIGN toys only
    ///   --execute    refused: the campaign is sealed and execution is not authorised
    /// The sealed parcels M0 and M1 are constructed (their parameters verified) and never ticked.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--calibrate")
            {
                return Calibration.RunAll() ? 0 : 1;
            }

            if (args.Length >= 1 && args[0] == "--execute")
            {
                Console.WriteLine("Execution of Campaign S1-004 is not authorised.");
                Console.WriteLine("Sealed at b114a34; the founder authorised the seal and the implementation only.");
                Console.WriteLine("Gate 7 (conformance) and gate 8 (a second authorisation) come first.");
                return 2;
            }

            Console.WriteLine("S1-004 laboratory (Campaign S1-004 - The First Changed Surface)");
            Console.WriteLine("  --calibrate  discharge the instrument obligations on foreign toys");
            Console.WriteLine("  --execute    (refused until authorised)");
            return 0;
        }
    }

    /// <summary>
    /// The eight obligations sealed before execution, each on a world built for the check and
    /// belonging to no campaign — never M0, never M1.
    /// </summary>
    public static class Calibration
    {
        private static readonly List<string> Failures = new List<string>();

        public static bool RunAll()
        {
            Check("1. the SolidSurface reader is exactly Base + Rock + Sediment", ReaderIsExact);
            Check("2. a conversion pair changes neither local nor global surface", ConversionLeavesSurface);
            Check("3. a Sediment transfer moves the two readings by opposite equal deltas", TransferMovesSurface);
            Check("4. the cross-kind ledger detects a dropped half-pair", LedgerCatchesHalfPair);
            Check("5. the transport reconstruction detects an incorrect edge amount", ReconstructionCatchesWrongAmount);
            Check("6. identical toy worlds produce no cross-world surface difference", IdenticalWorldsAgree);
            Check("7. the spy resolver records a planted (+1,-1) conflict once, committing 0", SpyRecordsTheConflict);
            Check("8. a provenance record missing a contribution breaks the collision witness", ProvenanceMustBeComplete);
            Check("sealed parcels build to their sealed parameters (no ticks run)", SealedPairBuilds);

            Console.WriteLine();
            if (Failures.Count == 0)
            {
                Console.WriteLine("CALIBRATION: all obligations discharged. No sealed parcel was executed.");
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

        // --- foreign toys ---------------------------------------------------------------------
        // Deliberately unlike the sealed pair: two places, Rock 5, Sediment pre-loaded, Water
        // present in the initial state instead of arriving through the membrane.

        private sealed class Toy
        {
            public IReadOnlyList<Place> Places;
            public SimulationState State;
            public RelationSet Relations;
            public List<IFixture> Fixtures;
            public FixtureSet Set;
            public SpyResolver SpySediment;
            public ExternalEventTrace NoCrossings;
        }

        private static Toy BuildToy(bool transport, long water = 20, long rock = 5, long sediment = 1)
        {
            var places = new List<Place> { new Place(0), new Place(1) };
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i < places.Count; i++)
            {
                cells[new Cell(places[i], K4.Base)] = 0;
                cells[new Cell(places[i], K4.Rock)] = rock;
                cells[new Cell(places[i], K4.Sediment)] = i == 0 ? sediment : 0;
                cells[new Cell(places[i], K4.Water)] = i == 0 ? water : 0;
            }

            var state = new SimulationState(Tick.Zero, cells);
            var relations = new RelationSet(state,
                new Relation(places[0], places[1]), new Relation(places[1], places[0]));

            var additive = new AdditiveResolver();
            var spyWater = new SpyResolver(K4.Water, additive);
            var spyRock = new SpyResolver(K4.Rock, additive);
            var spySediment = new SpyResolver(K4.Sediment, additive);

            var fixtures = new List<IFixture>
            {
                new SurfaceFlowFixture(places, Worlds4.Constant2, spyWater),
                new SurfaceConversionFixture(places, Worlds4.Constant2, Worlds4.Threshold, spyRock, spySediment),
            };

            if (transport)
            {
                fixtures.Add(new SurfaceSedimentTransportFixture(places, Worlds4.Constant2, spySediment));
            }

            return new Toy
            {
                Places = places,
                State = state,
                Relations = relations,
                Fixtures = fixtures,
                Set = new FixtureSet(fixtures.ToArray()),
                SpySediment = spySediment,
                NoCrossings = new ExternalEventTrace(new Membrane(new[] { K4.Water })),
            };
        }

        private static SimulationState Tick1(Toy toy)
        {
            var runner = new TickRunner(new TransitionRunner(toy.Set.Resolvers));
            return runner.Run(toy.State, toy.Relations, toy.Set.Transitions, toy.NoCrossings, 1);
        }

        // --- obligations ----------------------------------------------------------------------

        private static void ReaderIsExact()
        {
            var place = new Place(0);
            var cells = new Dictionary<Cell, long>
            {
                [new Cell(place, K4.Base)] = 3,
                [new Cell(place, K4.Rock)] = 7,
                [new Cell(place, K4.Sediment)] = 2,
                [new Cell(place, K4.Water)] = 99,
            };
            var state = new SimulationState(Tick.Zero, cells);

            Assert(K4.SolidSurface(state, place) == 12, "3 + 7 + 2 should read 12");
            Assert(K4.WaterPotential(state, place) == 111, "the potential should add Water");
        }

        private static void ConversionLeavesSurface()
        {
            Toy toy = BuildToy(transport: false);
            long[] before = Surface.SolidSurfaces(toy.State, toy.Places);
            long matterBefore = Surface.MatterTotal(toy.State, toy.Places);

            SimulationState after = Tick1(toy);
            long[] surfaces = Surface.SolidSurfaces(after, toy.Places);

            Assert(after.ValueAt(new Cell(toy.Places[0], K4.Rock)) < toy.State.ValueAt(new Cell(toy.Places[0], K4.Rock)),
                "the toy must actually convert, or it proves nothing");
            for (int i = 0; i < surfaces.Length; i++)
            {
                Assert(surfaces[i] == before[i], $"place {i}: surface moved without transport");
            }

            Assert(Surface.MatterTotal(after, toy.Places) == matterBefore, "global matter moved");
        }

        private static void TransferMovesSurface()
        {
            Toy toy = BuildToy(transport: true);
            long[] before = Surface.SolidSurfaces(toy.State, toy.Places);
            SimulationState after = Tick1(toy);
            long[] surfaces = Surface.SolidSurfaces(after, toy.Places);

            long deltaA = surfaces[0] - before[0];
            long deltaB = surfaces[1] - before[1];
            Assert(deltaA != 0, "the toy must actually transport, or it proves nothing");
            Assert(deltaA == -deltaB, $"opposite equal deltas expected, found {deltaA} and {deltaB}");
            Assert(Surface.MatterTotal(after, toy.Places) == Surface.MatterTotal(toy.State, toy.Places),
                "a transfer must not change total matter");
        }

        private static void LedgerCatchesHalfPair()
        {
            // A hand-built sequence in which one half of a pair never lands.
            var place = new Place(0);
            var places = new List<Place> { place };
            var before = new SimulationState(Tick.Zero, new Dictionary<Cell, long>
            {
                [new Cell(place, K4.Base)] = 0,
                [new Cell(place, K4.Rock)] = 10,
                [new Cell(place, K4.Sediment)] = 0,
                [new Cell(place, K4.Water)] = 0,
            });
            SimulationState honest = before.WithValue(new Cell(place, K4.Rock), 9)
                                          .WithValue(new Cell(place, K4.Sediment), 1);
            SimulationState maimed = before.WithValue(new Cell(place, K4.Rock), 9); // the +Sediment never lands

            Assert(Surface.MatterAudit(new[] { before, honest }, places).Count == 0,
                "a complete pair must pass the ledger");
            Assert(Surface.MatterAudit(new[] { before, maimed }, places).Count == 1,
                "a dropped half-pair must be caught");
        }

        private static void ReconstructionCatchesWrongAmount()
        {
            Toy toy = BuildToy(transport: true);
            SimulationState after = Tick1(toy);

            IReadOnlyList<string> reconstructed = Surface.Flux(
                toy.State, toy.Relations, toy.Places, K4.Sediment, Worlds4.Constant2, cappedByHolding: true);
            IReadOnlyDictionary<int, long> conversions = Surface.ConversionsAt(
                toy.State, toy.Relations, toy.Places, Worlds4.Constant2, Worlds4.Threshold);

            Assert(Explains(reconstructed, conversions, toy.State, after, toy.Places, K4.Sediment),
                "the true reconstruction must explain the observed state change");

            var corrupted = new List<string>(reconstructed);
            Assert(corrupted.Count > 0, "the toy must produce at least one edge to corrupt");
            corrupted[0] = corrupted[0].Substring(0, corrupted[0].LastIndexOf(':') + 1) + "7";
            Assert(!Explains(corrupted, conversions, toy.State, after, toy.Places, K4.Sediment),
                "an incorrect edge amount must be detected");

            // And a reader that forgets the conversion source must fail too — the failure this
            // obligation actually caught the first time it was run.
            var noConversions = new Dictionary<int, long>();
            foreach (Place place in toy.Places)
            {
                noConversions[place.Value] = 0;
            }

            Assert(!Explains(reconstructed, noConversions, toy.State, after, toy.Places, K4.Sediment),
                "a reader blind to conversion emissions must not appear to balance");
        }

        /// <summary>
        /// Whether the declared per-edge flux **and** the declared conversion emissions together
        /// account exactly for every cell's change of that kind.
        /// </summary>
        private static bool Explains(IReadOnlyList<string> edges, IReadOnlyDictionary<int, long> conversions,
            SimulationState before, SimulationState after, IReadOnlyList<Place> places, Kind kind)
        {
            var delta = new Dictionary<int, long>();
            foreach (Place place in places)
            {
                delta[place.Value] = 0;
            }

            foreach (string edge in edges)
            {
                string[] parts = edge.Split(new[] { "->", ":" }, StringSplitOptions.None);
                int from = int.Parse(parts[0]);
                int to = int.Parse(parts[1]);
                long amount = long.Parse(parts[2]);
                delta[from] -= amount;
                delta[to] += amount;
            }

            if (kind == K4.Sediment)
            {
                foreach (Place place in places)
                {
                    if (conversions.TryGetValue(place.Value, out long pairs))
                    {
                        delta[place.Value] += pairs;
                    }
                }
            }

            foreach (Place place in places)
            {
                long observed = after.ValueAt(new Cell(place, kind)) - before.ValueAt(new Cell(place, kind));
                if (observed != delta[place.Value])
                {
                    return false;
                }
            }

            return true;
        }

        private static void IdenticalWorldsAgree()
        {
            Toy one = BuildToy(transport: true);
            Toy two = BuildToy(transport: true);
            long[] a = Surface.SolidSurfaces(Tick1(one), one.Places);
            long[] b = Surface.SolidSurfaces(Tick1(two), two.Places);

            for (int i = 0; i < a.Length; i++)
            {
                Assert(a[i] == b[i], $"identical worlds disagreed at place {i}");
            }
        }

        private static void SpyRecordsTheConflict()
        {
            Toy toy = BuildToy(transport: true);
            Tick1(toy);

            var conflicts = new List<SpyResolver.Invocation>(toy.SpySediment.Invocations);
            Assert(conflicts.Count == 1, $"expected exactly one Sediment conflict, saw {conflicts.Count}");
            Assert(conflicts[0].Amounts.Length == 2, "the conflict must carry two contributions");

            long sum = conflicts[0].Amounts[0] + conflicts[0].Amounts[1];
            Assert(sum == 0 && conflicts[0].Committed == 0,
                $"a (+1,-1) conflict must commit 0, committed {conflicts[0].Committed}");

            // And the resolver is not invoked where no cell is contested.
            Toy quiet = BuildToy(transport: false);
            Tick1(quiet);
            Assert(quiet.SpySediment.Invocations.Count == 0, "no conflict should be reported without transport");
        }

        private static void ProvenanceMustBeComplete()
        {
            Toy toy = BuildToy(transport: true);
            IReadOnlyList<Provenance.Entry> entries =
                Provenance.Collect(toy.State, toy.Relations, toy.Fixtures);
            Tick1(toy);

            IReadOnlyList<KeyValuePair<Cell, List<Provenance.Entry>>> collisions =
                Provenance.Collisions(entries);
            int sedimentCollisions = 0;
            foreach (KeyValuePair<Cell, List<Provenance.Entry>> collision in collisions)
            {
                if (collision.Key.Kind == K4.Sediment)
                {
                    sedimentCollisions++;
                }
            }

            Assert(sedimentCollisions == toy.SpySediment.Invocations.Count,
                "the two witnesses must agree on the number of contested Sediment cells");
            Assert(sedimentCollisions == 1, "the toy must contest exactly one Sediment cell");

            // Plant a gap: drop one of the two contributions to the contested cell.
            Cell contested = collisions[0].Key;
            var maimed = new List<Provenance.Entry>();
            bool dropped = false;
            foreach (Provenance.Entry entry in entries)
            {
                if (!dropped && entry.Target == contested)
                {
                    dropped = true;
                    continue;
                }

                maimed.Add(entry);
            }

            int afterDrop = 0;
            foreach (KeyValuePair<Cell, List<Provenance.Entry>> collision in Provenance.Collisions(maimed))
            {
                if (collision.Key.Kind == K4.Sediment)
                {
                    afterDrop++;
                }
            }

            Assert(afterDrop != toy.SpySediment.Invocations.Count,
                "an incomplete provenance record must disagree with the resolver's own count");
        }

        private static void SealedPairBuilds()
        {
            Parcel4 m0 = Worlds4.M0();
            Parcel4 m1 = Worlds4.M1();

            Assert(m0.Places.Count == 3 && m1.Places.Count == 3, "three places: A, B, C");
            Assert(m0.Relations.Count == 4 && m1.Relations.Count == 4, "A<->B and B<->C, both directions");
            Assert(m0.Fixtures.Count == 2, "M0 carries flow and conversion");
            Assert(m1.Fixtures.Count == 3, "M1 carries flow, conversion and transport");
            Assert(m1.Fixtures.Count - m0.Fixtures.Count == 1, "the pair differs by exactly one fixture");

            foreach (Place place in m0.Places)
            {
                Assert(m0.Initial.ValueAt(new Cell(place, K4.Base)) == 0, "Base 0 everywhere");
                Assert(m0.Initial.ValueAt(new Cell(place, K4.Rock)) == 10, "Rock 10 everywhere");
                Assert(m0.Initial.ValueAt(new Cell(place, K4.Sediment)) == 0, "Sediment 0 everywhere");
                Assert(m0.Initial.ValueAt(new Cell(place, K4.Water)) == 0, "Water 0 everywhere");
                Assert(K4.SolidSurface(m0.Initial, place) == 10, "the initial surface reads 10");
            }

            Assert(m1.Crossings.Events.Count == 2, "two crossings, boundaries 0 and 1");
            foreach (ExternalEvent crossing in m1.Crossings.Events)
            {
                Assert(crossing.Target.Place == m1.Places[0], "the crossings enter at A");
                Assert(crossing.Target.Kind == K4.Water, "only Water crosses the membrane");
                Assert(crossing.Amount == 10, "+10 each");
                Assert(crossing.Boundary.Value <= 1, "boundaries 0 and 1; silence afterwards");
            }

            Assert(m0.Ticks == 6 && m1.Ticks == 6, "six ticks");
            // Neither parcel is ticked here. Execution is not authorised.
        }
    }
}
