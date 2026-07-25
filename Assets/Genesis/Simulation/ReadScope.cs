using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The set of addresses a transition declares it may read. The runner gives the transition a view
    /// containing only these locations; anything else is simply not present to read. Deliberately a
    /// closed, explicit set of <see cref="CounterAddress"/> — not a query language, property paths, or
    /// string keys.
    /// </summary>
    public sealed class ReadScope
    {
        private readonly HashSet<CounterAddress> _addresses;

        /// <summary>A scope that permits reading nothing.</summary>
        public static readonly ReadScope Empty = new ReadScope();

        public ReadScope(params CounterAddress[] addresses)
        {
            _addresses = new HashSet<CounterAddress>(addresses);
        }

        /// <summary>The addresses this scope permits reading.</summary>
        public IReadOnlyCollection<CounterAddress> Addresses => _addresses;

        /// <summary>Whether this scope permits reading <paramref name="address"/>.</summary>
        public bool Includes(CounterAddress address)
        {
            return _addresses.Contains(address);
        }
    }
}
