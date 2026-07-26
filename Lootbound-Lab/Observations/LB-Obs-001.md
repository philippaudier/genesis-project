# LB-Obs-001

> **The player left the clearing carrying Sword-1000 while Sword-1001 remained available.**

Date: 2026-07-27
Build: commit `ffcc37e` (world L-002; laws, membrane and trace as recorded there)

## Context

```
Run:      000001
Object:   Sword-1000
World:    L-002
Producer: Bot (scripted)
```

## Observed facts

```
t=29   Player enters the clearing. Sword-1001 lies there (Location = Clearing).
t=32   Player leaves the clearing.
       Carried object:   Sword-1000  (wear 0, repairs 1)
       Remaining object: Sword-1001  (Location = Clearing, unchanged)
```

Nothing else about this interval is in the record. No Act event was produced at the clearing at
any tick.

## What the laboratory does not know

```
Unknown (from the record — no run's record can tell these):

- Did the producer register Sword-1001's presence?
- Did the producer understand it to be better?
- Was there an intention?
- Was it forgotten?
- Was it deliberate?
```

**Honesty note, specific to this run:** the producer is a bot whose script is public (the L-002
DoD test). The laboratory therefore knows, *out of band*, that walking past was scripted. The
unknowns above are what the **record alone** cannot tell — and the record is all a future reader,
or a future human run, will have. This section exists precisely for the day the producer is a
person and no script can be consulted. Provenance-blindness works both ways: the world could not
know its producer was a bot; the record cannot certify that a future one is not.

## Derived behavioural observation

```
The available replacement was not taken.
```

Not "refused". Refusal implies an intention, and the laboratory does not yet have the right to
one. (The Chronicler's own output says "Replacement refused" — see Alternative explanations and
RD-L6: the reader's vocabulary is itself under study.)

## Alternative explanations (preserved, none privileged)

```
A1   Sword-1001 was unnoticed.
A2   Sword-1000 already had value (one repair had been invested in it).
A3   Inventory friction (taking would have required laying the carried sword down).
A4   Bot policy (known true for THIS run, out of band; unknowable from the record).
```

All remain alive. A4's out-of-band truth for Run-000001 does not close A1–A3 for any future run —
it demonstrates instead that identical records can be produced by different explanations, which is
the entire reason this section exists.

## Ledger

```
RD-L1 strengthened.
```

Not paid. Not confirmed. The debt becomes more interesting: the first recorded instance of
keep-despite-better now exists, with its ambiguity intact.

## Status

Open — first official observation produced by a world. Fills the *bot form* of watchlist slot 1;
the lived (human) instance is still awaited.
