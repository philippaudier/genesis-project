# Experiment 01 — Protocol Review (pre-registration peer review)

Status: **Closed with the experiment (2026-07-26).** Experiment-01 was closed as superseded by
ADR-0001 and the validated kernel; neither of this review's dispositions (freeze narrow /
amend-then-freeze) was ever selected, and none is needed. The review's findings remain the standard
any future field-experiment protocol must meet.

*Original review, preserved as written:*

> Scope: does Experiment 01, exactly as written, constitute a **scientifically fair comparison** of
> C-N (node), C-E (edge/flux), and C-G (global)? The seven questions posed for review are answered in
> order, each finding carrying a severity: *confound*, *non-discriminating observable*, *missing
> observable*, *underspecification*, *interpretation ambiguity*, *weak/invalid falsifier*,
> *scale limitation*, or *acceptable limitation*.

---

## Headline verdict (stated first, defended below)

**The protocol is a strong and fair test of C-N versus C-E, and a weak, under-powered, partly unfair
test of C-G.** It is well-tuned to reveal the node-versus-edge conservation distinction — its centre
of gravity — but it (a) cannot exhibit C-G's one genuine advantage (unconditional stability is barely
stressed by a tiny, closed, few-step grid), (b) masks C-G's one genuine weakness (composability is
nullified by the linearity of diffusion — see Q2), and (c) never operationalises locality, the axis
that defines C-G, as an observable at all. **As written, it should not be frozen as a fair three-way
comparison.** It can be frozen honestly as a two-way (C-N/C-E) comparison, or amended on the specific
points below and then frozen as a three-way. That choice is the user's; this review only establishes
which conclusions the current design can and cannot support.

## Q1 — Does the protocol unintentionally favour one candidate?

**Yes — it favours the conservation axis, and with it the C-N/C-E contrast, while disadvantaging
C-G.** Three structural reasons:

- **The phenomenon is defined as closed and conserved.** Conservation and the maximum principle are
  written into the definition of "faithful model." That is a legitimate modelling choice, but it
  selects the regime where C-E's structural-conservation advantage is maximally visible and where
  C-G's stability advantage is least exercised. *Severity: confound (regime selection).*
- **The scale under-stresses stability.** A 3×3 grid, closed, one or a few steps, cannot approach the
  stiff / high-rate regime where implicit methods earn their keep. C-G's defining strength is
  therefore untestable here even in principle. *Severity: scale limitation.*
- **C-G's weakness is masked (see Q2, composition).** So the experiment can neither show C-G's
  strength nor fairly probe its weakness.

Net: the design is not neutral across candidates. It is a fair contest between C-N and C-E and an
unfair one for C-G.

## Q2 — Are any observables incapable of distinguishing two mechanisms?

Four, one of them serious.

- **E1 in exact arithmetic cannot separate C-N from C-E.** Both conserve exactly in exact arithmetic;
  only the *rounded* pass discriminates. An experimenter who reports the exact pass alone would
  wrongly conclude equivalence. *Severity: non-discriminating observable (partial); needs the
  protocol to state that only the rounded pass is decisive.*
- **E2 on Trial 1 cannot detect order dependence.** T1 is symmetric, and symmetry forces equal
  outputs regardless of visiting order. The protocol markets this as "a free order-independence
  check," but that is backwards: symmetry *masks* order dependence — an order-dependent mechanism can
  still produce symmetric output on symmetric input. Only the asymmetric Trial 2 can test E2.
  *Severity: non-discriminating observable, plus a misleading description in the protocol.*
- **E3 (cross-execution identity) discriminates nothing.** Two people following the same deterministic
  hand-spec obtain the same result for *all three* mechanisms; E3 tests whether the *specification* is
  deterministic, not whether the *mechanism* is. The only place L1 genuinely fails — an *iterative*
  global solve with a convergence tolerance — is never reached by a direct hand-solve. *Severity:
  non-discriminating observable; the intended failure mode is out of the protocol's reach.*
- **E4 (composition) is nullified by linearity — serious.** Scalar diffusion is linear, so
  superposition holds: for *every* mechanism, the result with a source equals the result without it
  plus the source's response. C-G's real composability cost is *computational* (it must re-run the
  solve), not *result-level* — and a result-level paper experiment cannot observe computational
  coupling. So E4, as an observable of logical states, cannot detect the composability difference for
  any mechanism. It would report "all compose," which is true and uninformative. *Severity:
  non-discriminating observable (the property E4 exists to test cannot be seen without a nonlinear
  co-Rule, which is out of scope).*

