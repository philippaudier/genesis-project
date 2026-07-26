using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// Thrown when a transition reads a cell outside its declared contract — neither directly
    /// declared in its <see cref="ReadScope"/> nor granted through a declared origin's relations.
    /// The value is not present in the view the transition received, so reading it fails rather than
    /// silently returning data.
    /// </summary>
    public sealed class ReadOutOfScopeException : Exception
    {
        /// <summary>The cell that was read outside the declared contract.</summary>
        public Cell Cell { get; }

        public ReadOutOfScopeException(Cell cell)
            : base($"Transition read {cell}, which is outside its declared contract.")
        {
            Cell = cell;
        }
    }
}
