using System;
using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// The per-edge flux counter — the instrument S1-001's sealed spec named and its World
    /// Corrections recorded as missing; completed here under S1-002's implementation
    /// authorisation. It is a reader: a pure function of the record plus a declared convention
    /// (the divisor policy). It computes, for a state, the transfer each edge carries during the
    /// tick that transforms that state — and its consistency check proves it against the record:
    /// for every cell, next = previous + crossings + inflow − outflow, or the instrument (or the
    /// laboratory's understanding) is wrong. Calibrated only on toy worlds that belong to no
    /// campaign; it never sees a sealed parcel before execution.
    /// </summary>
    public static class FluxCounter
    {
        public readonly struct EdgeFlux
        {
            public Place Source { get; }
            public Place Target { get; }
            public long Amount { get; }

            public EdgeFlux(Place source, Place target, long amount)
            {
                Source = source;
                Target = target;
                Amount = amount;
            }

            public override string ToString() => $"{Source.Value}->{Target.Value}:{Amount}";
        }

        /// <summary>
        /// The transfers each edge carries during the tick transforming <paramref name="state"/>,
        /// under the declared convention: potential = ground + moved; an edge carries
        /// floor(diff / divisor(outdegree of source)) of the moved kind when diff &gt; 0.
        /// </summary>
        public static IReadOnlyList<EdgeFlux> FluxAt(SimulationState state, RelationSet relations,
            IReadOnlyList<Place> places, Kind ground, Kind moved, DivisorPolicy divisor)
        {
            var flux = new List<EdgeFlux>();
            foreach (Place place in places)
            {
                IReadOnlyList<Relation> outgoing = relations.OutgoingFrom(place);
                long here = state.ValueAt(new Cell(place, ground)) + state.ValueAt(new Cell(place, moved));
                long div = divisor(outgoing.Count);
                foreach (Relation relation in outgoing)
                {
                    long there = state.ValueAt(new Cell(relation.Target, ground)) + state.ValueAt(new Cell(relation.Target, moved));
                    long diff = here - there;
                    if (diff <= 0)
                    {
                        continue;
                    }

                    long transfer = diff / div;
                    if (transfer > 0)
                    {
                        flux.Add(new EdgeFlux(place, relation.Target, transfer));
                    }
                }
            }

            return flux;
        }

        /// <summary>The first tick at which <paramref name="edge"/> carries non-zero flux, or −1.</summary>
        public static long FirstTransferTick(IReadOnlyList<SimulationState> states, RelationSet relations,
            IReadOnlyList<Place> places, Kind ground, Kind moved, DivisorPolicy divisor, Relation edge)
        {
            for (int t = 0; t + 1 < states.Count; t++)
            {
                foreach (EdgeFlux flux in FluxAt(states[t], relations, places, ground, moved, divisor))
                {
                    if (flux.Source == edge.Source && flux.Target == edge.Target)
                    {
                        return t;
                    }
                }
            }

            return -1;
        }

        public sealed class Mismatch
        {
            public long Tick { get; }
            public Place Place { get; }
            public long Expected { get; }
            public long Found { get; }

            public Mismatch(long tick, Place place, long expected, long found)
            {
                Tick = tick;
                Place = place;
                Expected = expected;
                Found = found;
            }

            public override string ToString() =>
                $"tick {Tick}, place({Place.Value}): expected {Expected}, found {Found}";
        }

        /// <summary>
        /// C5's trial: for every tick and every place, the moved kind's next value must equal
        /// previous + crossings + counted inflow − counted outflow. Any mismatch refutes the
        /// counter's convention against the record.
        /// </summary>
        public static IReadOnlyList<Mismatch> ConsistencyCheck(IReadOnlyList<SimulationState> states,
            RelationSet relations, IReadOnlyList<Place> places, ExternalEventTrace trace,
            Kind ground, Kind moved, DivisorPolicy divisor)
        {
            var mismatches = new List<Mismatch>();
            for (int t = 0; t + 1 < states.Count; t++)
            {
                var delta = new Dictionary<Place, long>();
                foreach (Place place in places)
                {
                    delta[place] = 0;
                }

                foreach (EdgeFlux flux in FluxAt(states[t], relations, places, ground, moved, divisor))
                {
                    delta[flux.Source] -= flux.Amount;
                    delta[flux.Target] += flux.Amount;
                }

                foreach (ExternalEvent crossing in trace.Events)
                {
                    if (crossing.Boundary == states[t].CurrentTick && crossing.Target.Kind == moved)
                    {
                        delta[crossing.Target.Place] += crossing.Amount;
                    }
                }

                foreach (Place place in places)
                {
                    long expected = states[t].ValueAt(new Cell(place, moved)) + delta[place];
                    long found = states[t + 1].ValueAt(new Cell(place, moved));
                    if (expected != found)
                    {
                        mismatches.Add(new Mismatch(t, place, expected, found));
                    }
                }
            }

            return mismatches;
        }
    }
}
