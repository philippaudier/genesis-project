using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// Thrown when two or more contributions target the same address and no resolver is defined for
    /// it. Rejection is the default conflict policy (DN-001): without an explicit, commutative
    /// resolver, a conflict is refused rather than resolved by any order-dependent rule.
    /// </summary>
    public sealed class UnresolvedConflictException : Exception
    {
        /// <summary>The address that received conflicting contributions with no resolver.</summary>
        public CounterAddress Address { get; }

        public UnresolvedConflictException(CounterAddress address)
            : base($"Conflicting contributions to {address} with no resolver defined.")
        {
            Address = address;
        }
    }
}
