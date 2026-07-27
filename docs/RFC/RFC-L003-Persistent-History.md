# RFC-L003 — Persistent History

Status: **Draft** (awaiting review)
Demand-side documents: `Lootbound-Lab/Observations/LB-Obs-008.md` and the Sprint 001 synthesis —
six of ten human narratives presuppose a continuity the world does not provide.
Founded on day one, deferred until a world demanded it. **The world has asked.**

## Which phenomenon, made inevitable by a world, does this RFC make observable?

**Lived continuity.** "L'être a fait comme à son habitude" is objectively impossible — every run
resets; no habit exists — and yet the sentence is true, because it describes the world the
narrator lives in. The player never requested a feature; they began speaking as if the feature
existed. The debt appeared not as a wish but as a **contradiction between the lived world and the
simulated one** — the narrated world became richer than the simulation. This RFC ends the
contradiction in the only honest direction: it makes the simulation catch up with the narrator.

## The scientific question

> **When does a world become as continuous as the lives lived inside it?**

## The founding question (scope — the Minimum Phenomenon Principle guards here)

> **What do we now refuse to reset?**

Not everything by enthusiasm — everything *already lived as continuous*: biographies, object
positions, repairs, wear, movements. Which is to say: **the world's state and its trace** — and
nothing that does not exist yet (no weather, no seasons, no economy; the world did not ask).

## The discovery that shapes the decision

Reset was never the world's property. `BuildInitialState()` is called by the *presentation* at
every Play — **the reset is a presentation-side habit, not a simulation-side fact.** The world has
never once asked to start over; the window has been discarding it. And the kernel already contains
the entire persistence mechanism, built for other reasons:

> **run = initial state + relations + laws + trace** (RFC-L001)

Therefore:

## Proposed decision

1. **A session is an observation window on a continuing world — not a new world.** Opening the
   window continues; it does not create. Creation becomes an explicit, rare, recorded act (a new
   world lineage), never the default.
2. **Persistence is achieved by replay, not by saving state.** The world's file *is its trace*
   (plus the identity of its initial state and law-set). Session N+1 replays
   `initial + full trace` to the current tick and continues appending. Nothing is serialised but
   what already exists append-only. L-007's sentence becomes literal engineering: *the world
   persists because nothing forgot — not because anything saved.*
3. **One world, many sessions.** Run files become session files of a named world lineage
   (World-001, sessions appended). The laboratory's readers need no change: they already consume
   traces.
4. **The reset key becomes a ceremony.** "R" no longer quietly discards a world; abandoning a
   world is an act the record keeps.

## What this buys beyond the phenomenon

**A standing audit of invariant 6, for free.** A world reconstructible from initial + trace has,
provably, no hidden state; the day replay fails to reproduce the continued world, hidden state has
been found. Persistence and explicitness become the same test.

## Consequences for the phenomena the narratives already live

Habit becomes fact (the being's routine accumulates in one world). Concealment becomes possible
(what is undone before a return is genuinely never seen). "Comme avant" becomes a true sentence.
The stranded sword at the Field stays stranded — consequences now outlive the evening that caused
them. Every fiction of Sprint 001 becomes, at minimum, checkable.

## What this RFC does not decide

Producer policy across sessions (does the being return each session? its script is a producer's
freedom, not the world's law); replay-cost management (snapshots/compaction — deferred until a
world is old enough to hurt); trace format canonicalisation beyond what exports already do;
**law changes over a persistent world** (a trace must survive a rewrite of the laws — RFC-L001 —
but a *world* continued across a law change is a migration question, explicitly deferred);
multi-world management; RFC-L002 (identity) remains its own future RFC.

## Kill criteria

This RFC dies if: replay-based persistence cannot reproduce a continued world exactly (which
would also convict the kernel of hidden state — invariant 6); or if the trace proves insufficient
in principle (not merely slow); or if a smaller mechanism yields the same lived continuity.

## The price, stated

Replay time grows with a world's age (linear; trivial at this scale; the deferred snapshot
question exists for the day it is not). Session boundaries become part of the record. And the
laboratory inherits a new responsibility, learned from LB-Obs-007 the same night as the demand:
in a persistent world, every mechanical convention a player collides with can harden into a moral
law in their head — permanence raises the stakes of every arbitrary choice we ever wrote.
