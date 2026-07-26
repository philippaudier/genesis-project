using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The explicit, authoritative, immutable state of a simulation run (invariant 6; ADR-0001):
    /// its position in logical time and a fixed set of cells — (place, kind) pairs, each holding an
    /// integer value (RFC-0003). Equality depends on the set of (place, kind, value) triples and the
    /// tick — never on insertion or enumeration order. Place existence is derived: a place exists iff
    /// at least one cell is declared at it (D5). The internal dictionary is a lookup structure only;
    /// its layout is an implementation detail (D6) and its ordering is never part of world semantics.
    /// </summary>
    public sealed class SimulationState : IEquatable<SimulationState>
    {
        private readonly Dictionary<Cell, long> _cells;

        /// <summary>The tick this state currently sits at.</summary>
        public Tick CurrentTick { get; }

        /// <summary>The cells this state defines. Fixed at construction — no lifecycle yet.</summary>
        public IReadOnlyCollection<Cell> Cells => _cells.Keys;

        public SimulationState(Tick currentTick, IReadOnlyDictionary<Cell, long> cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            CurrentTick = currentTick;
            _cells = new Dictionary<Cell, long>(cells.Count);
            foreach (KeyValuePair<Cell, long> pair in cells)
            {
                _cells[pair.Key] = pair.Value;
            }
        }

        private SimulationState(Tick currentTick, Dictionary<Cell, long> owned)
        {
            CurrentTick = currentTick;
            _cells = owned;
        }

        /// <summary>The value at <paramref name="cell"/>. The cell must exist in this state.</summary>
        public long ValueAt(Cell cell)
        {
            if (_cells.TryGetValue(cell, out long value))
            {
                return value;
            }

            throw new ArgumentException($"No cell exists at {cell}.", nameof(cell));
        }

        /// <summary>Whether this state defines <paramref name="cell"/>.</summary>
        public bool Defines(Cell cell)
        {
            return _cells.ContainsKey(cell);
        }

        /// <summary>Whether any cell is declared at <paramref name="place"/> (derived existence, D5).</summary>
        public bool DefinesPlace(Place place)
        {
            foreach (Cell cell in _cells.Keys)
            {
                if (cell.Place == place)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Produces a copy with the value at <paramref name="cell"/> set to <paramref name="value"/>.
        /// The cell must already exist — there is no lifecycle; writing cannot create locations.
        /// </summary>
        public SimulationState WithValue(Cell cell, long value)
        {
            if (!_cells.ContainsKey(cell))
            {
                throw new ArgumentException($"No cell exists at {cell}.", nameof(cell));
            }

            var copy = new Dictionary<Cell, long>(_cells);
            copy[cell] = value;
            return new SimulationState(CurrentTick, copy);
        }

        /// <summary>
        /// Produces a copy advanced by exactly one tick, cells unchanged. Deliberately
        /// <c>internal</c>: only the simulation's own runner advances logical time, and it does so by
        /// producing the next state — never by mutating this one.
        /// </summary>
        internal SimulationState WithTickAdvanced()
        {
            return new SimulationState(CurrentTick.Next(), _cells);
        }

        public bool Equals(SimulationState other)
        {
            if (other is null)
            {
                return false;
            }

            if (CurrentTick != other.CurrentTick || _cells.Count != other._cells.Count)
            {
                return false;
            }

            foreach (KeyValuePair<Cell, long> pair in _cells)
            {
                if (!other._cells.TryGetValue(pair.Key, out long otherValue) || otherValue != pair.Value)
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
            // Commutative aggregation (sum) over triple hashes, so the hash — like equality — cannot
            // depend on enumeration order.
            long aggregate = 0;
            foreach (KeyValuePair<Cell, long> pair in _cells)
            {
                aggregate += (pair.Key.GetHashCode() * 397L) ^ pair.Value.GetHashCode();
            }

            return (CurrentTick.GetHashCode() * 397) ^ aggregate.GetHashCode();
        }

        public override string ToString()
        {
            return $"SimulationState(tick={CurrentTick.Value}, cells={_cells.Count})";
        }
    }
}
