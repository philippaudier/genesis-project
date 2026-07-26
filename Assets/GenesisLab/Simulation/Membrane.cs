using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// The world's declared boundary (ADR-0005; RFC-L001; invariant 7): the set of external kinds a
    /// world admits crossings for. Everything external must cross through cells of a declared kind;
    /// nothing can appear directly in the world. The membrane carries no behaviour and no state
    /// beyond the declaration itself — producers stand outside it and are invisible to laws
    /// (provenance-blindness); laws are its only consumers.
    /// </summary>
    public sealed class Membrane
    {
        private readonly HashSet<Kind> _externalKinds;

        public Membrane(IEnumerable<Kind> externalKinds)
        {
            if (externalKinds == null)
            {
                throw new ArgumentNullException(nameof(externalKinds));
            }

            _externalKinds = new HashSet<Kind>(externalKinds);
        }

        /// <summary>Whether <paramref name="kind"/> is declared as external by this membrane.</summary>
        public bool Declares(Kind kind)
        {
            return _externalKinds.Contains(kind);
        }
    }
}
