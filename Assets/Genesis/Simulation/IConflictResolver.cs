using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Resolves several conflicting contributions to one committed amount. It is invoked exactly once
    /// per conflicting target, with all of that target's contribution amounts.
    ///
    /// A resolver MUST be commutative — its result must not depend on the order of the amounts —
    /// because Genesis requires that reordering transitions never changes the committed state
    /// (DN-001). This contract is currently guaranteed by tests and implementation discipline, not by
    /// the type system.
    /// </summary>
    public interface IConflictResolver
    {
        /// <summary>Folds the conflicting <paramref name="amounts"/> into one committed amount.</summary>
        long Resolve(IReadOnlyList<long> amounts);
    }
}
