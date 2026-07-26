# Current

> Live working state only. **Not** history (→ `docs/Journal/`), **not** cross-session memory, **not**
> philosophy (→ `CLAUDE.md`). Keep it short. Update as things change; delete what is no longer current.

- **Genesis-010 Relational Views: COMPLETE** and **Genesis-011 Redistribution: COMPLETE** — 64/64 EditMode tests green.
  - 010 proved: *a transition can observe relation-discovered addresses and read their snapshot values through a deterministic, one-hop, explicitly declared relational view.*
  - 011 proved: *the kernel's existing laws can redistribute a quantity across symmetrically related addresses — deterministically, locally, and emergently, without creating or destroying quantity — and the phenomenon vanishes when the relations are removed.* First phenomenon · first theorem (conservation) · first evidence for ADR-0001's one-mechanism bet. Kernel untouched — the whole implementation is one test file; the guard sentence held.
- **The two eras are now dated:** Construction (Genesis-001→010: *which laws are necessary?*) → Discovery (Genesis-011→: *what do those laws permit?*). Genesis-011 is arguably the first world: three addresses, a few relations, one quantity — and a behaviour that exists because of structure and dies with it.
- **The witness-counter era is concluded** (as the 011 spec committed). Future milestones introduce domain concepts instead of anonymous witnesses.
- **Next: Genesis-012 — meditation CONCLUDED** (`docs/Journal/2026-07-26-the-source-meditation.md`); next instrument is **DN-003 — What is a Recognisable Pattern?** (title chosen; not yet written). The crystallised shape:
  - **Core fact:** the first *difference of nature* in state — `Quantity` (affected) + `Production` (affecting) at the same address. The Source is the *narrative*; heterogeneity is the *fact*. Source = recognised category via deterministic predicate (`IsSource(a) := rate(a) > 0`), never a kernel primitive.
  - **Precision:** what's new is not "state causes change" (flux was always state-determined) but the first **dedicated, causally asymmetric** state — affects without being affected, a *standing* cause.
  - **Decided in discussion:** source-as-state, not law; the production law is **universal, instantiated everywhere, inert at rate 0** — "the law is universal; the state localises it" → the ablation compares two worlds under *strictly identical laws*. Conservation → accounting (`ΔTotal = ΣProduction`): *the kernel forbids the inexplicable, not creation*. **Double ablation:** rate=0 ⇒ no growth (cause is in the world); no relations ⇒ no transport (origin/transport/mechanism separated).
  - **Glossary:** *Entity* stays **Reserved** (a Source has no identity/mobility — scope-honesty); a new **`Source`** entry becomes the first Active world category.
  - **Corpus closure:** DN-001's per-kind resolvers + 008's recorded limitation become literal now; the kind axis returns; representation choice ((identity) with structured values vs (identity × kind)) is RFC-grade after DN-003 and does NOT force the numeric RFC.
  - **DN-003 criteria sketch:** state-grounded · deterministic · causally relevant · vocabulary-late · non-reifying · ablatable · scope-honest.
  - Future companion (not now): a Sink → through-flow, first sustained non-equilibrium dynamic.
- **Honest limitations carried forward:** quantised steady states (integer flux halts at small differences); flux non-negativity guard is witness-scale (≤2 neighbours; general guard = constraint-layer territory); 3 nodes ≠ dense field — W1 open beyond this first witness.
- **Standing opens:** constraint layer · numeric-representation RFC · event-scheduling vigilance · topology placement · address lifecycle · type-level commutativity/scope enforcement · write-scoping (deliberately parked) · resolver-algebra conjecture (Journal 2026-07-26) · observational-tests family (noticed, not formalised).
- **Active RFCs:** RFC-0001 — Accepted · RFC-0002 — Resolved by ADR-0001.

**Milestone naming convention:** `Genesis-NNN Name`.
