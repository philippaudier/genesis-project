using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Contributes a fixed amount to a chosen cell. It reads nothing and observes no relations —
    /// both scopes are empty — so it is a clean witness that a transition may write without declaring
    /// any observation. An increment is simply an amount of one.
    /// </summary>
    public sealed class AddToCounterTransition : ITransition
    {
        private readonly Cell _target;
        private readonly long _amount;

        public AddToCounterTransition(Cell target, long amount)
        {
            _target = target;
            _amount = amount;
        }

        public ReadScope ReadScope => ReadScope.Empty;

        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            return new[] { new Contribution(_target, _amount) };
        }
    }
}
