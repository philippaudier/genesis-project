# Genesis Glossary

Words mean specific things here. This document defines them.

Each term carries a status:

- **Active** — implemented in the kernel; the word and the code agree.
- **Reserved** — part of Genesis's language, awaiting its incarnation; defined so the concept is not
  invented twice.
- **Avoided** — deliberately not part of Genesis's language, with the reason.

Rebuilt after the kernel's completion (2026-07-26) so the glossary speaks the language the code
actually speaks. The Constitution's vocabulary (Articles I–XIV) is not repeated here; this glossary
covers the engineering language beneath it.

**Admission rule** (DN-003, plus this glossary's editorial rule): a pattern earns a name when the
world can demonstrate a theorem about it — objectively derivable from declared state, with a statable
signature that vanishes under ablation. And: *a demonstrated theorem is necessary for a new term, but
a new term is justified only if it compresses the language used to describe the world.* The world
demonstrates; this glossary decides whether the demonstration deserves a word.

---

## Active

### World
The totality of simulation state. Everything that exists, exists in the world; there is no state
outside it. The world is not a container that holds things — the world *is* the state, and what we
call things are patterns within it.

### State
Data that represents what is — held in a `SimulationState`, which is **immutable**: a tick never
mutates a state, it produces the next one. State is always observable; there is no hidden state in
Genesis (invariant 6).

### Snapshot
The immutable state a tick begins from. Every transition in a tick reads the same snapshot — direct
reads and relation-discovered reads alike — and none observes another's writes. The snapshot is what
makes simultaneity well-defined.

### Tick
A **logical execution step** — an ordinal, not a duration. Advancing by one tick carries the world
from one defined state to the next; real time is only a playback rate on the presentation side. The
tick is machinery in service of transformation, not the fundamental unit of the model (RFC-0001,
Accepted).

### Transformation
The generic act of change: how one state becomes the next. The Constitution's primary concept
(Article V) — in the kernel it is realised by *transitions*.

### Transition
The atomic unit of transformation (ADR-0001): a pure function that reads its declared view of the
snapshot and returns contributions to the next state. Transitions never know each other, never see
the full state, and never mutate anything.

### Contribution
An identifiable write proposed by a transition: an amount targeting a specific address. Contributions
— not opaque states — are what transitions produce, which is what makes writes explicit and conflicts
detectable.

### Place
The stable, explicit identity of one location (RFC-0003). A place says nothing about position or
meaning; it makes a location distinguishable and stable. Relations connect places. A place exists iff
at least one cell is declared at it — existence is derived, never registered.

### Kind
The identity of a *nature* of value (RFC-0003). A kind carries a causal role, not a value type — two
integer values with different kinds are not interchangeable. Conflict semantics attach to kinds: a
kind's resolver is uniform wherever it occurs.

### Cell
One value-holding location: the pair (Place, Kind) — the addressable unit. The cell is the unit of
writing and conflict; equality of state depends on (place, kind, value) triples, never on storage
order. Storage layout is an implementation detail: the curried and uncurried representations are the
same world.

### Relation
An explicit, directed connection between two places — source → target — carrying no meaning of its
own: no kind, weight, distance, or space. The relation discovers places only; which kinds a
transition reads across it is the transition's declaration. Relations live in a `RelationSet`,
validated against existing places, insertion-order independent.

### Scope (ReadScope / RelationScope)
A transition's declared contract of observation. `ReadScope` names the cells it may read directly;
`RelationScope` names the origin places whose outgoing relations it may observe, plus the kinds it
may read at the places they discover — strictly one hop, non-transitive. Undeclared state is absent
from the transition's view, not hidden.

### Conflict / Resolver
A conflict is two or more contributions targeting the same cell in one tick. It resolves through the
cell's *kind's* explicit **commutative** resolver, invoked exactly once — or is rejected if none is
defined (DN-001). Same place, different kinds: no conflict. Order-independence is the law:
enumeration order never determines a committed result.

### Determinism
Identical initial state and inputs produce identical results. Genesis requires **Level 1 — logical
determinism** (same decisions, same transformations); desires Level 2 (platform-identical); defers
Level 3 (bit-identical across platforms) to a future numeric-guarantees RFC. Determinism is not a
feature; it is a requirement.

### Emergence
Complex behaviour arising from simple rules — discovered, not designed. Genesis does not script
complexity; it defines simple laws and observes what their interaction produces.

### Causality
Every state change has a cause, and that cause can be traced. In the kernel: every change is a
contribution, produced by a transition, from a declared view, at a specific tick.

### Persistence
State endures; changes are permanent unless another transformation reverses them. Persistence is what
makes consequence possible.

---

## Active — world categories

*Words the world has earned. Each entry here passed the admission rule: an objective predicate, a
demonstrated theorem, a signature that vanishes under ablation — and the editorial test of
compression. The engine knows none of these words.*

### Source
*The world's first word. Admitted 2026-07-26, upon Genesis-012.*

A persistent local pattern of state whose declared production rate is positive, and whose presence
causes unpaired positive contribution to a transported quantity.

- **Predicate:** `IsSource(place) := production rate at place > 0` — deterministic, derived from
  declared state alone.
- **Theorem:** `ΔTotal = ΣProduction` — the total grows by exactly the declared production;
  accounting where conservation used to be.
- **Signature & ablation:** set the rate to zero under identical laws and the growth vanishes —
  conservation returns. Remove the relations and production continues but stays local.
- **Standing:** a Source affects without being affected — its rate is never written by what it
  drives. The asymmetry is structural, not asserted.

The word compresses a predicate, a theorem, an ablation result, and a causal role into one term; no
kernel primitive, object, or identity corresponds to it. A Source is somewhere a predicate is true —
nothing more, and demonstrably nothing less.

---

## Reserved

### Constraint
A predicate on legal states — it *forbids*, it does not produce. The third element of the provisional
triad (State · Transition · Constraint) established by the research arc; its layer is not yet built.
Example: an object cannot occupy the same space as another solid object.

### Rule
A declarative definition of when and how transformations occur ("flammable material adjacent to fire
becomes fire"). Reserved: today all transitions are code; rules-as-data (interpreted, authorable) are
a possible future layer above transitions.

### Process
An ongoing transformation spanning many ticks, tracking progress toward completion (a fire burning,
wood rotting). Reserved: RFC-0001's open Q8; today long-running change must be expressed as state
that per-tick transitions read.

### Entity
A pattern of state that humans recognise as a "thing." Not fundamental — the world contains state,
not entities; we group related state and name it. Reserved until a world exists to pattern.

### Event
A transformation scheduled for a specific future tick. Reserved: ADR-0001 chose uniform per-tick
evaluation; event scheduling remains a future semantics-preserving optimisation, never a semantic
model.

---

## Avoided

### Agent
Implies autonomy and decision-making. In Genesis, nothing decides — the world transforms state. Where
agent-like patterns appear, they are state plus transitions, like everything else.

### Behavior
Implies intention. Entities do not behave; the world transforms their state.

### AI
There is no artificial intelligence in Genesis. There are laws. Complexity emerges from laws, not
from intelligence.

### Script
Scripts dictate sequences of outcomes. Rules define transformations that apply when conditions hold.
Genesis has no scripts, and the difference is the project.

---

*Precision in language enables precision in thought.*
