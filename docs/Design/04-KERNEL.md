# The Kernel — Completion Record

*The formal close of Genesis's first construction: the computational kernel. Frozen at tag
`genesis-core-kernel`. This document records what questions existed, what was decided, what was
proven, and what remains deliberately open. In five years, read this before reading any changelog.*

---

## Declaration

**The computational kernel of Genesis is complete.**

It is the machinery by which a world — any world — advances one step:

```
snapshot → scoped reads → transitions → contributions → conflict resolution → commit → next snapshot
```

It contains no world. No creature, terrain, item, or player appears anywhere in it. This is not an
omission; it is the point. Genesis built the laws before permitting the world to exist, so that the
world, when it arrives, must obey physics that predate it.

## The questions that existed at the start

The kernel began as one naive question — *how does a tick transform a state?* — which refused to stay
one question. Answering it honestly required answering six:

How does time advance without belonging to the renderer? How do many computations combine without
knowing each other? What happens when two changes want the same thing? What is a cause allowed to
observe? What does it mean for two pieces of state to be distinct? What does it mean for them to be
connected?

## The decisions that were taken

Each was taken deliberately, in a document, never as a side effect of code:

- **ADR-0001 — The Snapshot Transition Model.** Every tick, every transition reads an immutable
  snapshot and produces contributions to the next state; no transition sees another's writes within a
  tick; one mechanism serves fields and agents alike. (Resolves RFC-0002; informed by the
  Transformation-Model research arc and its adversarial critiques.)
- **RFC-0001 — The Tick (Accepted).** A logical, integer, host-independent step under Level-1 logical
  determinism. Real time is a playback rate, not a property of the world.
- **DN-001 — Conflict policies.** Order-independence disqualified last-writer-wins before taste could
  enter; conflicts resolve through explicit commutative resolvers, with rejection as the default.
- **ADR-0002 — The methodology itself.** One proved property per milestone; proofs executable and
  cumulative; decisions never made silently; stop when done.

## The proof timeline

| Milestone | Property proved |
|---|---|
| Genesis-004 | **Snapshot Transition** — deterministic state advancement over arbitrary ticks, no within-tick mutation exposed |
| Genesis-005 | **Composition** — independent transitions combine order-independently, in mutual ignorance |
| Genesis-006 | **Conflict Resolution** — competing contributions resolve exactly once, deterministically, or are rejected |
| Genesis-007 | **Scoped Reads** — a transition observes only its declared state; the rest is absent, not hidden |
| Genesis-008 | **Addressable State** — homogeneous locations hold stable identity; equality ignores storage order |
| Genesis-009 | **Explicit Relations** — locations connect through directed, meaningless, validated relations |
| Genesis-010 | **Relational Views** — a transition observes one-hop relation-discovered state, deterministically* |

Every proof is executable, and every earlier proof still runs — the suite is cumulative, so the
kernel's specification and its regression guard are the same artifact.

\* *Genesis-010's suite was authored with the kernel frozen at 009 (45 proofs green); its EditMode
validation completes the record.*

## What remains deliberately open

None of these is forgotten; each is parked by name, waiting for the milestone or RFC that owns it:

- the **constraint layer** (the triad's third element — predicates that forbid, distinct from
  transitions that produce);
- the **numeric representation** of world quantities (float / fixed-point / integer quanta — a
  dedicated RFC);
- **event scheduling** as a semantics-preserving performance optimisation over uniform per-tick
  evaluation;
- **topology placement** (relations currently live beside the state; whether they may enter it and
  change under transitions is undecided);
- **address lifecycle** (locations can neither appear nor die yet);
- **causality metadata / history**, **external inputs**, **serialisation**, and everything
  presentation.

Two honest structural notes, standing: resolver commutativity and read-scope honesty are enforced by
tests and discipline, not yet by the type system; and the one-mechanism choice of ADR-0001 remains a
monitored bet whose first real test arrives with the first field phenomenon.

## Close

Eleven milestones separate an empty Unity project from a proven computational kernel. The counters
that witnessed every proof never mattered in themselves; they stood in for a world that did not yet
exist. The next chapter gives that world its first phenomenon — behaviour that could not exist
without the laws recorded here.

The kernel is done. The world is next.
