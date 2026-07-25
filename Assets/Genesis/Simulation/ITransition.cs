using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// A transition: the atomic unit of transformation (ADR-0001). It declares, up front, the state
    /// it may read (<see cref="ReadScope"/>), and reads only that — through the scoped
    /// <see cref="IStateView"/> it is handed. It returns the identifiable contributions it wishes to
    /// make to the next state.
    ///
    /// A transition receives only a scoped view of the start-of-tick snapshot — never the full state,
    /// never the accumulating next state, never another transition's contributions. Its reads are
    /// therefore explicit and bounded, and its snapshot isolation is structural. A transition knows
    /// only <see cref="IStateView"/> / <see cref="Contribution"/> and never calls, nor knows of,
    /// another transition.
    /// </summary>
    public interface ITransition
    {
        /// <summary>The state this transition declares it may read.</summary>
        ReadScope ReadScope { get; }

        /// <summary>Reads its declared state from <paramref name="view"/> and returns its contributions.</summary>
        IReadOnlyList<Contribution> Apply(IStateView view);
    }
}
