using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The read-only, doubly-scoped view of the simulation given to a transition. It exposes exactly
    /// what the transition declared and nothing else:
    /// <list type="bullet">
    ///   <item>values of the cells in its <see cref="ReadScope"/>;</item>
    ///   <item>outgoing relations of origin places in its <see cref="RelationScope"/>, in canonical
    ///   order;</item>
    ///   <item>values of the granted kinds at the places those relations discover — one hop,
    ///   non-transitive.</item>
    /// </list>
    /// All values come from the same immutable start-of-tick snapshot, whether read directly or
    /// through a relation. A transition never receives the full state or the full relation set.
    /// </summary>
    public interface IRelationalStateView
    {
        /// <summary>
        /// Reads a cell that is either directly declared or granted through a declared origin's
        /// outgoing relations. Throws <see cref="ReadOutOfScopeException"/> otherwise.
        /// </summary>
        long Read(Cell cell);

        /// <summary>
        /// The outgoing relations of a declared origin place, in canonical order (target ascending).
        /// Throws <see cref="RelationOutOfScopeException"/> if the origin was not declared —
        /// including places that were merely discovered as targets.
        /// </summary>
        IReadOnlyList<Relation> OutgoingRelations(Place origin);
    }
}
