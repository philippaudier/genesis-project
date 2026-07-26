# LB-Obs-003

> **The better sword was held twice and returned twice. Neither the biography nor the narrative
> contains this.**

Date: 2026-07-27
Build: L-002 (commit `f761bc4`)
Method: forensic replay of Run-000002's trace (first use of L-001's replay guarantee on a lived
session). Replay reproduced the exported end-state audit to the digit — old sword in hand,
repairs 2, better sword at the clearing, wood 29 — before any claim below was recorded.

## Context

```
Run:      000002
Object:   Sword-1000 (old) and Sword-2000 (better)
World:    L-002
Producer: Human (the founder)
```

## Observed facts (replay-verified)

```
t=250   Swap at the clearing:  old laid down, better IN HAND
t=262   Swap back:             old IN HAND, better on the ground
t=267   Swap:                  better IN HAND again
t=269   Swap back:             old IN HAND — final

The better sword was in the subject's hand for 12 ticks, then again for 2 ticks.
Four Act-Clearing crossings produced this (t=248, 260, 265, 267 in the trace).

Also compressed away by the biography: at the shelter, TWO full store/retrieve cycles
(t=507 stored, t=513 retrieved, t=517 stored again, t=525 retrieved again) — the biography
records only the two firsts.

In total: nine sword-location transitions lived; three told.
```

```
Biography: says "Replacement refused (left the clearing carrying the old sword)" at t=218 —
           true at t=218, and silent about everything at t=248–269.
Narrative: does not mention holding the better sword.
Analysis (the founder's, pre-replay): read the run as leaving the better sword without
           taking it. The record shows otherwise.
```

Only the world remembered.

## What the laboratory does not know

```
- Whether the swaps were deliberate trial, curiosity, input fumbling, or confusion between
  the two swords (the interface distinguishes them by label only).
- Whether the subject remembers the episode and did not report it, or does not remember it.
- What, if anything, the 12 ticks of holding felt like.
```

## Derived behavioural observation

```
The replacement was taken, held, and given back — twice — and the run ended with the old
sword in hand. "Not taken" (LB-Obs-001's bot form) does not describe the human record.
```

## Alternative explanations (preserved, none privileged)

```
A1   Deliberate comparison: take, weigh, return.
A2   Input fumbling (the t=265/t=267 pair is 2 ticks apart; the t=248/t=260 pair is 12).
A3   Curiosity followed by preference for the invested object (repairs had been spent on it).
A4   Label confusion: the subject may not have known which sword was in hand.
```

## Ledger

```
RD-L1 strengthened and transformed: keep-despite-better now includes take-and-return —
      a richer phenomenon than refusal, and the bot/human records are NOT structurally
      similar (the bot never held it).
RD-L6 second limit documented: a firsts-only reader missed six of nine transitions.
RD-L7 proven by instance: the biography lost the run's most telling episode — and so did
      the subject's own narrative. The compression is not only the reader's; memory
      compresses too.
```

## Status

Open — first observation produced by forensic replay. The world is the only source of truth;
this is what that article looks like as data.
