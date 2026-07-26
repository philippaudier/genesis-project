# Current

> Live working state only. **Not** history (→ `docs/Journal/`), **not** cross-session memory, **not**
> philosophy (→ `CLAUDE.md`). Keep it short. Update as things change; delete what is no longer current.

- **RFC-0003 — State Representation: ACCEPTED** (D1–D6 ratified; D4 clarification + closing sentence added on review: *"Accepted, this RFC does not introduce heterogeneous state. It merely removes the last architectural obstacle to expressing it."*)
- **Milestone:** Genesis-012 **First Standing Cause** — implemented, awaiting EditMode validation.
  - **Kernel re-keyed under RFC-0003** (this is implementation of an accepted decision, not a new law): `Place` · `Kind` · `Cell` replace `CounterAddress`; `SimulationState` = tick + cells (order-independent triple equality; `DefinesPlace` derived per D5); `Contribution`/`ReadScope` cell-keyed (D1/D4); `RelationScope` = origin places + granted target kinds (D3/D4: the relation discovers places only; kind visibility granted by the transition); resolvers **per-Kind** (D2, DN-001 literal); commits in canonical cell order. Single-kind worlds behave exactly as before — all 004–011 suites re-keyed and preserved.
  - **The standing cause lives entirely in tests** (`StandingCauseTests.cs`): kinds Q (quantity) + R (rate) at the same places; `ProductionTransition` = universal law instantiated at every place, **inert where rate = 0** ("the law is universal; the state localises it"); R is never written — causal asymmetry is structural. The candidate word "Source" appears nowhere in simulation code (DN-003: the name is earned by the theorem; Glossary admission is a separate editorial act).
  - **The proof (7 tests):** Total_Grows_By_Exactly_The_Declared_Production (accounting: ΔTotal = ΣProduction, per tick) · The_Standing_Cause_Is_Never_Affected · Produced_Quantity_Enters_The_Existing_Phenomenon (feeds Genesis-011, two hops) · **double ablation** — rate 0 under identical laws ⇒ conservation returns and the Genesis-011 world re-emerges exactly (5,4,3); no relations ⇒ production continues but stays local (20,0,0 after 10 ticks) · recognition-not-creation predicate · 500-tick determinism.
  - **Hand-verified:** Q0=(0,0,0), R=(2,0,0): T1 (2,0,0) → T2 (3,1,0) → T3 (4,2,0) → T4 (5,2,1); total = 2t at every tick.
- **Blockers:** run EditMode → Run All in Unity (expect **~70**: re-keyed 004–011 suites + 7 new + D1 heterogeneity test). Proof statement pending green: *Genesis-012 proved that declared heterogeneous state can sustain the first recognisable world cause — the world can contain a cause of its own change.*
- **After green:** Glossary editorial act — admit **Source** as the first Active world category (its theorem now demonstrated); consider tag. Then the Sink (through-flow) and beyond.
- **Honest limitations:** production law instantiated per place by experiment setup (universal by convention of the fixture, not by a kernel mechanism); relational kind-grants are scope-wide (one kind-set for all origins); standing opens unchanged (constraint layer · numeric RFC · write-scoping parked · address lifecycle · type-level enforcement · resolver-algebra conjecture).
- **Active RFCs:** RFC-0001 Accepted · RFC-0002 Resolved by ADR-0001 · RFC-0003 **Accepted**.

**Milestone naming convention:** `Genesis-NNN Name`.
