using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// Thrown when a transition asks for the outgoing relations of an origin outside its declared
    /// <see cref="RelationScope"/>. Like out-of-scope reads, this is a real restriction, not a
    /// convention: undeclared origins expose nothing — including origins merely <em>discovered</em>
    /// through another origin's relations, since the one-hop grant is non-transitive.
    /// </summary>
    public sealed class RelationOutOfScopeException : Exception
    {
        /// <summary>The origin whose relations were requested outside the declared scope.</summary>
        public CounterAddress Origin { get; }

        public RelationOutOfScopeException(CounterAddress origin)
            : base($"Transition observed outgoing relations of {origin}, which is outside its declared relation scope.")
        {
            Origin = origin;
        }
    }
}
