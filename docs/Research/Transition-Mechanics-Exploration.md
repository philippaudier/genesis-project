# Transition Mechanics — Exploration

Status: **Research — exploration only.** Opens a new arc. No decision. No implementation. The
ontology below is treated as a **fixed working hypothesis**, not reopened for simplification.

> **Phase change.** The first research arc asked *what is Genesis?* and converged on a provisional
> triad (`Transformation-Model*` and `*-Critique.md`, summarised in
> `../Journal/2026-07-24-architecture-convergence.md`). It stopped deliberately, because the next
> simplification would have removed explanatory power rather than accidental complexity.
>
> This arc asks a different question: **how does the world evolve?** It moves *sideways* from the
> triad into transition mechanics, taking **deterministic continuous fields and conservation laws**
> as the first and hardest substrate — the problem (W1) that no ontology collapse addressed, because
> it was always orthogonal to *what things are* and entirely about *how they change*.

---

## Inherited ground (working hypothesis — not under review in this arc)

The following are assumed, not re-argued. If this arc finds one of them untenable, that is a finding
to record, not an invitation to restart the ontology.

- **The triad.** The world is `State · Transition(Rules) · Constraint`, observed across an external
  boundary. Transition produces next state; Constraint forbids illegal states; neither collapses into
  the other.
- **The boundary.** External Interfaces supply the input `u(t)` and the initial condition `x(0)`.
  They provide information; they do not evolve the world. Evolution is Rules over State.
- **Determinism levels.** Level 1 (logical) is required; Level 2 (platform) desired; Level 3
  (cross-platform bit-identical) deferred to a future numeric-guarantees RFC.
- **The tick.** A logical execution step with no intrinsic real duration; snapshot-vs-sequential
  semantics within a tick remain *open* — and this arc is where that question stops being abstract.

## The central problem

> **How does a continuous, conserved quantity take a deterministic, discrete step — under local
> rules, without violating conservation, and reproducibly?**

The prior arc found three specific difficulties (Critique §2, §6, §7), none yet resolved:

1. **No natural grain.** An update per unit of flux is absurd; an update per whole field is a
   monolith that Locality forbids. There is no obvious middle unit.
2. **Conservation is global; rules are local.** A cell updating from its neighbours cannot, alone,
   guarantee that the total is preserved — the naïve case where three neighbours each pull 4 from a
   cell holding 10 destroys 2 units from nothing.
3. **The step is a solver, not an arbiter.** Enforcing conservation means *scaling* competing flows,
   which is quantitative constraint-solving, not accept/deny.

This arc exists to understand — not yet to choose — how a field step should be structured so these
three stop being problems.

## The question map

Six clusters. Each states the tension and the positions; none is answered here.

### A. Grain — what is the unit of a field transition?

- **Per cell (node).** A rule reads a cell's neighbourhood and writes the cell's next value. Local
  and Constitution-friendly, but conservation is not structural: what one cell "takes" must exactly
  equal what its neighbour "gives", and two independent node-rules do not coordinate that.
- **Per edge (flux).** The unit is the *flow between two cells*. A flux moved from A to B is
  subtracted from A and added to B *by construction*, so conservation becomes structural rather than
  enforced. But an edge is not an entity; it reads and writes two cells at once, which complicates
  the "a rule owns a cell" locality picture.
- **Per field (global step).** One rule advances the whole field. Conservation trivial to enforce
  globally; Locality abandoned; parallelism and composition lost.

Working intuition to *test, not adopt*: flux (edge) grain may make conservation a property of the
representation instead of a constraint to police. If so, it would move W1's difficulty out of
Constraint and into the *form* of the Transition — which is itself an important claim about the triad
worth verifying rather than assuming.

### B. Simultaneity and ordering — the Jacobi / Gauss–Seidel fork

This is the snapshot-vs-sequential question the first arc left open, now unavoidable.

- **Snapshot (Jacobi-like).** Every cell reads the start-of-tick state; all writes land in the next
  state. Order-independent, deterministic, parallelisable — but the *cause* of difficulty #2: with
  node grain, simultaneous independent pulls can over-draw a source.
- **Sequential (Gauss–Seidel-like).** Cells update in a fixed traversal order, each seeing prior
  writes. Conserves more naturally and can be more stable — but the outcome depends on traversal
  order, so determinism demands that order be *fixed and explicit*, and the update is inherently
  serial.

Note the interaction with grain (A): **edge grain plus snapshot** may conserve where **node grain
plus snapshot** does not, because the conserved unit is the flow itself. The fork is not independent
of the grain choice — that coupling is a primary thing this arc should map.

### C. Where does conservation live — Transition or Constraint?

A direct test of the triad against a hard case.

