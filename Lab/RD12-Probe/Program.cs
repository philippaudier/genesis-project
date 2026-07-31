using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_001;
using Genesis.Lab.S1_004;
using Genesis.Lab.S1_005;

namespace Genesis.Lab.RD12_Probe
{
    public static class Program
    {
        private const int Horizon = 256;
        private const int SearchBeginsAtTick = 18;
        private static readonly int[] Lengths = { 2, 3, 5, 9 };
        private static readonly long[] Competences = { 0, 1 };
        private static readonly long[] CrossingBoundaries = { 0, 1, 8, 9, 16, 17 };
        private static readonly Policy[] Policies =
        {
            new Policy("constant2", Worlds5.Constant2),
            new Policy("degree+1", Divisors.DegreeAware),
        };

        private sealed class Policy
        {
            public string Name;
            public DivisorPolicy Divisor;

            public Policy(string name, DivisorPolicy divisor)
            {
                Name = name;
                Divisor = divisor;
            }
        }

        private sealed class Parcel
        {
            public IReadOnlyList<Place> Places;
            public SimulationState Initial;
            public RelationSet Relations;
            public FixtureSet Set;
            public ExternalEventTrace Crossings;
        }

        private sealed class Repeat
        {
            public int First;
            public int Again;
            public int Period => Again - First;
        }

