# RFC-0001 — Tick System

Status: **Accepted** — 2026-07-26.

> **Cold-pass revision (2026-07-26).** This RFC's settled core — the tick as a logical, integer,
> host-independent step, and Level-1 logical determinism — was implemented and proven by
> Genesis-002..010 (see `docs/Design/04-KERNEL.md`). This revision folds in the journal amendments of
> 2026-07-24 (determinism levels; tick/duration separation), records how ADR-0001 resolved the
> preliminary direction, and moves the RFC to Accepted so its status tells the truth about running,
> tested code. Sections below are annotated where the original text has been superseded.

## Purpose

This RFC exists to define how time advances in Genesis, and to do so before any
simulation code depends on an implicit answer. It should be read by anyone about to
build a system that changes world state, schedules future change, or observes the
world as it evolves — that is, by almost everyone, almost always.

This document decides **the problem**: what "a moment of simulation" means, when the
world is permitted to change, how simultaneous changes are ordered, and what
properties the advancement of time must guarantee. It surveys the mechanisms that
could satisfy those requirements and identifies a preliminary direction.

This document does **not** decide the final mechanism, the tick frequency, the concrete
representation of time, the phase ordering, or the amount of causality metadata
retained. Those remain open questions to be closed by analysis and experiment before
this RFC moves from Draft to Accepted. Where this RFC states a preference, it states it
as preference, not as settled fact.

## Context

Genesis is a deterministic simulation framework. The simulation is the product; every
form of presentation is a downstream observer of it. Three properties of the framework
force the question of time to the front:

1. **The simulation must run without presentation.** A world that only advances while
   being rendered is a world whose time is defined by its audience. Genesis requires the
   opposite: the world advances on its own terms, and presentation, if present at all,
   only reads the result.