- **As a Constraint.** Conservation is a predicate (`sum(field) = invariant`) enforced after a
  producing step by rejecting or *scaling* violations. Honest to the triad, but "scaling" is a
  solver, and rejection has no meaning for a continuous field (you cannot reject a diffusion).
- **As Transition form.** Conservation is built into the representation (edge/flux grain), so no rule
  *can* violate it and there is nothing for a Constraint to enforce.

The likely answer is *per-invariant*, and that itself is a finding: some invariants (conservation of
a quantity) may be structural in the Transition; others (`quantity ≥ 0`, `no two solids in a cell`)
may be irreducibly Constraints. If so, the triad's Constraint element is real but *not universal* —
it holds the invariants that cannot be dissolved into representation. Worth establishing carefully.

### D. Determinism under continuous mathematics

- **Logical determinism (L1) is achievable** for fields *if* the discretisation and the traversal
  order are fixed — independent of floating-point reproducibility. This must be separated cleanly
  from L3, or the arc will conflate "the algorithm is deterministic" with "the arithmetic is
  bit-identical."
- **Representation of the quantity.** Floating-point invites drift and makes exact conservation
  literally impossible (rounding loses quanta every step). An alternative worth examining: **integer
  or fixed-point quantities** — conserved *quanta* that move between cells and sum exactly. This
  trades physical smoothness for exact conservation and exact determinism, and it interacts with the
  deferred numeric-guarantees RFC. Not decided; flagged as a live option with real consequences.

### E. Discretisation, rate, and stability

- **Rate is per-tick.** Because a tick has no duration, a field's rate of change is a per-step
  constant. This is the D1 observation made concrete: change the tick granularity and every field's
  dynamics rescale. The field's rate constants are *design content*, and this arc should treat them
  as such.
- **Determinism does not imply stability.** A perfectly deterministic explicit update can still
  *oscillate or diverge* if the per-step rate is too aggressive (the classic stability limit of
  explicit schemes). This is new territory the ontology never had to face: a Rule can be pure,
  local, and deterministic and still produce a physically nonsensical, exploding world. Stability is
  therefore its own concern, possibly its own kind of Constraint on rate.

### F. Boundaries, sources, and sinks

- Fields meet world edges, walls, and reservoirs. A **source** injecting quantity is exactly the
  external input `u(t)` from the ontology — a satisfying connection: sources/sinks are where the
  Interface boundary meets a field. Reflecting vs absorbing vs periodic boundaries each change the
  conservation bookkeeping and should be enumerated, not assumed.

## Proposed method (not decided)

Field mechanics are only real when computed, so this arc should proceed by **concrete testbeds**
rather than abstraction, each chosen to isolate a subset of A–F. Candidate first testbeds:

1. **Scalar diffusion on a grid** (heat / concentration). Conserves a total; exhibits grain (A),
   Jacobi/Gauss–Seidel (B), the conservation-location question (C), and stability (E) — with the
   *least* additional constraint machinery. The minimal case that already stresses the core.
2. **Pressure / gas equalisation.** Adds directional flow and stronger stability limits.
3. **Incompressible fluid.** Hardest: adds a global divergence-free Constraint that cannot be made
   structural cell-locally — the sharpest test of C and of the Constraint element itself.

Recommendation to weigh, not adopt: **start with scalar diffusion.** It is the smallest system in
which W1's three difficulties all appear, so a mechanism that handles it honestly is a real result,
and one that fails it fails cheaply. Fluids are where the Constraint element earns or loses its
universality, but they are the wrong place to *begin* because they conflate too many questions at
once.

## What this arc will not do

- It will not reopen or "simplify" the ontology; the triad is fixed input.
- It will not decide the tick frequency, the numeric representation (deferred), or any
  implementation.
- It will not touch the Constitution or RFC-0001.
- It will not write code. Testbeds are analysed as mechanics on paper before any of them is built.

## Open questions, collected

- A — Is the unit of a field step the cell, the edge/flux, or the whole field?
- B — Snapshot (Jacobi) or fixed-order sequential (Gauss–Seidel) within a tick, and how does the
  choice couple to grain?
- C — Is conservation structural in the Transition, or enforced as a Constraint — and is the answer
  per-invariant? If so, which invariants are irreducibly Constraints?
- D — Are conserved quantities represented as floating-point (smooth, drifting, non-exact) or as
  integer/fixed-point quanta (exact, granular)? How is L1 determinism guaranteed independently of L3?
- E — How are per-tick rates chosen, and where does a *stability* limit live in the triad — is it a
  Constraint on rate, or a property the Transition must self-limit?
- F — How do sources, sinks, and world boundaries interact with conservation, and are sources simply
  the input `u(t)` at a field cell?
- G — Does any field phenomenon fail to fit the triad at all? (The standing adversarial question,
  carried forward from the last arc.)
