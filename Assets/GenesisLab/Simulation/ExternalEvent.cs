namespace Genesis.Simulation
{
    /// <summary>
    /// One crossing of the observable membrane (ADR-0005; RFC-L001): a value contributed to an
    /// external cell at a declared tick boundary. An event with <see cref="Boundary"/> t is applied
    /// during the tick that transforms the state at t, and is therefore first visible in the state
    /// at t+1 — never mid-tick, never earlier, never later. An event is a fact of the boundary, not
    /// an interpretation: what it <em>means</em> is decided by whichever laws read its cell.
    /// </summary>
    public readonly struct ExternalEvent
    {
        /// <summary>The tick boundary at which this event crosses. First visible at Boundary + 1.</summary>
        public Tick Boundary { get; }

        /// <summary>The external cell this event writes to.</summary>
        public Cell Target { get; }

        /// <summary>The amount contributed to the target cell.</summary>
        public long Amount { get; }

        public ExternalEvent(Tick boundary, Cell target, long amount)
        {
            Boundary = boundary;
            Target = target;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"ExternalEvent(at {Boundary}, {Target}, {Amount})";
        }
    }
}
