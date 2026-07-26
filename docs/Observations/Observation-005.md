# Observation-005

Date: 2026-07-26
Genesis version: commit `dec671c`
World: centre-loaded stars (centre connected symmetrically to n leaves), no production, leaves at 0.
Ten runs varying **one factor at a time**: degree n ∈ {2,3,4,5,6}, load D ∈ {12,13,14,15,24}.
Reproduction: headless console harness (see Observation-001); divergence guard at |value| > 10⁶.

---

## Observation

| Run | Degree | Load | Outcome |
|---|---|---|---|
| S6 | 2 | 12 | **Damped** — frozen at (4,4,4) by t=3 |
| S1 | 3 | 12 | **Periodic** — period 2, amplitude ±6 at centre, forever |
| S3 | 3 | 14 | **Periodic** — period 2, amplitude ±7 |
| S4 | 3 | 24 | **Periodic** — period 2, amplitude ±12 |
| S2 | 3 | 13 | **Damped** — staircase decay, frozen at (3,3,3,4) at t=6 |
| S5 | 3 | 15 | **Damped** — staircase decay, frozen at (4,4,4,3) at t=7 |
| S7 | 4 | 12 | **Divergent** — |values| grow without bound (>10⁶ by t=29) |
| S8 | 4 | 13 | **Divergent** (>10⁶ by t=30) |
| S9 | 5 | 12 | **Divergent** (>10⁶ by t=17; per-tick growth ratio → −2 exactly) |
| S10 | 6 | 12 | **Divergent** (fastest observed) |

Facts, stated plainly:

1. **Three regimes exist**: damped (settles), periodic (oscillates forever), divergent (grows without
   bound). All three were observed.
2. **Degree is the master factor**: n ≤ 2 damps; n = 3 splits; n ≥ 4 diverges — for every load tried.
3. **Parity decides only at n = 3**: even loads oscillate forever; odd loads bleed one unit per
   bounce through integer truncation and settle.
4. **Capacity does not change the regime** (D = 24 behaves as D = 12, with doubled amplitude).
5. **Conservation held in every run at every tick** — including deep inside divergence (total = 12
   exactly while individual values exceeded ±10⁶).
6. Observation-004's perpetual oscillator is therefore **not generic**: it sits precisely at the
   marginal degree (n = 3) with the resonant parity (even).

## Reproduction

Star of n leaves = 2n directed relations (centre↔each leaf); laws as in Observation-001.

## Measurements

Divergence growth ratios (centre value, successive extremes): n=4 → ≈ −1.5 · n=5 → −2 (exact from
t≈5 on) · n=6 → faster. Damping at n=3-odd: one unit lost from the oscillation per rebound.

## Hypotheses

*(analysis, kept apart from the facts)*

- The three regimes match a linear stability analysis of the flux rule: for the centre–leaves mode,
  the per-tick amplification is **λ = 1 − (n+1)/2**. n=2 → λ=−½ (damped) · n=3 → **λ=−1 (marginal)**
  · n=4 → −1.5 · n=5 → −2 — matching the measured ratios exactly. If correct, Observation-004 was a
  world sitting precisely **on the stability boundary**, where integer truncation (parity) casts the
  deciding vote.
- This is the stability limit the closed research arc predicted abstractly ("a deterministic explicit
  update can still oscillate or diverge if the per-step rate is too aggressive",
  Transition-Mechanics-Exploration §E) — now observed, with its eigenvalue.
- The flux divisor (2) and the degree jointly set λ; a larger divisor should shift the boundary to
  higher degrees. Untested.
- Risk noted for divergent worlds: unbounded growth will eventually overflow Int64, at which point
  the conservation *measurement* itself would silently break (no checked arithmetic). Divergence is
  therefore not merely inelegant — it eventually invalidates the world's own accounting. This
  strengthens, without deciding, the case filed under Observation-004.

## Status

Open — supersedes the "parity phenomenon" and "capacity phenomenon" readings of Observation-004;
merges with it into a single question: *the stability structure of the redistribution law*.
