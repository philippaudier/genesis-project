# Observation-006

Date: 2026-07-26
Genesis version: commit `7e7ffd5`
World(s): topology zoo (K4 complete graph; deg-3 stars with competing sources) + random sweep of
1,200 closed deterministic worlds (3–7 places, ~45% edge density, seeded, 400 ticks each) + 800
sourced worlds.
Reproduction: headless harness (see Observation-001); sweep fully reproducible from seeds 1 and 2.

---

## Observation

**Violations of positivity fall into three temporal classes, now confirmed at scale:**

| Class | Behaviour | Count (closed sweep, n=1200) |
|---|---|---|
| **Transient** | negativity occurs, then the world settles into a fully legal frozen state | 21 |
| **Persistent-periodic** | negativity recurs forever (period 2) | 55 |
| **Divergent** | values grow without bound | 413 |
| (never illegal) | frozen legal (673) or periodic legal (38) | 711 |

- K4 (complete graph, every node degree 3) reproduces the star's marginal behaviour exactly: load 12
  → period-2 oscillation through negativity, forever; load 13 → transient violation, self-healed,
  frozen legal at (4,3,3,3) by t=6. The parity split is not a star artifact.
- **Across 1,200 random closed worlds, no period other than 2 was ever observed.**
- Sourced worlds (Z7: deg-3 star with competing sources r2+r1; Z8: K4 with two sources): growth with
  **no negativity at all** in the zoo runs — continuous injection kept every value non-negative where
  the same topologies violate under a concentrated initial load. In the sourced sweep (n=800), only
  36 of 800 ever saw negativity.

## Measurements

Closed sweep totals above. Self-healed exemplars (reproducible): seed=1, worlds #65, #67, #105,
#219, #247, #314.

## Hypotheses

*(kept apart)* The three classes are exactly what candidate legality policies would treat
differently: a policy that halts on violation would freeze transient-class worlds short of the legal
equilibrium they reach on their own; a policy that tolerates would leave persistent-class worlds
illegal forever and divergent-class worlds unbounded. If policies differ anywhere, they differ here —
this is the distinguishing-specimen family Genesis-015 was sent to find. The sourced-world calm
suggests forcing smooths gradients before they can overdraw; untested beyond these runs.

## Status

Open
