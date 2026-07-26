# Observation-008

Date: 2026-07-26
Genesis version: commit `7e7ffd5`
World: the divergent deg-5 star (D=12), run **without** a divergence guard, 90 ticks — deliberately
across the Int64 frontier.
Reproduction: headless harness (see Observation-001).

---

## Observation

Observation-005's hypotheses predicted that unbounded growth would eventually overflow Int64, "at
which point the conservation *measurement* itself would silently break."

**That prediction is refuted. Two things happened instead:**

1. **The accounting never broke.** Through wraparound and beyond, the displayed total remained
   exactly 12 at every tick — including ticks where the centre held ±4.6 × 10¹⁸.
2. **The divergence was arrested.** Around t≈63 the world reached the wrapped value
   4,611,686,018,427,387,906 (≈ 2⁶²) at the centre and **froze** — a fixed point at the overflow
   frontier, unchanged from t=63 through t=90.

```
t=60  centre = -6917529027641081854   total = 12
t=63  centre =  4611686018427387906   total = 12
t=90  centre =  4611686018427387906   total = 12   (frozen)
```

## Measurements

First wrap: between t=60 and t=63. Post-wrap state: strictly frozen (state signature identical each
tick). Total: 12, exact, at all 90 ticks.

## Hypotheses

*(kept apart)*

- Conservation held because **the theorem is stronger than stated**: paired ±q contributions under
  additive folding conserve in *modular* arithmetic too — Int64 addition is a group operation, so
  the sum is invariant mod 2⁶⁴, and the wrapped values happened to display the true total. The
  first theorem apparently lives in ℤ/2⁶⁴ℤ, not just ℤ.
- What died was not the arithmetic but **the meaning**: a world whose "true" content is 12 units
  carries a cell holding 4.6 × 10¹⁸. Every theorem intact; every value senseless. The collapse at
  the frontier is *semantic*, not accounting — which sharpens what any future legality policy must
  protect: not the sums (they protect themselves), but the interpretability of state.
- The freeze is unexplained: at the wrapped magnitudes, the flux rule's comparisons/truncations
  apparently reach a configuration where no flux fires. Whether every divergent world freezes at the
  frontier, or this one was lucky, is untested.

## Status

Open — **refutes the overflow hypothesis of Observation-005** (negative evidence recorded as
evidence, per the method). RFC-0004 is untouched by this refutation: what fell is a hypothesis about
consequences, not the irreducibility demonstration.
