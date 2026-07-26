# Current

> Live working state only. **Not** history (→ `docs/Journal/`), **not** cross-session memory, **not**
> philosophy (→ `CLAUDE.md`). Keep it short. Update as things change; delete what is no longer current.

- **Genesis-010 Relational Views: COMPLETE** and **Genesis-011 Redistribution: COMPLETE** — 64/64 EditMode tests green.
  - 010 proved: *a transition can observe relation-discovered addresses and read their snapshot values through a deterministic, one-hop, explicitly declared relational view.*
  - 011 proved: *the kernel's existing laws can redistribute a quantity across symmetrically related addresses — deterministically, locally, and emergently, without creating or destroying quantity — and the phenomenon vanishes when the relations are removed.* First phenomenon · first theorem (conservation) · first evidence for ADR-0001's one-mechanism bet. Kernel untouched — the whole implementation is one test file; the guard sentence held.
- **The two eras are now dated:** Construction (Genesis-001→010: *which laws are necessary?*) → Discovery (Genesis-011→: *what do those laws permit?*). Genesis-011 is arguably the first world: three addresses, a few relations, one quantity — and a behaviour that exists because of structure and dies with it.
- **The witness-counter era is concluded** (as the 011 spec committed). Future milestones introduce domain concepts instead of anonymous witnesses.
- **Next: Genesis-012** — the first thing that exists for itself. Not yet designed; direction open (candidates flow from the reconciled Roadmap Phase 1: world-state representation beyond witnesses, numeric-representation RFC, spatial structure as a use of relations, the constraint layer). Await design discussion.
- **Honest limitations carried forward:** quantised steady states (integer flux halts at small differences); flux non-negativity guard is witness-scale (≤2 neighbours; general guard = constraint-layer territory); 3 nodes ≠ dense field — W1 open beyond this first witness.
- **Standing opens:** constraint layer · numeric-representation RFC · event-scheduling vigilance · topology placement · address lifecycle · type-level commutativity/scope enforcement · write-scoping (deliberately parked) · resolver-algebra conjecture (Journal 2026-07-26) · observational-tests family (noticed, not formalised).
- **Active RFCs:** RFC-0001 — Accepted · RFC-0002 — Resolved by ADR-0001.

**Milestone naming convention:** `Genesis-NNN Name`.