2. **The same initial state and the same inputs must reproduce the same result.**
   Precisely: Genesis **requires Level 1 — logical determinism** (identical decisions and
   transformations on every run); **desires Level 2** — platform determinism (identical
   results on a given platform and runtime version); and **defers Level 3** —
   cross-platform bit-identical results — to a future numeric-guarantees RFC, since it
   depends on numeric types and concurrency strategy not yet chosen. (An earlier draft
   demanded "bit for bit, machine for machine"; that over-promised — it is Level 3, and
   it is not this RFC's to guarantee.) Reproducibility at any level is unachievable
   unless the *order and timing* of every state change is itself deterministic. Time is
   the spine that ordering hangs from.

3. **Transformations must occur in an explicit, traceable order.** If two changes happen
   "at the same time," the world must still have a single, defined, inspectable answer
   for what that means. Ambiguity about ordering is ambiguity about truth.

Today, Genesis has no explicit model of time. Absent one, time would be defined by
accident — by whatever loop happens to drive the process, at whatever rate the host
delivers frames. That is precisely the implicit, presentation-coupled answer the
framework must reject. This RFC is the deliberate alternative.

## Problem Statement

The engineering problem is to define an explicit model of simulation time with the
following characteristics, each stated as a question this RFC must ultimately answer:

- **Fundamental unit.** What is the indivisible unit of simulation time — the smallest
  interval within which the world is considered not to change?
- **When change is permitted.** At what points is world state allowed to mutate, and at
  what points is it guaranteed stable and safe to read?
- **Ordering of the simultaneous.** When multiple transformations are scheduled for the
  same unit of time, by what defined rule are they ordered so the outcome is
  reproducible?
- **Representation of time.** How is the current moment represented such that it is
  exact, comparable, serialisable, and free of the rounding error inherent in
  continuous quantities?
- **Decoupling of rate from outcome.** How can the simulation run faster than real time,
  slower than real time, be paused, or be advanced one unit at a time — without any of
  these changing the result it produces?
- **Independence from the host.** How is the simulation kept independent of the host
  environment's frame rate, so that a display updating at any rate — or not at all —
  produces the same world?
- **Exact replay.** How can a run be reproduced exactly from a recorded starting point
  and a recorded sequence of inputs?
- **Traceability.** How can any transformation be traced back to the specific moment at
  which it occurred and the cause that scheduled it?

These questions are interdependent. A representation of time that permits rounding
error undermines exact replay. An ordering rule that depends on host timing undermines
independence from the host. The model must answer them as a coherent whole, not
individually.

## Requirements

### Functional Requirements

- **F1 — Deterministic progression.** Given identical initial state and identical
  inputs, the sequence of world states produced must be identical on every run.
- **F2 — Explicit ordering.** Every transformation within a unit of time must have a
  defined position in a total order. No outcome may depend on an undefined or
  incidental ordering.
- **F3 — Pause.** The simulation must be able to stop advancing while retaining its full
  state, and resume without discontinuity.
- **F4 — Single-step.** The simulation must be able to advance exactly one unit of time
  and then hold, for inspection and debugging.
- **F5 — Variable execution speed.** The simulation must be able to advance at any wall-
  clock rate — including as fast as the host allows and as slow as desired — without
  the chosen rate altering the produced result.
- **F6 — Replayability.** A run must be reproducible from a recorded initial state and a
  recorded input timeline.
- **F7 — Headless execution.** The simulation must run to completion with no presentation
  layer attached.
- **F8 — Observable simulation time.** The current simulation time must be readable at
  any stable point, exactly and cheaply, by any observer.
- **F9 — Input insertion.** External inputs must be insertable into the timeline at
  defined moments such that their effect is deterministic and replayable.

### Non-Functional Requirements

- **N1 — Clarity.** The model must be simple enough to be held whole in one mind and
  explained without reference to its implementation.
- **N2 — Testability.** Determinism, ordering, pause, single-step, and replay must each
  be directly verifiable by automated tests.
- **N3 — Low coupling.** The time model must not depend on the internals of any
  particular simulation system, nor those systems on the time model's internals.
- **N4 — Host-timing independence.** No behaviour of the simulation may depend on the
  host's frame timing, delta-time, or scheduler.
- **N5 — Long-term stability.** The model must remain valid across long runs without
  drift, overflow within reasonable bounds, or accumulation of representational error.
- **N6 — Scalability.** The model must accommodate growth in the number of systems and
  transformations without requiring its own redefinition.

### Constitutional Requirements

- **C1 (Article IV — Determinism).** The time model is the ordering spine that makes
  reproducibility possible; it must introduce no source of irreproducibility. Satisfies
  and is required by F1, F6, N4.
- **C2 (Article V — Transformation).** Time is defined by the boundaries between states,
  not by states themselves; the model must describe *when and how one moment becomes the
  next*, treating change as primary. Shapes the whole model; see F2.
- **C3 (Article VI — Causality).** Every transformation must be attributable to a cause
  acting at a specific moment; the model must make "when" a first-class, traceable
  property. Required by the traceability problem and F9.
- **C4 (Article XI — Observation).** Advancement of time must be a property of the
  simulation alone. Presentation may read simulation time but must never define, drive,
  or gate it. Required by F5, F7, N4.
- **C5 (Article XII — Explicitness).** The current time, the pending schedule, and the
  ordering rule must all be explicit, inspectable state — never implicit in a loop or
  hidden in host timing. Required by F8; enables F1 and F6.
- **C6 (Article XIV — Comprehensibility).** The model must remain understandable to
  future contributors; complexity is a cost weighed against every option. Governs the
  choice among options; see N1.

## Terminology

The following terms are used precisely and are **not** interchangeable.

- **Tick.** A **logical execution step** — an ordinal, not a duration. It carries no
  intrinsic real-time meaning; advancing the simulation by one tick moves it from one
  defined state to the next, and nothing more. (D1, journal 2026-07-24.) Whether world
  *durations* are expressed directly in ticks or in a distinct integer **Simulation Time
  Unit** that the tick advances is deliberately left open — defining them as necessarily
  identical would couple tick granularity to the meaning of every process in the world.
- **Simulation Time.** The internal, authoritative clock of the world: an ordinal integer
  counter of ticks. It has no fixed relationship to real time.
- **Real Time.** Wall-clock time in the host environment. Relevant only to *how fast*
  ticks are executed, never to *what* a tick produces.
- **Frame.** One iteration of the host's presentation/update loop. A frame is a
  presentation concept. It may execute zero, one, or many ticks, and must not itself
  define a tick.
- **Step.** A single, deliberate advancement of the simulation by one tick, typically
  invoked manually for inspection. "Single-step" is the debugging use of this.
- **Transformation.** A defined change to world state, applied at a specific tick as part
  of the world's rules. The atomic act the time model orders.
- **Phase.** A named stage *within* the processing of a single tick, establishing a
  coarse deterministic order among classes of transformation (for example, "sense"
  before "decide" before "apply"). Phases order transformations that share a tick.
- **Scheduler.** The component that determines which transformations occur at which tick
  and in which order. It owns the explicit schedule and the ordering rule.
- **Input.** An influence originating outside the world's own rules (an observer's
  action, an external signal), injected into the timeline at a defined tick so its effect
  is deterministic and replayable.
