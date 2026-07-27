# L-009 — Persistent History

> Status: **APPROVED — RFC-L003 accepted; implemented; headless-verified; Unity validation
> pending.** The prediction (habit, not relief) was sealed in the RFC's accepting commit, before
> any implementation.

*The most important milestone since Genesis began — not because it adds a large feature, but
because it changes the nature of the world. And therefore the most radical in its minimalism.*

## The guiding principle

The words **save**, **load**, **serialize** no longer exist. They belong to the engine, not the
laboratory. The laboratory builds exactly one thing: **a world that continues.** The rest is
engineering.

## The one new property

> **The world no longer starts over.**

Everything else follows.

## The practice article (deposited by the founder; practice, not Constitution)

> **A session is never allowed to know whether it is the first.**

A session is a window; a window has no awareness of being the first or the hundredth — it simply
looks at a world. Honoured literally in the implementation: opening a world is ONE code path with
no branch on session ordinality — a new world is the case of nothing to replay, not a different
operation.

## What was built (and what was not)

**Zero kernel changes. Zero law changes. Zero new kinds.** The window stops discarding the world:

- `Lootbound-Lab/Worlds/World-NNN.log` — the world's file IS its trace, append-only like the
  trace itself: `e <tick> <kind> <place> <amount>` lines, `s <tick>` session marks, `a <tick>`
  for the ceremony. The file never rewrites; it only grows.
- **Opening = replaying.** `OpenWorld()` finds the current lineage, feeds every recorded crossing
  back through the membrane, and replays `initial + trace` to the world's age. The world catches
  up with itself; the session then continues appending — every new crossing is persisted the tick
  it happens (crash-safe by append).
- **The ceremony.** `R` no longer silently discards: it writes `a <tick>` into the record and
  opens the next lineage. The abandoned world remains on the shelf, closed, replayable forever.
- **One honest guard, tested:** a fresh producer that discovers its body already absent — a
  continued world whose visit happened in an earlier session — retires silently instead of
  emitting dead intents forever. The being does *not* return in L-009: producer policy across
  sessions stays deferred, per the RFC. Its one visit happened; the world remembers it.

Tests (`PersistenceTests.cs`): **a continued world equals an uninterrupted one** — the RFC's kill
criterion and the invariant-6 audit as one assertion; opening a new world = replaying an empty
history (the practice article, mechanical); the producer retirement guard. All verified headless.

## Losses, expected

The RFC predicts its own losses and this milestone inherits them: the first encounter dies (every
world now has at most one first meeting, ever); interference becomes ordinary; attachment may
migrate to places. Losses are observations. The founder's unsealed intuition is on record in the
carnet: the new magic will be **waiting** — opening a world and wondering *"where is he?"* not
because he appears, but because he already exists.

## The tradition

> **What became observable that wasn't before?**

*Yesterday. For the first time the world understands the word: what was done in an earlier
session is simply true — the emptied cache stays emptied, the stranded sword stays stranded, and
"comme avant" can now be checked against the record instead of against a memory.*

> **What illusion did the laboratory lose?**

*The illusion that a session is a world. Sessions were never worlds — they were windows, and for
seven milestones the window was quietly discarding what it watched. The world never once asked to
start over.*

> **What surprised the founder?**

*(pending — after the first sessions of a continuing world)*

## Epoch

With this milestone, **Genesis I closes** — the project that learned to build a world — and
**Genesis II opens**: the project that learns to inhabit one. Before L-009, every session was a
birth. After it, every session is a return. (`docs/Journal/2026-07-28-genesis-i-closes.md`)
