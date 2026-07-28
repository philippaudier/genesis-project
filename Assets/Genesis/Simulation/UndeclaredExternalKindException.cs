using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// Thrown when an external event targets a cell whose kind the membrane does not declare
    /// (ADR-0005). The refusal happens at the membrane — the event never enters the trace and never
    /// touches the world. Whether refused crossings should themselves be recorded is an open
    /// question RFC-L001 explicitly defers; today they are simply rejected.
    /// </summary>
    public sealed class UndeclaredExternalKindException : Exception
    {
        /// <summary>The cell the refused event targeted.</summary>
        public Cell Target { get; }

        public UndeclaredExternalKindException(Cell target)
            : base($"The membrane declares no external kind for {target}; the event may not cross.")
        {
            Target = target;
        }
    }
}
