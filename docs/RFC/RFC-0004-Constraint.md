# RFC-0004 — Constraint

Status: Draft

**Origin: Observation-004 · Observation-005** — this is the first RFC in Genesis's history demanded
by an observation rather than conceived by a designer. The hierarchy *Observation → Measurement →
Journal → RFC* completes its first full path here.

## Purpose

For thirteen milestones Genesis has described what exists, what changes, and how changes compose. The
observations of 2026-07-26 demonstrated that a family of properties is still missing: the world can
produce a state, but it cannot say whether that state is **legal**.

This RFC does not propose an improvement. It **acknowledges an independence**. It defines what a
Constraint is, what it is not, and deliberately stops before deciding anything about behaviour.

## The demonstrated origin

A world exhibited a negative quantity and a perpetual oscillation fed by it (Observation-004), and a
factor battery located the phenomenon within a three-regime stability structure (Observation-005).
The question the world posed — *can "Quantity ≥ 0" be expressed with the existing primitives?* — was
subjected to the **Reduction Test** (`docs/Journal/2026-07-26-the-fossil-and-the-reduction-test.md`).
Four reductions were attempted; each fails structurally, and each failure destroys a proven theorem
if forced:

| Attempted reduction | Killed by |
|---|---|
| Guard inside the law | Composition (F3) — snapshot isolation prevents joint-outflow knowledge |
| Guard inside the resolver | Commutativity by design — resolvers fold deltas blind, on conflicts only |
| Guardian transition | One tick too late — the illegal state has observably existed |
| Paired rejection at commit | D1 — no primitive has cross-cell atomicity; adding it *is* the primitive |

Therefore:

> **The legality of states is reducible neither to Transitions, nor Contributions, nor Resolvers,
> nor Relations.** Expressing it with current primitives would require un-proving something.

The independence is not asserted. It is demonstrated.

## Provisional definition

> **A Constraint is a predicate over world states.**

Nothing more. Nothing less.

A Transition answers: *what does the world become?* A Constraint answers: *is the world obtained
permitted?* The difference is ontological, not algorithmic.

One precision keeps this primitive distinct from what Genesis already has: DN-003's recognisable
patterns are also predicates over state — **descriptive** ones, which earn names. A Constraint is a
**normative** predicate: its truth-value carries the standing of legality, declared as world content.
The form is shared; the status is not. Recognition says *this is a Source*; Constraint says *this
state is not allowed to exist*. Genesis now distinguishes predicates that describe from predicates
that legislate — and only the latter are this RFC's subject.

## What a Constraint is not

Not a Transition, not a Rule, not a Resolver, not a Contribution, not a Relation, not an Observation.
Every one of those expresses a production or a connection. A Constraint produces nothing.

## Theorems carried

**First theorem — *a predicate is not a producer*.** No longer a philosophical intuition: the
research arc derived it a priori (the triad's irreducibility, 2026-07-24), and Observations 004/005
supplied its first experimental specimen. The a-priori argument and the empirical failure are the
same argument, met from both ends.

**Second theorem — a Constraint never participates in producing the next state.** It never describes
`S → S'`; it describes only `Legal(S)` or `Illegal(S)`. Any Constraint that acted would thereby
become a Transition, and the independence this RFC acknowledges would collapse.

**The property no other primitive has — silence.** A Transition acts; a Relation connects; a
Resolver composes. A Constraint may do *nothing at all*. It observes the world's states and holds a
truth-value. That silence is not a limitation; it is the definition.

## The question this RFC refuses to answer

*What happens when a Constraint is violated?*

**Nothing.** That is not its job. It does not correct, reject, roll back, saturate, or repair. It
states. Everything that follows a violation — rejection, rollback, clamping, atomic validation,
repair — is a **policy decision**, not an ontological one, and belongs to a separate, future
decision. The world asked only: *can you express that a state is illegal?* It never asked: *what do
you do when it is?* Fusing those two questions is exactly the trap this RFC exists to avoid — three
paragraphs into a "Constraint Layer" RFC written six months ago, rollback would already have
appeared.

Naming a responsibility is more important than deciding its implementation.

## First example

Constraint: `Quantity ≥ 0`. Result: `true` or `false`. Nothing more.

## Requirements (on any eventual realisation — not decided here)

- **R1 — Objectivity.** A Constraint is deterministically derivable from declared world content, like
  everything else in Genesis (kinship with DN-003's first criterion).
- **R2 — Silence.** Evaluating a Constraint mutates nothing, contributes nothing, schedules nothing.
- **R3 — Explicitness.** Constraints are declared world content — never configuration hidden in the
  law-set, never logic buried in an observer (the Article XII discipline, applied to legality).
- **R4 — Independence from policy.** The primitive must be definable, declarable, and evaluable
  without any violation-handling policy existing.

## Open questions (deliberately deferred, catalogued so none can sneak in)

- **Q1 — Violation policy.** What follows illegality — rejection, rollback, saturation, repair,
  tolerance? A separate RFC, after this one is settled.
- **Q2 — Evaluation moment.** When is a Constraint consulted — every tick, at commit, on demand, by
  observers? Undecided.
- **Q3 — Declaration.** Who declares Constraints, and where do they live relative to laws and state?
- **Q4 — Scope.** Are Constraints cell-scoped, kind-scoped, place-scoped, or global — and do they
  declare reads like transitions do?
- **Q5 — Constraints versus theorems.** Conservation needs no Constraint — it is guaranteed by the
  algebra (paired contributions + additive resolution). Constraints exist for what no law-discipline
  can guarantee under free composition. The boundary between *guaranteed-by-construction* and
  *legislated-by-constraint* deserves its own examination (the resolver-algebra conjecture, Journal
  2026-07-26, is adjacent).
- **Q6 — Observability of legality.** Is a Constraint's truth-value itself observable state?

## Kill criterion

This RFC dies if anyone exhibits a reduction: an expression of state-legality in the existing
primitives that preserves every proven theorem (order-independence, commutativity, D1, snapshot
isolation, conservation). The Reduction Test's failure is this RFC's foundation; refute the failure
and the fossil returns to the drawer.

## Non-goals

No implementation, no API, no evaluation semantics, no violation policy, no changes to kernel,
Glossary (Constraint remains **Reserved** until a realisation earns its activation), laws, or the
laboratory. Genesis-014's implementation milestone is gated on this RFC's acceptance *and* on the
follow-up decisions it deliberately defers.

## Dependencies

- **Depends on:** Observations 004 & 005 (the specimens); the Reduction Test record (Journal
  2026-07-26); the ontology arc's triad (Minimal-Architecture-Critique — the a-priori prediction);
  DN-003 (the descriptive/normative predicate distinction); the Constitution (Articles VIII, XII,
  XIV).
- **Depended on by** (intended, speculative): the Violation-Policy RFC (Q1); any future world whose
  laws cannot guarantee its legalities.

## Decision Record

Decision: Pending
Date: —
Rationale: Drafted from the field. Acceptance acknowledges legality as an independent concern of the
world — and nothing else.

---

> **This RFC does not introduce legality.**
>
> **It merely acknowledges that legality exists as an independent concern.**
