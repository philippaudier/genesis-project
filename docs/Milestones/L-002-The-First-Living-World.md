# L-002 — The First Living World

*Not Lootbound. The smallest world able to produce a biography — a creature in the experimental
sense. Constraint, fixed at launch: the first world must be boring. Its mission is not to be fun;
it is to answer: **are we finally able to observe a biography?***

## The world

Five spatial places on a star graph — Shelter, Tree, Repair station, Clearing, Field — walking is
crossing a relation; there is no other space. (Design decision, declared: the first living world is
a **graph, not a 3D scene**. Continuous space is a frozen open that no milestone decides in
passing; and Places-connected-by-Relations is what the kernel has proven since Genesis-008/009.)
Two swords, addressed as places (a place is a unit of addressing, not necessarily spatial —
RFC-0003): the old sword begins in the shelter's chest — its first retrieval *is* the acquisition;
the better one lies in the clearing. One pack, one resource (wood), one repair gesture.

Everything enters through the membrane: three external Kinds (`Go`, `Act`, `Attack`), five
interpreting laws (Move, Harvest, Repair, Swap, Stow) — the RFC-L001 price, paid in full. Every
law consumes the intent it reads; none can know who produced it. The trace records `Act`; only the
laws decide whether it meant acquire, repair, swap, stow, or nothing.

**Timestamps are ticks.** The example DoD showed wall-clock times; the world may not read a clock
(invariant 5), and real-time mapping is RFC-L001-deferred. The biography says `t=23`. A
presentation journal may someday decorate with hours; that will be its knowledge, never the
world's.

**The refusal is a derived non-event.** No law records "replacement refused". The player stood in
the clearing beside a better sword and left carrying the old one — the chronicler deduces it from
the record. Behavioural observation, exactly as the Lootbound Lab constitution demands.

## What was built

- `Simulation/Lootbound/LootboundWorld.cs` — world content, zero kernel changes: places, kinds,
  membrane, initial state, relations, laws, resolvers.
- `Simulation/Lootbound/LootboundLaws.cs` — the five interpreting laws, contributions only.
- `Simulation/Lootbound/BiographyChronicler.cs` — the laboratory's reader: replays
  (initial + relations + laws + trace) and derives the irreversible firsts. A pure function of the
  record: two researchers reading the same run must produce the same biography.
- `Presentation/LootboundLabObserver.cs` — the human producer's window: text only, keys →
  external events on the trace, nothing else. The simulation never learns a human is here.
- `Tests/LootboundWorldTests.cs` — walking follows relations only; the first interaction at home
  is the acquisition; striking the tree yields wood and wears the carried sword; and the DoD test:
  **the world writes the first biography**.

## The Definition of Done — met, headless

A scripted session (a bot producer — legitimate since L-001) drove the world purely through the
trace. The world produced:

```
LB-Obs (draft, produced by a world)

Subject: Run-000001
Object:  Sword-1000

Biography:

t=2    Acquisition
t=11   First wear
t=23   First repair
t=29   First superior sword discovered
t=32   Replacement refused (left the clearing carrying the old sword)
t=38   Stored at the shelter
t=41   Voluntary re-equipment (a better sword still lay in the clearing)

Status: First complete biography observed.
```

No developer wrote this document. A world did. (End-state audit: player home, old sword in hand,
better sword still in the clearing, wear repaired once, one wood remaining — every number accounted
for.)

**Unity EditMode validation pending** (existing suite + 8 membrane tests + 4 Lootbound tests);
human play via `LootboundLabObserver` awaits the founder's session — the first *lived* biography.

## Deliberately absent

Naming (needs a decision about strings and expression channels — RFC-L002 territory, and the
reflexivity question is live), affixes, rarity, XP, HUD, enemies, second anything. One player
marker, two swords, one resource. It is boring, as ordered.

## The deposited property: irreversibility

The founder's intuition, recorded at launch: a biography is not merely append-only — it is
**irreversible**. A first repair can only happen once. Append-only is not a storage decision; it is
a property of time. L-002's firsts are exactly that: seven events that could each occur only once,
in an order that can never be re-lived — the first past.

## The tradition

> **What became observable that wasn't before?**

*For the first time, a world has a past. Not a tick counter — a succession of events that could
each happen only one first time, and the world can tell them. Before L-002, Genesis worlds had
trajectories; now a world has a biography, and it wrote the document itself.*

> **What illusion did the laboratory lose?** *(second tradition question, added upon L-003's
> close, asked retroactively)*

*The illusion that a biography is a life. Run-000002 proved it within a day: nine transitions
lived, three told — and memory compresses too.*

**Exited** — Unity EditMode validation green (82 tests, 2026-07-27); the first human run happened
(Run-000002).
