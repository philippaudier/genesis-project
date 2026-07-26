using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The set of cells a transition declares it may read directly (RFC-0003 D4: scopes declare at
    /// cell precision). The runner gives the transition a view containing only these cells; anything
    /// else is simply not present to read. A closed, explicit set — not a query language.
    /// </summary>
    public sealed class ReadScope
    {
        private readonly HashSet<Cell> _cells;

        /// <summary>A scope that permits reading nothing.</summary>
        public static readonly ReadScope Empty = new ReadScope();

        public ReadScope(params Cell[] cells)
        {
            _cells = new HashSet<Cell>(cells);
        }

        /// <summary>The cells this scope permits reading.</summary>
        public IReadOnlyCollection<Cell> Cells => _cells;

        /// <summary>Whether this scope permits reading <paramref name="cell"/>.</summary>
        public bool Includes(Cell cell)
        {
            return _cells.Contains(cell);
        }
    }
}
