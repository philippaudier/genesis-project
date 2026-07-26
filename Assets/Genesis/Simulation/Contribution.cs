namespace Genesis.Simulation
{
    /// <summary>
    /// An identifiable write proposed by a transition: an <see cref="Amount"/> contributed to a
    /// specific <see cref="Target"/> cell (RFC-0003 D1). Contributions — not opaque states — are what
    /// transitions produce, so the runner can see which cell each transition wrote and detect when
    /// two target the same one. Same place, different kinds: no conflict.
    /// </summary>
    public readonly struct Contribution
    {
        /// <summary>The cell this contribution targets.</summary>
        public Cell Target { get; }

        /// <summary>The amount contributed to the target.</summary>
        public long Amount { get; }

        public Contribution(Cell target, long amount)
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
