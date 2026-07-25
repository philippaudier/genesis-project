using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Applies a set of transitions to a state for a single tick, under snapshot semantics (ADR-0001),
    /// and resolves conflicts per address. Every transition reads a scoped view of the same
    /// start-of-tick snapshot and produces identifiable contributions; the runner groups them by
    /// target address and commits each:
    /// <list type="bullet">
    ///   <item>no contribution — the address carries over from the snapshot;</item>
    ///   <item>one contribution — it applies directly (not a conflict);</item>
    ///   <item>several contributions — the address's resolver is invoked exactly once; if none is
    ///   defined, the conflict is rejected (<see cref="UnresolvedConflictException"/>).</item>
    /// </list>
    /// Contributed addresses are committed in ascending address order. Because addresses are disjoint
    /// and each commit derives from the snapshot, this order cannot influence the committed state —
    /// it only makes the runner's behaviour (including which unresolved conflict is reported first)
    /// fully deterministic. The runner holds only immutable resolver configuration — no mutable
    /// simulation state (invariant 9).
    /// </summary>
    public sealed class TransitionRunner
    {
        private static readonly IReadOnlyDictionary<CounterAddress, IConflictResolver> NoResolvers =
            new Dictionary<CounterAddress, IConflictResolver>();

        private readonly IReadOnlyDictionary<CounterAddress, IConflictResolver> _resolvers;

        /// <summary>Creates a runner with no resolvers — any conflict is rejected.</summary>
        public TransitionRunner() : this(NoResolvers)
        {
        }

        public TransitionRunner(IReadOnlyDictionary<CounterAddress, IConflictResolver> resolvers)
        {
            if (resolvers == null)
            {
                throw new ArgumentNullException(nameof(resolvers));
            }

            _resolvers = resolvers;
        }

        /// <summary>
        /// Produces the next state by collecting every transition's contributions (each read through
        /// its scoped view of the snapshot), grouping them by target address, and committing each
        /// address's result.
        /// </summary>
        public SimulationState Apply(SimulationState snapshot, IReadOnlyList<ITransition> transitions)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (transitions == null)
            {
                throw new ArgumentNullException(nameof(transitions));
            }

            // 1. Collect every contribution. Each transition receives only a scoped view of the
            //    immutable snapshot — it can read the addresses it declared, and nothing else.
            var amountsByAddress = new Dictionary<CounterAddress, List<long>>();
            for (int i = 0; i < transitions.Count; i++)
            {
                ITransition transition = transitions[i];
                IStateView view = new ScopedStateView(snapshot, transition.ReadScope);
                IReadOnlyList<Contribution> contributions = transition.Apply(view);
                if (contributions == null)
                {
                    continue;
                }

                for (int j = 0; j < contributions.Count; j++)
                {
                    Contribution contribution = contributions[j];
                    if (!amountsByAddress.TryGetValue(contribution.Target, out List<long> amounts))
                    {
                        amounts = new List<long>();
                        amountsByAddress[contribution.Target] = amounts;
                    }

                    amounts.Add(contribution.Amount);
                }
            }

            // 2. Commit each contributed address, in ascending address order so the runner's
            //    behaviour never depends on dictionary enumeration. The committed values themselves
            //    are order-independent regardless: each derives from the snapshot's value plus its
            //    own resolved delta, and addresses are disjoint.
            var contributedAddresses = new List<CounterAddress>(amountsByAddress.Keys);
            contributedAddresses.Sort();

            SimulationState next = snapshot;
            for (int i = 0; i < contributedAddresses.Count; i++)
            {
                CounterAddress address = contributedAddresses[i];
                List<long> amounts = amountsByAddress[address];

                long committedDelta;
                if (amounts.Count == 1)
                {
                    committedDelta = amounts[0];
                }
                else if (_resolvers.TryGetValue(address, out IConflictResolver resolver))
                {
                    committedDelta = resolver.Resolve(amounts); // invoked exactly once per conflicting address
                }
                else
                {
                    throw new UnresolvedConflictException(address);
                }

                next = next.WithCounter(address, snapshot.CounterOf(address) + committedDelta);
            }

            return next;
        }
    }
}
