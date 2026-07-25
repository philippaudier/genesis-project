# Minimal Architecture — Adversarial Review

Status: **Research — adversarial review.** No decision. No repairs. No new model concepts.

> Third in the arc: `Transformation-Model-Critique.md` (Intent over-claims universality) →
> `Producers-Hypothesis-Critique.md` (two producers is the wrong number; likely one producing
> mechanism plus a constraint layer the framing can't see) → **this document**, attacking the most
> minimal architecture proposed so far.
>
> **Under review:**
> ```
> External Interfaces  →  World State  →  Rules  →  New World State
> ```
> Claim: External Interfaces (player, networking, scripting, replay, tooling) *provide information*
> to the world but do not produce change. The World alone applies its Rules to State to produce the
> next State. Everything previously called Intent / Agent / Producer is now either information at the
> boundary or a Rule over state.
>
> Classification vocabulary unchanged: *fatal contradiction*, *architectural weakness*,
> *implementation concern*, *acceptable trade-off*, *wording problem*.

---

## What this architecture actually is

Before attacking, name it precisely, because the name is half the review. The diagram is, exactly,
a **discrete dynamical system with input**:

```
x(t+1) = f( x(t), u(t) )        x(0) given
```

- **World State** = x — the configuration at a tick.
- **Rules** = f — the transition function carrying x(t) to x(t+1).
- **External Interfaces** = the source of **u(t)** (input during the run) and **x(0)** (the initial
  world).
- **New World State** = x(t+1).

This identification is not a criticism by itself. But it means every question below is really asking:
*is `f` a primitive, or is "Rule" just our name for the transition function of a system that has
been understood for a century?* Held in that light, the four questions almost answer themselves — so
the review's job is to find what the dynamical-system framing **omits**, because that omission is
where the remaining architecture lives.

## Question 1 — Is Rule now too broad to be useful?

Two senses, opposite answers.

**As a discipline: no, it is not too broad.** "Rule" here is not "anything that changes state." It
carries real, forbidding constraints inherited from the Constitution: a Rule is **pure** (no side
effects beyond producing next state), **deterministic** (Article IV), reads only **declared or
scoped** state (Articles VII, XII), and **local**. That rules out ambient mutation, hidden IO, and
nondeterminism. A concept that forbids that much is doing work. "Rule" is broad the way "pure
function" is broad — universal in scope, yet a genuine discipline, not a tautology.

**As a claimed primitive: yes, it is too broad — because it is not a primitive at all.** If every
internal change is a Rule, then "the world changes by Rules" reduces to "the world changes by its
transition function," which is true of *any* system that changes. The word carries no distinguishing
power at that level. What "Rules" (plural) actually denotes is a *methodology*: **factor the single
transition function `f` into local, composable fragments** (Article XIII, composition). That is a
real and good architectural commitment — but it makes "Rule" the name of *a fragment of the
transition*, one level above the primitive, not the primitive itself.

**Verdict Q1.** *Wording / architectural clarification.* Rule is a sound discipline and a poor
primitive. The primitive it is standing in front of is **transition** (`f`), and transition is only
meaningful against **state**. Rule is how we *factor* the primitive, not the primitive.

## Question 2 — Does this collapse lose important distinctions?

Two are lost, one trivial and one serious.

**Lost distinction A — local rules vs arbitration rules.** A wolf-eats-rabbit contention resolves,
in this model, as "a Rule that reads the competing claims and picks one." But that Rule must read
*all* claims on rabbit R — which is **non-local**, exactly the property every other Rule is forbidden
from having (Article VII). The flat "Rules over state" model does not distinguish a local rule from a
gather-and-arbitrate rule, so a naïve author writes a local rule that silently misses a competing
claim, and determinism or correctness breaks. Steelman: "it's still just a Rule." Rebuttal: yes, but
it is a Rule with a *different visibility and determinism profile*, and flattening that difference is
how the model invites the bug. *Architectural weakness*, moderate.

**Lost distinction B — producing vs forbidding (the serious one).** This is the finding that has
survived every collapse in the arc. Rules *produce* next state. **Constraints / invariants do not
produce — they forbid**: "energy is conserved", "no two solids in one cell", "quantity ≥ 0". Under
"Rules → New World State", where do they live?

- *Baked into every Rule* — every rule author re-implements conservation. Decentralised, unenforceable,
  violates Article XIV, and one forgetful rule corrupts the invariant globally.
- *As a correcting Rule after the others* — but a correcting rule must run *after* production and see
  its output, which reintroduces the two-phase pipeline (produce, then validate) the collapse
  claimed to remove. And correction is not always possible: rescaling repairs a conservation breach,
  but what is the "repair" for two solids in one cell? Ejecting one is a *policy*, not the
  constraint. So some constraints have no productive form at all.

A constraint is a **predicate on legal states**, not a producer of states. It cannot honestly be a
"Rule over state producing new state." The minimal architecture, by naming only Rules, has *hidden*
the constraint layer rather than absorbed it. *Architectural weakness*, fundamental — and the single
most durable finding of the entire review arc.

## Question 3 — Are there mechanisms that cannot honestly be described as Rules over state?

Three. Notably, they are exactly the parts of a dynamical system that *are not the transition
function* — which is the tell for Question 4.

- **The initial condition, x(0)** (world generation). A Rule transforms an existing state; genesis
  has none to transform. `x(0)` is *given*, and a transition function never claims to produce its own
  initial value. **Not a Rule — and legitimately so.** *Acceptable*, provided the architecture states
  that the initial world is given, not derived.

- **External input, u(t)** (player, scripting, tooling writing information in). The moment external
  information becomes state, state has changed by something that is *not a function of the prior
  world*. That is not a Rule. But it is not a violation either: it is the **input term** of the
  system, `u(t)`, legitimately outside `f`. The model survives this **only if it states honestly that
  state has two origins — evolution by Rules, and boundary information from interfaces**. If it
  claims "only Rules change state", it is simply false, because injection changes state. *Acceptable
  trade-off bordering on wording problem* — survives with an explicit boundary declaration, fails
  silently without one.

- **Constraints, I(x)** — from Q2. A predicate bounding the legal state space. Not a Rule. Not input.
  Not initial condition. A genuinely distinct kind. *Architectural weakness* (the one the model
  omits).

Note the near-misses that *are* honestly Rules: **randomness** (a Rule reading an explicit seeded
state), **scheduled events** (re-entrant products of a prior Rule), and **rule-content that changes
over time** (data the base interpreter reads — though this last quietly reveals that "Rule" splits
into a fixed *evaluator* and mutable *rule-data*, i.e. Rules-as-content are really State).

**Verdict Q3.** Three mechanisms escape "Rules over state": initial condition and input escape
*legitimately* (they are the boundary and the seed of any dynamical system), and constraints escape
*damagingly* (the model has no place for them).

## Question 4 — A genuine primitive, or a renamed transition function?

**A renamed transition function — and that is the honest and correct outcome, but it means the atom
is not "Rule".**

The architecture has not invented a primitive. It has *converged onto* the discrete dynamical
system: `x(t+1) = f(x(t), u(t))`, `x(0)` given, subject to invariants `I(x)`. This convergence is a
sign of *correctness*, not failure — a world that evolves deterministically over time simply *is* a
constrained dynamical system, and arriving there by stripping away Intent, Agent, and Producer is the
right destination reached the right way. The value produced across this arc was the *subtraction*,
not a new noun at the end of it.

But the dynamical-system framing shows precisely what "Rule" is and is not. The irreducible parts of
such a system are not one thing; they are a small set, and each fails to reduce to the others:

- **State** — what is. Remove it and there is nothing to transition. (Article V: the noun.)
- **Transition** (factored as Rules) — how state becomes next state. Remove it and nothing changes;
  Article V ranks it above state, and it cannot *be* state without something to interpret it.
- **Constraint** — which states are legal. Remove it and the two above can produce incoherent worlds;
  it forbids rather than produces, so it cannot *be* a transition.

None of the three collapses into another. State cannot be transition (Article V keeps the verb
distinct from the noun), transition cannot be state (something must interpret rule-data as change),
and constraint cannot be transition (a predicate is not a producer). So the "first true atom" is not
singular. It is a **triad: State / Transition / Constraint** — with an **input boundary** where
External Interfaces attach.

**Verdict Q4.** Not a new primitive; a rediscovery. The atom is not "Rule" — Rule is the factoring
of Transition. The bedrock is the irreducible triad plus its boundary.

---

## Verdict — have we reached the atom?

**Yes — and this is where the subtraction must stop, provably, not from fatigue.** You proposed a
test: if the next document can no longer remove a layer without losing explanatory power, the atom
has been reached. Apply it literally:

- Removing **Intent / Agent / Producer** lost no explanatory power — they were patterns, and the
  earlier reviews removed them cleanly. Good subtraction.
- Removing **Rule-as-primitive** loses no power either — it dissolves into "the transition function,
  factored," and everything still stands. This review just removed it.
- Removing **Constraint** as distinct from Rule **does** lose power: the model can no longer express
  invariants without either decentralising them into every rule (unenforceable) or smuggling back a
  validation phase (un-removing the layer). By your own criterion, the collapse has hit rock: the
  next attempted subtraction destroys a distinction reality requires.

So the honest resting point is not the single primitive the arc's momentum was reaching for. It is:

```
External boundary  →  [ State  ·  Transition (Rules)  ·  Constraint ]  →  next State
```

Three irreducible internal elements, one external boundary. The **External Interface / World**
boundary you drew is real, survives attack, and — worth noting — was *already* in the Constitution:
Article II (the player is not central) and Article XI (observation is downstream) drew exactly this
line before this arc began. That is not a coincidence to celebrate loosely; it is evidence the
boundary is load-bearing, since two independent derivations landed on it.

**The one correction to your framing:** the atom is a *triad*, not a monad. The arc's instinct to
keep collapsing is correct right up until it tries to fold Constraint into Rule — and that fold is
the one that must be refused. Genesis's conceptual bedrock is a constrained dynamical system over
explicit state, observed from outside. There is no smaller honest description that keeps invariants
first-class.

---

## Summary

| Question | Finding | Classification |
|---|---|---|
| Q1 — Rule too broad? | sound discipline; not a primitive — it factors Transition | wording / clarification |
| Q2A — local vs arbitration rules | non-local gather rules flattened into local ones | architectural weakness |
| Q2B — producing vs forbidding | constraints forbid, don't produce; model hides them | **architectural weakness** |
| Q3 — initial condition x(0) | not a Rule, legitimately (the seed) | acceptable (if stated) |
| Q3 — external input u(t) | not a Rule, legitimately (the input term) | acceptable / wording |
| Q3 — constraints I(x) | not a Rule, damagingly (no place in model) | **architectural weakness** |
| Q4 — primitive or rename? | renamed transition function; atom is a triad, not "Rule" | — |
| Boundary — External vs World | real, survives, already in Constitution (II, XI) | confirmed |

**No fatal contradiction. The architecture is a constrained dynamical system correctly rediscovered.
The atom is not "Rule" but the irreducible triad State / Transition / Constraint over an external
boundary. Subtraction should stop here: folding Constraint into Rule is the one collapse that loses
explanatory power.**
