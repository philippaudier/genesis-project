# RFC-0002 — Simulation Transformation Pipeline

Status: **Resolved** — decision recorded in [ADR-0001 — Simulation Transformation Model](../Decisions/ADR-0001-Simulation-Transformation-Model.md) (Accepted 2026-07-25).

> **Revision v2 (per `RFC-0002-Review.md`).** The decision space was widened — *without choosing an
> architecture* — by adding the **temporal-scope** and **mutation / state-access** dimensions, an
> **emergence-enablement** evaluation axis, a **unity-versus-plurality of mechanism** note, and a
> **de-collinearised matrix**. No option was selected in the revision itself; the decision was
> subsequently made in [ADR-0001](../Decisions/ADR-0001-Simulation-Transformation-Model.md).

## Purpose

This RFC exists to decide the **fundamental abstraction by which a tick transforms a state** — the
mechanism that carries `State(tick n)` to `State(tick n+1)`. It should be read before any code that
changes world state beyond advancing the tick counter, and it governs the shape of roughly the next
ten milestones.

This document **decides the problem and compares the candidates. It deliberately does not choose
one.** The comparison is the deliverable; a selection is a later, separate act, made only once the
axes below have been weighed and — where necessary — tested. Where this RFC reports a leaning from
prior work, it reports it as *input*, never as a verdict.

This RFC does **not** decide how gameplay works, how the world is populated, what specific
transformations exist, how the world is represented numerically, or how anything is rendered. It
decides only the *shape of the transforming mechanism*.

## Context

Genesis has been built by milestones to this point:

```
Genesis-001 Foundation  →  assemblies, the Simulation/Presentation boundary
Genesis-002 First Tick  →  logical time (Tick, TickClock)
Genesis-003 State       →  an explicit, externally owned SimulationState, advanced in place
```

The `TickRunner` advances the state, but a tick currently *does nothing* to the state except move its
clock forward. The missing piece is the subject of this RFC: **what a tick does to the state.**

This question is not encountered fresh. It was explored at length in the research arc
(`docs/Research/Transformation-Model.md` and its adversarial critiques, summarised in
`docs/Journal/2026-07-24-architecture-convergence.md`). That arc, by repeated subtraction, reached a
provisional bedrock: the world is a **constrained dynamical system** —

```
External boundary  →  [ State · Transition · Constraint ]  →  next State
```

— in which **Transition** is precisely the mechanism this RFC must now give concrete form. The arc
also produced two findings this RFC treats as prior evidence, not settled law:

- **"There is one way to transform the world; there are many ways to schedule the evaluation of a
  transformation."** The *unit* of change and the *ordering* of units are separable concerns.
- **Constraints are not transformations.** A constraint forbids illegal states; it produces nothing.
  Whatever transformation abstraction is chosen must leave room beside it for a distinct constraint
  layer (its own future RFC).

The naive answer — "systems transform the state" — is rejected as a starting point precisely because
it is already a choice. "System," "pipeline," "phase," "rule" each carry consequences; picking one by
reflex would be an architectural decision made silently, which the project forbids.

## Problem Statement

Define the abstraction by which a tick produces the next state from the current one. Concretely, the
chosen abstraction must answer:

- **Unit** — what is the atomic unit of transformation? (A transition? A system? A command? A rule?)
- **Ordering** — how are units ordered *within* a single tick, deterministically?
- **Composition** — how do many units combine into one tick's worth of change?
- **State access** — how does a unit read the state it depends on and write the state it changes?
- **Determinism** — how is the same-input-same-output guarantee (invariant 4) preserved across units?
- **Constraint boundary** — where do invariants live relative to transformation (given they are not
  themselves transformations)?
- **Mutation model** — does a unit mutate the state in place, or produce a new state value? (This is
  the still-open Q4 of the Transformation research and Q-mutation of RFC-0001.)

These interlock with RFC-0001's open questions — same-tick ordering, cascades, and snapshot-vs-
sequential evaluation — which is why RFC-0002 depends on RFC-0001 and should be read beside it.

