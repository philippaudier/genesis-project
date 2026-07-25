# Genesis Roadmap

How we get from here to there.

---

## A note on the order of things (reconciliation, 2026-07-26)

The original roadmap planned to build the world's *representation* first (Phase 1) and its *laws of
change* second (Phase 2). Reality inverted this — and the inversion was better.

We thought we would build the world before the laws. We discovered that the laws could be built,
proven, and frozen with almost no world at all — three counters sufficed as witnesses — and that
every question about representation is easier to answer once the laws that govern it already exist.
So Phase 2 happened first, is complete, and is recorded in the
[Kernel Completion Record](04-KERNEL.md). This document now tells the truth about that order rather
than preserving the planned one. The phases' *content* was sound; only their sequence moved.

---

## Phases

### Phase 0 — The Project Begins ✅ Complete

Foundations: Constitution, Vision, Glossary, Roadmap, repository structure, engineering documents
(invariants, conventions, unity, workflow), the AI development environment, and the assembly boundary
that makes Simulation/Presentation a compile-time fact (Genesis-001).

### Phase 2 — The World Changes ✅ Complete (built first, deliberately)

Time and transformation — the computational kernel, built through seven proof-milestones
(Genesis-004..010) under the Proof Rule (ADR-0002):

- **Materialised decisions:** RFC-0001 Tick System (Accepted) · RFC-0002 Transformation Pipeline
  (resolved by **ADR-0001 — the Snapshot Transition Model**) · DN-001 Conflict Resolution Policies.
- **Proven properties:** snapshot transition, composition, conflict resolution, scoped reads,
  addressable identity, explicit relations, relational views.
- Exit criteria met: time advances through ticks; transformations occur deterministically; identical
  inputs produce identical outputs — verified by a cumulative executable test suite.

### Phase 1 — The World Exists 🔜 Next

Now that the laws exist, give them a world worth governing. The witness counters become genuine
world state.

Questions to answer: how is state represented beyond homogeneous counters (kinds, quantities, the
numeric-representation RFC)? How is space organised — as structure built on explicit relations? What
does the first *phenomenon* look like (behaviour impossible without relations — the planned
Genesis-011)? How do addresses acquire lifecycle?

Expected decisions: World State Representation · Numeric Guarantees · Spatial Structure (as a use of
relations, not a replacement for them) · the Constraint layer (the triad's third element).

### Phase 3 — The World Grows

Processes and propagation: long-running change, effects spreading through structure, the first
sustained emergent behaviour. (The parked field/conservation questions — W1 of the research arc —
return here, and with them the first real test of ADR-0001's one-mechanism bet.)

### Phase 4 — The World Remembers

Persistence and history: serialisation, save/load continuity (an obligation inherited from
RFC-0001's validation plan), causality tracking (the future Causality & History RFC), history depth.

### Phase 5 — The World Adapts

Feedback loops, equilibrium, long-horizon stability and evolution.

### Phase 6 — The Player Arrives

Observation and interaction: external inputs bound to ticks (RFC-0001 Q7), the presentation bridge,
consequences flowing from actions — and the gate to Lootbound.

---

## Principles for Progress

1. **No phase is skipped.** Foundations matter.
2. **No phase is rushed.** Correctness over speed.
3. **Decisions before code.** RFCs and ADRs before implementation.
4. **One proved property per milestone** (ADR-0002). The suite is cumulative; regressions are
   forbidden.
5. **Exit criteria are real.** A phase ends when its criteria are met — and when reality teaches a
   better order, the roadmap is reconciled honestly, not silently.

---

*Patience is not the absence of progress. Patience is sustainable progress.*
