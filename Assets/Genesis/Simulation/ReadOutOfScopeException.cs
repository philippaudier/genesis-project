using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// Thrown when a transition reads an address outside its declared <see cref="ReadScope"/>. Read
    /// scopes are a real restriction, not a convention: the value is not present in the view the
    /// transition received, so reading it fails rather than silently returning data.
    /// </summary>
    public sealed class ReadOutOfScopeException : Exception
    {
        /// <summary>The address that was read outside the declared scope.</summary>
        public CounterAddress Address { get; }

        public ReadOutOfScopeException(CounterAddress address)
            : base($"Transition read {address}, which is outside its declared read scope.")
        {
            Address = address;
        }
    }
}
