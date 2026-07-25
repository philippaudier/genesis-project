# Experiment 01 — Scalar Diffusion on a Regular Grid

Status: **Closed — superseded (2026-07-26). Never executed.**

> **Reason.** The question this experiment was designed to answer — which transfer mechanism Genesis
> should adopt — was resolved by ADR-0001 (the Snapshot Transition Model) and the validated kernel
> (Genesis-004..010). Running it now would compare candidates against a decision already made and
> proven. The best experimenter also knows when an experiment is no longer necessary; closing it is
> more honest than freezing it.
>
> What remains valuable is preserved: the conserved-fields questions this protocol probed (grain,
> conservation-as-structure-vs-constraint, stability) return with the first field milestone — where
> they will test ADR-0001's one-mechanism bet directly, under a fresh protocol written against the
> kernel that now exists.

*Original design, preserved as written:*

> This document designs the first mechanics experiment of the transition-mechanics arc
> (`Transition-Mechanics-Exploration.md`). Its purpose is to convert an architectural discussion
> into a falsifiable experimental program: to define a phenomenon, the invariants any faithful model
> must respect, the competing mechanisms and their differing predictions, the observable criteria
> that separate them, a by-hand experiment, and — for each candidate — the exact observation that
> would refute it.
>
> **The goal is not to design Genesis. The goal is to learn something true** about how a conserved
> scalar redistributes on a grid under local rules — a truth that should hold regardless of what
> Genesis is later built to be. This document does **not** solve the experiment; it only designs it.

---

## 1. The phenomenon

A single non-negative scalar quantity — read it as heat, concentration, or "amount of stuff" — is
distributed unevenly across the cells of a regular grid. Left to evolve, it spreads: quantity moves
from where there is more to where there is less, and the distribution tends over time toward
uniformity. Nothing is created or destroyed in the interior; the grid is a **closed system** with
no-flux boundaries, so quantity cannot leave or enter except through explicitly declared sources or
sinks.

The phenomenon is defined here by its **qualitative, observer-independent behaviour**, deliberately
*not* by any particular equation, because each candidate mechanism is itself a different
discretisation and privileging one equation would prejudge the comparison. A faithful model of this
phenomenon is one that exhibits, at minimum:

- **Redistribution down-gradient.** Quantity moves from higher-valued cells toward lower-valued
  neighbours, never spontaneously up-gradient.
- **Tendency to equilibrium.** From any closed initial state, repeated steps approach a uniform
  distribution (equal in every cell) and remain there once reached.
- **Conservation.** The total quantity summed over all cells is invariant across a step.
- **The maximum principle.** No cell's value rises above the initial maximum or falls below the
  initial minimum of the closed system; in particular, no cell becomes negative.

These four are the phenomenon. Everything below asks which mechanisms preserve them, under what
conditions, and how we would *observe* the answer rather than argue it.

## 2. Properties every candidate mechanism must preserve

Two families: **physical** (fidelity to the phenomenon) and **architectural** (fitness for the
Genesis working hypothesis — the triad of State · Transition · Constraint over an external boundary).
Each is stated *operationally*, as something an experimenter can check, not as a virtue.

**Physical**

- **P1 — Conservation.** The sum of the field over all cells before a step equals the sum after,
  exactly. "Exactly" is meant literally and will be tested against exact by-hand arithmetic, and
  again under deliberately rounded arithmetic (see §4).
- **P2 — Positivity.** No cell holds a negative amount at any point.
- **P3 — Bounded / monotone approach (stability).** Values remain within the initial [min, max]; the
  total variation of the field does not grow; the sequence does not oscillate or diverge.

**Architectural**

- **A1 — Determinism (L1, logical).** Two independent executions that follow the specification
  produce identical logical states. This is distinct from bit-identical floating arithmetic (L3,
  deferred): L1 asks whether the *defined procedure* has a single outcome.
- **A2 — Locality.** A cell's next value depends only on a bounded neighbourhood of the current
  state, not on the global field.
- **A3 — Explicitness.** Every quantity that influences the step is part of declared state; nothing
  is read from an undeclared or ambient source (Constitution, Article XII).
- **A4 — Composability.** The mechanism can run in the same step alongside an unrelated Rule (a
  second field, a source, an independent cell update) without the two Rules needing private knowledge
  of each other.

A candidate need not be assumed to satisfy all six. The experiment's value is precisely in observing
*which* it preserves and *which it sacrifices*, and under what conditions the preservation fails.

## 3. Candidate mechanisms and their predictions

Three mechanisms are compared. **None is chosen.** For each, only its *conceptual form* and its
*predicted behaviour* on the properties above are stated — no update equations, no implementation.

### C-N — Node-based transfer

Each cell computes its next value from its own current value and those of its neighbours, by a fixed
local rule, all cells reading the same start-of-step state (a simultaneous, "snapshot" update).

