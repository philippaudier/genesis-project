using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// A view containing only the counters a transition declared in its <see cref="ReadScope"/>. The
    /// out-of-scope values are not merely hidden — they are absent from the view — so a transition
    /// structurally cannot observe undeclared state. Built from the immutable snapshot once per
    /// transition per tick.
    /// </summary>
    public sealed class ScopedStateView : IStateView
    {
        private readonly Dictionary<CounterAddress, long> _values;

        public ScopedStateView(SimulationState snapshot, ReadScope scope)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            _values = new Dictionary<CounterAddress, long>();
            foreach (CounterAddress address in scope.Addresses)
            {
                _values[address] = snapshot.CounterOf(address);
            }
        }

        public long Read(CounterAddress address)
        {
            if (_values.TryGetValue(address, out long value))
            {
                return value;
            }

            throw new ReadOutOfScopeException(address);
        }
    }
}
