using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Contributes a fixed amount to the counter at a chosen address. It reads nothing — its
    /// <see cref="ReadScope"/> is empty — so it is a clean witness that a transition may write without
    /// declaring any read. An increment is simply an amount of one.
    /// </summary>
    public sealed class AddToCounterTransition : ITransition
    {
        private readonly CounterAddress _target;
        private readonly long _amount;

        public AddToCounterTransition(CounterAddress target, long amount)
        {
            _target = target;
            _amount = amount;
        }

        public ReadScope ReadScope => ReadScope.Empty;

        public IReadOnlyList<Contribution> Apply(IStateView view)
        {
            return new[] { new Contribution(_target, _amount) };
        }
    }
}
