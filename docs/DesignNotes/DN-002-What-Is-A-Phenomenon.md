# DN-002 — What is a Phenomenon?

Status: **Discussion.** This note decides nothing. It defines the threshold that separates **engine
properties** from **world behaviour**, so that Genesis-011 cannot answer a design question by
accident. As DN-001 cleared the ground before conflicts, DN-002 clears the ground before the first
phenomenon — but where every previous Design Note prepared an architectural decision, this one
prepares an **experiment**.

## Context

Genesis-010 completed the computational kernel. The engine now possesses: immutable snapshots; scoped
observation; explicit transitions; explicit contributions; deterministic conflict resolution;
addressable state; explicit relations; deterministic relational views.

No milestone has yet demonstrated behaviour whose *existence* depends on those laws. Until
Genesis-010, the witness counters existed only to prove properties of the engine itself.

The next milestone changes the nature of the project. Genesis stops asking *how the engine works* and
begins asking *what the engine can sustain* — the manifesto of Phase 1, already written at the close
of the Kernel Record: *from here, Genesis stops inventing its laws and begins discovering their
consequences.*

This note does not choose a phenomenon. It defines what qualifies as one.

## Candidate definition

A **phenomenon** is a behaviour emerging from the interaction of multiple local transitions operating
under the kernel's laws.

A phenomenon is **not**: a new engine mechanism; a special-purpose algorithm; a scripted sequence;
global orchestration. It is a consequence of rules already present.

## Necessary properties — and how each is recognised

A candidate phenomenon must satisfy all five. Per ADR-0002, a property that cannot be checked is an
assertion, not a criterion — so each carries its operational test.

### 1. Structural dependence

Removing the relation structure destroys the behaviour. The phenomenon depends on connectivity, not
coincidence.

*Recognised by:* the **ablation test** — run the identical transitions on the identical initial
state with an **empty RelationSet**. The behaviour must vanish (the observable pattern must not
form). This is the sharpest falsifier the kernel offers, and it is fully executable.

### 2. Locality

Every transition observes only what its declared relational view permits. No transition possesses
global knowledge.

*Recognised by:* construction plus inspection — every participating transition declares bounded
`ReadScope`/`RelationScope` (no scope enumerating "everything"), and the kernel already enforces that
nothing outside the declaration is observable. The check is that the *declarations themselves* are
local, since the enforcement is already structural.

### 3. Emergence

No transition describes the global behaviour. The observable global pattern exists only because many
local computations interact.

*Recognised by:* an asymmetry of vocabulary — the global pattern (equalisation, arrival, spread…) is
named **only in the tests that observe it**, never in simulation code. If any single transition's
code states the global outcome, the milestone has scripted a result, not grown one. This criterion is
review-level, not test-level, and is honestly recorded as such.

### 4. Determinism

The same initial snapshot always produces the same evolution. The phenomenon inherits the kernel's
guarantees.

*Recognised by:* the kernel's standing pattern — identical runs compared strictly, tick-for-tick and
end-state, exactly as every milestone since Genesis-004.

### 5. Mechanism neutrality

The phenomenon introduces no new computational primitive. It must be expressible entirely through
snapshots, relational views, transitions, contributions, and conflict resolution.

*Recognised by:* the diff — Genesis-011 adds transitions and tests, and touches **no kernel type**.
If a new primitive becomes necessary, the milestone is no longer demonstrating the kernel; it is
extending it, and must stop and say so (ADR-0002: no silent decisions).

## The vocabulary constraint

Genesis-011 introduces no new nouns. The counters remain counters; there is no `HeatField`, no
`Temperature`, no `Pressure` — the phenomenon precedes the vocabulary, as the laws preceded the
world. Naming a phenomenon's domain is a later act, done deliberately, once there is something real
to name.

## Candidate witnesses

Possible witnesses include: value copy; diffusion; averaging; propagation; signalling.

This note deliberately selects none — their suitability belongs to the Genesis-011 design
discussion. One neutral observation is recorded for that discussion: a field-like candidate (one
where a conserved or quasi-conserved quantity redistributes) would additionally begin testing
ADR-0001's one-mechanism bet — the research arc's W1 question — while a signal-like candidate would
not. That is a reason to weigh, not a decision.

## What Genesis-011 must prove

The milestone should prove a statement equivalent to:

> **Genesis-011 proved that the existing kernel can produce a deterministic emergent phenomenon
> whose behaviour disappears when the relation structure is removed.**

The specific phenomenon is secondary. The dependency on the kernel is primary — and the ablation
clause belongs *inside* the proof sentence, because it is what distinguishes a phenomenon from a
computation that merely happens near some relations.

## Non-goals

This note does not define: fields; spatial semantics; terrain; entities; biology; fluids; heat;
networking; AI. It defines only the threshold separating engine properties from world behaviour.
