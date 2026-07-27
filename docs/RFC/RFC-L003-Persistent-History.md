# RFC-L003 — Persistent History

> **Status: Proposed by the world. Accepted by the founder (2026-07-28). Awaiting implementation
> (L-009).** The first RFC in the corpus not born of an intuition — born of a vote of the world.
> Less a design proposal than a scientific act. The prediction below is sealed by the commit that
> contains this text, before any implementation exists.

## Origin

RFC-L003 was founded on the first day of the laboratory and deliberately deferred. Its rule was
simple: **do not build persistence until a world asks for it.** Research Sprint 001 was designed
to determine whether such a demand would ever appear. The sprint is now closed. The demand
exists. It was not expressed as a feature request. It emerged repeatedly in human narratives.

## The evidence

The world reset after every run. The narrator did not. Across ten specimens, six narratives
described continuity that the simulation did not provide:

- *"l'être est revenu…"*
- *"comme avant…"*
- *"à son habitude…"*
- *"je ne voulais pas qu'il s'en aperçoive…"*

The laboratory therefore records:

> **The narrator already lives in the persistent world. Only the simulation disagrees.**
> (LB-Obs-008)

RFC-L003 is no longer justified by design. It is justified by observation. The debt appeared not
as a wish but as a **contradiction between the lived world and the simulated one** — the narrated
world became richer than the simulation.

## Scientific question

> **When does a world become as continuous as the lives lived inside it?**

Not "how do we save a game?", not "how do we serialize data?" — those are engineering questions.
The laboratory asks instead: **what must refuse to reset so that the simulated world matches the
world already described by its inhabitants?**

## Scientific hypothesis

The observed contradiction does **not** require full persistence. It requires only that the
simulation cease contradicting the narratives. Therefore:

> **The smallest persistence capable of eliminating the contradiction should be preferred.**
> (Article 7 — the Minimum Phenomenon Principle.)

## Observable prediction (sealed before implementation)

> **The first major phenomenon will probably not be relief. It will be habit.** The player will
> stop writing *"comme avant"* as a remembered fiction, and begin writing it as an observed fact.

The laboratory may be completely wrong. If another phenomenon appears first, the prediction
fails. That is acceptable. *(Scoring: first human sessions of the continued world; the standard
rules bind — no comment between run and narrative; refutable by an unimagined phenomenon.)*

## Scope

Persistent History does **not** mean permanent simulation, infinite world memory, background AI,
or global persistence. Those ideas have not yet been demanded. The world has only requested
continuity where continuity is already being narrated.

**Initial persistence candidates** — only phenomena already lived as continuous: object
positions · object biographies · wear · repairs · traveller identity · traveller inventory.
Everything else remains transient until observation demands otherwise.

**Explicit non-goals:** seasons, economy, villages, relationships, reputation, internal memory,
adaptive behaviour. Those remain debts of the future.

## Realisation (the discovery that shapes it)

Reset was never the world's property. `BuildInitialState()` is called by the *presentation* at
every Play — the reset is a window-side habit, not a simulation-side fact. The world has never
once asked to start over; the window has been discarding it. And the kernel already contains the
entire persistence mechanism, built for other reasons: **run = initial state + relations + laws +
trace** (RFC-L001). Therefore:

1. **A session is an observation window on a continuing world — not a new world.** Creation
   becomes an explicit, rare, recorded act; never the default.
2. **Persistence by replay, not by saving.** The world's file *is its trace* (plus the identity
   of its initial state and law-set). Session N+1 replays `initial + full trace` and continues
   appending. Nothing is serialised but what already exists append-only. L-007's sentence becomes
   literal engineering: *the world persists because nothing forgot — not because anything saved.*
3. **One world, many sessions.** Run files become session files of a named world lineage; the
   laboratory's readers already consume traces and need no change.
4. **The reset key becomes a ceremony.** Abandoning a world is an act the record keeps.

**Bought for free: a standing audit of invariant 6.** A world reconstructible from initial +
trace provably has no hidden state; the day replay fails to reproduce the continued world, hidden
state has been found. Persistence and explicitness become the same test.

## Expected risks — the losses, predicted alongside the gains

Persistence may destroy existing phenomena: recognition may disappear because repetition becomes
ordinary; interference may become commonplace instead of surprising; attachment may migrate
toward places rather than objects. The laboratory therefore predicts not only gains but losses.
**Losses are observations.** And one responsibility, learned from LB-Obs-007 the same night as
the demand: in a persistent world, every mechanical convention a player collides with can harden
into a moral law in their head — permanence raises the stakes of every arbitrary choice ever
written.

## What this RFC does not decide

Producer policy across sessions (does the being return each evening? a producer's freedom, not
the world's law); replay-cost management (snapshots/compaction — deferred until a world is old
enough to hurt); law-change migration over a persistent world (explicitly deferred); multi-world
management; RFC-L002 (identity), its own future RFC.

## Success criterion

RFC-L003 succeeds **not** when persistence exists, but when the contradiction disappears.

```
Today:              After RFC-L003:
Narrative:          Narrative:
"He came back."     "He came back."
Simulation:         Simulation:
No previous         Yes.
run existed.        He did.
```

Only then will the laboratory consider the debt paid.

## Kill criteria

This RFC dies if replay-based persistence cannot reproduce a continued world exactly (which would
also convict the kernel of hidden state — invariant 6); if the trace proves insufficient in
principle (not merely slow); or if a smaller mechanism yields the same lived continuity.

## Provenance

Founded: Genesis day one (deferred intentionally). Demanded: Research Sprint 001. Primary
evidence: LB-Obs-008 — *the narrator lives in a persistent world; the world is not one* — and the
Sprint 001 synthesis, where persistence across sessions became the world's strongest recurring
demand (6 of 10 narratives).

## Closing note

> **The laboratory did not decide that the world should remember.
> It waited until forgetting became the least believable thing in the world.**
