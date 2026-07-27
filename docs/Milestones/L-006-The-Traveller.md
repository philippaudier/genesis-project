# L-006 — The Traveller

*The first milestone of Era II — **the worlds live**. Article 6 is satisfied: the reader is ahead;
the world may grow. The Traveller is not a feature. It is Era II's first phenomenon.*

Status: **IMPLEMENTED, headless-verified; Unity validation pending.** (The prediction below was
sealed at commit `7f7eeed`, strictly before any Traveller code existed — the ancestry is the
proof.)

## Implementation record

- The world carries **two bodies**: `BodyB` marker + `GoB`/`ActB`/`LeaveB` external kinds; sword
  `Location` conventions: 0 = held by body A, 1 = held by body B (neither is a place id). The
  world knows bodies; it never knows drivers.
- `MoveLaw` generalised to (place, body kind, intent kind) — **one walking law for everyone**;
  `PickDropLaw` (body B's single toggle gesture, canonical order); `DepartLaw` (leave from the
  field only; departure puts down what is held — objects conserved, only drivers leave).
- `TravellerProducer` — NOT world content, NOT a law: a producer (ADR-0005) standing outside the
  membrane; deterministic, meagre itinerary (tree → clearing: pick up → station: put down →
  field: leave); advances only on observed completion, trusts the world's record over its own
  emissions.
- Reader: hand conventions declared ("in hand" = body A, "held (body B)"); ground names per place.
- Observer: the Traveller inhabits live sessions (toggleable); the display says only *"Someone
  else is at: …"* and "carried by someone" — no name, no explanation.

**Headless co-run** (player script = Run-000001 verbatim, written in L-002; traveller = default
producer, written blind to it — two biographies genuinely not written for each other):
Sword-2000's biography: `t=8 clearing ground -> held (body B) · t=17 held (body B) -> station
ground`. When the player entered the clearing (t≈29), it was empty — and the player-sword's
biography shows the absence as *firsts that could not be born* (no "presence at the clearing while
the other sword lay there"). The traveller left; no body remained; **replay without the producer
is exact** — the trace suffices, one record, several biographies. The scientific question is
answered mechanically: **yes — two biographies interacted without having been written for each
other.** (The sealed prediction is NOT scored on this — per its own rule, it awaits the first
human run with the Traveller active.)

## The tradition

> **What became observable that wasn't before?**

*Consequence without presence. A biography can now contain the marks of another that it never
met — a sword gone from a clearing, moved by hands nobody saw. And an absence is observable as
the firsts that could not be born.*

> **What illusion did the laboratory lose?**

*The illusion that a world's story is one story. The trace is single; the biographies it carries
are already plural.*

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
