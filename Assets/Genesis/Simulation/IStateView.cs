namespace Genesis.Simulation
{
    /// <summary>
    /// A read-only, scoped view of the simulation state given to a transition. It exposes only the
    /// addresses the transition declared it may read; requesting anything else fails
    /// (<see cref="ReadOutOfScopeException"/>). A transition never receives the full state — this is
    /// what makes read dependencies explicit and bounded.
    /// </summary>
    public interface IStateView
    {
        /// <summary>Reads a declared counter. Throws if the address is outside the declared scope.</summary>
        long Read(CounterAddress address);
    }
}