- **Event.** A transformation scheduled to occur at a specific future tick, as opposed to
  one recomputed every tick. Events populate the scheduler's timeline.

A note the model must respect: a **frame is not a tick**, and **real time is not
simulation time**. Conflating either pair reintroduces the host-coupling this RFC exists
to prevent.

## Options Considered

Each option is evaluated on the same axes: how it works, advantages, disadvantages, and
its impact on determinism, debugging, scalability, and consistency with Genesis
principles. Unity's conventions are noted where relevant but are not treated as evidence
for a choice.

### Option 1 — Variable delta time

**How it works.** The simulation advances by whatever interval of real time elapsed
since the previous update. Each update receives a `delta` and integrates change
proportionally. This is the default of most real-time host loops, including Unity's
standard per-frame update.

**Advantages.** Trivially smooth against a variable frame rate; minimal machinery; the
world always "keeps up" with real time.

**Disadvantages.** The size of each step is a floating, host-dependent quantity. Two
runs on different hardware, or the same hardware under different load, take different-
sized steps and accumulate different floating-point results.

**Determinism.** Fails **C1/F1** outright. Because step size is a function of host
timing, identical inputs do not produce identical results. This alone disqualifies it as
the model for the simulation.

**Debugging.** Poor. A divergence cannot be reproduced because the timeline itself is not
reproducible; "same bug again" is not achievable on demand.

**Scalability.** Neutral mechanically, but the determinism failure compounds as more
systems accumulate floating error.

**Compatibility.** Violates Determinism (IV), Observation (XI, by letting host rate shape
outcome), and Explicitness (XII, since the effective clock is implicit in host timing).
Rejected as a simulation model. It may remain relevant *only* to the presentation layer,
which is out of scope here.

### Option 2 — Fixed timestep

**How it works.** Simulation time advances in fixed, equal increments — one tick of
constant magnitude. The host executes as many whole ticks as needed to consume elapsed
real time (an accumulator pattern), or as many as a chosen rate dictates, but each tick
is identical in duration and every system sees the same constant step. Simulation time
is naturally represented as an integer tick counter.

**Advantages.** Deterministic step size; integer time representation with no rounding
drift; execution rate fully decoupled from step size, so pause, single-step, and
variable speed follow directly; straightforward to reason about and to test.

**Disadvantages.** Every tick processes the whole active world, which can be wasteful when
most of the world is quiescent; representing genuinely long-dormant processes as "checked
every tick" is inefficient and inexpressive; very fine-grained events must be quantised to
the tick.

**Determinism.** Strong. Satisfies **C1/F1**: identical inputs over identical ticks yield
identical results. Integer time satisfies the exactness half of the representation
problem.

**Debugging.** Excellent. Any moment is an integer tick; a run can be advanced one tick
at a time; a divergence can be reproduced and bisected by tick number.

**Scalability.** Adequate but not free: cost scales with active systems × ticks. Mitigable
by only updating systems that opt in, but the base model still iterates per tick.

**Compatibility.** Aligns with Determinism (IV), Observation (XI, rate is presentation-
side only), Explicitness (XII, the tick counter is explicit state), and Comprehensibility
(XIV, it is simple). Its weakness is expressing sparse, long-horizon change.

