# L-006 — The Traveller

*The first milestone of Era II — **the worlds live**. Article 6 is satisfied: the reader is ahead;
the world may grow. The Traveller is not a feature. It is Era II's first phenomenon.*

Status: **Prediction sealed; implementation not yet begun.** (This document is committed before any
Traveller code exists — the ancestry is the proof.)

## The opening sentence of Era II

Not "let's add an NPC." The sentence is:

> **The world becomes capable of producing several simultaneous biographies.**

## The scientific question

Not "is the Traveller believable?" —

> **Can two biographies interact without having been written for each other?**

If yes, Era II is launched.

## The Traveller's capabilities (deliberately meagre)

Choose a destination · walk · pick up an object · put down an object · leave the world.
No dialogue. No combat. No sophisticated inventory. No personality.

## The founder's sealed prediction (filed before one line of code)

> **The first major phenomenon of Era II will probably not be an encounter. It will be an
> absence.** The player will arrive somewhere; something they expected will no longer be there —
> not because the game wanted it, but because another biography passed first. The observation will
> not read "the player met the Traveller." It will read: **"the player met the consequences of a
> biography they never saw."**

*Scoring rule, declared now: this prediction is NOT scored on scripted co-runs (a scripted
traveller that moves objects makes absences likely by construction). It is scored on the first
human run with the Traveller active: does the human's observation — behaviour or narrative —
register an absence before an encounter?*

## Architectural commitments (declared before building)

- The Traveller is a **producer**, not a law (ADR-0005): it stands outside the membrane, reads
  snapshots as any observer may, and emits external events. The world cannot know a traveller
  from a player from a replay — provenance-blindness extends to embodiment.
- The world gains a second **body** (a position marker and its intent kinds) — the world knows
  two bodies; it never knows who drives them.
- Every act crosses the membrane and lands in the same trace. One trace, several biographies —
  replay stays exact and free.
- Departure may not destroy: a body that leaves the world puts down what it holds. Objects are
  conserved; only drivers leave.