## Requirements

### Functional Requirements

- **F1 — Deterministic transformation.** Identical state + identical inputs ⇒ identical next state
  (Level 1, logical).
- **F2 — Explicit, total ordering** of units within a tick. No outcome may depend on an undefined or
  incidental order.
- **F3 — Composition.** Many units combine into one tick without private knowledge of one another,
  beyond a declared ordering/dependency.
- **F4 — Extensibility.** A new transformation can be added without reshaping the mechanism or
  editing unrelated units.
- **F5 — Inspectability.** A unit, and the set of units run in a tick, are inspectable and testable
  in isolation.
- **F6 — Constraint compatibility.** The mechanism leaves a clean seam for a separate constraint
  layer to accept, reject, or arbitrate the change it produces.
- **F7 — Temporal expressiveness.** The mechanism can express change that is not uniform per tick —
  sparse, event-scheduled, or continuous — without forcing every such phenomenon into a per-tick
  no-op (reconciled with RFC-0001 Option 4).

### Non-Functional Requirements

- **N1 — Clarity / comprehensibility** (Article XIV): the mechanism is simple enough to hold whole.
- **N2 — Testability** (backs F1/F5).
- **N3 — Low coupling** between units.
- **N4 — Replay-friendliness**: a run reproduces from initial state + inputs.
- **N5 — Save-friendliness**: the transforming mechanism does not obstruct serialising state at a
  tick boundary.
- **N6 — Performance-awareness**: the abstraction does not foreclose future parallelism, but is not
  optimised prematurely.
- **N7 — Emergence-enablement** (Article VIII): the mechanism enables rich *interaction* between
  simple units, not merely their sequential application. Evaluated as an axis in the matrix.

### Constitutional Requirements

- **C1 (Art. IV Determinism)** ⇐ F1, F2, N4.
- **C2 (Art. V Transformation)** — transformation is primary; the abstraction *is* the concrete form
  of Article V and must treat change, not state, as the thing being described.
- **C3 (Art. VI Causality)** — every unit's effect is attributable to a cause; F5 supports tracing.
- **C4 (Art. VII Locality)** — a unit should read a bounded neighbourhood, not the global state
  (interacts with state-access and parallelism).
- **C5 (Art. VIII Emergence)** — units stay simple; richness comes from their interaction, not from
  any one elaborate unit.
- **C6 (Art. XII Explicitness)** — ordering, dependencies, and reads/writes are explicit, not
  implicit in call order or hidden state.
- **C7 (Art. XIII Composition)** — the mechanism is built by composing units, not by inheritance.
- **C8 (Art. XIV Comprehensibility)** ⇐ N1.

## Terminology

Used precisely below; several are candidate *answers* and must not be assumed interchangeable.

- **Transformation** — a change applied to the state; the generic act this RFC gives a shape to.
- **Unit** — the atomic piece of transformation (the thing this RFC must name).
- **Ordering / Scheduling** — the rule by which units are sequenced within a tick.
- **Transition** — a unit expressed as a function of state (+ inputs) → next state.
- **System** — a named unit owning a slice of transformation logic, run each tick by a scheduler.
- **Command** — a unit expressed as a data object describing a change, applied by an applier.
- **Phase** — a named stage *within* a tick establishing coarse ordering among classes of unit.
- **Pipeline** — a fixed ordered sequence of units through which a tick's transformation flows.
- **Rule** — a unit with declared dependencies; a **Rule Graph** is a set of rules ordered by those
  dependencies.
- **Constraint** — a predicate on legal states; it forbids, it does not produce. Not a unit.

## The dimensions hidden in the question

"How does a tick transform state?" is not one question but several, and separating them is the most
useful thing this RFC can do before comparing candidates. At least **four** orthogonal dimensions are
in play. This decomposition is offered as the best current map, **not** as proven complete — a later
revision may find a fifth.

