# Experimental Methodology — Distilled

Status: **Adopted — via ADR-0002 (2026-07-26).** The adoption this document refused to perform on
itself (per its own Principle III) was made as a deliberate decision:
`docs/Decisions/ADR-0002-Development-Methodology.md` adopts the three principles as the epistemic
base of Genesis's development methodology. Preserved below as distilled.

> A subtraction pass, applied to method the way the ontology arc was applied to concepts. Every
> methodological principle raised across this discussion is listed, then each is removed if another
> entails it. What remains cannot be reduced without losing the rest. Three principles survive.

---

## The candidates (before subtraction)

Gathered from the whole discussion:

1. A claim is the unit of attribution.
2. An experiment tests exactly one claim.
3. Protocols exist before results (pre-registration).
4. A claim names the conditions it holds fixed (auxiliaries explicit).
5. Scope is part of every claim; falsification is scoped, never global.
6. Interaction claims are first-class claims.
7. Every claim is falsifiable.
8. Results compose into a knowledge graph.
9. Experiments inform decisions but do not make them.
10. Architecture changes by weight of evidence, not by one experiment.

## The subtraction

**2, 3, 4, 5, 6, 8 all collapse into 1.** Each is an operational face of *attribution*, not a
separate principle:

- **2 (one claim)** — attribution is impossible if two claims move at once; isolation is what
  attribution *means*, not an added rule.
- **3 (protocol before results)** — to attribute an outcome to a claim, the claim must be fixed
  *independently* of that outcome; a claim chosen after the result is fitted to it, and attribution is
  then circular. Independence is a precondition of attribution.
- **4 (auxiliaries explicit)** — you cannot attribute a result to a claim without holding the rest
  fixed and known; naming the held-fixed conditions is how attribution is made possible (Duhem–Quine).
- **5 (scope)** — the held-fixed conditions *are* the scope. A claim's attribution is valid only where
  those conditions held; scope is not additional, it is the boundary of the same fixed conditions.
- **6 (interaction claims first-class)** — once the *claim* is the unit, nothing privileges a
  single-property claim over a claim about a coupling. "Interaction claims are first-class" only needs
  saying to correct the error of making a *property* the unit; with the claim as the unit, it is
  automatic.
- **8 (results compose)** — attributed, scoped facts compose because each is independently valid. A
  consequence, not a premise.

**10 collapses into 7 + 9.** Since a single experiment only refutes a scoped claim (7) and cannot
select (9), the protected architecture changes only under accumulated refutations plus a decision. A
theorem, not an axiom.

**7 does not collapse.** Testability is a property of the claim's *content*, independent of
attribution's *control*. A claim can be perfectly isolated yet unfalsifiable (vacuous), and a
falsifiable claim can be tested in a confounded way (Experiment-01: "edge conserves" is falsifiable,
but uncontrolled rate and rounding destroy its attribution). Isolation and refutability do different
jobs; neither entails the other. **7 is irreducible.**

**9 does not collapse.** It concerns a different category — the relation between experiments and
decisions. Attribution and falsifiability describe what an experiment *does*; neither says an
experiment cannot *select*. Selection rests on criteria experiments do not supply (the confirmation
gap, and the trade-offs of cost and simplicity that are not empirical). **9 is irreducible.**

## The irreducible set

### Principle I — The claim is the unit

An experiment tests exactly one claim, fixed before the result and independently of it, together with
the conditions it holds fixed. Those conditions are the claim's scope; outside them the claim says
nothing.

*Its single job:* make an outcome mean something — attribution. Isolation, pre-registration, explicit
auxiliaries, scope, the admissibility of interaction claims, and the composability of results are all
this principle, seen from different sides.

### Principle II — The claim must be refutable

For a claim to be tested, some possible observation must be able to refute it. A claim that no
observation could refute is not tested, only asserted.

*Its single job:* testability. It governs the claim's content, where Principle I governs the
experiment's control. The two are independent: a claim can satisfy one and fail the other.

### Principle III — Experiments eliminate; they do not decide

Falsification removes claims. It cannot select or construct. Choosing what to build rests on criteria
experiments do not supply, and is a separate act.

*Its single job:* demarcation — the boundary of what an experiment can do. It introduces the one thing
Principles I and II never mention: the decision.

## Why these three and no fewer

Pairwise, none derives from another, because each does a distinct job:

- **I without II** — you can perfectly control an experiment around a claim nothing could refute. Vacuous but well-attributed. So II is not contained in I.
- **II without I** — you can state a refutable claim and test it amid uncontrolled confounds. Refutable but un-attributable. So I is not contained in II.
- **III without I or II** — the limit of experiments (they eliminate, not decide) is silent in both I
  and II, which describe only what a single experiment does. III names the decision, which the others
  never do.

Remove any one and something essential is lost: remove I and results stop meaning anything; remove II
and claims stop being testable; remove III and the programme mistakes elimination for choice and never
builds. This is the same stopping condition the ontology arc used — the next removal costs explanatory
power — reached here at three.

## What follows (to confirm the reduction)

Everything set aside is recovered as a consequence:

- one claim per experiment, pre-registration, explicit auxiliaries, scope, interaction claims,
  composable results — from **I**;
- design-to-refute, a falsifier per claim — from **II**;
- selection belongs to an RFC not an experiment, architecture moves by weight of evidence — from
  **II + III**.

## A note the method makes about its own adoption

By Principle III, adopting this set is a *decision*, and this document is an *experiment's* kind of
artifact — it eliminates, it does not choose. So it does not enact the methodology; it presents the
distilled result for a decision. If adopted, it graduates from Research to a **Decisions/** record, as
any durable process commitment does. The methodology declining to adopt itself is not evasion — it is
the first instance of the methodology being obeyed.

---

*Three principles. The claim is the unit. The claim must be refutable. Experiments eliminate; they do
not decide. Everything else in this discussion follows from these.*
