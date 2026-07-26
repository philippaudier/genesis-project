using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// Thrown when a transition asks for the outgoing relations of a place outside its declared
    /// <see cref="RelationScope"/>. Undeclared origins expose nothing — including places merely
    /// <em>discovered</em> through another origin's relations, since the one-hop grant is
    /// non-transitive.
    /// </summary>
    public sealed class RelationOutOfScopeException : Exception
    {
        /// <summary>The place whose relations were requested outside the declared scope.</summary>
        public Place Origin { get; }

        public RelationOutOfScopeException(Place origin)
            : base($"Transition observed outgoing relations of {origin}, which is outside its declared relation scope.")
        {
            Origin = origin;
        }
    }
}
