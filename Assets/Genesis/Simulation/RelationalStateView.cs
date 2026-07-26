using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The materialisation of a transition's declared contract (Genesis-010), built once per
    /// transition per tick from the immutable start-of-tick snapshot and the external
    /// <see cref="RelationSet"/>. It contains:
    /// <list type="bullet">
    ///   <item>the values of the directly declared addresses (<see cref="ReadScope"/>);</item>
    ///   <item>per declared origin (<see cref="RelationScope"/>), that origin's outgoing relations in
    ///   canonical order — and the snapshot values of their targets, which become readable;</item>
    /// </list>
    /// and nothing else. Out-of-contract state is absent, not hidden: reading it or observing an
    /// undeclared origin's relations fails. The grant is strictly one hop — discovered targets are
    /// readable but are not origins, so their own relations remain invisible unless separately
    /// declared.
    /// </summary>
    public sealed class RelationalStateView : IRelationalStateView
    {
        private readonly Dictionary<CounterAddress, long> _values;
        private readonly Dictionary<CounterAddress, IReadOnlyList<Relation>> _outgoingByOrigin;

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

            _values = new Dictionary<CounterAddress, long>();
            _outgoingByOrigin = new Dictionary<CounterAddress, IReadOnlyList<Relation>>();

            // Directly declared reads.
            foreach (CounterAddress address in readScope.Addresses)
            {
                _values[address] = snapshot.CounterOf(address);
            }

            // Declared origins: their outgoing relations become visible (canonical order), and each
            // discovered target's snapshot value becomes readable. One hop only — targets are not
            // added as origins.
            foreach (CounterAddress origin in relationScope.Origins)
            {
                IReadOnlyList<Relation> outgoing = relations.OutgoingFrom(origin);
                _outgoingByOrigin[origin] = outgoing;

                for (int i = 0; i < outgoing.Count; i++)
                {
                    CounterAddress target = outgoing[i].Target;
                    _values[target] = snapshot.CounterOf(target);
                }
            }
        }

        public long Read(CounterAddress address)
        {
            if (_values.TryGetValue(address, out long value))
            {
                return value;
            }

            throw new ReadOutOfScopeException(address);
        }

        public IReadOnlyList<Relation> OutgoingRelations(CounterAddress origin)
        {
            if (_outgoingByOrigin.TryGetValue(origin, out IReadOnlyList<Relation> outgoing))
            {
                return outgoing;
            }

            throw new RelationOutOfScopeException(origin);
        }
    }
}
