# Observation-004

Date: 2026-07-26
Genesis version: commit `714e929`
World: 4 places — centre D symmetrically connected to leaves A, B, C (a degree-3 star); no
production; Q: D=12, leaves 0
Reproduction: headless console harness (see Observation-001).

---

## Observation

```
t=0  A=0  B=0  C=0  D=12    total 12
t=1  A=6  B=6  C=6  D=-6    total 12
t=2  A=0  B=0  C=0  D=12    total 12
   -> PERIODIC: t=2 equals t=0; period = 2
```

Two facts, stated plainly:

1. **A negative quantity appeared.** At t=1, D holds −6.
2. **The world never settles.** It oscillates between the two states above with period 2 —
   *forever*, by determinism. This is the first observed closed world that does not freeze.

The total remained exactly 12 at every tick, including while D was negative.

## Reproduction

Star = six directed relations {D→A, A→D, D→B, B→D, D→C, C→D}. Laws as in Observation-001. The
mechanism, step by step: at t=0, three neighbours at 0 each draw (12−0)/2 = 6 from D — total outflow
18 > 12, leaving D at −6; at t=1, each leaf (6) sees D at −6 and pushes (6−(−6))/2 = 6 back — D
receives 18, returning exactly to 12.

## Measurements

Oscillation period: 2. Amplitude at D: 18 (from +12 to −6). Conservation: exact throughout —
**conservation held while positivity fell.**

## Hypotheses

*(kept strictly apart)*

- The Genesis-011 record stated the honest limitation precisely: the flux rule's non-negativity
  "relies on ≤ 2 neighbours (witness-scale; a general guard is constraint-layer territory)." A
  degree-3 node is the first world to step outside that scale, and the limitation became a fact.
- Positivity was never a theorem — nothing in the kernel or the laws ever guaranteed it. This world
  demonstrates the difference experimentally: conservation (guaranteed by paired contributions +
  additive resolution) survived; positivity (guaranteed by nothing) fell at the first opportunity.
- The perpetual oscillation is *fed* by the negativity: the −6 creates the steep gradient that pulls
  everything back. Whether bounded-positive dynamics can oscillate at all is a separate, open
  question.
- **If any observation so far makes a new law inevitable in the sense of the Discovery-era motto, it
  is this one:** a predicate on legal states ("quantity ≥ 0") that transitions cannot express — the
  triad's third element, the constraint layer, asked for by a world rather than by a designer. This
  remains a hypothesis about *what should follow*, not part of the observation.

## Status

Open