- **Unit** — *what* a transformation is: Transition, System, Command, Rule.
- **Scheduling** — *how* units are ordered *within* a tick: Pipeline (sequence), Phases, Rule Graph,
  or a flat set with a tie-break.
- **Temporal scope** — *when* transformation happens: uniformly **every tick**, **event-scheduled** (a
  transformation dated to a future tick and evaluated once — the model RFC-0001 leaned toward in its
  Option 4), or **continuous** (a field re-evaluated over a region). Earlier drafts omitted this,
  which silently assumed "every tick, everything"; it is restored here and must be reconciled with
  RFC-0001 Option 4, not left implicit.
- **Mutation & state access** — *how* a unit touches state: **in-place mutation** vs **new-state
  production**, and **declared / scoped / ambient** reads. This is not a footnote for the open
  questions: it governs saves, replay, parallelism, and locality, and it co-determines the
  snapshot-vs-sequential choice. It is a first-class dimension.

The named candidate families each answer only some of these, and conflate the first two:

| Candidate | Unit | Scheduling |
|---|---|---|
| Pipeline | — (agnostic) | ordered sequence |
| Systems | a system | (implicit fixed order) |
| Transitions | a state→state function | — (agnostic) |
| Commands | a data object | — (needs an applier order) |
| Phases | — (agnostic) | named ordered stages |
| Rule Graph | a rule | dependency-derived order |

Seen this way, some "alternatives" are not rivals: *Phases* can order *Systems*; a *Pipeline* can
carry *Transitions*. A decision therefore likely fixes a *value on each dimension* — a unit, a
scheduler, a temporal scope, a mutation/access model — rather than picking one conflated family. And
because the dimensions are orthogonal, they may be decided (and versioned) independently. This
disentangling is a candidate *structure* for the decision, not the decision.

## Options Considered

Each family is described by how it works and its advantages/disadvantages, then scored qualitatively
on the seven axes the decision cares about. No family is chosen. Ratings are *relative* and
provisional.

### Option A — Pipeline (ordered steps)

A tick is a fixed, ordered sequence of steps; the state flows through them, each step transforming it.

- **Advantages.** The order *is* the pipeline — maximally explicit and readable. Trivially
  deterministic given a fixed sequence. Each step is testable in isolation and end-to-end.
- **Disadvantages.** Answers only the scheduling axis; says nothing about what a step *is*. Insertion
  is order-sensitive — adding a step means choosing where, and mis-placement is a silent bug. No
  inherent parallelism (sequential by definition).
- Determinism: **strong**. Tests: **strong**. Replay: **strong**. Saves: **neutral**. Performance:
  **weak** (serial). Readability: **very strong**. Extensibility: **medium** (order coupling).

### Option B — Systems (ECS-inspired, without ECS)

Named systems each own a slice of transformation and are run each tick by a scheduler in a defined
order.

- **Advantages.** Familiar; maps cleanly to "one concern per system"; individually testable; a
  natural home for future parallelism across disjoint systems.
