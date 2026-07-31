using System;
using System.Collections.Generic;
using Genesis.Simulation;
using Genesis.Lab.S1_004;
using Genesis.Lab.S1_005;

namespace Genesis.Lab.S1_006
{
    public static class Instrument6
    {
        public sealed class FixedWitness
        {
            public int Boundary { get; }
            public int QuietSuffix { get; }

            public FixedWitness(int boundary, int quietSuffix)
            {
                Boundary = boundary;
                QuietSuffix = quietSuffix;
            }
        }

        /// <summary>
        /// A fixed point is a complete-state equality across a silent transition. The scan begins
        /// after the final forcing boundary and ends at boundary 127, exactly as sealed.
        /// </summary>
        public static FixedWitness FirstFixed(IReadOnlyList<SimulationState> states,
            IReadOnlyList<Place> places, IReadOnlyList<int> contributionCounts,
            IReadOnlyList<int> crossingCounts, int firstBoundary = 12, int lastBoundary = 127)
        {
            int limit = Math.Min(lastBoundary, states.Count - 2);
            for (int boundary = firstBoundary; boundary <= limit; boundary++)
            {
                if (contributionCounts[boundary] == 0 && crossingCounts[boundary] == 0 &&
                    StateInstrument.CompleteSignature(states[boundary], places) ==
                    StateInstrument.CompleteSignature(states[boundary + 1], places))
                {
                    int suffix = 1;
                    for (int next = boundary + 1; next < states.Count - 1; next++)
                    {
                        if (contributionCounts[next] != 0 || crossingCounts[next] != 0 ||
                            StateInstrument.CompleteSignature(states[next], places) !=
                            StateInstrument.CompleteSignature(states[next + 1], places))
                            break;
                        suffix++;
                    }
                    return new FixedWitness(boundary, suffix);
                }
            }
            return null;
        }

        public static bool AllocationSensitive(long holding, params long[] eligibleProspective)
        {
            int eligible = 0;
            long demand = 0;
            foreach (long amount in eligibleProspective)
            {
                if (amount > 0)
                {
                    eligible++;
                    demand += amount;
                }
            }
            return eligible > 1 && holding < demand;
        }

        public static bool CrossingsMatch(IReadOnlyList<ExternalEvent> events,
            IReadOnlyList<long> boundaries, Cell target, long amount)
        {
            if (events.Count != boundaries.Count) return false;
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].Boundary != new Tick(boundaries[i]) ||
                    events[i].Target != target || events[i].Amount != amount) return false;
            }
            return true;
        }

        /// <summary>Mechanical A–G precedence sealed for S1-006.</summary>
        public static string Classify(bool valid, bool strictPair, bool derivedFirstDifference,
            bool bothFixed, bool surfacesEqual, bool completeStatesEqual)
        {
            if (!valid) return "G";
            if (!strictPair) return "F";
            if (!derivedFirstDifference) return "E";
            if (!bothFixed) return "D";
            if (surfacesEqual && !completeStatesEqual) return "B";
            if (completeStatesEqual) return "C";
            return "A";
        }
    }
}
