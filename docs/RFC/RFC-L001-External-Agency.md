# RFC-L001 — External Agency

Status: **Accepted** (2026-07-26 — reviewed by the founder; revisions from that review integrated:
invariant-before-mechanism ordering, the membrane formulation, law provenance-blindness, the
event/command guard, "external event" vocabulary)
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

## The problem: causal legitimacy

Genesis-003 decided that state is externally owned: the runner transforms states, it never owns
them. That decision left a door open — the host can hand any state it likes to the next tick. So
external agency is already *possible* today, trivially, by mutating state between ticks.

It is possible and it is **illegitimate**. A host mutation leaves the engine correct and destroys
the laboratory: the trajectory acquires a cause that belongs to no law —

```
State → ??? → State
```

— and the `???` is documented nowhere. For Genesis, that is exactly an observation without a
protocol. The problem this RFC solves is not agency; it is **causal legitimacy**: agency existed
already — it was merely illegitimate.

## The invariant (before any mechanism)

> **Invariant 7 — No hidden hand.** *A world under observation is never modified directly by its
> host.* (Now standing in `docs/Engineering/invariants.md`; descends from Determinism and
> Explicitness. This is a constitutional consequence, not an API decision.)

**Consequence:** the laboratory ceases to be a purely closed system. It becomes **permeable, with
a perfectly defined membrane**. The only things that may cross the membrane are **external
events**: declared, recorded, replayable, and interpreted only by laws.

The mechanism below is the chosen *realisation* of that membrane — it exists because the invariant
demands one, not because the architecture wanted a feature.

## The membrane, realised (the accepted mechanism)

1. **Worlds declare their external Kinds.** External cells are ordinary cells — same reads, same
   scopes, nothing privileged about them.
2. **An external event** = values for external cells at a tick boundary. It is applied as
   contributions to external cells, resolved by the same per-Kind resolvers as everything else —
   no second write path, ADR-0001 untouched. Simultaneous events on one cell meet a resolver like
   any other conflict.
3. **Boundary only.** Events become visible in the next snapshot, never mid-tick. Snapshot
   immutability is preserved exactly.
4. **The external event trace.** Every event is appended to the trace *before* it is applied.
   The trace is part of the world's record:

   > **run = initial state + relations + laws + external event trace**

   Replay of a played session is exact, forever. The trace is append-only — the first stone of the
   chronology RFC-L003 will need: one record, two consumers.
5. **Laws alone convert events into effects.** The meaning of an event lives in the laws that read
   its cells — where Genesis has always kept meaning. "Intent" stays subtracted.

Constitutional grade, both directions now:

> The world is what the laws produce — **and what the laws receive.**
> The agent does not act on the world. The agent's act becomes world — then the laws act.

## Provenance-blindness (from review; binding)

**Laws cannot know whether an event came from a player, an AI, a script, a benchmark, or a
replay.** This is not a guideline — it is enforced by construction: provenance lives only in the
trace, which laws cannot read; the world's state contains no provenance. Therefore no law can
contain the moral equivalent of `if (playerInput)`. A player, a bot, and a replay that write the
same values to the same cells are **the same experiment**.

## The event/command guard (from review; binding)

The trace records **facts of the boundary, never interpretations**:

```
legal in the trace:    InteractPressed = 1        (with world state: facing = repair station)
illegal in the trace:  RepairSword
produced by a law:     sword repaired             (an effect, in state — never in the trace)
```

Nineteen milestones were spent moving interpretation into laws; the membrane must not smuggle it
back out. **Mechanical criterion:** *the trace must survive a rewrite of the laws.* If replacing
an interpreting law would change what a recorded event means, the event was a command, and it is
illegal. Corollary — a capability, not a cost: **counterfactual replay**. The laboratory may
replay a session's trace under modified laws and ask *what would this session have been?* That
question is only well-posed because the trace is interpretation-free.

## Consequences

- **Observability**: "why did this cell change?" is always answerable from laws + trace. The
  hidden hand is dead; causality stays traceable through a human being.
- **The laboratory reads, the game plays**: LB-001's telemetry reads trace and state; the gameplay
  layer never learns the laboratory exists.
- **Reproducibility of experience**: a play session is a world object — replayable, attachable to
  an observation, comparable across players and across law-versions.
- **Future externals for free**: weather, NPC migration, server events, seasons — all are external
  events of exactly this nature. The membrane does not care who knocks.
- **Kernel impact stays small**: external cells are cells; event applications are contributions;
  the only new objects are the trace and the boundary at which it is applied.

## What this RFC does not decide

Deferred, each waiting for its own demand: event *validation* (is a refused event recorded?
probably yes, as a refused entry — undecided); real-time-to-tick mapping (the presentation clock);
multi-agent arbitration; the full chronology object (RFC-L003); object identity under repair and
renaming (RFC-L002).

## Kill criteria

This RFC dies if: a world is exhibited whose required agency cannot be expressed as external-cell
events without loss (an act that *must* bypass laws to mean what it means); or the trace mechanism
is shown to break conditional determinism; or a simpler mechanism keeps all four membrane
properties (observable, attributable, replayable, resolver-uniform) with less.

## The price, accepted

Every act needs an interpreting law — the first Lootbound laws will be written sooner than
expected. Reviewed and accepted knowingly: the alternative was acts with law-power and no
law-duties.
