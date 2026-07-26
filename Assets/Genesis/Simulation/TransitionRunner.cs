using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Applies a set of transitions to a state for a single tick, under snapshot semantics (ADR-0001),
    /// and resolves conflicts per address. For each transition the runner materialises the declared
    /// contract — a <see cref="RelationalStateView"/> built from the immutable start-of-tick snapshot,
    /// the external immutable <see cref="RelationSet"/>, and the transition's two scopes. The runner
    /// answers no domain question and performs no traversal beyond that one-hop projection.
    ///
    /// Contributions are grouped by target address and committed in ascending address order:
    /// one contribution applies directly; several invoke that address's resolver exactly once; a
    /// conflict with no resolver is rejected. The runner holds only immutable resolver configuration —
    /// no mutable simulation state (invariant 9).
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

        /// <summary>Applies <paramref name="transitions"/> with no relations declared in the world.</summary>
        public SimulationState Apply(SimulationState snapshot, IReadOnlyList<ITransition> transitions)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            return Apply(snapshot, new RelationSet(snapshot), transitions);
        }

        /// <summary>
        /// Produces the next state by collecting every transition's contributions — each read through
        /// its declared relational view of the snapshot — grouping them by target address, and
        /// committing each address's result.
        /// </summary>
        public SimulationState Apply(
            SimulationState snapshot,
            RelationSet relations,
            IReadOnlyList<ITransition> transitions)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (relations == null)
            {
                throw new ArgumentNullException(nameof(relations));
            }

            if (transitions == null)
            {
                throw new ArgumentNullException(nameof(transitions));
            }

            // 1. Collect every contribution. Each transition receives only the view materialising its
            //    declared contract: direct reads, declared origins' outgoing relations (canonical
            //    order), and one-hop discovered targets.
            var amountsByAddress = new Dictionary<CounterAddress, List<long>>();
            for (int i = 0; i < transitions.Count; i++)
            {
                ITransition transition = transitions[i];
                IRelationalStateView view = new RelationalStateView(
                    snapshot, relations, transition.ReadScope, transition.RelationScope);
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
