namespace Genesis.Simulation
{
    /// <summary>
    /// An identifiable write proposed by a transition: an <see cref="Amount"/> contributed to the
    /// counter at a specific <see cref="Target"/> address. Contributions — not opaque full states —
    /// are what transitions produce, so the runner can see which location each transition wrote and
    /// detect when two target the same address.
    /// </summary>
    public readonly struct Contribution
    {
        /// <summary>The address of the counter this contribution targets.</summary>
        public CounterAddress Target { get; }

        /// <summary>The amount contributed to the target.</summary>
        public long Amount { get; }

        public Contribution(CounterAddress target, long amount)
        {
            Target = target;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"Contribution({Target}, {Amount})";
        }
    }
}
