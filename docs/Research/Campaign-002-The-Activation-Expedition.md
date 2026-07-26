# Campaign-002 — The Activation Expedition

Status: **Pre-registered and SEALED. NOT EXECUTED.** The seal is the commit containing this text.
Per Genesis-017 (L1), the seal proves **record antecedence only** — any third party can verify these
predictions predate the data; that no unrecorded run preceded them remains testimony. Execution is a
separate, separately-authorised act.

Centre of gravity: **RD-8** — *when does the activation pattern stabilise?*

This campaign does not crown a winner. Its product is a **map of validity domains**: where each
claim of the working candidate holds, and where it stops.

---

## Conventions (pre-registered, per Genesis-017 L8)

- **Active edge** at tick t: a directed relation whose transition contributes **nonzero** flux that
  tick. (A gradient of 1 satisfies `self > target` but fires ⌊1/2⌋ = 0: *not active*.)
- **Activation pattern** A(t): the set of active directed edges at tick t.
- **Stabilised**: A(t) eventually periodic (a fixed pattern is period 1).
- **Episode**: a maximal interval during which A(t) follows one periodic orbit.
- **Freeze tick**: first t with state(t) = state(t−1).
- **Growth ratio**: value(t)/value(t−1) at the highest-|value| place.
- **Domain**: all claims apply to closed worlds with |values| < 2⁴⁰ throughout. The overflow régime
  (RD-2, Obs-008) is declared **out of domain** — not explained, excluded.
- Horizon 2 000 ticks unless stated. The harness logs A(t)'s signature every tick.

## The working candidate

**CT-003 (working; unnamed; the merger Campaign-001's evidence pointed at).**

> Activation governs the transitions between episodes; the linearised spectrum governs each episode
> exactly, once the activation orbit is set.

Its claims, separated so they can die separately (Genesis-017 L3):

- **A1 — Stabilisation.** In every closed world, A(t) becomes eventually **periodic**. *(Corrected
  at design time: "converges to a fixed pattern" is already refuted by published data — the even
  star's activation alternates forever between centre-fires and leaves-fire. Orbits, not fixed
  points.)*
- **A2 — Episode exactness.** While A(t) follows a stable orbit, growth matches the linearised
  operator's λ within 1 %.
- **A3 — Transition.** Qualitative breaks in behaviour coincide with changes of activation orbit —
  and the early-tick deviations of C-001's DP-3 were *activation-settling*, not truncation noise.
- **A4 — Truncation floor.** A closed world is frozen **iff** every adjacent gradient ≤ 1. The ⇐
  direction is claimed as a theorem (gradient ≤ 1 ⇒ flux ⌊g/2⌋ = 0 ⇒ no contributions — from the
  law's text, no simulation). The risky direction is ⇒: every in-domain freeze lands in such a
  state, and any state holding a gradient ≥ 2 cannot stay frozen.

## Shared Blind Spot Audit (mandatory, per Genesis-017 L5)

> **What does CT-003 assume without explaining?**

**Orbit selection.** CT-003 says episodes follow orbits; it does not say *why this orbit* — why
(3,3,3,3) and not another frozen state; why period 2 and never any other (RD-3, resurfacing: the
parents' shared blind spot, inherited intact by their merger). W2 is instrumented against it: the
full A(t) log will show whether any orbit of period ≠ 2 ever appears, even transiently.

**Declared limitation:** this campaign has **no adversary theory** — it is a domain-mapping, and
its adversarial structure lives inside the per-claim death conditions and the worlds built to kill
them. But no second mind imagined a rival. If every claim confirms smoothly, the absent adversary
is the first suspect (L5: a same-mind campaign tests claims, not imagination).

## The worlds

- **W1 — The quiet chain** *(tests A1, A4).* Chain of 5 places, load 16 at one end. Path Laplacian
  μ_max = 2−2cos(4π/5) ≈ 3.618 < 4 ⇒ the operator predicts damping.
- **W2 — Activation warfare** *(tests A1, A3; attacks the blind spot).* A deg-4 star (centre a,
  leaves l1..l4) bridged to a deg-3 star (centre b: neighbours m1, m2, l4) via the edge l4–b.
  Load 12 at a. A divergent core wired to a marginal appendage: growing amplitudes keep renegotiating
  the bridge. Full A(t) log; every orbit change tick recorded against every regime-break tick.
- **W3 — The gradient frontier** *(tests A4).* (a) chain-3 loaded (2,1,0) — all gradients ≤ 1;
  (b) chain-3 loaded (3,1,0) — one gradient ≥ 2. Property-level predictions only; no trajectories
  filed.
- **W4 — Scale control** *(tests A3 vs A4; replaces "artificially frozen activation").* C-001's
  double star, load 12 × 4096 = 49 152 at centre A. Gradients scale with load ⇒ the sign structure
  — hence the activation trajectory — is scale-invariant, while truncation becomes relatively
  negligible. If DP-3's early excursions (up to 39 % for t ≤ 9 at load 12) were truncation, they
  collapse; if they were activation-settling, they persist at the same relative size and ticks.

## SEALED DECLARATIONS

*Filed blind. Derivations from the law's text and from already-published data only; no world of
this campaign has been run or hand-simulated. Trajectory-level claims are deliberately absent where
theory cannot derive them.*

| Claim | Prediction | Confidence | Death condition |
|---|---|---|---|
| A1 | Every campaign world's A(t) is periodic by t ≤ 50 (W1: the empty pattern — freeze; W2: periodic even while diverging) | **80 %** | Any world aperiodic in A(t) through the horizon |
| A2 | W2's asymptotic \|ratio\| ≥ 1.5 (its graph contains K₁,₄; edge/vertex addition cannot lower μ_max below 5); during any stable orbit, measured growth within 1 % of the operator's λ | **85 %** | A stable orbit whose growth misses its λ by > 1 % |
| A3 | W4's early relative excursions **persist** under ×4096 load (same ticks, same relative sizes within a few %); in W2, every regime break coincides with an orbit change | **70 %** | Excursions collapse under scaling, or a W2 break with unchanged orbit |
| A4 | W3a never leaves its initial state (theorem instance); W3b does leave its initial state; every in-domain freeze in the campaign lands in an all-gradients-≤1 state; W1 freezes in one | **90 %** | Any in-domain frozen state holding a gradient ≥ 2, or a gradient-≥2 state that persists unchanged |

**Whole-candidate death:** only A1's death kills CT-003 outright (no stabilisation ⇒ no episodes ⇒
nothing for the spectrum to govern). A2's death would also retroactively demote C-001's
five-decimal result to luck. A3/A4 deaths carve the map without killing the candidate.

**The files are immutable from the sealing commit onward. Execution requires: a separate
authorisation, and nothing else — every slot in this protocol is already filled.**

## Verdict structure (bound now, per Genesis-017 L3)

Per-claim verdicts, independent: **CONFIRMED** (all its worlds match, death condition unmet) ·
**REFUTED** (death condition met) · **UNRESOLVED** (horizon reached without discrimination). No
aggregate winner row exists by design. The campaign's product is the four verdicts plus the A(t)
logs — and whatever the Blind Spot Audit's instrument catches. Naming is not triggered by any
outcome (DN-005 stands). RD-8 is *paid in part* if A1+A2 confirm with the orbit-settling times
measured; *deepened* otherwise.