*Predicts:* Local (A2) and explicit (A3) by construction; composable (A4) because each cell's update
is self-contained. Conservation (P1) is **analytic, not structural** — it holds because each pair of
neighbours exchanges equal-and-opposite amounts, a cancellation that depends on symmetric
coefficients, on snapshot reads, and on *exact* arithmetic. The prediction is therefore sharp:
conservation should hold exactly by hand, but should be **vulnerable** to (i) reading updated rather
than snapshot values, and (ii) rounding. It is an **explicit** scheme, so it predicts a **stability
threshold** (P3/P2): above some transfer rate, cells overshoot, oscillate, or go negative.

### C-E — Edge / flux-based transfer

The unit of change is the flow across each edge between two adjacent cells. A single flux value is
computed once per edge and then subtracted from one cell and added to the other.

*Predicts:* Local (A2) and explicit (A3). Conservation (P1) is **structural** — because each edge's
flux is one number removed from one cell and added to another, the total cancels *by construction*,
regardless of arithmetic or of the order edges are processed. This is the strongest conservation
prediction of the three, and the one most sharply falsifiable. However, structural conservation says
nothing about **positivity** (P2): a large flux can still drive a cell negative, so C-E predicts it
conserves *even while* it may violate positivity — a deliberate dissociation of two properties that
node reasoning tends to conflate. Like C-N it is explicit, so it predicts the same kind of
**stability threshold**. Composability (A4) is predicted intact. One subtle prediction: individual
cell values are order-independent *in exact arithmetic*, but under rounding the *order in which
fluxes are summed into a cell* may perturb low-order bits — so L1 determinism (A1) predicts a
requirement for a *fixed summation order*, even though the *total* stays exactly conserved.

### C-G — Global solve

The whole field is advanced at once by solving a single system relating every next-state value to the
current field (an implicit / whole-grid solve).

*Predicts:* **Not local** (A2 sacrificed) — after one step, a change in any one cell's initial value
influences every other cell. Conservation (P1) depends on the structure of the global operator, not
on local cancellation. Its distinguishing prediction is **unconditional stability** (P3/P2): unlike
the explicit schemes, it predicts *no* transfer rate produces oscillation, overshoot, or negativity.
Composability (A4) is predicted **poor**: an independent Rule acting in the same step cannot simply
be added, because the solve must account for it inside the system. Determinism (A1) is predicted to
depend on the solve method (a direct solve is determinate up to arithmetic; an iterative solve with a
convergence tolerance introduces a nondeterminism risk).

### The design logic (why these three)

The three properties that matter most each **partition the candidates differently**, which is what
lets the experiment attribute an outcome to a cause rather than to a coincidence:

| Property | C-N node | C-E edge | C-G global |
|---|---|---|---|
| Conservation basis | analytic (fragile) | structural (robust) | operator-dependent |
| Stability | conditional | conditional | unconditional |
| Locality | local | local | non-local |

Conservation separates node from edge. Stability separates explicit (node, edge) from implicit
(global). Locality separates global from the rest. Because no two properties partition the set the
same way, any observed difference can be traced to the property responsible for it.

## 4. Objective evaluation criteria

Each is an **observable outcome**, recorded as data, not a judgement. For a given trial the
experimenter fills in the answer; opinions do not enter.

- **E1 — Total invariance.** Record Σ(field) before and after each step. Criterion: is Σ_after =
  Σ_before *exactly*? Run twice — once in exact arithmetic, once with values deliberately rounded to a
  fixed few digits after each transfer — and record whether the answer changes between the two.
- **E2 — Order independence.** Re-run the identical step with the cell-visiting order (C-N) or
  edge-visiting order (C-E) permuted. Criterion: does any cell's recorded value change?
- **E3 — Cross-execution identity (L1).** Have the step carried out twice, independently, strictly
  per the specification. Criterion: do the two logical states agree cell-for-cell?
- **E4 — Composition.** Add an independent second Rule (a fixed source injecting a known amount into
  one cell — i.e. an external input `u(t)` at that cell). Criterion: does the diffusion result match
  what each mechanism produced without the source, plus the source's contribution — with no
  coordination between the two Rules? Record whether conservation (now counting the injected amount)
  still holds.
- **E5 — Instability existence.** Increase the transfer rate across a sequence of trials. Criterion:
  does there exist a rate at which a cell goes negative (violates P2), or at which values overshoot
  the initial [min, max] or oscillate (violate P3)? Record the smallest rate at which it first occurs,
  if any. *(The threshold value is an experimental unknown to be measured, not stated here.)*

These five criteria, crossed with the three candidates, are the entire result surface. Nothing else
needs to be judged.

## 5. The smallest paper experiment

All trials are hand-executable. Fixed (controlled) conditions unless a trial explicitly varies them:
**von Neumann (4-neighbour) connectivity; closed, no-flux boundaries; a single scalar field;
non-negative initial values.** The independent variables are: the mechanism (C-N / C-E / C-G), the
transfer rate, the update order, the arithmetic mode (exact / rounded), and the presence of the
second Rule.

