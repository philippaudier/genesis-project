using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The materialisation of a transition's declared contract, built once per transition per tick
    /// from the immutable start-of-tick snapshot and the external <see cref="RelationSet"/>. It
    /// contains the directly declared cells, and — per declared origin place — that origin's outgoing
    /// relations in canonical order plus the granted kinds' values at each discovered target place.
    /// Out-of-contract state is absent, not hidden. The grant is strictly one hop: discovered places
    /// are readable (in the granted kinds) but are not origins, so their own relations remain
    /// invisible unless separately declared.
    /// </summary>
    public sealed class RelationalStateView : IRelationalStateView
    {
        private readonly Dictionary<Cell, long> _values;
        private readonly Dictionary<Place, IReadOnlyList<Relation>> _outgoingByOrigin;

        public RelationalStateView(
            SimulationState snapshot,
            RelationSet relations,
            ReadScope readScope,
            RelationScope relationScope)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (relations == null)
            {
                throw new ArgumentNullException(nameof(relations));
            }

            if (readScope == null)
            {
                throw new ArgumentNullException(nameof(readScope));
            }

            if (relationScope == null)
            {
                throw new ArgumentNullException(nameof(relationScope));
            }

            _values = new Dictionary<Cell, long>();
            _outgoingByOrigin = new Dictionary<Place, IReadOnlyList<Relation>>();

            // Directly declared cells.
            foreach (Cell cell in readScope.Cells)
            {
                _values[cell] = snapshot.ValueAt(cell);
            }

            // Declared origins: their outgoing relations become visible (canonical order), and at
            // each discovered target place, the granted kinds' cells become readable — where the
            // state defines them. One hop only: targets are not added as origins.
            foreach (Place origin in relationScope.Origins)
            {
                IReadOnlyList<Relation> outgoing = relations.OutgoingFrom(origin);
                _outgoingByOrigin[origin] = outgoing;

                for (int i = 0; i < outgoing.Count; i++)
                {
                    Place target = outgoing[i].Target;
                    foreach (Kind kind in relationScope.TargetKinds)
                    {
                        var cell = new Cell(target, kind);
                        if (snapshot.Defines(cell))
                        {
                            _values[cell] = snapshot.ValueAt(cell);
                        }
                    }
                }
            }
        }

        public long Read(Cell cell)
        {
            if (_values.TryGetValue(cell, out long value))
            {
                return value;
            }

            throw new ReadOutOfScopeException(cell);
        }

        public IReadOnlyList<Relation> OutgoingRelations(Place origin)
        {
            if (_outgoingByOrigin.TryGetValue(origin, out IReadOnlyList<Relation> outgoing))
            {
                return outgoing;
            }

            throw new RelationOutOfScopeException(origin);
        }
    }
}
