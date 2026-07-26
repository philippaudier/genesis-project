# L-001 — The First Crossing

*The first milestone of the L branch — named as the first Genesis milestones were named, because it
is not an engineering ticket: it is the birth of a new branch of the laboratory.*

## Objective

> **Build the first observable membrane between an external producer and a world.**

No gameplay. No AI. No Lootbound. Only: one event crosses the membrane correctly. If this milestone
succeeds, every future season, NPC, player, replay, benchmark and script already has its entrance.

## Definition of Done (eight points, fixed at launch)

1. An external Kind exists — nothing else.
2. An append-only trace exists: every crossing recorded; no erasure, no modification, only growth.
3. The tick boundary applies the trace — never before, never after, always at the same place.
4. A law reads the entrance and produces a change — through ADR-0001 only.
5. Replay works: same initial state + same trace + same laws = same result. Invariant 7 satisfied.
6. The provenance test: the same trace produced by a player and by a bot yields an identical final
   world. Not "almost". Identical.
7. The causality test: direct host mutation fails — by architecture, not by convention.
8. The laboratory can tell the crossing: not logs — a chronology, so a developer can *see* the
   membrane work, not merely know it exists.

**Deliberately not implemented:** movement, combat, inventory, objects, swords, repairs. One cell.
One law. One crossing. That is all.

## What was built

Kernel (`Assets/GenesisLab/Simulation/`), four new files + two extensions, all under ADR-0001's one
mechanism:

- `ExternalEvent` — one crossing: (boundary tick, target cell, amount). A fact, not an
  interpretation.
- `Membrane` — the declared set of external Kinds. Carries nothing else; producers stand outside
  it and no law can see one.
- `ExternalEventTrace` — append-only by API shape (add and read exist; remove, reorder, rewrite do
  not). The membrane is its gate: undeclared kinds are refused at `Append` and never enter the
  record (`UndeclaredExternalKindException`).
- `TransitionRunner.Apply(..., externalContributions)` — crossings merge into the same contribution
  pool as law contributions: same grouping, same per-Kind resolvers, same canonical commit. No
  second write path.
- `TickRunner.Run(initial, relations, laws, trace, count)` — the observed loop: each tick applies
  the crossings recorded for the current boundary; events become visible in the next snapshot,
  never mid-tick. The loop exposes no intermediate state and accepts no callback — the host has no
  seam.

Tests (`Assets/GenesisLab/Tests/MembraneTests.cs`) — eight tests, one per DoD point, including the
fixture law `InteractCountingTransition` (reads the external cell, counts one interaction, consumes
the value — contributions only; the meaning of the crossing lives in the law, per the
event/command guard) and the observational chronicle test, which narrates:

```
external event → append-only trace → tick boundary → law → state Δ → replay verified
```

## Verification

Headless harness (kernel sources verbatim): all eight DoD points pass —

```
DoD1 declaration gate: OK (refused; trace count=0)
DoD3 boundary: before-clean=True at-boundary=True once-only=True
DoD4 law: landed-not-seen-midtick=True consumed=True counted=True
DoD5 replay: tick-for-tick=True counter=3 (expect 3)
DoD6 provenance: identical=True
DoD7 hidden hand: initial-untouched=True rerun-identical=True detached-orphan=True
DoD8 chronicle: … replay verified True
```

**Unity EditMode validation: pending** — the suite (70 existing + 8 new) awaits its run in the
editor. The milestone exits when it is green there.

## The tradition (deposited at this milestone's launch, first instance)

> **What became observable that wasn't before?**

**Rule, added by the founder upon reviewing this milestone: the answer must always speak of the
phenomenon — never of the code.** The first answer, restated in that spirit:

*For the first time, an external influence has a history. It is born, it crosses, it acts, it can
be found again, replayed, compared. It ceases to be an intervention. It becomes a phenomenon.*

(The engineering restatement — "an external event can be observed, replayed, and causally
attributed; a played session is a world object" — is kept as the implementation's echo of the same
fact.)