**Missing observable.** Locality — the axis in the design-logic table that separates C-G from the
rest — has *no* entry among E1–E5. It appears only inside C-G's falsifier. A key discriminating axis
with no assigned measurement is a gap. *Severity: missing observable.* (There is a latent probe: on
the centre-loaded 3×3, corner cells stay zero after one *explicit* step but become nonzero under an
*implicit* solve — a clean local/non-local signal the protocol neither names nor measures.)

## Q3 — Are hidden variables still changing simultaneously?

- **Rate is not commensurable across mechanisms.** Each mechanism parameterises "rate" differently,
  and the protocol says only "small/moderate/large." Any cross-mechanism comparison at a nominally
  matched rate is confounded, because the rates are not the same physical input. Within-mechanism
  observations are unaffected. *Severity: confound (fatal to cross-mechanism comparison; harmless
  within-mechanism).*
- **The rounding rule is underspecified, and it is the decisive discriminator.** "Rounded to a fixed
  few digits after each transfer" does not fix *where* rounding is applied (per flux, per cell sum) or
  *how* (half-up, half-even). Two experimenters could round differently and reach different
  E1-rounded verdicts — the exact verdict that separates C-N from C-E. *Severity: underspecification,
  high impact.*
- **Boundary realisation is folded into "mechanism."** "No-flux" can be realised by fewer neighbours,
  ghost cells, or coefficient adjustment; C-N and C-E realise it differently, and that difference is
  uncontrolled. A conservation difference could be attributed to the mechanism when it is really the
  boundary treatment. *Severity: confound, moderate.*
- **Step count varies across trials** (one step vs "several"), so multi-step trials mix per-step drift
  with number-of-steps. *Severity: minor confound.*

## Q4 — Uncontrolled assumptions that would invalidate conclusions?

- **That exact by-hand arithmetic is actually performed.** With fractional rates, exact values are
  rationals with growing denominators; an experimenter who lapses into decimals has silently switched
  the arithmetic mode and *merged* the exact and rounded passes — destroying the very contrast that
  separates C-N from C-E. The protocol assumes clean exact arithmetic without mandating rationals.
  *Severity: uncontrolled assumption, high impact (fixable by mandating rational arithmetic and
  choosing values/rates that stay tractable).*
- **That one explicit step and one implicit step are comparable.** C-N/C-E realise a forward
  (explicit) step; C-G realises a backward (implicit) step. These approximate *different* one-step
  maps. Any comparison of C-G's cell *values* to C-N/C-E's cell values assumes a correspondence that
  does not exist. The protocol does not forbid such comparison. *Severity: uncontrolled assumption,
  fundamental for value-level cross-mechanism claims.*
- **That C-E is executed flux-once.** C-E's structural conservation holds only if each edge flux is
  computed once and reused with opposite sign. If an experimenter recomputes per cell, a resulting Σ
  change would falsely refute C-E when it in fact refutes the execution. *Severity: uncontrolled
  assumption (execution fidelity).*
- **Strength worth recording:** the protocol correctly refuses to define a reference equation, so it
  does *not* assume a single "correct" solution to compare against. That avoids a classic confound and
  should be preserved.

## Q5 — Outcomes open to multiple incompatible interpretations?

- **Sequential-node identity (Trial 2).** C-N is *defined* as snapshot. Trial 2 also runs a
  *sequential* node pass. If sequential node breaks conservation, one reading is "node is fragile";
  another is "sequential is not C-N, so this says nothing about C-N." The protocol does not fix
  whether sequential-node is a candidate, a variable of C-N, or a fourth mechanism. *Severity:
  interpretation ambiguity (mechanism-versus-variable).*
- **Null results read as robustness.** "No drift observed under rounding ⇒ conservation robust"
  conflates *absence of an effect* with *evidence of absence*. The rounding may simply have been too
  coarse or too fine to reveal drift. *Severity: interpretation ambiguity (null result).*
- **Finite sweeps read as universals.** E5's "find that none exists" and C-E's "never negative at any
  rate" are universal claims a finite hand-sweep cannot establish. Finding instability refutes; *not*
  finding it proves nothing. *Severity: interpretation ambiguity (asymmetric falsifiability).*

