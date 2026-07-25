# 2026-07-24 — Deliberation on RFC-0001 and the nature of the Tick

Context: first cold review of the RFC-0001 Draft. This entry records decisions and open
forks. **RFC-0001 itself was left untouched, in Draft, resting deliberately.** These notes
are to be folded in on the next pass, not before.

## Decisions reached

### D1 — A Tick is a logical unit, not a duration (closes Q1, Q4)
A tick has no intrinsic real-time meaning. It is the next step of the simulation, nothing
more. Execution rate (10 Hz, 1000 Hz, paused, stepped, ×1000) is a property of *observation*,
never of the world. Durations inside the world are expressed in whole ticks only; there are
no sub-tick quantities.

Consequence to note when folding in: because a tick carries no real duration, every rate in
the world (how many ticks make a day, a season, a lifetime) becomes an explicit design
constant. The world's *internal proportions* are now first-class content that must be kept
consistent. This is acceptable and correct, but it should be stated, not discovered later.

Open sub-point (naming): whether to keep "Simulation Time" (an ordinal integer counter) or
rename to "Simulation Progression." Coining a new term has a comprehensibility cost
(Article XIV). Not decided here.

### D2 — Causality is not part of RFC-0001; it is its own future RFC (extracts Q9)
Two distinct concepts were being conflated:

- **Enacted (simulation) causality** — the mechanism by which a transformation reads causes
  from state and writes new state. Intrinsic to how rules work. *Not stored* — it is enacted,
  then gone. Always present.
