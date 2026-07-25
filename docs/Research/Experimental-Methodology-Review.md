# Experimental Methodology — Review

Status: **Research — methodological review.** Reviews a *proposed research methodology*, not any
experiment. Proposes no mechanism, redesigns nothing, executes nothing.

> **Proposal under review:** *Every experiment exists to falsify exactly one architectural claim.*
> Each experiment is intentionally narrow; if another property needs testing it earns its own
> experiment. Motivation: mirror the ontology arc, which reached understanding by **subtraction**.
>
> The five questions posed — adopt it? strengths? risks? where does it fail? stronger programme? —
> are answered below. Short version up front: **the principle is right in spirit and wrong as
> stated.** "One falsifiable claim per experiment" is sound and should be adopted. "Exactly *one*
> claim, and every other property gets its own experiment" is both epistemically impossible in the
> strict sense and in direct tension with Genesis's own emergence thesis. The corrected form is
> given at the end.

---

## What the proposal actually asserts

Two separable commitments hide inside one sentence:

- **(a) Isolation** — an experiment targets exactly one claim, not a bundle.
- **(b) Falsification** — the experiment is designed to *refute*, not confirm.

Both are individually defensible and individually limited. The proposal also imports a third,
implicit commitment by analogy:

- **(c) Subtraction** — knowledge advances by *removing* candidates, as the ontology arc removed
  concepts.

The review separates these because they do not stand or fall together, and the strongest objection
lands on the seam between (a) and the project's philosophy.

## Strengths (credited fully)

1. **Confound elimination by construction.** This is the strongest argument and it is real — it is
   exactly what the Experiment-01 review found missing. A single-claim design has clean causal
   attribution: when the result comes in, you know *what* it is about. Bundled experiments (rate,
   arithmetic, boundary, linearity all moving at once) cannot attribute; isolated ones can.
2. **Pre-registration and decisiveness.** Forcing each experiment to name the one claim it could
   refute makes it pre-registerable and its refuting result decisive. It structurally prevents the
   confirmation-friendly, fish-for-a-win design.
3. **Results compose into a knowledge graph.** Small, independently-checkable facts compose the way
   the architecture's own Rules compose (Article XIII, composition over monolith). Each survives on
   its own and can be cited by later work — the Research equivalent of the dependency graph.
4. **Cheap, local failure.** A narrow experiment that fails, fails cheaply, and tells you precisely
   what broke rather than leaving you to bisect a tangle.
5. **Consonant with explicitness.** One claim is one explicit, inspectable hypothesis with no hidden
   bundled assumptions — the epistemic form of Article XII.

These are not small. They are the reason to take the proposal seriously rather than dismiss it.

## Risks and weaknesses (the substantive part)

### R1 — Interaction-blindness contradicts Genesis's own thesis (the central objection)

Article VIII: *systems remain simple; complexity emerges from their interaction.* The entire ontology
arc concluded that the important behaviour lives in the **interactions**, not the parts. A methodology
that tests only one property at a time, in isolation, is **structurally blind to interaction
effects** — the very place Genesis believes the interesting behaviour lives.

Concretely: conservation (Experiment-01) and stability might be *jointly* constrained — a mechanism
could conserve alone and stabilise alone, yet their combination under a particular representation
(float vs integer quanta) drifts. No experiment that tests conservation *or* stability *alone* can
surface a joint constraint. Taken as an absolute, "every property gets its own experiment" forbids the
study of coupling — and coupling is the subject Genesis most wants to understand. *This is the risk
that makes the strict form incoherent with the project.*

### R2 — "Exactly one claim" is epistemically unattainable (Duhem–Quine)

No hypothesis is ever tested alone. Experiment-01's own review showed "conservation emerges from edge
transfer" silently carries the meaning of conservation (exact vs approximate), the arithmetic model,
the boundary treatment, and the representation. A falsification always refutes the *conjunction* of
the claim and its auxiliary assumptions — never the claim in pure isolation. So "exactly one" is a
goal you approach, never reach. The methodology must therefore specify *which auxiliaries are held
fixed* for each experiment; if it pretends the claim stands alone, a falsification will be
mis-attributed to the claim when it belonged to a background assumption.

### R3 — Falsification alone never builds (the confirmation gap)

Popper's asymmetry: you can refute, never confirm. A programme built purely on falsification produces
a growing graveyard of dead claims and *no positive design*. Subtraction narrowed the ontology because
ontology is the identification of essence — removing the inessential converges on a core. But mechanics
is **selection and construction**: at some point Genesis must *choose* a mechanism and *build* it, and
that is a constructive act falsification cannot perform. The subtraction analogy breaks precisely at
the transition from "understanding what Genesis is" to "deciding how it moves." A methodology that only
falsifies can indefinitely defer the decision it exists to inform.

### R4 — Separability is itself an untested theory

To isolate one claim you must already believe the variables *are* separable and that this one is the
one that matters. That belief is a theory, and the methodology provides no way to test it. If the
separability assumption is wrong (the properties are genuinely coupled — R1), every isolated result is
misleading, and the methodology cannot detect its own founding error. Single-claim design *presupposes*
a decomposition it cannot validate.

