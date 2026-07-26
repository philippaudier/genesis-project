# Observation-009

Date: 2026-07-26
Genesis version: commit `ab8f2e0` (the seal — predictions and confidences were committed **before**
this run; the git ancestry is the proof of blindness)
World: the three pre-registered worlds of Campaign-001 (W1 leaf-loaded deg-3 star; W2 deg-3 star
load-scaling series; W3 shared-leaf double deg-4 star). Reproduction: headless harness
(see Observation-001), configurations exactly as pre-registered.

---

## Observation

The first adversarial theory trial in the corpus. All three discriminating predictions produced
clean, unambiguous answers.

**DP-1 — load placement changes the regime.** The deg-3 star with 12 at a *leaf* damps to perfect
equality and freezes — `(12,0,0,0) → (3,3,3,3)` frozen at t=6, **no negativity at any tick**. The
same graph loaded at the centre oscillates at period 2 forever, violating Q ≥ 0 on alternate ticks.
Same graph, same total, opposite regime — *and opposite legality*.

```
W1 (leaf):   12 0 0 0 → 6 0 0 6 → 6 3 3 0 → 3 2 2 5 → 4 3 3 2 → 3 3 3 3  (frozen, legal throughout)
W1-control:   0 0 0 12 → 6 6 6 -6 → 0 0 0 12 → …                          (period 2, illegal forever)
```

**DP-2 — the odd-load freeze time is exactly linear: t_freeze = (load+1)/2.**

| load | outcome | freeze/period tick | centre extreme |
|---|---|---|---|
| 12 | period 2 | — | −6 |
| 13 | frozen | 7 | −5 |
| 15 | frozen | 8 | −6 |
| 120 | period 2 | — | −60 |
| 121 | frozen | 61 | −59 |
| 1200 | period 2 | — | −600 |
| 1201 | frozen | 601 | −599 |
| 12001 | frozen | 6001 | −5999 |

Two exact laws, one per parity: even loads oscillate forever with centre extreme **exactly −load/2**
(pure linear scaling); odd loads freeze at **exactly t = (load+1)/2** (one unit bled per rebound,
quantified). The "parity perturbation" is not a perturbation: its lifetime grows without bound with
load. (Convention note: freeze tick here = first tick whose state equals the previous one;
Obs-005's S2 reported the last-change tick, hence 13 → "6" there and "7" here.)

**DP-3 — the sealed ratio was exact to five decimal places.** The double star diverged with
alternating sign, and the measured per-tick ratio converged to **−1.65137** at t=60 against the
blind-filed **−(3+√13)/4 ≈ −1.65139** — filed from the operator M = I − L/2 before any run. Error:
0.001 %. Convergence profile: within 5 % of the filed value from t=10, within 1 % from t≈19;
excursions up to 39 % during t ≤ 9 while values were small. Conservation: total = 12 exactly at
every tick, through amplitudes of 4 × 10¹³.

## Verdict (under the sealed criteria — the table binds)

- DP-1: **CT-002 right, CT-001 wrong** (regime is not a property of the graph alone).
- DP-2: **CT-002 right, CT-001 wrong** (damping time scales with load; the advocate's 30 %
  confidence recorded that it saw this coming). CT-001's even-load sub-claim (exact −load/2) held.
- DP-3: **CT-001 right, CT-002 wrong as stated** (the sealed deviation window claimed > 5 % through
  t ≤ 15; the ratio entered the 5 % band at t=10 and never left).

Sealed table row matched: *CT-002 right on DP-1 and DP-2; CT-001 wrong on ≥ 2* →
**Outcome B — CT-002 preferable.**

The verdict stands because it was bound in advance. But the same run contains the strongest
confirmation CT-001 has ever received: a five-decimal blind numeric prediction on an unseen
topology. The evidence pattern is precisely the merger reading sealed under Outcome C: **the linear
spectrum rules exactly wherever the activation pattern has stabilised; the activation trajectory and
truncation decide which episode a world enters and how long it lasts.** The letter of the criteria
picked B; the shape of the evidence points at one theory with two orders. That tension is itself a
finding — about how to write outcome tables, not about how to bend them.

## Calibration audit (first in the corpus)

| | DP-1 | DP-2 | DP-3 | Brier |
|---|---|---|---|---|
| CT-001's advocate | 55 % ✗ | 30 % ✗ | 75 % ✓ | **0.152** |
| CT-002's advocate | 60 % ✓ | 85 % ✓ | 55 % ✗ | **0.162** |

The theory that lost the trial was defended with the better calibration. CT-001's advocate earned
its score by conceding DP-2 against its own theory when the published record (S2/S5) demanded it;
CT-002's advocate overreached exactly once, on the window it was most confident about culturally and
least entitled to numerically.

## Hypotheses

*(kept apart)*

- The merger: CT-002 is the first-order activation/truncation layer; CT-001 is the exact theory of
  stabilised episodes. One candidate, two orders. If a successor candidate is written, it should be
  *this*, and it must predict **when activation stabilises** — the question both parents dodge.
- t_freeze = (load+1)/2 suggests the odd-case dynamics are exactly "one unit lost to truncation per
  two-tick rebound"; a proof from the law's integer division should be within reach.
- DP-1's legality flip (same world, legal from a leaf, illegal from the centre) means any future
  answer to Q1 cannot classify *worlds* — only *trajectories*. Strengthens RD-7.

## Status

Open — pays the trial phase of RD-1 (see DEBTS). Campaign-001 closed. Naming: **not triggered**;
DN-005's question remains open, but this observation is the kind of evidence a future naming would
cite — for the merged candidate, not for either parent.