**Trial 1 — Conservation & symmetry (core).**
A 3×3 grid. Initial condition: the centre cell holds 12, every other cell holds 0. Perform **one**
step at a small transfer rate. *Record:* every cell value after the step; Σ before and after (E1);
whether the four edge-adjacent cells are equal to one another and the four corners equal to one
another. The four-fold symmetry of the setup makes this a free order-independence check: if visiting
order mattered, the symmetry would break (E2). Repeat the step in **rounded** arithmetic and record
whether Σ still matches (E1, second half).

**Trial 2 — Order dependence (probe).**
A 1×5 line of cells (closed at both ends). Initial condition: the leftmost cell holds 12, the rest 0
— an asymmetric gradient chosen so that a "read updated values as you go" (sequential) pass and a
"read the snapshot" (simultaneous) pass are *capable* of disagreeing. Perform one step each way and
record whether the two passes agree, and whether each conserves Σ (E1, E2, E3). This is the trial
that exposes the Jacobi/Gauss–Seidel distinction concretely.

**Trial 3 — Stability (probe).**
Reuse the 3×3 centre-loaded grid. Perform single steps at a small rate, a moderate rate, and a
deliberately large rate. *Record:* for each rate, the minimum cell value (does it go negative? E5/P2)
and the maximum (does it exceed 12? P3), and whether repeating the step several times settles toward
uniformity or oscillates. Do **not** assume where the threshold is; find it, or find that none
exists for a given mechanism.

**Trial 4 — Composition (probe).**
Reuse Trial 1's grid and add a second Rule: a source that injects a fixed amount into one corner cell
each step. Run diffusion + source together for one step. *Record:* whether the combined result equals
the diffusion-only result plus the injected amount without the two Rules referencing each other (E4),
and whether the augmented total (initial + injected) is conserved.

The complete raw output of this experiment is small: four short tables of cell values and totals.
Producing and interpreting those tables is the work of the **next** document. **This one stops at the
design; it computes nothing.**

## 6. Falsification — what result refutes each candidate

Each candidate makes claims (§3). A claim is scientific only if an observation could refute it. The
following are those observations. If one is seen, the corresponding claim is false and the mechanism's
predicted profile is wrong — which is itself a true thing learned.

**C-N — node-based**
- *Refutes its conservation claim:* Trial 1, exact arithmetic, Σ_after ≠ Σ_before. (Would mean node
  conservation is not even analytic.)
- *Refutes its "fragile conservation" prediction:* Trial 1 rounded, or Trial 2 sequential, in which Σ
  remains **exactly** invariant. (Would mean node conservation is robust after all — i.e. behaves like
  the edge mechanism, collapsing the C-N/C-E distinction.)
- *Refutes its stability prediction:* Trial 3, no rate — up to transferring a cell's entire contents —
  ever produces negativity, overshoot, or oscillation. (Would mean the explicit scheme is
  unconditionally stable, contradicting the prediction.)

**C-E — edge / flux-based**
- *Refutes its structural-conservation claim (strongest test):* Trial 1 or Trial 2, under **any**
  arithmetic mode and **any** processing order, Σ changes. Because the claim is that conservation is
  structural, a *single* such observation refutes it outright.
- *Refutes its exact-arithmetic order-independence claim:* Trial 1/2, exact arithmetic, permuting edge
  order changes a cell value (E2).
- *Refutes its positivity-dissociation prediction:* Trial 3, the mechanism conserves yet **never**
  produces a negative cell at any rate. (Would mean conservation *does* imply positivity here,
  contradicting the predicted dissociation.)

**C-G — global solve**
- *Refutes its unconditional-stability claim:* Trial 3, some rate produces a negative cell, an
  overshoot beyond [0, 12], or oscillation.
- *Refutes its non-locality claim:* a single-cell change in the initial condition leaves at least one
  distant cell's one-step result unchanged. (Would mean the global solve is effectively local — the
  distinction from C-N/C-E dissolves.)
- *Refutes its poor-composability prediction:* Trial 4, the source can be composed by simple addition
  and the result matches the full re-solve. (Would mean C-G composes as cleanly as the local
  mechanisms.)

Note the symmetry of these falsifiers: several would refute a claim by **collapsing a distinction**
between candidates (node behaving like edge; global behaving like local). Those outcomes are as
valuable as any — a dissolved distinction is a discovered truth about the phenomenon, and would feed
directly back into the transition-mechanics question of whether conservation is a property of a
*mechanism* or of a *representation*.

---

## Controlled and independent variables (summary)

- **Held fixed:** grid geometry per trial, 4-neighbour connectivity, closed boundaries, a single
  non-negative scalar.
- **Varied deliberately:** mechanism (C-N / C-E / C-G); transfer rate; update order; arithmetic mode
  (exact / rounded); presence of the second Rule.
- **Measured:** the five observables E1–E5.

## What this document is and is not

It **is** the design of a scientific experiment: a phenomenon, its invariants, three competing
predictions, five observable criteria, four by-hand trials, and a falsifier for every claim. It
**is not** a solution (no trial is computed), an implementation (no equations, no code), or a choice
(no mechanism is preferred). Running the trials and recording E1–E5 is the subject of the next
document; interpreting them against the falsifiers is what will decide, on evidence, what is true.
