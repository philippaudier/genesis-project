# Campaign S1-002 — Execution Report

Date: 2026-07-29. Sealed design + hand derivations: `4885d04`. Gate opened: `4dcf41b`. Twelve
parcels run exactly as sealed, 30 ticks each. This report contains claims, statuses, and record
facts — no interpretation, no promotion, no evaluation of F-1. E4 was never invoked: the hand
never needed to blink.

## The witness first (founder's watch order)

**Consistency Check: 0 mismatches, in all twelve worlds.** Every transfer the counter reports
balances every cell at every tick against the record, crossings included. The conservation
audit: 0 violations, all twelve worlds. Every timing below is therefore credible.

## Claims

| # | Claim | Sealed hand prediction | Record | Status |
|---|---|---|---|---|
| C0 | V0 stasis reproduces at 2 cells | zero transfers | zero transfers, 30 ticks | **CONFIRMED** |
| C1 | V1 first-transfer ticks | d0→2 · d1→1 · d2→0 · d3→0 · d4→0 | 2 · 1 · 0 · 0 · 0 | **CONFIRMED — exact** |
| C2 | V2 first-transfer tick = k | k1→1 · k2→2 · k3→3 | 1 · 2 · 3 | **CONFIRMED — exact** |
| C3 | V3 cascade | 2 · 5 · 9 · 14 | 2 · 5 · 9 · 14 | **CONFIRMED — exact** |
| C4 | V4 locality pair | V4a→14 · V4b→13 | 14 · 13 | **CONFIRMED — exact** |
| C5 | Counter consistent with record | exact equality | 0 mismatches × 12 worlds | **CONFIRMED** |

Six of six. The hand and the machine agree to the tick, in every world, on every edge.

The campaign's namesake is in the record: the first transfers in Science-001's history occurred
— earliest at tick 0 (V1-d2, V1-d3, V1-d4), exactly where the sealed hand said they would.

## Failed / confirmed expectations (sealed informal list)

1. V1-d0 alternation — **confirmed**: firing ticks 2,4,6,8,… every other tick, the whole run.
2. V1-d4 first negative value at tick 1 — **confirmed**: tick 1, place(0), exactly.
3. Constant per-hop cascade delay — **failed**: delays are 3, 4, 5 (increasing). Of record:
   the sealed hand derivation had *already predicted this failure at seal*; the world sided
   with the slow derivation against the quick intuition. Three layers were compared — quick
   intuition, hand derivation, world — and the world agreed with the hand.
4. V4 pair differs in timing — **confirmed**: 14 vs 13.
5. No freeze while rain falls — **confirmed**: every world's state changed at every tick
   while crossings continued.

## Unexpected Manifestations

```text
Unexpected phenomenon: None
```

Unclaimed facts present in the record, unread further (available to any future reduction):
the firing *rhythms* — V2 stars fire with period k+1; V1-d3 fires at ticks 0 and 1 then
alternates; V3's upstream edges fire in irregular patterns (e.g. 2,4,6,7,9,10,11,…) once
downstream traffic interacts. No claim covered rhythms; none is elevated here.

## World Corrections

None. No builder was touched, no fixture was touched, no derivation was rewritten (E4 stood
untested), no instrument gap surfaced.

## Traces

Per world under `Runs-S1-002/<name>/`: `states.csv`, `flux.csv` (per-tick per-edge counted
flux), `summary.txt` (first-transfer ticks, firing ticks, negativity, consistency, audit).

## Decision

Withheld. The reduction is a separate authorisation.
