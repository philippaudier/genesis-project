using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// A transition's declared relational observation (RFC-0003 D3/D4): the origin <em>places</em>
    /// whose outgoing relations it may observe, and the <em>kinds</em> it may read at the places
    /// those relations discover. The relation discovers places only; kind visibility is granted
    /// entirely by the observing transition. The grant is strictly one hop and non-transitive.
    /// </summary>
    public sealed class RelationScope
    {
        private readonly HashSet<Place> _origins;
        private readonly HashSet<Kind> _targetKinds;

        /// <summary>A scope that observes no relations and grants no discovered reads.</summary>
        public static readonly RelationScope Empty = new RelationScope(new Place[0], new Kind[0]);

        public RelationScope(IReadOnlyCollection<Place> origins, IReadOnlyCollection<Kind> targetKinds)
        {
            _origins = new HashSet<Place>(origins);
            _targetKinds = new HashSet<Kind>(targetKinds);
        }

        /// <summary>The origin places whose outgoing relations may be observed.</summary>
        public IReadOnlyCollection<Place> Origins => _origins;

        /// <summary>The kinds readable at discovered target places.</summary>
        public IReadOnlyCollection<Kind> TargetKinds => _targetKinds;

        /// <summary>Whether this scope permits observing the outgoing relations of <paramref name="origin"/>.</summary>
        public bool IncludesOrigin(Place origin)
        {
            return _origins.Contains(origin);
        }
    }
}
