# LB-001 — The Ordinary Sword

Status: **Designed. Not runnable yet** — no playable build exists. This protocol is sealed by its
commit, before any player and before any build: the design must be finished before anyone is
allowed to look. (The seal proves record antecedence; with human subjects it can prove nothing
more.)

## Question

> **At what moment does a weapon stop being equipment and become a memory?**

Targets RD-L1. Captures the sentence that has accompanied Lootbound from the beginning:
*ordinary moments are the most precious ones.* A rusty old sword is an ordinary moment. If the
laboratory understands why it becomes precious, it has begun to understand Lootbound.

## What is measured (behaviour only — nothing requires asking anything)

- Is it **kept**? (still in inventory/storage N sessions after a strictly better one exists)
- Is it **repaired**? (resources spent on it when replacement is cheaper)
- Is it **named**? (the naming feature used on it, and when)
- Is it **told**? (does it appear in the post-session story, unprompted)
- Is it **taken up again**? (equipped for an expedition while a better one sits in storage)

Each measure is an event with a timestamp. The object's full event history — acquired, used,
damaged, repaired, stored, retrieved, named — is the trajectory; the laboratory reads
trajectories, exactly as it always has.

## What is forbidden

Asking *"why do you love it?"* — or any *why* — before the behavioural record is closed. Watch,
listen, note. Explanations come after, and are filed as hypotheses, never as findings. Post-session
stories are received, not solicited beyond a single neutral prompt ("how was it?"), recorded
verbatim.

## What this experiment demands from the game (the experiment dictates the slice)

The first playable slice of Lootbound is **defined** by what LB-001 needs to be measurable, and by
nothing else:

1. An ordinary sword, acquired early, with nothing mechanically special about it.
2. Wear — the sword degrades with use.
3. Repair — possible, at a cost that can exceed replacement.
4. Replacement pressure — strictly better swords appear, unambiguously better.
5. Naming — any object can be named, with zero mechanical effect.
6. Storage — a place to keep things, with zero mechanical benefit to keeping.
7. **No sentimentality subsidies**: no achievement, bonus, or hint that rewards keeping the old
   sword. If the world bribes the player into attachment, the experiment measures the bribe.

Feature requests beyond this list are, for the first slice, out of scope by protocol.

## Death conditions for the experiment itself

- If every player discards the sword the moment a better one appears, and no keep/repair/name/tell
  event ever fires: the result is **recorded, not rescued** — "no attachment forms under these
  conditions" is a finding, and RD-L1 deepens.
- If the naming or storage features go entirely unused, they are questioned, not force-promoted.

## Outcomes (fixed before any build)

- **A** — Attachment events occur unprompted: RD-L1 gains its first trajectories; hypotheses may
  be filed.
- **B** — No attachment events at all: the ordinary-moments thesis takes its first honest wound;
  recorded as such.
- **C** — Attachment forms but only around measurement artefacts (e.g. players name things because
  a naming feature visibly exists): the observer effect is measured instead — also a finding.

*No outcome validates a design decision. Outcomes produce understanding; design changes require
their own justification later, citing observations.*
