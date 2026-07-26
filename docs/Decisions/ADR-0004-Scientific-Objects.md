# ADR-0004 — Scientific Objects

## Status

**Accepted** — 2026-07-26. Proposed by the project's reviewer-author, verified against the corpus,
accepted the same day. Like ADR-0002 and ADR-0003, this document codifies retroactively: it
introduces no new documents and no bureaucracy. It introduces a vocabulary for objects that already
exist.

## Context

Genesis gradually developed a scientific method (ADR-0003). During that evolution, several distinct
intellectual objects emerged naturally: observations, hypotheses, research debts, theories,
predictions, RFCs. Until now these coexisted without an explicit ontology — explanations sometimes
appear inside observations, predictions hide inside journals, theories remain implicit.

## Decision

Genesis recognizes that scientific work manipulates different kinds of intellectual objects. They
have different purposes, different lifetimes, and different evidential requirements. **They must not
be confused.**

## The Scientific Objects

### 1. Observation
Record something the world actually did. Factual, reproducible, evidence only; survives until
refuted. Answers: *what happened?*

### 2. Hypothesis
Propose a possible explanation. Local, tentative, falsifiable, disposable. Belongs to an
observation; never authority. Answers: *why might this have happened?*

### 3. Theory
Explain multiple observations with one coherent model. Predictive, general, falsifiable, expected to
evolve. **A theory earns confidence by surviving predictions — never by age.** Answers: *what general
mechanism could explain these observations?*

### 4. Prediction
Describe an observation expected if a theory is approximately correct. Testable, risky, specific.
Predictions are where theories touch reality. Answers: *what should we observe next?*

### 5. Research Debt
Record a question created by discovery. Created by observation; paid by observation; never paid by
design. **Not ignorance — structured ignorance.** Answers: *what does the world now owe us an
explanation for?*

### 6. RFC (and its exploratory sibling, the DN)
Record consequences that became unavoidable. Normative, architectural, stable. RFCs stabilize
consequences, not explanations; Design Notes explore thresholds before them. Answers: *what has
become inevitable?*

## Relationships — two cycles

**The scientific cycle** (discovers; nothing authoritative; everything may evolve):

```
Observation → Hypothesis → Theory → Prediction → Observation
```

**The architectural cycle** (stabilizes; nothing explains; everything constrains):

```
Observation → Journal → Reduction Test → RFC → Kernel
```

The two cycles interact only through observations; the world is their common source. (Historical
honesty: during the Construction era the architectural cycle was entered from design necessity. The
Discovery-era motto changed the entrance: evidence now comes first.)

## Lifetime

| Object | Expected lifetime |
|---|---|
| Observation | Permanent unless refuted |
| Hypothesis | Often short |
| Theory | Long but revisable |
| Prediction | Ends once tested |
| Research Debt | Until paid or forgiven |
| RFC | Stable — **mortal only by its own kill criterion, never by a theory's death** |

Genesis treats the death of a hypothesis as ordinary scientific progress.

## Consequences

Genesis distinguishes, from now on: facts, explanations, obligations, and architectural commitments.
A discovery may invalidate a hypothesis without weakening an RFC; an accepted RFC certifies no
particular explanation. Scientific understanding remains free to evolve; architecture remains
stable.

No new modules are created. A theory earns a file the day one earns the name — the vocabulary-late
discipline, applied to the method's own objects. (The first embryo is already identifiable: the
spectral stability candidate of Observations 005/007, which has survived its first risky prediction —
the measured deg-4/5 growth ratios.)

## Rationale — the laboratory already demonstrated the distinction

Observation-007 produced the beginning of a spectral theory without requiring any RFC.
Observation-008 refuted an overflow hypothesis while leaving RFC-0004 entirely intact — the clearest
possible exhibition of the fact/commitment separation, since RFC-0004's only death is a
theorem-preserving reduction, which no falling hypothesis can supply. Genesis-015 introduced
Research Debt as a first-class object that generates expeditions without creating architectural
work. And the method itself correctly predicted the existence of distinguishing specimens even as
individual hypotheses evolved — methodology and explanation are separate concerns, demonstrably.

## An asymmetry worth preserving

Science seeks explanations. Architecture seeks inevitabilities. Genesis deliberately allows
explanations to change while protecting inevitabilities.

A theory may die tomorrow. An RFC should survive its death. If both disappear together, the project
has confused understanding with commitment. If theories remain free while commitments remain
justified, Genesis preserves both scientific honesty and architectural stability.
