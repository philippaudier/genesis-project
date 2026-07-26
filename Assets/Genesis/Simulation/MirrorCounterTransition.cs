using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Declares a direct read scope of one source cell, reads it from its view, and contributes that
    /// value to a target cell. It observes no relations. A witness that a transition reads only what
    /// it declares — and, because the view is built from the start-of-tick snapshot, that it sees the
    /// snapshot's source value even when another transition writes the source in the same tick.
    /// </summary>
    public sealed class MirrorCounterTransition : ITransition
    {
        private readonly Cell _from;
        private readonly Cell _to;
        private readonly ReadScope _scope;

        public MirrorCounterTransition(Cell from, Cell to)
        {
            _from = from;
            _to = to;
            _scope = new ReadScope(from);
        }

        public ReadScope ReadScope => _scope;

        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return new[] { new Contribution(_to, view.Read(_from)) };
        }
    }
}
