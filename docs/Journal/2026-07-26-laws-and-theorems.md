# 2026-07-26 — Laws and theorems

A design conversation ahead of Genesis-011 crystallised several things worth keeping. No code was
written; the kernel was not touched.

## The distinction

A **law** is introduced. A **theorem** is demonstrated. The kernel contains laws — snapshot
semantics, identifiable contributions, commutative conflict resolution. Genesis-011 will demonstrate
the kernel's **first theorem**: conservation. No type, flag, or system states it; it follows
mathematically from paired `−q`/`+q` contributions and additive resolution, and it will be named only
in the tests that observe it. From Genesis-011 onward the project's story gains a category: RFCs and
ADRs describe the axioms; milestones begin discovering what follows from them.

(No `docs/Theorems/` registry is created — that would be documentary anticipation at the exact moment
the project turns empirical. Theorems live in tests and in this Journal until there are enough of
them to earn a record.)

## The conjecture to watch (not for Genesis-011)

First stated as: *the total quantity is independent of transition evaluation order.* On examination,
that is already proven — and more strongly: the kernel's tests show the **entire state** is
order-independent. The sharper conjecture underneath it:

> **The resolver's algebra determines which global invariants survive conflict.**
> Commutativity ⇒ order-independence. **Additivity** ⇒ conservation. A merely-commutative resolver
> (e.g. `max`) preserves order-independence while destroying conservation — `max(+2, +3)` discards
> two units.

So "where does conservation live — Transition or Constraint?" (the research arc's old question)
receives the beginning of an answer: **in the algebra.** What Experiment-01 called structural
conservation returns for the third time, now as a theorem family rather than a mechanism choice.
To be watched, not proven yet.

## Two commitments recorded

- **WriteScope: deliberately nothing.** Reads are structurally scoped; writes are not. The asymmetry
  is real, noted, and *left alone*: adding a WriteScope today would answer a question no phenomenon
  has asked. The kernel has earned the right to wait for facts.
- **The guard sentence for the Genesis-011 spec:** *"Genesis-011 demonstrates the kernel; it does not
  extend it."* Every attractive idea during implementation (`OutgoingContribution`, `Flux`,
  `Neighbour`…) faces one question: does it demonstrate, or extend? If it extends, it belongs to
  another milestone. (The first such idea was proposed and self-retracted in the same conversation —
  ADR-0002 working as a reflex, not a rule.)

## The turn

The architecture has changed régime. During the kernel's construction the question was *"what does
the engine need?"* From here it is *"what do phenomena actually force us to add?"* — constructive
before, empirical now. The closing line of the Kernel Record was a promise; this is what keeping it
looks like in practice: the engine stops being built and starts being learned from.
