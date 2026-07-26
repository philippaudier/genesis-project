using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// A transition: the atomic unit of transformation (ADR-0001). It declares, up front, both what
    /// it may read directly (<see cref="ReadScope"/>) and which origins' outgoing relations it may
    /// observe (<see cref="RelationScope"/>) — and receives a view materialising exactly that
    /// contract, nothing more. It returns the identifiable contributions it wishes to make to the
    /// next state.
    ///
    /// A transition never receives the full <see cref="SimulationState"/>, the full
    /// <see cref="RelationSet"/>, or another transition's contributions. All its reads — direct or
    /// relation-discovered — come from the same immutable start-of-tick snapshot, so snapshot
    /// isolation remains structural. A transition never calls, nor knows of, another transition.
    /// </summary>
    public interface ITransition
    {
        /// <summary>The addresses this transition declares it may read directly.</summary>
        ReadScope ReadScope { get; }

        /// <summary>The origins whose outgoing relations this transition declares it may observe.</summary>
        RelationScope RelationScope { get; }

        /// <summary>Reads its declared contract from <paramref name="view"/> and returns its contributions.</summary>
        IReadOnlyList<Contribution> Apply(IRelationalStateView view);
    }
}
