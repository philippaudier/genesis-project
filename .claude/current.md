# Current

> Live working state only. **Not** history (→ `docs/Journal/`), **not** cross-session memory, **not**
> philosophy (→ `CLAUDE.md`). Keep it short. Update as things change; delete what is no longer current.

- **Phase III — Documentation Reconciliation: COMPLETE (2026-07-26).** The docs now tell the same story as the code:
  - Lot 1 mechanical fixes done (workflow current.md ref, ADR-0001 parenthetical, CLAUDE.md reading path + structure, unity.md Tests wording + harness wiring folded back, invariant 4 → test-enforced).
  - **ADR-0002 — Development Methodology** (Accepted): Proof Rule codified; distilled experimental principles adopted.
  - **RFC-0001 → Accepted** after cold pass (D1 tick/duration split, D3 determinism levels — "bit for bit" removed, Option-4 reframed as future optimisation, open questions annotated closed/open).
  - **Kernel Completion Record:** `docs/Design/04-KERNEL.md` (the ceremony — questions, decisions, proof timeline, deliberate opens).
  - **README, Glossary, Roadmap rebuilt**: README aligned with Article V ("State over behavior" is dead; Unity 6); Glossary rebuilt on the living vocabulary with **Active / Reserved / Avoided** statuses (Rule, Process, Constraint, Entity, Event = Reserved); Roadmap reconciled — the Phase 1↔2 inversion told honestly, Phase 2 complete, Phase 1 next.
  - **Experiment-01: Closed — superseded** (never executed; review closed with it; methodology distillation marked Adopted; transition-mechanics arc marked Overtaken, W1 still live).
- **Milestone:** Genesis-010 Relational Views — implementation + tests ready; **awaiting EditMode validation (expect 57 green)**. Proof statement pending: *Genesis-010 proved that a transition can observe relation-discovered addresses and read their snapshot values through a deterministic, one-hop, explicitly declared relational view.* Genesis-010 code is deliberately **uncommitted** until green (docs committed separately).
- **Next after green:** commit Genesis-010 → **Genesis-011, the first phenomenon** (Phase 1 begins). **DN-002 — What is a Phenomenon?** (`docs/DesignNotes/`, Discussion) defines the threshold: five properties, each with its operational test — structural dependence (**ablation test**: empty RelationSet ⇒ behaviour vanishes), locality (bounded declarations), emergence (global pattern named only in tests, never in simulation code — review-level), determinism (standard identical-runs), mechanism neutrality (diff touches no kernel type). Plus the vocabulary constraint: no new nouns — counters stay counters; the phenomenon precedes the vocabulary. Witness NOT chosen (copy/diffusion/averaging/propagation/signalling; noted neutrally: a field-like witness would begin testing ADR-0001's one-mechanism bet / W1). Target proof: *Genesis-011 proved that the existing kernel can produce a deterministic emergent phenomenon whose behaviour disappears when the relation structure is removed.*
- **Standing opens:** constraint layer · numeric-representation RFC · event-scheduling-as-optimisation vigilance · topology placement · address lifecycle · type-level enforcement of resolver commutativity & scope honesty.
- **Active RFCs:** RFC-0001 — **Accepted** · RFC-0002 — Resolved by ADR-0001.

**Milestone naming convention:** `Genesis-NNN Name`.