### Option 3 — Pure event-driven simulation

**How it works.** There is no fixed heartbeat. The world advances by jumping from one
scheduled event to the next; simulation time is whatever timestamp the next pending event
carries. Between events, nothing is computed because, by construction, nothing changes.

**Advantages.** Highly efficient for sparse worlds — no work is done when nothing happens;
naturally expresses long-dormant and irregularly-timed processes; time can be fine-grained
where needed.

**Disadvantages.** Ordering of events sharing a timestamp requires a carefully defined
tie-break or determinism is lost. Continuous or densely-interacting processes (diffusion,
spreading fields, mutual influence) are awkward to express as discrete events and can
produce event storms. The global schedule becomes a complex, central structure — in
tension with locality and with comprehensibility.

**Determinism.** Achievable but *conditional*: it holds only if the tie-break among
same-timestamp events is total, explicit, and stable. This is a real and easily-violated
burden. Timestamps must be integers to avoid floating comparison hazards.

**Debugging.** Mixed. "Advance to next event" is a natural step, but "what is the state at
an arbitrary moment" requires reconstruction, and event storms are hard to inspect.

**Scalability.** Excellent for sparse activity; potentially poor for dense, continuous
activity where the event count explodes.

**Compatibility.** Compatible with Determinism (IV) *if* ordering is disciplined, and with
Explicitness (XII) *if* the schedule is inspectable. It strains Comprehensibility (XIV)
via the central scheduler, and Locality (Article VII, out of this RFC's scope but relevant)
via the global timeline.

### Option 4 — Hybrid: fixed tick plus scheduled events

**How it works.** A fixed tick provides the deterministic heartbeat and the coarse
ordering frame (via phases). On top of it, a scheduler holds events keyed to specific
future ticks, so that sparse or long-horizon changes need not be recomputed every tick but
are still resolved at exact, integer moments. Dense/continuous processes run per tick;
sparse/scheduled processes run as events; both share one integer clock and one ordering
rule.

**Advantages.** Retains fixed-timestep determinism and integer time; recovers much of the
efficiency and expressiveness of the event model for sparse change; gives a single, clear
answer for ordering (phase within tick, then a defined tie-break among events in a tick);
supports both continuous and discrete phenomena without forcing one into the other's shape.

**Disadvantages.** More moving parts than a pure fixed timestep — there are now two ways a
transformation can occur (per-tick and scheduled), and the interaction between them must be
specified carefully. The scheduler is still a central structure whose ordering rules must be
explicit and total.

**Determinism.** Strong, on the same basis as Option 2, provided the event tie-break within
a tick is total and explicit (the same discipline Option 3 requires, but bounded to
same-tick events rather than the whole timeline).

**Debugging.** Strong. Integer ticks give reproducible, bisectable moments; single-step
advances one tick and resolves that tick's events; the schedule is inspectable state.

**Scalability.** Good in both regimes: dense work scales per tick, sparse work scales per
event, and neither pays the other's cost.

**Compatibility.** Aligns with Determinism (IV), Transformation (V, change is explicitly
"what happens at this tick"), Causality (VI, every event carries its scheduling cause and
its tick), Observation (XI), Explicitness (XII, both the tick and the schedule are explicit
state), and — with care about the scheduler's simplicity — Comprehensibility (XIV).

## Open Questions

> **Resolution status (2026-07-26).** Several of these were closed by ADR-0001 and the kernel
> implementation; the remainder stay open without blocking acceptance, because none is load-bearing
> for the implemented core. **Closed:** Q1 (a tick is a logical unit with no default real-time
> mapping; real time is a presentation-side playback rate), Q3 (an integer tick counter — implemented
> as a 64-bit ordinal), Q5 (no phases — ADR-0001 chose a flat set; phases remain a possible future
> extension), Q6 (per-address commutative resolvers with Reject as default — DN-001, Genesis-006),
> Q10 (moot for transitions under snapshot semantics — ordering reduces to commutative conflict
> resolution plus canonical enumeration; a tie-break for a future event layer would be specified with
> that layer). **Still open:** Q2 (selective update — subsumed by the future event-scheduling
> optimisation), Q4 (duration units — linked to the Simulation Time Unit question, D1), Q7 (input
> insertion — no external inputs exist yet), Q8 (long-running processes), Q9 (causality metadata —
> deferred to a future Causality & History RFC).

The original questions, as posed:

- **Q1 — Initial tick frequency.** What is the initial relationship between one tick and
  one unit of intended world time? Is a tick a fixed real-duration target, or purely a
  logical unit with no default real-time mapping?
- **Q2 — Universal vs. selective update.** Should every system be processed every tick, or
  should systems declare the cadence at which they wish to be invoked?
- **Q3 — Time representation.** Should simulation time be an integer tick counter only? What
  integer width is required to avoid overflow across the longest runs Genesis intends to
  support?
- **Q4 — Duration units.** Should all durations (delays, lifetimes, intervals) be expressed
  in whole ticks, forbidding sub-tick quantities entirely?
- **Q5 — Phase ordering.** What phases exist within a tick, and how is their order defined
  and made inspectable? Is the phase set fixed or extensible?
- **Q6 — Conflict resolution.** When two transformations in the same tick target the same
  state, how is the conflict resolved deterministically (last-writer by defined order,
  accumulation, rejection, or an explicit arbitration phase)?
- **Q7 — Input insertion.** By what mechanism are external inputs bound to a specific tick,
  and how is that binding recorded so replay is exact?
- **Q8 — Long-running processes.** How is a process spanning many ticks represented — as a
  per-tick system, a re-scheduling event, or an explicit stateful process object?
- **Q9 — Causality metadata.** How much cause information does each transformation retain
  (originating tick, originating cause, scheduling system), and where does the cost of
  retaining it fall? Article VI wants traceability; the model must decide how much is stored
  versus reconstructable.
- **Q10 — Same-tick event tie-break.** What is the total, explicit ordering rule for events
  scheduled at the identical tick? (The determinism of Options 3 and 4 depends entirely on
  this answer.)

## Preliminary Direction

> **Superseded by ADR-0001 (2026-07-25).** The decision went to the conservative fallback this
> section itself named: **Option 2 — uniform fixed tick** — because ADR-0001 chose uniform per-tick
> temporal scope, with scheduled behaviour expressed as *state* (due-ticks a per-tick transition
> reads). Option 4's event layer was thereby **reframed from a semantic model into a future,
> semantics-preserving performance optimisation**: it may be added the day per-tick evaluation cost
> demands it, without changing any world outcome. The standing vigilance (recorded at the ADR's
> acceptance): event-scheduling must remain an optimisation — if evaluation cost ever forces it to
> become a *model* change, ADR-0001 is revisited, not quietly bent. The original analysis is kept
> below as the reasoning of record.

The evidence in this document points toward **Option 4 — the hybrid of a fixed tick with a
scheduled-event layer** as the most promising direction. It preserves the deterministic,
integer-timed foundation that Determinism (IV) and Explicitness (XII) demand, while
answering the sparse-and-long-horizon expressiveness that a pure fixed timestep handles
poorly. It gives a single, inspectable clock and a single ordering discipline, which serves
Causality (VI) and Comprehensibility (XIV).

This is a **direction, not a decision.** Two risks must be retired before acceptance:

1. **Ordering complexity.** The interaction between per-tick transformations and same-tick
   scheduled events (Q6, Q10) must be shown to have a total, explicit, comprehensible
   ordering rule. If that rule cannot be kept simple, Option 4's comprehensibility advantage
   over pure event-driven simulation collapses, and Option 2 becomes preferable.
2. **Scheduler simplicity.** The scheduler must be shown to remain a small, inspectable
   structure. If it grows into a complex central authority, it strains the Constitution's
   preference for simplicity and locality.

The following evidence is needed to retire these risks: a written specification of the
phase model and the same-tick tie-break rule (closing Q5, Q6, Q10); and small experiments —
described below as the Validation Plan — demonstrating that a fixed tick with a modest event
layer reproduces identically across execution rates and hosts. Until that evidence exists,
Option 2 remains the conservative fallback, since Option 4 degrades to Option 2 when the
event layer is empty.

## Validation Plan

The eventual decision will be validated by tests that exercise each required property
directly. These are stated as validation obligations, not as an implementation.

- **Deterministic replay.** From a fixed initial state and a recorded input timeline, two
  independent runs must produce identical world states at every tick. Extended across
  different hosts to confirm host-independence (F1, F6, N4).
- **Pause and single-step.** A run must pause with state intact, resume without
  discontinuity, and advance exactly one tick on demand, with the post-step state matching
  the same tick reached by continuous running (F3, F4).
- **Frame-rate independence.** The same run driven under widely different host frame rates —
  and with no presentation loop at all — must produce identical results (F5, F7, N4, C4).
- **Variable execution speed.** Running the same scenario slow, fast, and as-fast-as-possible
  must yield identical world states tick-for-tick; only wall-clock duration may differ (F5).
- **Long-duration stability.** A run of very many ticks must show no representational drift,
  no ordering degradation, and no overflow within the supported range (N5, Q3).
- **Ordering.** Constructed scenarios with deliberately simultaneous transformations must
  resolve identically every run according to the defined phase and tie-break rules (F2, Q6,
  Q10).
- **Save/load continuity.** A run serialised mid-simulation and resumed must continue
  identically to a run that was never interrupted, confirming that simulation time and the
  pending schedule are fully captured as explicit state (F8, C5).

A candidate mechanism is acceptable only when it passes all seven obligations.

## Consequences

Adopting an explicit tick system has the following architectural consequences.

**Benefits.**
- World state changes at defined, inspectable moments rather than continuously or
  incidentally, giving every transformation an exact timestamp (serves IV, V, VI).
- Reproducibility becomes a structural property rather than an aspiration; bugs become
  reproducible and bisectable by tick.
- The simulation is fully decoupled from presentation timing, enabling headless runs,
  arbitrary execution speed, pause, and single-step without special cases (serves XI).
- Simulation time becomes explicit, readable state that can be saved, restored, and
  reasoned about (serves XII).

**Costs.**
- All simulation logic must be written to change state only at tick boundaries; systems may
  not mutate the world at arbitrary moments. This is a real discipline imposed on every
  future system.
- Continuous real-world phenomena must be modelled as discrete tick-quantised change, which
  is an approximation the framework accepts deliberately.
- A scheduler and (under Option 4) an event layer introduce a central structure that must be
  kept simple and inspectable, an ongoing maintenance obligation.
- Retaining causality metadata (Q9) has a storage and bookkeeping cost that must be
  consciously bounded.

These costs are accepted as the price of the properties in the requirements. They are not
incidental; they are the shape determinism takes.

## Non-Goals

This RFC does **not** define, and no reader should infer, the following:

- The representation of world state or the entity/component model.
- Spatial organisation or partitioning of the world.
- Any rendering, animation, or audio concern.
- The mapping of player or observer actions to concrete inputs (only *that* inputs are
  inserted at defined ticks, not *how* they are captured).
- Networking, distribution, or multi-process concerns.
- Final performance optimisation, memory layout, or concurrency strategy.
- The concrete phase set, tick frequency, integer width, or scheduler design — these are
  open questions above, to be closed by a later revision or a subordinate RFC.

## Decision Record

Decision: **Accepted**
Date: 2026-07-26
Rationale: The tick model this RFC defined — a logical, integer, host-independent step under Level-1
logical determinism — is implemented (`Tick`, `TickRunner`, Genesis-002..010) and proven by the
kernel's executable test suite, including deterministic 1000-tick replays, frame-independence by
construction (no presentation dependency exists), and single-step/pause as trivial consequences of
caller-driven `Run(count)`. Temporal scope was resolved by ADR-0001 as uniform per-tick (Option 2),
with Option 4's event layer reframed as a future semantics-preserving optimisation. Remaining open
questions (Q2, Q4, Q7, Q8, Q9) are recorded above and belong to future RFCs; none undermines the
accepted core. Validation-plan obligations not yet exercised (recorded-input replay, save/load
continuity) become obligations of the milestones that introduce inputs and serialisation.
