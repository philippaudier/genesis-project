using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The read-only, doubly-scoped view of the simulation given to a transition (Genesis-010). It
    /// exposes exactly what the transition declared and nothing else:
    /// <list type="bullet">
    ///   <item>values of addresses in its <see cref="ReadScope"/>;</item>
    ///   <item>outgoing relations of origins in its <see cref="RelationScope"/>, in canonical order;</item>
    ///   <item>values of the addresses those relations discover — one hop, non-transitive.</item>
    /// </list>
    /// All values come from the same immutable start-of-tick snapshot, whether read directly or
    /// through a relation. A transition never receives the full state or the full relation set.
    /// </summary>
    public interface IRelationalStateView
    {
        /// <summary>
        /// Reads an address that is either directly declared or discovered through a declared
        /// origin's outgoing relations. Throws <see cref="ReadOutOfScopeException"/> otherwise.
        /// </summary>
        long Read(CounterAddress address);

        /// <summary>
        /// The outgoing relations of a declared origin, in canonical order (target ascending).
        /// Throws <see cref="RelationOutOfScopeException"/> if the origin was not declared —
        /// including origins that were merely discovered as targets.
        /// </summary>
        IReadOnlyList<Relation> OutgoingRelations(CounterAddress origin);
    }
}
