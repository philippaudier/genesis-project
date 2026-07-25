using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The explicit, authoritative, immutable state of a simulation run (invariant 6; ADR-0001). It
    /// holds its position in logical time and a fixed set of homogeneous counter locations, each with
    /// a stable <see cref="CounterAddress"/> (Genesis-008).
    ///
    /// Equality depends on the set of address/value pairs and the tick — never on insertion or
    /// enumeration order: two states built from the same pairs in different orders are strictly
    /// equal, and the hash is a commutative aggregation over pairs for the same reason. The internal
    /// dictionary is a lookup structure only; its ordering is never part of world semantics.
    /// </summary>
    public sealed class SimulationState : IEquatable<SimulationState>
    {
        private readonly Dictionary<CounterAddress, long> _counters;

        /// <summary>The tick this state currently sits at.</summary>
        public Tick CurrentTick { get; }

        /// <summary>The addresses this state defines. Fixed at construction — no lifecycle yet.</summary>
        public IReadOnlyCollection<CounterAddress> Addresses => _counters.Keys;

        public SimulationState(Tick currentTick, IReadOnlyDictionary<CounterAddress, long> counters)
        {
            if (counters == null)
            {
                throw new ArgumentNullException(nameof(counters));
            }

            CurrentTick = currentTick;
            _counters = new Dictionary<CounterAddress, long>(counters.Count);
            foreach (KeyValuePair<CounterAddress, long> pair in counters)
            {
                _counters[pair.Key] = pair.Value;
            }
        }

        private SimulationState(Tick currentTick, Dictionary<CounterAddress, long> owned)
        {
            CurrentTick = currentTick;
            _counters = owned;
        }

        /// <summary>The value at <paramref name="address"/>. The address must exist in this state.</summary>
        public long CounterOf(CounterAddress address)
        {
            if (_counters.TryGetValue(address, out long value))
            {
                return value;
            }

            throw new ArgumentException($"No counter exists at {address}.", nameof(address));
        }

        /// <summary>Whether this state defines a counter at <paramref name="address"/>.</summary>
        public bool Defines(CounterAddress address)
        {
            return _counters.ContainsKey(address);
        }

        /// <summary>
        /// Produces a copy with the counter at <paramref name="address"/> set to
        /// <paramref name="value"/>. The address must already exist — Genesis-008 has no address
        /// lifecycle; writing cannot create locations.
        /// </summary>
        public SimulationState WithCounter(CounterAddress address, long value)
        {
            if (!_counters.ContainsKey(address))
            {
                throw new ArgumentException($"No counter exists at {address}.", nameof(address));
            }

            var copy = new Dictionary<CounterAddress, long>(_counters);
            copy[address] = value;
            return new SimulationState(CurrentTick, copy);
        }

        /// <summary>
        /// Produces a copy advanced by exactly one tick, counters unchanged. Deliberately
        /// <c>internal</c>: only the simulation's own runner advances logical time, and it does so by
        /// producing the next state — never by mutating this one.
        /// </summary>
        internal SimulationState WithTickAdvanced()
        {
            return new SimulationState(CurrentTick.Next(), _counters);
        }

        public bool Equals(SimulationState other)
        {
            if (other is null)
            {
                return false;
            }

            if (CurrentTick != other.CurrentTick || _counters.Count != other._counters.Count)
            {
                return false;
            }

            // Pairwise comparison over the set of address/value pairs — order plays no part.
            foreach (KeyValuePair<CounterAddress, long> pair in _counters)
            {
                if (!other._counters.TryGetValue(pair.Key, out long otherValue) || otherValue != pair.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is SimulationState other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Commutative aggregation (sum) over pair hashes, so the hash — like equality — cannot
            // depend on enumeration order.
            long aggregate = 0;
            foreach (KeyValuePair<CounterAddress, long> pair in _counters)
            {
                aggregate += (pair.Key.GetHashCode() * 397L) ^ pair.Value.GetHashCode();
            }

            return (CurrentTick.GetHashCode() * 397) ^ aggregate.GetHashCode();
        }

        public override string ToString()
        {
            return $"SimulationState(tick={CurrentTick.Value}, counters={_counters.Count})";
        }
    }
}