## Q6 — Does every falsifier genuinely falsify its claim?

- **Valid and strong:** C-E's structural-conservation falsifier ("any arithmetic, any order, Σ changes
  ⇒ refuted") — one counterexample kills a universal claim. Valid *given faithful flux-once
  execution* (Q4). C-G's unconditional-stability falsifier ("some rate ⇒ negative/overshoot/oscillate
  ⇒ refuted") — valid; finding instability is conclusive.
- **Conditionally valid:** C-N's exact-conservation falsifier is valid *only if arithmetic was truly
  exact* (Q4). Under a decimal lapse, a Σ change is a rounding artifact, not a refutation.
- **Weak / invalid falsifiers:**
  - C-N's "fragile conservation" refuter — a *single* rounded trial that happens to conserve does not
    establish robustness; specific values can conserve by coincidence. A universal-robustness claim
    cannot be confirmed from one lucky null. *Severity: weak falsifier.*
  - C-E's positivity-dissociation refuter ("conserves yet *never* negative at any rate") and C-G's
    unconditional-stability *confirmation* both require a universal over all rates that a finite sweep
    cannot deliver. Only the refuting directions are sound. *Severity: invalid in the confirming
    direction.*
  - C-G's non-locality falsifier references a "distant cell," but the observable was never defined
    (Q2, missing observable) and "distant" is ill-posed on 3×3 without designating the corner probe.
    The falsifier is not executable as written. *Severity: invalid as specified (no operational
    observable).*

## Q7 — Criticisms a peer reviewer would raise before accepting

Consolidated. **Must-fix for a fair three-way comparison:**

1. **Define a common, cross-mechanism rate normalisation**, or explicitly restrict *all* comparisons
   to within-mechanism and forbid cross-mechanism value comparison. (Q1, Q3, Q4)
2. **Specify the rounding protocol exactly** — where and how rounding is applied — since it is the
   decisive C-N/C-E discriminator. (Q3)
3. **Mandate exact rational arithmetic** for the exact pass, with values/rates chosen to stay
   tractable, so the exact and rounded modes cannot silently merge. (Q4)
4. **State that explicit and implicit single-step maps are not value-comparable**; confine
   cross-mechanism claims to property level (conserves? reorders? stable? local?). (Q4)
5. **Operationalise locality as an observable** (e.g., designate the corner cells of the centre-loaded
   3×3 as the local/non-local probe), so C-G's defining axis is measured, not merely asserted. (Q2)
6. **Resolve the sequential-node ambiguity** — declare whether sequential-node is a candidate, a
   variable of C-N, or excluded. (Q5)
7. **Restrict universal falsifiers to their refuting direction** — "found instability/negativity"
   refutes; "found none in a finite sweep" confirms nothing. (Q5, Q6)
8. **Strengthen the node-robustness refuter** to require multiple adversarial roundings, not one lucky
   null. (Q6)
9. **Correct the Trial-1 order-independence description** — symmetry masks, it does not reveal, order
   dependence; E2 must rest on Trial 2. (Q2)

**Acceptable limitations (may freeze *if* explicitly scoped):**

- E4 cannot test composability under linear diffusion; either drop the composability claim from this
  experiment or note that it requires a nonlinear co-Rule reserved for a later experiment. (Q2)
- C-G's stability advantage cannot be exhibited at this scale; the experiment does not claim to, and
  should say so. (Q1)
- E3 is a specification-determinism sanity check, not a mechanism discriminator; keep it, but label it
  as such. (Q2)

---

## Disposition

Experiment 01 does **not** survive review as a fair three-way comparison. It **does** survive as a
fair, well-powered comparison of **C-N versus C-E** on conservation and order-dependence — which is,
notably, its strongest and most original contribution. Two honest paths forward, both the user's to
choose; this review does not take one:

- **Freeze narrow.** Re-scope Experiment 01 to the C-N/C-E conservation question it tests fairly,
  label C-G as observed-but-not-fairly-tested, and freeze that.
- **Amend then freeze.** Apply must-fixes 1–9, which are protocol-level (rate normalisation, rounding
  spec, rational arithmetic, comparison discipline, a locality observable, falsifier tightening) and
  touch **no mechanism**, then freeze the amended protocol.

What should not happen is freezing the current text and reporting three-way conclusions from it; on
the points above, those conclusions would be confounded, non-discriminating, or unfalsifiable.
