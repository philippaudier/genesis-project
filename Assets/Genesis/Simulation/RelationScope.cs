using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The set of origin addresses whose <em>outgoing</em> relations a transition declares it may
    /// observe (Genesis-010). Declaring an origin grants, strictly one hop: visibility of the
    /// origin's outgoing relations, and read access to their target addresses' snapshot values.
    /// Discovered targets do not become origins — the grant is non-transitive.
    ///
    /// Parallel to <see cref="ReadScope"/>, deliberately not merged with it: one declares data, the
    /// other declares structure. Neither is a query language.
    /// </summary>
    public sealed class RelationScope
    {
        private readonly HashSet<CounterAddress> _origins;

        /// <summary>A scope that observes no relations.</summary>
        public static readonly RelationScope Empty = new RelationScope();

        public RelationScope(params CounterAddress[] origins)
        {
            _origins = new HashSet<CounterAddress>(origins);
        }

        /// <summary>The origin addresses whose outgoing relations may be observed.</summary>
        public IReadOnlyCollection<CounterAddress> Origins => _origins;

        /// <summary>Whether this scope permits observing the outgoing relations of <paramref name="origin"/>.</summary>
        public bool Includes(CounterAddress origin)
        {
            return _origins.Contains(origin);
        }
    }
}
