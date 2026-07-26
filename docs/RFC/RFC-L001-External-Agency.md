# RFC-L001 — External Agency

Status: **Draft** (awaiting review)
Demand-side document: `Lootbound-Lab/Campaigns/LB-001-The-Ordinary-Sword.md`
First of the `L` series: RFCs demanded by a world, not by architecture.

## Which phenomenon, made inevitable by a world, does this RFC make observable?

**Attachment.** LB-001 must observe a player keeping, repairing, naming and retrieving a sword.
None of those events can occur in a closed world: every one of them is an act of an agent who is
not a law. Genesis has only ever observed worlds whose entire future was contained in their initial
state. Lootbound's first experiment is impossible until an external act can enter a world — and
enter it in a way that leaves the act *observable, attributable, and replayable*. That is the
phenomenon this RFC serves: not "input handling", but **the external act as a reproducible
observation**.

## The problem

Genesis-003 decided that state is externally owned: the runner transforms states, it never owns
them. That decision left a door open — the host can hand any state it likes to the next tick. So
external agency is already *possible* today, trivially, by mutating state between ticks.

It is possible and it is **unobservable**. A host mutation is a hidden hand: the resulting
trajectory contains a change no law produced, no record explains, and no replay can reproduce. It
violates traceable causality without violating a single line of the kernel. The problem is
therefore not to *enable* agency — it is to **discipline** it: to define the one channel through
which an outside act becomes part of the world's record, and to ban every other.

## Constraints (inherited, not negotiable here)

1. **ADR-0001** — one mechanism per tick: snapshot → contributions → resolution → commit. No
   second write path.
2. **Determinism** — with agency, determinism must become *conditional determinism*:
   state(t+1) = F(state(t), inputs(t)). Same state, same inputs ⇒ same next state, always.
3. **The world is the only source of truth** — an act that influenced the world must be readable
   *from* the world's record, not from the host's memory.
4. **No special cases** — whatever inputs are, they must not be a privileged write that bypasses
   resolvers.
5. **Research-arc precedent** — "Intent" was subtracted as a primitive during the ontology arc.
   This RFC must not smuggle it back: an input is not a wish with semantics; it is data the laws
   interpret.

## The decision space

**Model A — Host mutation (status quo).** The owner edits state between ticks via `WithValue`.
*Rejected*: unobservable, unattributable, unreplayable — the hidden hand. This RFC's first
consequence is that Model A becomes **illegal for any world under observation**: a Part-B
invariant, test-guarded where possible.

**Model B — Input as boundary contributions.** An input is a set of `Contribution`s injected into
tick t's pool from outside, merged by the same per-Kind resolvers as law contributions.
*Sub-problem*: an arbitrary contribution ("+10⁶ to the sword's durability") is an act *on* the
world's internals, not an act *in* the world. It grants the agent the power of a law without the
accountability of one.

**Model C — Input as declared state (the production precedent).** The kernel already contains an
input channel and has since Genesis-012: the **rate cell**. A production law reads a rate cell and
acts; the rate cell is where the world's configuration meets the law. Generalised: worlds declare
**input Kinds** — ordinary cells, readable by laws like any state. An external act is a write to
input cells only, applied at a tick boundary, recorded in an append-only **input trace**
(tick, cell, value). Laws read input cells and produce all actual effects; the act itself never
touches a non-input cell.

## Proposed decision

**Model C, carried by Model B's machinery.** Concretely:

1. Worlds declare their input Kinds. Input cells are ordinary cells — same reads, same scopes.
2. An external act = values for input cells at a tick boundary. It is applied as contributions to
   input cells, resolved by the same resolvers as everything else (Constraint 4: no second
   mechanism; simultaneous acts on the same cell meet a resolver like any conflict).
3. Acts are visible in the *next* snapshot, never mid-tick (Constraint 1: snapshot immutability is
   untouched).
4. Every act is appended to the **input trace** before it is applied. The trace is part of the
   world's record: **run = initial state + relations + laws + input trace.** Replay of a played
   session is exact, forever (Constraint 2). This trace is also the first stone of the append-only
   chronology that RFC-L003 will need — one record, two consumers.
5. Laws alone convert inputs into effects. The player's swing is a value in a cell; what a swing
   *does* is the combat law's business (Constraint 5: no Intent primitive — meaning lives in laws,
   which is where Genesis has always kept it).

One sentence, constitutional grade:

> **The agent does not act on the world. The agent's act becomes world — then the laws act.**

## Consequences

- **Observability**: any observer can answer "why did this cell change?" by reading laws + trace.
  The hidden hand is dead; causality stays traceable through a human being.
- **The laboratory reads, the game plays**: Lootbound's telemetry (keep/repair/name events for
  LB-001) reads the trace and the state; the gameplay layer never knows the laboratory exists.
- **Reproducibility of experience**: a play session becomes a *world object* — replayable,
  attachable to an observation, comparable across players. LB-001's trajectories are exactly:
  input traces plus their deterministic consequences.
- **Kernel impact is small by design**: input cells are cells; input contributions are
  contributions; the only new object is the trace and the boundary at which it is applied.

## What this RFC does not decide

Deferred, each waiting for its own demand: input *validation* (illegal acts — is a rejected act
recorded? probably yes, as a refused entry); input *timing* across real time vs tick time (the
presentation layer's clock); multiplayer arbitration (several agents, one boundary); the full
chronology object (RFC-L003); object identity under repair and renaming (RFC-L002).

## Kill criteria

This RFC dies if: a world is exhibited whose required agency cannot be expressed as input-cell
writes without loss (i.e., an act that *must* bypass laws to mean what it means); or if the trace
mechanism is shown to break conditional determinism; or if a simpler mechanism is found that keeps
all four properties (observable, attributable, replayable, resolver-uniform) with less.

## Review question for the house

Model C makes every input pass through cells that laws must read. The cost: gameplay-rich acts
(move, swing, drop, name) each need a law that interprets them — the first Lootbound laws will be
written sooner than expected. The alternative costs more: acts with law-power and no law-duties.
Is the house prepared to pay the honest price?
