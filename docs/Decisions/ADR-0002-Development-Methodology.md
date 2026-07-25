# ADR-0002 — Development Methodology

## Status

**Accepted** — 2026-07-26.

This ADR codifies, retroactively and deliberately, the methodology that governed Genesis-004 through
Genesis-010. It stopped being an experiment the moment it produced a proven kernel; a rule that has
governed seven consecutive milestones is a project decision and deserves the same standing as the
architecture it built (ADR-0001).

## Context

During the kernel sprint, a working method emerged milestone by milestone rather than being designed
up front. By Genesis-010 it was fully formed and demonstrably effective — but it existed only in
conversation, in `.claude/current.md`, and in habit. A load-bearing process that is written nowhere
durable is hidden state, and Genesis forbids hidden state in its world; its own development should
meet the same bar.

Separately, the research phase produced a distilled experimental methodology
(`docs/Research/Experimental-Methodology-Distilled.md`) whose three principles were left awaiting
adoption — by its own third principle, a distillation cannot adopt itself; adoption is a decision.
This ADR is that decision.

## Decision

Genesis development follows the **Proof Rule** and its supporting disciplines:

1. **One property per milestone.** Every milestone exists to prove exactly one falsifiable property
   of the engine. Not two. Not three. One.
2. **The proof is a sentence, written first.** A milestone is defined by the statement
   *"Genesis-N proved that …"* — drafted before any code, and only allowed to switch to the past
   tense when executable tests make it true. A milestone whose sentence cannot be written is not
   done, whatever compiles.
3. **Proofs are executable.** Every proof is carried by tests. Prose claims nothing that a test does
   not enforce.
4. **Proofs are cumulative; regressions are forbidden.** Every prior milestone's tests keep running,
   forever. A milestone extends the ones before it; it never weakens them. The test suite is the
   living specification of the engine.
5. **A milestone may implement a decision, but must never silently make one.** Open architectural
   questions are closed deliberately — through an RFC, an ADR, or a Design Note — never as a side
   effect of implementation or cleanup. When work reveals an undecided question, the work stops or
   routes around it, and the question is recorded.
6. **The smallest witness.** Each proof uses the least domain content that can make it true. Witness
   state (the counters) is never the subject — the engine is.
7. **Stop when Done.** When the milestone's proof holds, work stops. The next improvement belongs to
   the next milestone.
8. **Honest limitations, recorded.** What a milestone does *not* prove, and what is enforced by
   discipline rather than structure, is written into `.claude/current.md` at the moment of stopping —
   never discovered later.

The **experimental principles** underlying this practice are adopted with it (from the distilled
methodology): *the claim is the unit* (one claim, fixed before the result, with explicit scope);
*the claim must be refutable*; *experiments eliminate, they do not decide* — selection and
construction are deliberate acts recorded in RFCs and ADRs, which experiments and milestones inform
but never replace.

## Consequences

- **Benefits (observed, not predicted):** seven milestones, zero regressions, a kernel whose whole
  behaviour is specified by its own test suite, and a decision trail in which every architectural
  commitment can be traced to a deliberate act. Reviews became cheap because each milestone's claim
  was small and explicit.
- **Costs (accepted):** the ceremony is real — proof sentences, scoped prompts, recorded
  limitations, and refusal of opportunistic improvements slow individual steps. Genesis accepts the
  trade: the Constitution values long-term comprehensibility over short-term velocity.
- The methodology applies to engine milestones. Documentation phases (like the one this ADR belongs
  to) follow the same spirit — single responsibility, deliberate decisions, honest records — without
  forcing a test-executable proof where none can exist.

## Related

- ADR-0001 — the architecture this methodology produced.
- `docs/Research/Experimental-Methodology-Distilled.md` — the epistemic base, now adopted.
- `docs/Engineering/workflow.md` — the day-to-day loop (Before / During / After, Definition of Done,
  Stop when Done), which this ADR elevates rather than replaces.
- `docs/Design/04-KERNEL.md` — the record of what this methodology proved.