        public static int Main()
        {
            Console.WriteLine("RD-12 PROBE — CONTAMINATED, NON-EVIDENTIAL");
            Console.WriteLine("Three forcing episodes; complete-state recurrence searched after tick 18.");
            Console.WriteLine();
            Console.WriteLine("policy    | length | competence | recurrence | cycle surface(s) | matter | first negative | reading");

            foreach (Policy policy in Policies)
            {
                foreach (int length in Lengths)
                {
                    foreach (long competence in Competences)
                    {
                        Console.WriteLine(Run(policy, length, competence));
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("This table locates resistance. It proves nothing.");
            return 0;
        }

        private static string Run(Policy policy, int length, long competence)
        {
            Parcel parcel = Build(policy.Divisor, length, competence);
            var runner = new TickRunner(new TransitionRunner(parcel.Set.Resolvers));
            SimulationState state = parcel.Initial;
            long initialMatter = Surface.MatterTotal(state, parcel.Places);
            long minimum = StateInstrument.Minimum(state, parcel.Places);
            string firstNegative = null;
            var seen = new Dictionary<string, int>();
            var surfaces = new List<string>();
            Repeat repeat = null;

            for (int tick = 0; tick <= Horizon; tick++)
            {
                string signature = StateInstrument.CompleteSignature(state, parcel.Places);
                surfaces.Add(StateInstrument.SurfaceSignature(state, parcel.Places));
                minimum = Math.Min(minimum, StateInstrument.Minimum(state, parcel.Places));
                if (firstNegative == null)
                {
                    firstNegative = FindNegative(state, parcel.Places, tick);
                }

                if (tick >= SearchBeginsAtTick)
                {
                    if (seen.TryGetValue(signature, out int first))
                    {
                        repeat = new Repeat { First = first, Again = tick };
                        break;
                    }

                    seen.Add(signature, tick);
                }

                if (tick < Horizon)
                {
                    state = runner.Run(state, parcel.Relations, parcel.Set.Transitions,
                        parcel.Crossings, 1);
                }
            }

            long finalMatter = Surface.MatterTotal(state, parcel.Places);
            string recurrence = repeat == null
                ? $"none≤{Horizon}"
                : $"t{repeat.First}->t{repeat.Again}(p{repeat.Period})";
            var cycleSurfaces = new List<string>();
            if (repeat != null)
            {
                for (int tick = repeat.First; tick < repeat.Again; tick++)
                {
                    if (!cycleSurfaces.Contains(surfaces[tick]))
                    {
                        cycleSurfaces.Add(surfaces[tick]);
                    }
                }
            }
            else
            {
                cycleSurfaces.Add(surfaces[surfaces.Count - 1]);
            }

            string representative = cycleSurfaces[0];
            long range = SurfaceRange(representative);
            string reading;
            if (initialMatter != finalMatter)
            {
                reading = "matter fault";
            }
            else if (minimum < 0)
            {
                reading = repeat == null ? "negative, unresolved" : $"negative orbit p{repeat.Period}";
            }
            else if (repeat == null)
            {
                reading = "unresolved";
            }
            else if (repeat.Period > 1)
            {
                reading = $"material orbit p{repeat.Period}";
            }
            else
            {
                reading = range == 0 ? "flat fixed point" : "non-uniform fixed point";
            }

            string cycle = string.Join("/", cycleSurfaces);
            string negative = firstNegative ?? "none";
            return $"{policy.Name,-9} | {length,6} | {competence,10} | {recurrence,-16} | " +
                $"{cycle,-36} | {initialMatter}->{finalMatter,-3} | {negative,-22} | {reading} (range {range})";
        }

        private static Parcel Build(DivisorPolicy divisor, int length, long competence)
        {
            var places = new List<Place>();
            var cells = new Dictionary<Cell, long>();
            for (int i = 0; i < length; i++)
            {
                var place = new Place(i);
                places.Add(place);
                cells[new Cell(place, K4.Base)] = 0;
                cells[new Cell(place, K4.Rock)] = 8;
                cells[new Cell(place, K4.Sediment)] = 0;
                cells[new Cell(place, K4.Water)] = 0;
            }

            var initial = new SimulationState(Tick.Zero, cells);
            var relationList = new List<Relation>();
            for (int i = 0; i < places.Count - 1; i++)
            {
                relationList.Add(new Relation(places[i], places[i + 1]));
                relationList.Add(new Relation(places[i + 1], places[i]));
            }

            var relations = new RelationSet(initial, relationList.ToArray());
            var additive = new AdditiveResolver();
            var fixtures = new List<IFixture>
            {
                new SurfaceFlowFixture(places, divisor, additive),
                new SurfaceConversionFixture(places, divisor,
                    Worlds5.ConversionThreshold, additive, additive),
                new CompetenceTransportFixture(places, divisor,
                    competence, additive),
            };

            var crossings = new ExternalEventTrace(new Membrane(new[] { K4.Water }));
            foreach (long boundary in CrossingBoundaries)
            {
                crossings.Append(new ExternalEvent(new Tick(boundary),
                    new Cell(places[0], K4.Water), 8));
            }

            return new Parcel
            {
                Places = places,
                Initial = initial,
                Relations = relations,
                Set = new FixtureSet(fixtures.ToArray()),
                Crossings = crossings,
            };
        }

        private static long SurfaceRange(string signature)
        {
            string[] values = signature.Trim('[', ']').Split(',');
            long minimum = long.MaxValue;
            long maximum = long.MinValue;
            foreach (string value in values)
            {
                long parsed = long.Parse(value);
                minimum = Math.Min(minimum, parsed);
                maximum = Math.Max(maximum, parsed);
            }

            return maximum - minimum;
        }

        private static int ChangedPlaces(string signature, long initial)
        {
            int count = 0;
            foreach (string value in signature.Trim('[', ']').Split(','))
            {
                if (long.Parse(value) != initial)
                {
                    count++;
                }
            }

            return count;
        }

        private static string FindNegative(SimulationState state,
            IReadOnlyList<Place> places, int tick)
        {
            var kinds = new[]
            {
                new KeyValuePair<string, Kind>("Base", K4.Base),
                new KeyValuePair<string, Kind>("Rock", K4.Rock),
                new KeyValuePair<string, Kind>("Sediment", K4.Sediment),
                new KeyValuePair<string, Kind>("Water", K4.Water),
            };

            foreach (Place place in places)
            {
                foreach (KeyValuePair<string, Kind> kind in kinds)
                {
                    long value = state.ValueAt(new Cell(place, kind.Value));
                    if (value < 0)
                    {
                        return $"t{tick}:p{place.Value}:{kind.Key}={value}";
                    }
                }
            }

            return null;
        }
    }
}
