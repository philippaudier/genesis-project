using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Applies a set of transitions to a state for a single tick, under snapshot semantics
    /// (ADR-0001), and resolves conflicts per cell (RFC-0003 D1) with resolvers attached to kinds
    /// (D2). For each transition the runner materialises the declared contract — a
    /// <see cref="RelationalStateView"/> built from the immutable snapshot, the external
    /// <see cref="RelationSet"/>, and the transition's two scopes — then groups contributions by
    /// target cell and commits each in canonical cell order: one contribution applies directly;
    /// several invoke the cell's kind's resolver exactly once; a conflict on a kind with no resolver
    /// is rejected. The runner holds only immutable resolver configuration — no mutable simulation
    /// state (invariant 9).
    /// </summary>
    public sealed class TransitionRunner
    {
        private static readonly IReadOnlyDictionary<Kind, IConflictResolver> NoResolvers =
            new Dictionary<Kind, IConflictResolver>();

        private readonly IReadOnlyDictionary<Kind, IConflictResolver> _resolvers;

        /// <summary>Creates a runner with no resolvers — any conflict is rejected.</summary>
        public TransitionRunner() : this(NoResolvers)
        {
        }

        public TransitionRunner(IReadOnlyDictionary<Kind, IConflictResolver> resolvers)
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

        private static readonly IReadOnlyList<Contribution> NoExternalContributions = new Contribution[0];

        /// <summary>
        /// Produces the next state by collecting every transition's contributions — each read through
        /// its declared relational view of the snapshot — grouping them by target cell, and
        /// committing each cell's result.
        /// </summary>
        public SimulationState Apply(
            SimulationState snapshot,
            RelationSet relations,
            IReadOnlyList<ITransition> transitions)
        {
            return Apply(snapshot, relations, transitions, NoExternalContributions);
        }

        /// <summary>
        /// As above, with crossings of the membrane merged into the same pool (RFC-L001; ADR-0005):
        /// external contributions enter the identical grouping and the identical per-kind resolution
        /// as law contributions — no second write path, no privilege. They are seeded first, because
        /// within a tick the crossing precedes the laws' action; resolvers are commutative by policy
        /// (DN-001), so this ordering carries no semantic weight.
        /// </summary>
        public SimulationState Apply(
            SimulationState snapshot,
            RelationSet relations,
            IReadOnlyList<ITransition> transitions,
            IReadOnlyList<Contribution> externalContributions)
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

            if (externalContributions == null)
            {
                throw new ArgumentNullException(nameof(externalContributions));
            }

            // 1. Collect every contribution. External crossings first (they preceded the tick), then
            //    each transition through the view materialising its declared contract.
            var amountsByCell = new Dictionary<Cell, List<long>>();
            for (int i = 0; i < externalContributions.Count; i++)
            {
                Contribution crossing = externalContributions[i];
                if (!amountsByCell.TryGetValue(crossing.Target, out List<long> crossingAmounts))
                {
                    crossingAmounts = new List<long>();
                    amountsByCell[crossing.Target] = crossingAmounts;
                }

                crossingAmounts.Add(crossing.Amount);
            }

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
                    if (!amountsByCell.TryGetValue(contribution.Target, out List<long> amounts))
                    {
                        amounts = new List<long>();
                        amountsByCell[contribution.Target] = amounts;
                    }

                    amounts.Add(contribution.Amount);
                }
            }

            // 2. Commit each contributed cell, in canonical cell order (place, then kind), so the
            //    runner's behaviour never depends on dictionary enumeration. The committed values are
            //    order-independent regardless: each derives from the snapshot's value plus its own
            //    resolved delta, and cells are disjoint.
            var contributedCells = new List<Cell>(amountsByCell.Keys);
            contributedCells.Sort();

            SimulationState next = snapshot;
            for (int i = 0; i < contributedCells.Count; i++)
            {
                Cell cell = contributedCells[i];
                List<long> amounts = amountsByCell[cell];

                long committedDelta;
                if (amounts.Count == 1)
                {
                    committedDelta = amounts[0];
                }
                else if (_resolvers.TryGetValue(cell.Kind, out IConflictResolver resolver))
                {
                    committedDelta = resolver.Resolve(amounts); // invoked exactly once per conflicting cell
                }
                else
                {
                    throw new UnresolvedConflictException(cell);
                }

                next = next.WithValue(cell, snapshot.ValueAt(cell) + committedDelta);
            }

            return next;
        }
    }
}
