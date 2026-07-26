# Genesis-013 — First Observation

*"The world becomes observable."*

## Purpose

> **Render the world without interpreting it.**

That is all. No mention of Source. No gameplay. No Lootbound.

For twelve milestones Genesis built a world and proved its properties with executable evidence. The
world is correct, deterministic, reproducible — and invisible. Genesis-013 adds no law. It builds the
first instrument through which the existing laws can be perceived, without changing a single kernel
behaviour.

This milestone also inverts the method for the first time:

```
before:  Question → Architecture → Implementation → Tests
now:     Observation → Questions → Tests → Architecture (if necessary)
```

Architecture becomes, at most, the last step.

## Invariants

### 1. Observation never changes reality.

The observer holds no mutable reference to the world, performs no Transition, invokes no Resolver.
It only looks. (Structurally cheap to honour: `SimulationState` is immutable — a snapshot cannot be
written through.)

### 2. The observer observes snapshots, never the simulation.

The kernel runs; it publishes an immutable state; the observer reads it. Never the reverse. The world
could run on another thread, another machine, or be replayed from disk — the observer would not know.

### 3. Everything visible must be derivable.

Every pixel must be explainable from a snapshot. A bright circle → a value exists. A halo → a value
exists. No hidden graphical state, no caches with meaning, no interpretation.

### 4. Removing the observer changes no theorem.

Delete all of Genesis-013 and the kernel's 70 proofs stay green, untouched. The kernel does not know
it is being watched. (Enforced at compile time: the dependency arrow runs Presentation → Simulation
and cannot run back.)

## The laboratory

Not a game. A light table. Open it, and the world is immediately breathing.

- **Place** — a plain circle. No sprite, no mesh, no icon: a place is not an object, it is a point of
  structure.
- **Relation** — an extremely thin, quiet line. Support for phenomena, never the attention's thief.
- **Cells** — not displayed at first. One kind is shown; others on demand.
- **Quantity** — the circle's fill: dark at zero, white at the maximum currently observed (a
  derivable normalisation).
- **Rate** — not a number: a halo. The eye is extraordinarily good at halos.

**Controls** — four buttons: `Play · Pause · Step · Reset`. No timeline, no ×100: time must stay
legible.

**Camera** — pan, zoom. Nothing else.

**Snapshot** — the current tick, displayed. That is all.

**Observation panel** — click a place, see only facts, never words:

```
Place 1
  Quantity : 43
  Rate     : 2
  Outgoing : 2   Incoming : 2
```

No "Source" appears anywhere in the interface. The Glossary stays in the reader's head, not in the
pixels.

**Record / Replay** — *planned from the start, not built now*: recording snapshots (not video) so an
observation can be shared exactly — "look at Replay #18" meaning the same world, not an
approximation.

## Deliberately excluded

No shaders, no complex animation, no effects, no modern UI kit, no custom Inspectors, no timeline —
everything that could mask what we are trying to observe. This is an oscilloscope, not a product.

## The hidden goal

This scene is not built for its authors. It is built for someone discovering Genesis: clone the
repository, press Play, and watch the world live — before reading a single line of documentation. At
that moment the documentation becomes secondary; the world speaks for itself.

## Completion

Genesis-013 is complete when:

- the world can be observed without being modified;
- the snapshots observed are fully deterministic;
- the interface knows no world-category words (Source, Entity, …);
- the phenomena are perceptible to the naked eye;
- the laboratory is simple enough that a newcomer understands Genesis by watching it run.

And one criterion unlike any before it:

> **Genesis-013 does not discover anything.**
> **It builds the instrument that will allow future discoveries.**

If this milestone ends with "we discovered X", it went too fast. Its success is a sentence of a
different kind: *we finally own an instrument capable of discoveries.* No law. No new word. A
laboratory.

## The symmetry

- Genesis-001 — *the world exists.*
- Genesis-006 — *the world acts.*
- Genesis-012 — *the world earns its first word.*
- Genesis-013 — *the world becomes visible.*
