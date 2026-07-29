# Campaign S1-004 — Execution Report

Sealed design: `b114a34`. Conformance: `76ccaf1` (CONFORMING). M0 and M1 run six ticks exactly as sealed. Claims, statuses and record facts only — no interpretation. The reduction is not authorised and is not here.

## Bound first events

| Event | Derived | Record |
|---|---|---|
| first Water flux | boundary 1, A→B, 5 | boundary 1, 0->1:5 |
| first conversion | boundary 1 at A | boundary 1, place(0):1 |
| first Sediment transport (M1) | boundary 2, A→B, 1 | boundary 2, 0->1:1 |
| first cross-fixture conflict (M1) | boundary 2, (A,Sediment), (+1,−1) → 0 | boundary 2: place(0) kind(4) [1,-1] place(1) kind(2) [5,-2]; resolver invocations 4, committed sum 6 |
| first changed SolidSurface (M1) | after tick 3, [9,11,10] | after tick 3, [9,11,10] |
| M0 SolidSurface | unchanged throughout | unchanged throughout |
| first tick the two worlds' surfaces differ | after tick 3 | after tick 3 |

## Claims

| # | Claim | M0 | M1 |
|---|---|---|---|
| C0 | dry stasis: no contribution before the crossings | held | held |
| C1 | Rock + Sediment invariant | held | held |
| C2 | each conversion pair is locally zero-sum | held | held |
| C3 | M0's SolidSurface never changes | held | — |
| C4 | Base never changes | held | held |
| C5 | the contested cell: two witnesses agree, committed 0 | — | **FAILED** |

## Surfaces, tick by tick

| after tick | M0 | M1 |
|---|---|---|
| 0 | [10,10,10] | [10,10,10] |
| 1 | [10,10,10] | [10,10,10] |
| 2 | [10,10,10] | [10,10,10] |
| 3 | [10,10,10] | [9,11,10] |
| 4 | [10,10,10] | [9,10,11] |
| 5 | [10,10,10] | [8,11,11] |
| 6 | [10,10,10] | [9,10,11] |

## Matter and positivity

- Rock + Sediment, M0: 30 → 30; audit faults: 0
- Rock + Sediment, M1: 30 → 30; audit faults: 0
- minima M0 — Base 0, Rock 8, Sediment 0, Water 0
- minima M1 — Base 0, Rock 8, Sediment 0, Water 0

## Classification

**Outcome E** — accounting or instrument failure.

## Traces

`Runs/M0/` and `Runs/M1/`: `states.csv` (every kind, every place, every tick, plus the SolidSurface and local-matter readings), `readings.txt` (per boundary: water flux, sediment flux, conversions, every fixture's contributions, and any contested cell), `resolver.txt` (the resolver's own record of what it was handed and what it committed).

## Decision

Withheld. The reduction is a separate authorisation.
