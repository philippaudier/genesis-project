using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The append-only record of every crossing of a membrane (RFC-L001; ADR-0005). The trace is
    /// part of the world's record: run = initial state + relations + laws + trace, and replaying the
    /// same four reproduces the same run exactly (invariant 7's test). The API is the enforcement of
    /// append-only-ness: events can be added and read — never removed, reordered, or rewritten. The
    /// trace records facts of the boundary, not interpretations; it must survive a rewrite of the
    /// laws (the event/command guard), which is what makes counterfactual replay well-posed.
    /// </summary>
    public sealed class ExternalEventTrace
    {
        private readonly Membrane _membrane;
        private readonly List<ExternalEvent> _events = new List<ExternalEvent>();

        public ExternalEventTrace(Membrane membrane)
        {
            if (membrane == null)
            {
                throw new ArgumentNullException(nameof(membrane));
            }

            _membrane = membrane;
        }

        /// <summary>Every crossing recorded so far, in append order.</summary>
        public IReadOnlyList<ExternalEvent> Events => _events;

        /// <summary>
        /// Records a crossing. The membrane is the gate: an event whose kind is not declared is
        /// refused here and never enters the record. Recording happens before application — the
        /// runner only ever applies what the trace already holds.
        /// </summary>
        public void Append(ExternalEvent crossing)
        {
            if (!_membrane.Declares(crossing.Target.Kind))
            {
                throw new UndeclaredExternalKindException(crossing.Target);
            }

            _events.Add(crossing);
        }

        /// <summary>
        /// The contributions this trace injects at <paramref name="boundary"/>, in append order.
        /// Internal: only the simulation's own runner applies crossings, and only at boundaries.
        /// </summary>
        internal IReadOnlyList<Contribution> ContributionsAt(Tick boundary)
        {
            var contributions = new List<Contribution>();
            for (int i = 0; i < _events.Count; i++)
            {
                if (_events[i].Boundary == boundary)
                {
                    contributions.Add(new Contribution(_events[i].Target, _events[i].Amount));
                }
            }

            return contributions;
        }
    }
}
