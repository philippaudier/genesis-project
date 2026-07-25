using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// Drives a simulation forward by a number of ticks. Each tick it produces the next state from
    /// the current one — applying the transitions to the current snapshot (via a
    /// <see cref="TransitionRunner"/>) and then advancing logical time — and hands the new state on to
    /// the next tick. This is the deterministic simulation loop (ADR-0001). It owns and mutates
    /// nothing: it consumes a state and returns the resulting state.
    /// </summary>
    public sealed class TickRunner
    {
        private readonly TransitionRunner _transitionRunner;

        public TickRunner(TransitionRunner transitionRunner)
        {
            if (transitionRunner == null)
            {
                throw new ArgumentNullException(nameof(transitionRunner));
            }

            _transitionRunner = transitionRunner;
        }

        /// <summary>
        /// Runs <paramref name="count"/> ticks starting from <paramref name="initial"/>, applying
        /// <paramref name="transitions"/> each tick, and returns the resulting state. Running zero
        /// ticks returns the initial state unchanged. The initial state is never mutated; each tick
        /// produces a new state.
        /// </summary>
        public SimulationState Run(SimulationState initial, IReadOnlyList<ITransition> transitions, long count)
        {
            if (initial == null)
            {
                throw new ArgumentNullException(nameof(initial));
            }

            if (transitions == null)
            {
                throw new ArgumentNullException(nameof(transitions));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Cannot run a negative number of ticks.");
            }

            SimulationState state = initial;
            for (long i = 0; i < count; i++)
            {
                SimulationState transformed = _transitionRunner.Apply(state, transitions);
                state = transformed.WithTickAdvanced();
            }

            return state;
        }
    }
}
