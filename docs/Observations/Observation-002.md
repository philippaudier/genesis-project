# Observation-002

Date: 2026-07-26
Genesis version: commit `714e929`
World: 3 places, **directed** cycle A→B→C→A (single directions only); two setups: (a) no production,
Q = (12,0,0); (b) production rate 2 at A, Q = (0,0,0)
Reproduction: headless console harness (see Observation-001).

---

## Observation

- Setup (a): trajectory **identical to the plain chain** — (12,0,0) → (6,6,0) → (6,3,3) → (5,4,3),
  frozen at t=3. The closing relation C→A carried **zero flux at every tick**.
- Setup (b), 60 ticks observed: A remained the maximum at every tick; the closing relation C→A
  carried **zero flux at every tick**. Values at t=60: (43, 39, 38), total 120 (= 2 per tick, exact).
- **No rotation of quantity around the cycle was observed in either setup.**

## Reproduction

Directed cycle = three relations {A→B, B→C, C→A}. Same laws as Observation-001.

## Measurements

Flux on C→A: 0, at every observed tick, in both setups. Directed-cycle world (a) and chain world:
state-identical at every tick.

## Hypotheses

The push-downhill rule only sends flux from larger to smaller; the closing edge points from the
cycle's low end to its high end, so it can never fire while the source (or the initial load) keeps
its origin the maximum. A directed edge whose source is never larger than its target is behaviourally
absent — the topology contains **edges the dynamics cannot use**. Rotation, if it is possible at all
under this law, would require the maximum to move — perhaps via sinks, or asymmetric rates. Untested.

## Status

Open
