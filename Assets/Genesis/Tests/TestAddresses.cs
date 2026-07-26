using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// Shared witness fixtures: three places, one kind (the single-kind special case under which
    /// every pre-RFC-0003 proof must behave exactly as before), and the three cells they define.
    /// Identity allocation is deliberately manual — no lifecycle, no automatic identity.
    /// </summary>
    internal static class TestAddresses
    {
        public static readonly Place A = new Place(10);
        public static readonly Place B = new Place(20);
        public static readonly Place C = new Place(30);

        /// <summary>The single witness kind of the pre-heterogeneity proofs.</summary>
        public static readonly Kind K = new Kind(1);

        public static readonly Cell CellA = new Cell(A, K);
        public static readonly Cell CellB = new Cell(B, K);
        public static readonly Cell CellC = new Cell(C, K);

        /// <summary>Tick zero; cells A, B, C all zero.</summary>
        public static SimulationState InitialState()
        {
            return StateWith(0, 0, 0);
        }

        /// <summary>Tick zero; cells A, B, C at the given values.</summary>
        public static SimulationState StateWith(long a, long b, long c)
        {
            return new SimulationState(Tick.Zero, new Dictionary<Cell, long>
            {
                { CellA, a },
                { CellB, b },
                { CellC, c }
            });
        }
    }
}
