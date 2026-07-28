# Observation-011

Date: 2026-07-28
Genesis version (commit/tag): sealed design `7a4814e`, gate `f54958c` (Lab/S1-001)
World (topology, initial cells, rates): five parcels of Campaign S1-001 — W0 (9×9 flat), W1
(9×9 bowl, naive divisor), W2 (9×9 bowl, degree-aware divisor), W3 (15×9 twin depressions,
ridge 5), W4 (9×3 incline with kinds 3/4) — 4-neighbour grids, uniform crossings of +1 into
kind(2) at every place, boundaries 0–19, then closed.
Reproduction seed / setup: `Lab/S1-001$ dotnet run -- --execute` (deterministic; no seed).

---

## Observation

In all five parcels, across every recorded tick (120–300 per world), **no unit of any kind ever
crossed a relation.** Every value change in the record is a declared crossing (rain). Each
parcel's kind(2) rose uniformly from 0 to 20 during boundaries 0–19 and never changed again.
The descent pattern (declared convention: strictly-lower-potential neighbours, potential =
kind(1)+kind(2)) was identical at every tick of every run — one pattern per world, locked from
tick 0. No value was ever negative. No value ever exceeded 20. W1 — the parcel both sealed
prediction columns marked divergent — froze like the others.

The only world whose state kept changing after boundary 19 was W4-B3, whose motion is entirely
the sacrificial pair of the negative control (Observation of record in the campaign report:
its kind(5) audit shows exactly +1 per tick, 150 violations, detected).

## Reproduction

Clone the repository at `f54958c` or later. `cd Lab/S1-001 && dotnet run -- --execute`.
Compare `Runs/*/states.csv` — determinism makes the record identical to the digit.

## Measurements

- Transfers across relations, all worlds, all ticks: **0**.
- Max driving potential gradient ever recorded: W0: 0 · W1: 1 · W2: 1 · W3: 2 · W4: 1.
- First unchanged post-rain state pair: tick 20 in W0–W4.
- kind(2) audits: 0 violations in six of six worlds; totals equal declared crossings exactly.
- Conversion events in W4: 0. kind(3)+kind(4): invariant at 2700 throughout.

## Hypotheses

Withheld — the reduction phase of Campaign S1-001 is not authorised. One filing
cross-reference, no causal claim: the campaign's sealed Blind Spot Audit, item 3, declared
distributed sources untested (corpus sources were single-point).

## Status

Open
