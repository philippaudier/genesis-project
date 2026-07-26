using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// Thrown when two or more contributions target the same cell and the cell's kind has no
    /// resolver. Rejection is the default conflict policy (DN-001): without an explicit, commutative
    /// resolver for the kind, a conflict is refused rather than resolved by any order-dependent rule.
    /// </summary>
    public sealed class UnresolvedConflictException : Exception
    {
        /// <summary>The cell that received conflicting contributions with no resolver for its kind.</summary>
        public Cell Cell { get; }

        public UnresolvedConflictException(Cell cell)
            : base($"Conflicting contributions to {cell} with no resolver defined for {cell.Kind}.")
        {
            Cell = cell;
        }
    }
}