### R5 — Over-pruning: local falsification read as global death

A claim falsified in a narrow setup may discard a mechanism that would have thrived in another regime.
If Experiment-01 falsifies "node conserves under rounding," subtracting node entirely would be an
error — node may conserve perfectly under integer quanta (a *different* experiment). Subtraction as a
reflex becomes amputation. The methodology needs an explicit rule: **a claim is falsified only within
its stated scope, never globally.** Without it, the elegance of subtraction produces premature loss.

### R6 — Combinatorial cost, and local optima

If every property × every mechanism × every relevant interaction earns its own experiment, the count
explodes, and the per-experiment overhead is not amortised the way a comparative design amortises it.
Worse, optimising each property in isolation risks selecting components that are individually best and
jointly incompatible — the best parts rarely make the best whole. Narrow experiments cannot see the
gestalt; something eventually must.

### R7 — Narrow experiments falsify *mechanism* claims, rarely *architectural* ones (Lakatos)

A mature programme has a protected **hard core** (here, the triad, declared un-relitigated) and a
**protective belt** of auxiliary hypotheses (mechanisms, representations). Single narrow experiments
almost always land on the belt, not the core — because the core is *insulated* by the belt. So the
claims most deserving the name "architectural" are the ones a single experiment can least falsify;
architecture is revised by the **accumulated weight** of many mechanism-level falsifications, not by
one. Calling each experiment's target "an architectural claim" is therefore aspirational: in practice
these experiments falsify *mechanics*, and architecture shifts only when a pattern emerges. This is
worth naming so the programme does not expect one experiment to overturn the triad.

## Where the principle fails (Q4, consolidated)

- **Irreducibly interactional phenomena.** A global constraint that couples all cells (e.g. fluid
  incompressibility) *is* a claim about coupling; it cannot be decomposed into one-property tests
  without destroying the thing being studied (R1).
- **When auxiliaries dominate.** Where the result is driven more by held-fixed background (rounding,
  representation) than by the named claim, the "single claim" is a fiction and the falsification
  mis-attributes (R2).
- **At the decision boundary.** When the programme must stop eliminating and start building, no
  falsification experiment can make the selection; that is an engineering act (R3).
- **When cost exceeds yield.** When the information from isolating a claim is worth less than the
  overhead of a dedicated experiment, comparative design is simply more efficient (R6).

## Would it produce a stronger research programme? (Q5)

**Stronger in rigour and attribution; weaker in interaction-coverage and constructive power.** Not a
binary win. The strongest programme is not "narrow only" and not "broad comparative only" — it is a
**layered** one:

- a base of **narrow falsification experiments** for clean attribution and decisive refutation (the
  proposal's real contribution — and the fix for Experiment-01's confounds);
- deliberate **interaction experiments** whose single claim is *about a coupling* ("conservation and
  stability are jointly satisfiable under representation R"), which are first-class, not exceptions —
  without them the programme is blind to exactly what Article VIII says matters;
- occasional **synthesis / selection experiments** at decision points, which are explicitly
  constructive and comparative, and which no amount of falsification can replace.

A programme of only the first layer would be rigorous and, on Genesis's own terms, incomplete.

## Recommendation

Adopt the principle in its corrected form. The error is not "one claim per experiment" — that is
sound. The error is equating **one claim** with **one property**. A claim about the interaction of two
properties is still *one claim*. The keeper is:

> **Every experiment states exactly one falsifiable claim and the scope in which it is valid, and
> bundles no claim it cannot separately attribute. A claim may concern an interaction. Experiments
> inform decisions; they do not make them.**

This preserves the anti-confound discipline the Experiment-01 review demanded, permits the interaction
studies the emergence thesis requires, forbids the over-pruning of R5 (scope is stated), and is honest
about R3/R7 (selection is an act experiments feed but do not perform — in Genesis terms, an RFC, not
an experiment, is where a mechanism is chosen).

Two operational riders follow from the review and are offered for the user's decision, not imposed:

- **State auxiliaries explicitly** in each experiment (what is held fixed), so a falsification can be
  attributed to the claim rather than to background (R2).
- **Falsification is scoped, never global** — a dead claim is dead *in its stated regime*; killing a
  mechanism outright requires the pattern of several experiments, or an explicit decision (R5, R7).

If this principle is adopted, it is not a Research note but a rule about how the project works, and it
should **graduate to a Decisions/ record** (an architectural decision on experimental method),
alongside the RFC conventions — exactly as any other durable process commitment does. This review does
not enact that; it recommends it.

---

## What this review is and is not

It **is** a methodological assessment: the strengths that make the proposal worth adopting, the risks
(interaction-blindness, Duhem–Quine, the confirmation gap, untested separability, over-pruning,
combinatorial cost, hard-core insulation), the regimes where it fails, and a corrected principle. It
**is not** a redesign of Experiment-01, a mechanism proposal, or an engineering decision. The single
most important finding: the strict form of the proposal contradicts Article VIII, and the correction —
*one claim, which may be an interaction* — is what removes the contradiction while keeping everything
the proposal was right to want.