- **Disadvantages.** "System" imports gameplay/engine expectations the project is wary of (Article
  II, and the Constitution's caution against player-centric, content-driven framing). Determinism
  demands an explicit total order and explicit reads (the research's declared/scoped-reads finding),
  or cross-system effects become order-dependent. Risks becoming the "central scheduler" the research
  flagged as a comprehensibility hazard.
- Determinism: **medium** (needs enforced order). Tests: **strong**. Replay: **medium**. Saves:
  **neutral**. Performance: **strong** (parallelism potential). Readability: **medium** (baggage).
  Extensibility: **strong**.

### Option C — Transitions (State → Transition → State)

A unit is a function of the current state (and inputs) producing the next state — the triad's
Transition, expressed directly. This is the Constitution's own language (Article V).

- **Advantages.** The purest expression of "transformation over state." A pure function is the most
  deterministic and testable thing there is (given input, assert output). Excellent replay and, under
  value semantics, excellent save behaviour. Aligns with Article V and the research bedrock.
- **Disadvantages.** Answers the unit axis but leaves composition/ordering open — many transitions
  per tick still need a scheduling answer. Value-semantics (new-state) may cost copying unless
  structural sharing is used (a deferred concern); the mutate-vs-produce fork (Q4) is unresolved and
  this option is most affected by it.
- Determinism: **very strong**. Tests: **very strong**. Replay: **very strong**. Saves: **strong**
  (if new-state). Performance: **medium** (copy cost vs sharing). Readability: **strong**.
  Extensibility: **medium** (needs a composition story).

### Option D — Commands

A transformation is a data object describing a change, applied to the state by an applier.

- **Advantages.** Change *is data* — inspectable, loggable, serialisable. A command log is, in
  effect, a replay log and a compact save; strong synergy with N4/N5. Explicit (each change is a
  named, examinable object).
- **Disadvantages.** Raises "who produces the commands?" — the research found the producer question
  real, and command-production risks re-importing the Intent-as-primitive over-claim the arc already
  refuted. Indirection and object overhead. Needs a defined application order (a scheduling answer).
- Determinism: **medium** (application order + producer determinism). Tests: **strong**. Replay:
  **very strong**. Saves: **very strong**. Performance: **medium** (object overhead). Readability:
  **strong**. Extensibility: **strong**.

### Option E — Phases

A tick is divided into named, ordered phases; transformations belong to phases, giving coarse
deterministic ordering.

- **Advantages.** Named phases tell the tick's story readably. Coarse ordering is explicit; a phase
  can batch its members (enabling the snapshot/parallel model from the research within a phase).
  Stable structure to extend into.
- **Disadvantages.** Answers only the scheduling axis; within a phase, sub-ordering is still needed.
  Too few phases under-specify order; too many recreate a pipeline.
- Determinism: **strong** (coarse) / **medium** (intra-phase). Tests: **strong**. Replay: **strong**.
  Saves: **neutral**. Performance: **strong** (intra-phase batching). Readability: **strong**.
  Extensibility: **strong**.

### Option F — Rule Graph

Units are rules with declared dependencies forming a DAG; execution order is derived by topological
sort.

- **Advantages.** Most powerful for correctness-under-parallelism: declared dependencies let
  independent rules run concurrently while staying deterministic (the research's declared-reads
  enabling safe parallelism). Highly extensible — add a rule and its deps; the graph self-orders.
- **Disadvantages.** The graph is a central structure that can grow beyond one mind (Article XIV, and
  the research's explicit caution about the scheduler becoming complex). Determinism still needs a
  total tie-break among independent-but-same-priority rules. Highest machinery cost.
- Determinism: **strong** (with tie-break). Tests: **medium** (graph behaviour). Replay: **strong**.
  Saves: **neutral**. Performance: **very strong**. Readability: **weak/medium** (graph complexity).
  Extensibility: **very strong**.

### Comparison matrix (de-collinearised)

Rated only on axes that are genuinely *independent* properties of the unit/scheduling family. Two
criteria from the earlier draft are removed as collinear: **Replay** is downstream of determinism
(folded into "Determinism & reproducibility"), and **Save-friendliness** is governed by the
*Mutation & state-access dimension*, not by the family, so it is not scored per-family. **Testability**
is kept but correlates with determinism. **Performance** is shown but **not weighted** at this stage
(a declared non-priority). A new axis — **Emergence-enablement** (Article VIII: how richly units may
interact) — is added, its earlier absence having been the largest gap.

| Axis | Pipeline | Systems | Transitions | Commands | Phases | Rule Graph |
|---|---|---|---|---|---|---|
| Determinism & reproducibility | strong | medium | very strong | medium | strong | strong |
| Emergence-enablement (Art VIII) | low | high | medium | medium | medium | high |
| Testability (~ determinism) | strong | strong | very strong | strong | strong | medium |
| Comprehensibility (Art XIV) | very strong | medium | strong | strong | strong | weak/med |
| Extensibility | medium | strong | medium | strong | strong | very strong |
| *Performance (unweighted)* | *weak* | *strong* | *medium* | *medium* | *strong* | *very strong* |
| Axis addressed | schedule | unit(+sched) | unit | unit | schedule | both |

Not a scoreboard to be summed. Save-friendliness — and the mutation-dependent aspects of the rows
above — follow from the **Mutation & state-access dimension**; no family can be scored in isolation
from that dimension's value.

## Where constraints live

Whatever unit/scheduling pair is chosen, the research's most durable finding stands: **constraints
(invariants) are not transformations.** They forbid; they do not produce. This RFC therefore requires
(F6) that the chosen mechanism expose a clean seam where a *separate* constraint layer can validate,
reject, or arbitrate the produced change — but it does **not** design that layer. Conservation,
legality, and arbitration are deferred to a dedicated Constraint RFC. Naming this now prevents the
transformation mechanism from silently absorbing constraint logic (which would repeat the "validation
becomes an unbounded solver" hazard the research identified).

## On unity versus plurality of mechanism

This RFC does **not** assume a single mechanism must serve all transformation. Whether the answer is
*one* value-per-dimension for the whole world or a *plurality* of specialised mechanisms keyed to the
kind of change is itself an open question — and a live one. The research arc's unresolved **W1**
finding was that **continuous conserved fields do not fit the same shape as discrete, agent-driven
change**: the very testbed this RFC names for validation — scalar diffusion, a uniform *local update
over every cell* — is not cleanly expressed by any of the six candidate families. That is evidence
(not proof) that Genesis may need a specialised field/local-update mechanism alongside whatever serves
discrete change.

The possibility is recorded and left open — neither adopted nor foreclosed. It is deliberately **not**
expanded into a catalogue of new families; that would trade one incomplete map for a sprawling one.
What matters before selection is only that **"one mechanism or several?" is on the table, not decided
by omission.**

## Open Questions

- **Q1 — Decide the dimensions independently?** Should the four dimensions (unit, scheduling, temporal
  scope, mutation/state-access) each be decided — and versioned — independently, per the dimensional
  analysis above?
- **Q2 — Mutation model.** In-place mutation or new-state production? (Q4 of the Transformation
  research; unresolved. Genesis-003 chose in-place *for tick advancement only* and explicitly did
  not settle it for transformations.) Together with Q5 this constitutes the **mutation / state-access
  dimension**.
- **Q3 — Intra-tick evaluation.** Snapshot (all units read start-of-tick state) or sequential (each
  sees prior writes)? (RFC-0001 Q6.) This binds determinism and parallelism.
- **Q4 — Same-tick tie-break.** The total, explicit order among units that could otherwise be
  simultaneous. (RFC-0001 Q10 — the linchpin of determinism.)
- **Q5 — State access.** Declared read/write sets, scoped regions, or ambient access? (Locality,
  parallelism, and causality all hinge on this.)
- **Q6 — Cascades.** May a unit produce further units, and if so do they run this tick or next?
  (RFC-0001 Q3.)
- **Q7 — How much to anticipate.** Minimal mechanism now vs. one that pre-provisions parallelism and
  graphs. (Article XIV vs N6.)
- **Q8 — Temporal scope.** Is transformation uniformly per-tick, event-scheduled (dated to a future
  tick, per RFC-0001 Option 4), continuous, or a combination? Left implicit until now; must be
  reconciled with RFC-0001, not assumed.
- **Q9 — One mechanism or several?** A single value-per-dimension for all transformation, or a
  plurality of specialised mechanisms keyed to the kind of change? Logically prior to "which one?"
  (see *On unity versus plurality of mechanism*).

## Preliminary Direction

**None is taken. This RFC deliberately declines to choose.** That is not indecision; it is the
milestone honouring its own scope — the choice is a distinct, deliberate act that follows the
comparison, not one bundled into it.

What can be recorded, as *input* rather than verdict:

- The prior research arc provisionally converged on the **Transition-over-State** unit (Option C),
  with **scheduling treated as a separate axis** and **constraints as a distinct layer**. This RFC
  places that leaning *beside* the alternatives rather than ratifying it.
- The dimensional framing suggests the eventual decision fixes a *value on each dimension* — a unit, a
  scheduler, a temporal scope, a mutation/access model — and possibly a *plurality* of mechanisms,
  rather than a single conflated family.

The decision should be driven by which axes the project weights highest (determinism, testability,
and comprehensibility have so far outranked performance) and, where analysis is insufficient, by a
concrete worked example and the scalar-diffusion experiment already designed
(`docs/Research/Experiment-01-Scalar-Diffusion.md`), which stresses exactly the state-access,
ordering, and conservation questions above.

## Kill Criteria

Since no direction is chosen, these are the disqualifiers any candidate must survive to remain viable
— stated now so the eventual choice cannot quietly ignore them:

- **Abandon any candidate that cannot guarantee F1/F2** (deterministic, totally-ordered evaluation)
  without ad-hoc special cases.
- **Abandon any candidate whose scheduling structure cannot be held in one mind** (Article XIV) as
  the number of units grows — the Rule Graph is on notice here.
- **Abandon any candidate that cannot expose the constraint seam (F6)** without absorbing constraint
  logic into the transformation unit.
- **Abandon any candidate that only works under one answer to Q2/Q3** if that answer is later
  reversed — i.e. prefer candidates robust to the mutation-model and snapshot decisions still open.

## Validation Plan

The eventual choice is validated by:

- **Determinism/replay** — the same initial state + inputs, run twice, produce identical states
  tick-for-tick through the chosen mechanism.
- **Ordering** — constructed units with deliberately competing effects resolve identically every run.
- **Composition** — independent units combine without cross-references beyond declared order/deps.
- **Constraint seam** — a placeholder constraint can reject a produced change without the unit
  knowing about it.
- **Worked example** — a minimal, non-gameplay transformation (e.g. the scalar-diffusion testbed)
  expressed in the chosen abstraction, by hand, with no special-casing.
- **Save/replay round-trip** — state serialised at a tick boundary and resumed continues identically.

## Dependencies

- **Depends on:** the Constitution (Articles IV, V, VI, VII, VIII, XII, XIII, XIV); RFC-0001 (Tick
  System) for the tick and its open ordering questions; the Transformation-Model research arc and the
  architecture-convergence journal as prior deliberation.
- **Depended on by** (intended, speculative): Genesis-004 (the implementation of whatever is chosen);
  a future **Constraint / Invariant RFC**; a future **Save & Replay RFC**; the deferred
  **numeric-guarantees RFC** (loosely, via determinism); a future **Causality & History RFC**.

## Consequences

Adopting an explicit transformation abstraction means every future change to the world is expressed
through it — a real, permanent discipline, and the point. Benefits: change becomes deterministic,
testable, composable, and traceable. Costs: the chosen unit and scheduling machinery must be built
and maintained; the constraint seam must be honoured; and units must be written to the abstraction's
rules (explicit ordering, declared access) rather than mutating the world freely. These costs are the
shape Article V takes in code.

## Non-Goals

This RFC does **not** define: any specific transformation or gameplay behaviour; the world's entities,
components, or content; the constraint/invariant layer (its own RFC); the numeric representation of
world quantities (deferred RFC); rendering or presentation; or performance optimisation. It also does
not, in this revision, *select* an option — selection is a subsequent, deliberate step.

## Decision Record

Decision: **Resolved by ADR-0001** (Accepted 2026-07-25).
Date: 2026-07-25
Rationale: The comparison this RFC structured was carried out and a value chosen on every dimension —
the **Snapshot Transition Model** (Transition · flat-set snapshot scheduling · uniform per-tick ·
snapshot mutation · one mechanism). The decision, its rationale, trade-offs, and the open questions it
leaves independent are recorded in `../Decisions/ADR-0001-Simulation-Transformation-Model.md`.
