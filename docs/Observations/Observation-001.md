# Observation-001

Date: 2026-07-26
Genesis version: commit `714e929` (kernel as of Genesis-013)
World: 3 places (A,B,C), symmetric ring A↔B↔C↔A, no production, Q = (12, 0, 0)
Reproduction: headless console harness (kernel sources verbatim, dotnet 8) — first headless run of
Genesis; the control world (chain) reproduced the EditMode trajectories exactly, tick for tick.

---

## Observation

```
t=0  (12, 0, 0)   total 12
t=1  ( 0, 6, 6)   total 12
t=2  ( 6, 3, 3)   total 12
t=3  ( 4, 4, 4)   total 12
t=4  frozen — identical to t=3, forever
```

- At t=1 the loaded place emptied **completely** (12 → 0) in a single tick.
- At t=2 it rebounded (0 → 6).
- The value at A was non-monotonic: 12 → 0 → 6 → 4.
- Final state: **(4, 4, 4)** — exact equality.
- The control world (same start, chain instead of ring) freezes at **(5, 4, 3)** — inequality.
- Total constant (12) at every tick.

## Reproduction

Ring = six directed relations {A→B, B→A, B→C, C→B, C→A, A→C}; laws = production (all rates 0) +
redistribution (flux `(self−target)/2`, push-downhill) at every place; resolver: addition on Q.
Deterministic — any run of this configuration is this run.

## Measurements

Non-monotonic excursion at A: amplitude 12 → 0 (full evacuation), overshoot count 1, settled by t=3.
Chain vs ring final spread: max−min = 2 (chain) vs 0 (ring).

## Hypotheses

*(kept apart from the facts)* The full evacuation at t=1 is the boundary case of the degree-2
outflow bound: two neighbours at 0 each draw `12/2 = 6`, total outflow exactly `self`. The perfect
final equality may be because the extra edge removes the quantisation plateau seen in chains
(pairwise differences can all reach ≤1 simultaneously). Whether more connectivity *generally* yields
tighter equalisation is untested.

## Status

Open
