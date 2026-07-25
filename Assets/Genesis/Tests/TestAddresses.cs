using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Shared witness fixtures: three homogeneous counter locations under stable, explicit addresses.
    /// Address allocation is deliberately manual — Genesis-008 has no lifecycle or automatic identity.
    /// </summary>
    internal static class TestAddresses
    {
        public static readonly CounterAddress A = new CounterAddress(10);
        public static readonly CounterAddress B = new CounterAddress(20);
        public static readonly CounterAddress C = new CounterAddress(30);

        /// <summary>Tick zero; counters A, B, C all zero.</summary>
        public static SimulationState InitialState()
        {
            return StateWith(0, 0, 0);
        }

        /// <summary>Tick zero; counters A, B, C at the given values.</summary>
        public static SimulationState StateWith(long a, long b, long c)
        {
            return new SimulationState(Tick.Zero, new Dictionary<CounterAddress, long>
            {
                { A, a },
                { B, b },
                { C, c }
            });
        }
    }
}
