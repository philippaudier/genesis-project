# Observation-003

Date: 2026-07-26
Genesis version: commit `714e929`
World: 3 places; two setups: (a) chain A↔B↔C with production rate 2 at A **and** at C, Q=(0,0,0);
(b) symmetric ring A↔B↔C↔A with production rate 2 at A only, Q=(0,0,0)
Reproduction: headless console harness (see Observation-001).

---

## Observation

- Setup (a): A = C at **every** observed tick (60 ticks) — the world's mirror symmetry is exactly
  preserved. Total grows by exactly 4 per tick.
- Setup (b): B = C at every observed tick — mirror symmetry through A preserved. Total grows by
  exactly 2 per tick.
- In both setups, the *differences* between places repeat with **period 3** while all values grow:
  e.g. in (a), A−B cycles 2, 1, 3, 2, 1, 3, … indefinitely.
- No steady state: the worlds grow forever (no sink exists), but their internal *shape* is periodic.

## Reproduction

As stated; laws as in Observation-001.

## Measurements

Growth rates exact: ΔTotal = ΣProduction at every tick (4/tick in (a), 2/tick in (b)).
Shape period: 3 ticks, both setups, stable across the full observed window.

## Hypotheses

A growing world with a periodic internal shape suggests the dynamics decompose into a linear-growth
component plus a bounded periodic residue — "the world breathes while it grows." The shared period 3
across two different topologies is unexplained; whether it relates to the place count (3), the flux
divisor (2), or the rate (2) is untested. Varying each independently would separate these.

## Status

Open
