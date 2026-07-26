# ADR-0003 — The Scientific Method

## Status

**Accepted** — 2026-07-26.

Like ADR-0002, this document codifies retroactively: it invents nothing. The method it records was
never designed — no one ever said "let us create a scientific method." It appeared by accumulation,
the way trails appear in a forest: several shortcuts were refused, one after another, until only one
practicable path remained. This ADR draws the path that was already being walked, because it has now
been walked end to end.

## Context

The method is currently distributed across the Constitution, the Observations, the Journal, RFC-0004,
DN-003, DN-004, and the jurisprudence entries. A newcomer must read six sources to learn how Genesis
discovers anything. Meanwhile the method has completed its first full path — Observations 004/005 →
measurements → Journal → Reduction Test → RFC-0004 → review → acceptance → corpus — and has run
partially many times. It is stable enough to write down.

**Relationship to ADR-0002:** complementary, neither superseding. ADR-0002 governs how Genesis
*builds* (the Proof Rule: one property per milestone, executable cumulative proofs, no silent
decisions). ADR-0003 governs how Genesis *learns*. They share the same epistemic base — the distilled
principles ADR-0002 adopted: the claim is the unit; the claim must be refutable; experiments
eliminate, they do not decide.

## Decision — the pipeline

```
Question
   ↓
Laboratory
   ↓
Observation ──── dies
   │        ──── merges
   │        ──── promoted
   ↓
Measurement
   ↓
Journal
   ↓
Reduction Test  (when a new primitive is implied)
   ↓
RFC / DN
   ↓
Review
   ↓
Acceptance
   ↓
Corpus (jurisprudence)
```

The branch matters: **not every observation becomes an RFC.** Some die (unreproducible, or trivially
explained). Some merge (the same phenomenon under two appearances). Some wait. Only what survives
measurement and the Journal may knock at a decision document's door — and only with its evidence in
hand.

### The stations

1. **The laboratory** (Genesis-013's invariants): observation never changes reality; snapshots, never
   the simulation; everything visible derivable; removing the observer changes no theorem.
2. **Observations** (`docs/Observations/`): numbered; facts strictly apart from hypotheses; no
   Glossary words; statuses Open / Died / Merged / Promoted. Even ignorance is documented before the
   field (Observation-000: recorded priors are the condition of measurable surprise).
3. **Measurement and Journal**: numbers before narratives; the Journal holds what was learned and how
   — chronology, not state.
4. **The Reduction Test** — *formally adopted by this ADR, resolving its provisional status* (Journal
   2026-07-26). Before any new primitive: (1) attempt to express the need with existing primitives;
   (2) fail honestly; (3) demonstrate the failure is structural — each blocker a proven theorem, not
   a missing convenience; (4) only then accept the primitive. First complete application: RFC-0004.
5. **RFC / DN, review, acceptance**: decision documents carry their origin (observations may be
   upstream dependencies), their kill criteria (how to destroy them), and their deferred questions
   (catalogued so none can sneak in). Acceptance may be granted for one reason above all others:
   *the demonstration could not be refuted.*
6. **Jurisprudence** — the corpus as precedent, in three temporal directions: **precedent** looks
   backward (has this fork been ruled before?); **invariants** hold the present (does something force
   this?); **prospective jurisprudence** looks forward (which shape remains valid in every admissible
   future?). A principle survives only if it passes the directions that apply.

### The admission rules downstream

Everything expensive in Genesis has a tariff, and the tariffs are part of the method:

- **A name** costs a theorem (DN-003 + the Glossary's editorial compression rule).
- **A primitive** costs a demonstrated irreducibility (the Reduction Test).
- **A law** costs inevitability (the Discovery-era motto: never because it seems useful; only when a
  world makes it inevitable).

Bad ideas are not forbidden. They are priced. They die because they cannot pay the entry.

## The motto

> **The world deserves evidence before design.**

Classical architectures design, then verify. Genesis observes, demonstrates, and only then designs.
The record of a single week: Observation-004 preceded RFC-0004; the Reduction Test preceded its
acceptance; DN-004 preceded any implementation; prospective jurisprudence was born from a review, not
a plan. Each time, the evidence came first. This inversion is what now distinguishes Genesis from a
classical project — and this ADR exists so the inversion is a discipline, not a streak.

## Consequences

- **Research becomes deterministic — in a precise sense.** Not that it always finds the same things:
  that two people applying the method honestly are led to the same evidence, the same precedents, and
  the same invariants. They may still disagree — but no longer at random. The method progressively
  shrinks the space of legitimate disagreement. This is the epistemic analogue of the kernel's own
  determinism, and the reason the project's reasoning is transmissible.
- **Costs, accepted:** the method is slow; discovery cannot be scheduled; observations die without
  shame; a beautiful idea with no specimen waits, sometimes forever. The Constitution values
  long-term comprehensibility over short-term velocity; this is what that costs at the level of
  knowledge.
- The method applies to discovery. Milestones remain governed by ADR-0002; the two meet where an
  accepted decision authorises a build.

## Related

- ADR-0002 — how Genesis builds; shares the epistemic base.
- DN-002, DN-003, DN-004 — the thresholds (phenomenon; name; declaration).
- `docs/Observations/TEMPLATE.md` — the field instrument.
- Journal 2026-07-26 (× 5) — the day the method completed itself and named its parts.
