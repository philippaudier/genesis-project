# Observation-012

Date: 2026-07-28
Genesis version (commit/tag): sealed design `7a4814e`, gate `f54958c` (Lab/S1-001)
World (topology, initial cells, rates): W3 of Campaign S1-001 — 15×9 4-neighbour grid, kind(1)
values forming two depressions (floors 0 at places 63 and 71) split by a column of value 5
(places 7, 22, 37, …, 127); degree-aware divisor fixture; uniform crossings +1 into kind(2) at
every place, boundaries 0–19, then closed.
Reproduction seed / setup: `Lab/S1-001$ dotnet run -- --execute`, artifacts `Runs/W3/`.

---

## Observation

W3 froze at tick 20 (first unchanged state pair) and remained identical for the remaining 280
recorded ticks **while holding a driving potential gradient of 2** across the relations from
the value-5 column to its neighbours. The sealed claim A4 read: *freeze ⇔ all driving gradients
≤ 1*. The record shows a world frozen, permanently, at gradient 2. The ⇐ direction of A4, as
written, is refuted by this world. (W2, under the same fixture, froze holding a maximum
gradient of 1 — consistent with A4 as written.)

## Reproduction

Clone at `f54958c` or later. `cd Lab/S1-001 && dotnet run -- --execute`. Read
`Runs/W3/summary.txt`: "first unchanged tick pair after rain: 20", "final max driving potential
gradient across a relation: 2".

## Measurements

- Freeze tick: 20 (rain end). States 20 through 300 identical in every cell.
- Max driving gradient in the frozen state: 2 (value-5 column against adjacent columns).
- Transfers across those relations, every tick: 0.
- Divisor at those places (declared fixture parameter): degree + 1, degrees 3–4.

## Hypotheses

Withheld — the reduction phase is not authorised. Timing note of record (from the campaign
report's World Corrections): the executor derived, from the fixture's text and before the run,
that the corrected family's transfer is zero whenever the difference does not exceed the local
degree; per rule E1 nothing was modified, and the run produced this refuting fact unprompted.

## Status

Open
