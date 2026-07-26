# Technical Invariants

> The technology-specific projection of the [Constitution](../Constitution/00-CONSTITUTION.md) onto
> the Genesis codebase (Unity 6, URP, C#).
>
> The Constitution is timeless and deliberately names no technology. **This document is where its
> principles become checkable, enforced constraints for the current implementation.** If Genesis is
> ever rewritten on another stack, this file is rewritten; the Constitution is not.
>
> An invariant is not advice. It is a property the project holds true at all times — ideally enforced
> so that violating it is *hard or impossible*, not merely discouraged.

| # | Invariant | Descends from | Enforced by |
|---|-----------|---------------|-------------|
| 1 | The Simulation never references Unity. | Art. XI (Observation); "Unity = Presentation" | **Compile-time.** `Genesis.Simulation.asmdef` declares no `UnityEngine` reference; any Unity API used in simulation code fails to build. |
| 2 | Dependencies point one way only: Presentation → Simulation, never the reverse. | Art. XI | **Compile-time.** The asmdef reference graph: Simulation references no Presentation assembly. A cycle will not compile. |
| 3 | Rendering — and any presentation — is replaceable. | Art. XI, Art. I | **Structural.** Follows from 1 & 2: the Simulation runs headless, with no presentation assembly loaded. If it still runs, presentation is replaceable. |
| 4 | The Simulation is **logically** deterministic (Level 1): identical initial state + identical inputs ⇒ an identical sequence of decisions and transformations. | Art. IV (Determinism) | **Test-enforced + convention.** Determinism tests run green in every suite (identical runs compared end-state and tick-for-tick, e.g. `Two_Runs_Over_1000_Ticks_End_Strictly_Identical`); conventions forbid wall-clock reads, unseeded randomness, and undefined iteration order. Replay-from-recorded-inputs infrastructure is still future work. *Logical* determinism is the target — bit-identical numeric results (Levels 2–3) are deferred to a future numeric-guarantees RFC. |
| 5 | Simulation logic never depends on frame rate or real time. | Art. IV; the tick is a logical step, not a duration | **Compile-time + model.** Real-time APIs live in `UnityEngine`, already blocked by (1); and the tick advances logically, so no simulation rule can read elapsed time. |
| 6 | All simulation state is explicit. | Art. XII (Explicitness) | **Test-time + review.** Nothing influences the world that is not part of its declared state: a snapshot → restore round-trip must reproduce an identical logical state; any divergence reveals hidden state. Held by review until that round-trip test exists. |
| 7 | **No hidden hand.** A world under observation is never modified directly by its host; every external influence crosses the declared membrane — recorded as an external event, applied at a tick boundary, interpreted only by laws. | Art. IV (Determinism), Art. XII (Explicitness); RFC-L001 | **Review + planned test.** `WithValue`-style host mutation between observed ticks is illegal by protocol; replay equivalence (initial state + relations + laws + external event trace ⇒ identical run) is the test once the membrane exists. Until then, held by review. |

## Reading notes

- **Prefer enforcement to trust.** Where "enforced by" is not yet compile- or test-time, the invariant
  is held by review until a mechanism exists. Converting a reviewed invariant into a compiled or
  tested one is always a welcome change.
- **This list is deliberately short.** It holds only *architectural* truths. Code style belongs in
  `conventions.md`; process belongs in `workflow.md`; neither belongs here.
- **Changing an invariant is a Constitutional-weight act.** These descend directly from the
  Constitution; a proposed change to one should be treated as evidence that either the code has
  drifted or a principle needs re-examination — the latter being rare and deliberate.