- **Recorded (historical) causality** — a durable, queryable provenance record ("this
  happened because of that"). Optional, toggleable, potentially very rich.

Key insight: **Article VI (Causality) is satisfied by enacted causality plus determinism.**
Every transformation already has an enacting cause; and because runs are deterministic and
replayable, the cause of any event can always be *re-derived* by replay. Therefore recorded
history is an optimisation for *understanding*, not a Constitutional obligation. A Release
build that keeps no history does not violate the Constitution.

This is why it deserves a full RFC (touches debug, replay, save, narration, visualisation,
performance) — not because it is hard, but because it spans subsystems. RFC-0001 should
retain only the minimal requirement: every transformation is enacted by a cause, and is
locatable to a specific tick.

## Process decisions

### P1 — Kill Criteria become a standard RFC section
Every RFC states its preferred direction *and* the explicit conditions under which that
direction is abandoned. Guards against sunk-cost commitment.

### P2 — Dependencies section becomes standard
Each RFC declares "Depends on" (upstream that must be stable first) and "Depended on by"
(known intended downstream consumers). This turns the RFC set into a navigable graph. Note:
"Depended on by" is speculative and will drift — treat it as intent, not contract.

### P3 — RFC numbering: assigned on creation, never reserved
"Causality" is not RFC-0005 yet; it is a named, unnumbered future RFC. Numbers are handed out
sequentially when a Draft is created, so the sequence stays honest.

### P4 — Consider a process document / template
An `docs/RFC/` template or process doc codifying: Status lifecycle (Draft → Review →
Accepted), the standard sections, Kill Criteria, Dependencies, Decision Record. Not yet
created.

## Open architectural fork (deliberately undecided)

Is **Transformation** more fundamental than the **Tick**?

Working analysis: State and Transformation are co-primary and mutually defining (Article V:
state is transformation caught mid-sentence). The Tick is *not* a peer of these — it is the
discipline that makes the application of transformations ordered, atomic, and deterministic.
Evidence it is machinery rather than foundation: the Tick appears nowhere in the Constitution;
Article V ranks transformation above state explicitly.

But Tick and Transformation are entangled: the Tick is precisely the answer to *when* a
transformation takes effect and *in what order* simultaneous ones resolve. So "Transformation
is more fundamental" does **not** shelve RFC-0001. Possible restructure to weigh on the next
pass: a dedicated "Transformation" RFC that establishes what a transformation *is* (a function
of state + inputs → new state, its causal inputs, its atomicity), which the Tick RFC then
depends on. To be discovered through the RFC process, not declared now.

## Second pass (same day) — three corrections raised on cold review of RFC-0001

These amend the notes above; **RFC-0001 still untouched and resting.**

### D1-amendment — separate execution step from world duration
Refines D1. Keep "Simulation Time = ordinal integer tick counter" (no coined "Simulation
Progression"). But do **not** define "a tick has no duration" and "all durations are expressed
in ticks" together — jointly they create a hidden coupling where changing tick granularity
silently rescales every process in the world. Leave open the possibility of two distinct
concepts:
- **Tick** — a logical execution step (ordinal).
- **Simulation Time Unit** — an integer unit of *world duration* that the tick advances.

They may be identical at first, but should not be *defined as necessarily* identical. World
durations (digestion = 400, growth = 2,000,000, combustion = 30) may belong to the duration
unit, not the execution step.

### D3 — determinism has three levels; "bit-for-bit, machine-for-machine" is too strong
The current RFC wording over-promises. Split the guarantee:
- **Level 1 — Logical determinism.** Same initial state + same inputs ⇒ same decisions and
  same transformations. Genesis **must** guarantee this. (Non-negotiable; underpins F1/F6.)
- **Level 2 — Platform determinism.** Identical result on a given platform + runtime version.
  Strongly desired.
- **Level 3 — Cross-platform bit-identical.** Identical binary result on all supported
  machines. **Open question** — depends on numeric types, supported platforms, and the future
  concurrency strategy. Must NOT be decided in RFC-0001; it belongs to a future
  numeric-guarantees RFC.

Action for next pass: soften RFC-0001's determinism claim to Level 1 (required) + Level 2
(desired), and explicitly defer Level 3.

### Note — single-transformation-pipeline thesis
Correction that "there are two ways a transformation can occur" is a danger. Preferred framing:
*one way to transform the world; many ways to schedule the evaluation of a transformation.*
Per-tick processes and scheduled events both **submit** transformations to one pipeline; neither
mutates the world directly. This preserves one atomicity, one conflict rule, one traceability,
one validation path — and reinforces Transformation as the atom. This thesis is explored (not
decided) in `docs/Research/Transformation-Model.md`.

## Third pass (same day) — the Intent layer

Reviewing the Research doc, a concept surfaced *before* Transformation: **Intent**. Woven into
`docs/Research/Transformation-Model.md`. **Constitution and RFC-0001 untouched.** Summary:

- **Two altitudes.** *Intent* = a request for change, in the actor's vocabulary ("Move North"),
  refusable/modifiable/mergeable/deferrable without touching the world. *Transformation* = the
  applied atomic fact, in world-state vocabulary ("position: x→y"), always past-tense. Validation
  is the translation between them.
- **Resolves Q1.** "Change as data vs act" collapses: the data-proposal is the Intent, the applied
  act is the Transformation. Only Q4 (mutate vs produce-new-state) remains of the old Q1.
- **Propose/validate/apply belong to the Intent**, not the Transformation. Accepted Intents
  *produce* Transformations.
- **New open question — cardinality.** Intent→Transformation is not 1:1: an accepted Intent may
  yield 0, 1, or many Transformations.
- **Hidden coupling found.** Intent-based conflict resolution (two wolves, one rabbit) requires
  collecting all intents in a tick before applying any — which *is* the snapshot/simultaneous model
  (Q6). So Intent leans Q6 toward snapshot. Flagged, not decided.
- **Cascades reframed (Q3).** Consequences may re-enter as new Intents (validated, next tick →
  tick = quantum of causal depth) or bind as Transformations (same commit). Probably both; the
  dividing criterion is open.
- **Q2 gains a third path.** Declared / **Scoped** / Ambient. Scoped ("neighbourhood around entity
  X") respects Locality and scales; caveat — weaker parallelism guarantee than an exact read-set
  (overlapping scopes must be assumed to conflict).
- **Meta-observation.** "The world exists only through the transformations that pass through it" is
  not new — it restates Article V's second sentence. Discipline holds: do not edit the Constitution;
  let the RFCs reveal it was already there.
