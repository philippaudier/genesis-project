# ADR-0001 — Simulation Transformation Model

## Status

**Accepted** — 2026-07-25. The first deliberate architectural commitment of Genesis.

Resolves the decision that `docs/RFC/RFC-0002-Simulation-Transformation-Pipeline.md` was designed to
enable. (RFC-0002's Decision Record has since been updated and reads *Resolved by ADR-0001*.)

## Context

Genesis has time (`Tick`, `TickClock`) and an explicit, externally owned `SimulationState`. A tick
currently does nothing to the state but advance its clock. The missing piece is **how a tick
transforms state** — the concrete form of the Constitution's Article V.

RFC-0002 mapped this as five orthogonal dimensions and, in v2, was made decision-ready (temporal scope
and mutation/state-access added as dimensions, emergence added as an evaluation axis, a
plurality-of-mechanism question opened, the matrix de-collinearised). Exploration is complete. This
ADR chooses a value on every dimension, verifies the combination is one consistent architecture, and
commits to it.

The choices are governed by Genesis's stated priority order — **Determinism, Comprehensibility,
Testability, Emergence, Extensibility** — with performance an explicit secondary concern, and by the
rule: *when two options are close, prefer the simpler unless doing so violates a constitutional
principle.*

## Decision

Genesis adopts the **Snapshot Transition Model**:

> Each tick, every **transition** reads the current state as an immutable snapshot and produces the
> next state. No transition observes another's writes within the same tick. Write conflicts are
> resolved by a defined, deterministic rule. Then the next state becomes current, and the tick
> advances.

### Chosen values for every dimension

| Dimension | Chosen value |
|---|---|
| **Unit** | **Transition** — a pure function of (current-state snapshot, external inputs) → contributions to the next state. |
| **Scheduling** | **Flat set** of transitions per tick, with a **defined deterministic conflict-resolution order** used only to reconcile writes to the same state. (No pipeline, phases, or graph.) |
| **Temporal Scope** | **Uniform per-tick.** Sparse or scheduled behaviour is expressed as *state* (due-ticks, timers) that per-tick transitions read and act on. |
| **Mutation & State Access** | **Snapshot semantics** — read the current state, produce the next; no within-tick write visibility (not sequential/in-place). Read-scoping (declared/scoped/ambient) is deferred. |
| **One vs Multiple Mechanisms** | **One mechanism.** The snapshot-transition shape expresses both continuous fields (per-cell local updates) and discrete agents (per-entity updates with conflict resolution). |

### Architectural rationale — dimension by dimension

**Unit — Transition.** *Why it best serves Genesis:* a pure function of the state is the most
deterministic (invariant 4) and most testable unit possible (given a snapshot, assert the output), and
it is the Constitution's own language — Article V's "state is transformation caught mid-sentence" made
literal. It wins the top three priorities (Determinism, Comprehensibility, Testability) outright, and
is the simplest possible unit (nothing is smaller than "old state → new state"). *Sacrificed:* the
turnkey extensibility of a Rule ("declare deps, the graph self-orders") and the advertised
emergence-affordance of a System — both lower-priority, and emergence is recovered by the snapshot
choice below. *Long-term consequence:* every world change is written as a transition; the world is
described by its verbs, not its nouns. *Open questions left independent:* the exact shape of a
transition (implementation); whether a transition may emit further transitions (cascades) — orthogonal
and deferrable.

**Mutation & State Access — Snapshot.** *Why:* an immutable snapshot removes read-after-write hazards
by construction, so determinism holds regardless of the order transitions run in (except for write
conflicts). It makes transitions genuinely pure and independently testable, and gives the cleanest
mental model in the whole design: *"read the world as it is; produce the world as it will be."* It is
also what makes emergence possible deterministically — see below. *Sacrificed:* within-tick write
visibility (a transition cannot see another's effect until next tick — one causal hop per tick), and
the memory cost of holding two states (performance, secondary). *Long-term consequence:* a tick becomes
a quantum of causal depth; consequences propagate one tick per hop, which is legible and reproducible.
*Open questions left independent:* **read-scoping** (declared/scoped/ambient) — snapshot is
deterministic under any of them, so this can be tightened later to enforce locality and enable
parallelism without changing the model; structural sharing is implementation.

**Scheduling — Flat set + deterministic conflict rule.** *Why:* under snapshot semantics, within-tick
*ordering* only matters when two transitions write the same state — everything else is genuinely
simultaneous. So the rich schedulers (pipeline, phases, graph) solve a problem the snapshot has
already removed. The simplest scheduling that stays deterministic is therefore a flat set plus a total,
deterministic rule to reconcile conflicting writes. This is maximally comprehensible and fully
extensible (phases can be layered on later if within-tick ordering is ever genuinely required).
*Sacrificed:* authored within-tick sequencing (deferred to possible future phases) and the Rule Graph's
parallel-scheduling power (performance, secondary). *Long-term consequence:* the tick loop stays a
one-line story ("apply every transition to the snapshot, reconcile, swap"), which protects Article XIV
as the world grows. *Open questions left independent:* the **specific conflict-resolution rule**
(last-writer / accumulate / reject / arbitrate — probably per-state-kind). The architecture requires
that *a* deterministic total rule exists; *which* rule is a separate decision that does not alter the
model.

**Temporal Scope — Uniform per-tick.** *Why:* one temporal model is simpler than a hybrid, and it is
expressive enough because a scheduled event is just *state* — a due-tick a per-tick transition checks.
This keeps determinism and comprehensibility maximal while losing only efficiency on sparse phenomena
(performance, secondary). *Sacrificed:* the sparse-evaluation efficiency of RFC-0001's preliminary
Option 4 (fixed tick + scheduled events). *Long-term consequence:* RFC-0001's scheduled-event model is
reframed from a *semantic necessity* to a future *performance optimisation* that preserves per-tick
semantics — it can be added without changing meaning. *Open questions left independent:* whether/when to
add event-scheduling as an optimisation (Q8) — purely a performance concern, independent of this
decision.

**One vs Multiple Mechanisms — One.** *Why:* RFC-0002's plurality question arose because the
*System/Command/Rule* unit forms fit agents but not continuous fields (the W1 finding — the diffusion
testbed fit no candidate). But the **Transition-under-snapshot** shape *is* the cellular-automaton
model, which is exactly how fields (diffusion, reaction) are expressed — and agents are transitions
that additionally contend for shared state, handled by the conflict rule. The shape spans both, so one
mechanism suffices; per "prefer the simpler," pluralism is not built. *Sacrificed:* the safety of
pre-provisioning a specialised field mechanism — this is a deliberate bet, monitored below. *Long-term
consequence:* fields and agents are the same kind of thing (transitions over a snapshot); their
difference is transition *content* and evaluation *density*, not mechanism. *Open questions left
independent:* whether a future phenomenon genuinely cannot be a snapshot-transition — testable by the
already-designed scalar-diffusion experiment; the bet is revisited only if refuted.

### Coherence verification

The five choices are not independent picks that happen to sit together — they *converge*:

- Choosing **Transition** (unit) forces a mutation model; the deterministic, testable, emergent choice
  is **snapshot**.
- **Snapshot** makes within-tick ordering irrelevant except for conflicts, which *reduces* scheduling
  to a **flat set + conflict rule**.
- **Per-tick** + **snapshot** + **flat set** *is* the generalised cellular-automaton model — the
  paragon of deterministic emergence (Article VIII), recovering the emergence that the Transition unit
  alone rated only "medium."
- Because that model expresses fields and agents alike, the mechanism is **one**.

There is no internal conflict; each choice makes the next simpler. The combination is a single
architecture — the Snapshot Transition Model — and it maximises the top four priorities
(Determinism, Comprehensibility, Testability, Emergence), serves the fifth (Extensibility: add a
transition; layer phases/scoping/scheduling later), and sacrifices only performance, as permitted.

## Alternatives rejected (and why)

- **System / Command / Rule as the unit** — Systems import gameplay/engine framing the Constitution
  guards against and rate lower on determinism/comprehensibility; Commands add data-object indirection
  and re-raise the producer question the research already refuted; Rule Graph's power costs
  comprehensibility (Article XIV) — a priority-2 value — for extensibility, a priority-5 one.
- **Pipeline / Phases / Rule Graph as scheduling (now)** — all solve within-tick ordering, which
  snapshot semantics largely dissolves. They add structure ahead of a demonstrated need; phases remain
  a clean future extension if that need appears.
- **Sequential / in-place mutation** — lets transitions see each other's within-tick writes, which
  reintroduces order-dependence and read-after-write hazards, weakening determinism and testability
  for expressive convenience Genesis does not yet need.
- **Hybrid temporal scope (scheduled events as semantics)** — rejected as a *semantic* model but
  retained as a future *optimisation*; carrying it now would add a second temporal model for a
  performance gain that is a secondary concern.
- **Multiple mechanisms** — rejected because the chosen unit/mutation pair already spans fields and
  agents; building pluralism now would add machinery against the project's subtraction discipline. Held
  as a monitored risk, not a foreclosed option.

## Known trade-offs

- **Performance is spent deliberately.** Every transition is evaluated every tick (including
  no-ops), and the model holds a current and a next state. Both are accepted as secondary costs;
  event-scheduling and structural sharing are compatible future optimisations.
- **One causal hop per tick.** A transition's effect is visible only next tick. Multi-step within-tick
  chains must span multiple ticks (or, later, phases). This is legible and reproducible, but it is a
  real expressiveness constraint.
- **Locality is enabled, not enforced.** With read-scoping deferred, a transition *may* read globally,
  which Article VII discourages. The model does not force non-locality, and scoped reads can enforce
  Article VII later — but until then, locality rests on discipline, not the compiler.
- **The one-mechanism choice is a bet** that fields and agents both fit the snapshot-transition shape.
  The diffusion experiment is its test; if a phenomenon genuinely resists the shape, this ADR is
  revisited.
- **Global invariants lean on the deferred constraint layer.** One transformation mechanism suffices
  only because conservation, legality, and global constraints are the (separate, deferred) constraint
  layer's job, not the transformation's.
- **Determinism depends on an as-yet-unspecified conflict rule.** The architecture requires a
  deterministic total conflict-resolution rule to exist before any conflicting transitions are built.

## Consequences for Genesis-004

Genesis-004 becomes the minimal realisation of the Snapshot Transition Model, and no more: a transition
concept (current state → contributions to the next state), and a tick that applies a set of transitions
under snapshot semantics to produce the next state, with tests proving deterministic progression under
transformation. Genesis-003's in-place tick advancement is reconciled into "produce the next state"
(the tick advance becomes part of producing the next state, not an in-place mutation). No domain
content, no specific transitions beyond what is needed to prove the model, no conflict rule until
conflicting transitions exist. The specifics are Genesis-004's to define within this frame; this ADR
fixes the frame, not the code.

## Open questions intentionally left unresolved

Each is independent of this decision and can be settled later without reopening it:

- **The conflict-resolution rule** (which writes win on the same state; likely per-state-kind).
- **Read-scoping** (declared / scoped / ambient) — the lever that would *enforce* Article VII locality
  and enable deterministic parallelism.
- **Cascades** — whether a transition may emit further transitions, and if so when they evaluate.
- **Within-tick phases** — to be added only if a demonstrated need for within-tick ordering appears.
- **Event-scheduling** — as a semantics-preserving performance optimisation over per-tick evaluation.
- **The numeric representation** of world quantities — its own deferred RFC; independent of the
  transformation shape.
- **The constraint / invariant layer** — its own future RFC; the transformation model requires only
  that a clean seam for it exists.
- **Structural sharing and other performance work** — implementation, not architecture.

This ADR makes one commitment: *how a tick transforms state.* Everything above is deliberately left
open, and none of it can force this decision to be remade.
