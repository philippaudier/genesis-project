# Invitation-001 — The Deposit (verbatim)

*Provenance: produced by the second researcher — a fresh context of the same model, given only the
sealed Evidence Dossier (commit `7afbef8`), forbidden tools and file access, forbidden to simulate
beyond qualitative checks. Recorded unedited. The first researcher read this document for the first
time after it was complete.*

---

## 1. THE EXPLANATION

**The core object.** Strip away the conditionals and this system is one linear map plus a rounding
perturbation. For an edge {s,n}, exactly one direction fires (the higher endpoint), moving
floor(diff/2). Ignoring the floor, the net effect on place s each tick is:

v'(s) = v(s) + (1/2) Σ_neighbours (v(n) − v(s))

That is v' = (I − L/2)·v, where L is the graph Laplacian. I will call this the **half-Laplacian
map**. Everything in the record is this map, distorted by an O(1)-per-edge-per-tick rounding term
(the floors), on the integers.

**Spectrum decides fate.** L has real eigenvalues 0 = λ₁ ≤ … ≤ λ_max. The map's multipliers are
μ = 1 − λ/2:

- λ = 0 (all-ones mode) → μ = 1: this is why **total is conserved exactly** in closed worlds —
  transfers move units, and the growth lives entirely in modes orthogonal to the total.
- 0 < λ < 4 → |μ| < 1: mode decays geometrically.
- λ = 4 → μ = −1: a **see-saw mode** — sign flips each tick, amplitude constant. Marginal.
- λ > 4 → μ < −1: sign-alternating exponential growth, |ratio| → λ_max/2 − 1.

Check against the record:
- Star K(1,k) has Laplacian spectrum {0, 1×(k−1), k+1}. k=3: μ = −1 → period 2 (observed). k=4:
  μ = −1.5 → |ratio| → 1.5 exactly (observed). k=5: μ = −2 → 2 exactly (observed).
- K4: spectrum {0, 4, 4, 4} → μ = −1 → period 2 (observed).
- Path of 5: λ_max = 2 + 2cos(π/5) ≈ 3.618 < 4 → contraction → freeze (observed, (6)).
- Ring of 3: λ_max = 3 → freeze (observed).
- Double star (5): measured |ratio| 1.65137 ⇒ λ_max = 5.30274. Consistent with the theorem
  λ_max ≥ Δ+1 = 5 for a graph with a degree-4 vertex that is not a pure star component. Same for
  the bridged graph (7): 1.56244 ⇒ λ_max = 5.12488.
- "A tree with maximum degree 3 grew unboundedly": for any connected graph that has a degree-3
  vertex and is not exactly K(1,3), λ_max > 4 strictly. So yes, trees of max degree 3 can grow.

**Why only period 2, ever.** L is symmetric → all multipliers are real. Real spectra cannot
produce rotation; the only non-decaying oscillation available is sign flip, i.e. period 2. The
survey's "every periodic world has period exactly 2" is a theorem of the linear part, not a
coincidence. (The sourced period-3 is different — see below.)

**The floor term: arithmetic friction.** The floors act as a state-dependent perturbation of size
≤ 1/2 per edge. Three regimes:

1. **Exact-parity orbits.** If every firing difference is even, floors do nothing and the linear
   map is exact. Centre-loaded 3-star, even load V: diff stays V forever, orbit
   (V; 0,0,0) ↔ (−V/2; V/2,V/2,V/2), period 2, centre minimum −V/2. Matches load 12 (−6),
   120 (−60), 1200 (−600).
2. **Arithmetic decay of the see-saw.** With odd load V, the centre-leaf difference is odd every
   tick, and the floor sheds exactly one unit of the difference per bounce: diff goes V, V−2,
   V−4, …, until diff ≤ 1. That gives (V−1)/2 firing ticks, freeze detected at **t = (V+1)/2**,
   and the first bounce is the deepest: centre minimum = V − 3·(V−1)/2 = **−(V−3)/2**. Check:
   13 → freeze 7, min −5 ✓; 15 → 8, −6 ✓; 121 → 61, −59 ✓; 1201 → 601, −599 ✓; 12001 → 6001,
   −5999 ✓. Crucial structural point: **marginal (λ=4) modes decay linearly in time (one quantum
   per tick), so relaxation time is O(amplitude); stable (λ<4) graphs decay geometrically, so
   freeze time is O(log amplitude)** — compare chain-of-5 load 16 freezing in 9 ticks vs the
   star's 6001.
3. **Growth beats friction.** When λ_max > 4, floors are O(1) against an exponentially growing
   amplitude, so the asymptotic ratio is exactly the eigenvalue prediction; only the transient
   depends on load. This explains (5): load 12 and load 49152 = 12×4096 would have identical ratio
   sequences if the map were linear; they diverge from tick 3 because at amplitude ~30 a floor
   error of 1 is 3%, while at ~120000 it is negligible — the big-load run is the *less* distorted
   one. Both should converge to the same 1.65137.

**Why negatives happen at all.** A place above d neighbours sends ~d/2 of the difference in total,
keeping (1 − d/2) of it. For d ≥ 3 it overshoots past the neighbours and past zero — sign flip.
For d ≤ 2 it lands at or above the neighbour level. Hence: **negatives require a vertex that is
simultaneously above ≥ 3 neighbours**; paths and rings starting nonnegative can never go negative.
The survey's "healed" cases (21) are stable graphs (λ_max < 4) where the overshoot pump fires
during the transient and then contraction wins; "persisted" (55) are λ=4 graphs; unbounded (413)
are λ>4.

**Frozen states.** floor(diff/2) = 0 iff diff ≤ 1, so a plateau is exactly a state where **every
connected pair differs by at most 1** — matching every frozen state in (9). Plateaus are not
unique and not "fair": (5,4,3), (2,1,1) — the dynamics stops at the diff-1 lattice, not at
equality. The leaf-loaded 3-star freezes rather than see-sawing because a leaf load excites the
λ=4 mode only weakly and with broken parity; the arithmetic friction eats a few-unit see-saw in a
few ticks.

**Locked firing sets and the "two-timescale" look of (7).** Once the λ_max mode dominates, the
state's sign pattern is (dominant eigenvector) × (−1)^t. All comparisons then flip in unison, so
F(t) alternates between one set and its exact reversal — the two sets seen in (7). Places where
the dominant eigenvector is small (m1, m2, far from the load) are ruled by subdominant modes early
and only join the global see-saw after a crossover time ~ log(coefficient ratio)/log|μ₁/μ₂|; that
is the two-timescale appearance. The oscillating convergence of the ratio in (5) is the
subdominant real-negative mode beating against the dominant one, decaying like |μ₂/μ₁|^t. (Note:
1.618 at t=14 is the golden ratio by coincidence only; the limit is 1.65137 = λ_max/2 − 1.)

**Never-firing edges.** If two places are exchangeable by a graph automorphism that also fixes the
load, determinism keeps their values identical forever, so any edge inside such an orbit never
fires. Additionally, in stable worlds, edges whose endpoints never develop a difference ≥ 2 stay
silent. Both mechanisms are mundane; no special rule needed.

**Sourced worlds.** Production pumps the μ=1 direction; the state converges (when λ_max ≤ 4
effectively) to a linearly growing profile with bounded internal differences. On that profile the
*residues* under the floor map live on a finite state space, so the difference-pattern must become
eventually periodic — period 3 is just one available residue cycle, and it does not contradict
"closed periodic = period 2" because it is a floor-residue cycle, not a linear mode. Few negatives
because the positive drift dominates transients.

**The 2^62 freeze (4).** Below overflow, freeze requires all diffs ≤ 1. At amplitude ~2^62 with
sign-alternating values, the computed difference s − n exceeds 2^63 and wraps negative in 64-bit
arithmetic: the guard "amount > 0" then fails on every edge even though s > n. The world is frozen
by **arithmetic glass**, not by equilibrium — the one plateau violating the diff≤1 law. The
displayed total stays 12 because wrap is addition mod 2^64 and transfers are anti-symmetric, so
conservation survives overflow.

---

## 2. PREDICTIONS (none of these worked through)

**P1 — 6-leaf star.** K(1,6), load 12 at centre, closed. Unbounded; centre sign alternates every
tick; |ratio| → (6+1)/2 − 1 = **2.5**, within 0.01 by t = 50 (not digit-exact at every tick,
unlike k=4,5, because differences go odd early, e.g. a 75 appears within the first few ticks).

**P2 — 3-star load laws, both parities.** Centre-loaded 3-star: (a) load **999** → freezes at
exactly **t = 500**, minimum value **−498**, occurring at the centre at t = 1. (b) load **998** →
period 2 forever, minimum **−499**. (b) deliberately breaks the record's confound: every period-2
load they tried was a multiple of 12; I claim evenness alone is sufficient.

**P3 — Complete graph K5.** Load 12 on one vertex, closed. Spectrum {0, 5×4} → μ = −1.5.
Unbounded sign-alternating growth, |ratio| → **1.5**; and the **6 edges among the four unloaded
vertices never fire once** in the entire run (automorphism orbit).

**P4 — Ring of 7.** Load 100 on one place, closed. λ_max ≈ 3.80 < 4. **No value is ever negative
at any tick** (degree-2 rule: no overshoot pump). Freezes by **t ≤ 30**, with final values all in
{14, 15} and **exactly two places at 15** (total 100 = 7×14 + 2), every adjacent diff ≤ 1.

**P5 — Twin-triad threshold test.** 7 places: centres A, B; A–a1, A–a2, A–S, B–b1, B–b2, B–S (two
3-stars sharing one leaf S). λ_max > 4 strictly but barely. (a) Load **12** at A: **bounded** —
freezes or settles to period 2 with |values| ≤ 12; the weak exponential growth (rate ~1.0–1.2 per
tick) loses to floor friction at this amplitude. (b) Load **2,000,000** at A: **unbounded**,
sign-alternating, |ratio| converging to a constant in **(1.00, 1.25)**; from some tick on, F(t)
alternates between exactly one set and its exact reversal.

---

## 3. CONFIDENCES

- P1: 88%
- P2a: 90% P2b: 92%
- P3: 85%
- P4: 82% (the "exactly two 15s" detail: 65%)
- P5a: 55% (genuinely uncertain — this is the friction-threshold claim at risk)
- P5b: 85% (ratio-in-range part 80%)

---

## 4. DEATH CONDITIONS

- **Half-Laplacian spectral law**: dead if any closed world whose graph has λ_max < 4 grows
  without bound; or if any measured asymptotic |ratio| differs from λ_max/2 − 1 by more than
  floor-noise; or if a graph with λ_max > 4 and a load ≥ 10^6 in the dominant mode stays bounded.
- **Real-spectrum → period-2-only (closed worlds)**: dead the moment any closed world exhibits a
  genuine period 3, 4, or higher in full state (not a growing shape).
- **Parity friction law**: dead if any odd-load centre-loaded 3-star oscillates forever, any even
  load freezes, or any odd load V freezes at a tick other than (V+1)/2.
- **Plateau law (diff ≤ 1)**: dead if a frozen state below the overflow regime contains a
  connected pair differing by ≥ 2.
- **Overshoot pump (degree ≥ 3 needed for negatives)**: dead if any path or ring with nonnegative
  initial values ever shows a negative.
- **Arithmetic-glass overflow freeze**: dead if inspection of the 2^62 frozen state shows all
  wrapped comparisons/differences actually satisfy diff ≤ 1, i.e. it froze "legitimately".
- **Automorphism silence**: dead if an edge inside a load-preserving automorphism orbit fires.
- **Firing-set lock**: dead if an unbounded closed world, deep in its asymptotic regime, shows
  three or more distinct firing sets recurring indefinitely.

---

## 5. BLIND SPOTS I DETECT

1. **Single-point loads only.** Every experiment ignites one place. The system is near-linear;
   superposition (two loads, load at A plus load at B vs each separately) was never probed, nor
   negative or mixed-sign initial states — despite negatives being a headline phenomenon.
2. **Parity confounded with divisibility.** All period-2 star loads were multiples of 12; all
   freezing loads were odd. Even-but-not-nice loads (14, 998) were never run, so the record cannot
   distinguish "even" from "multiple of the leaf count + 1" as the persistence condition.
3. **Ratio measured at one place.** "Ratio at the place with largest |value|" silently assumes a
   single global growth rate. The subdominant structure (which explains their own two-timescale
   data in (7)) was never measured.
4. **The survey buckets don't close.** 21 + 55 + 413 = 489 of 1200; the remaining 711 (presumably
   clean freezes) are never mentioned, and no per-world structural data (degree, spectrum) was
   recorded against outcome — they had the discriminating variable in hand and never computed it.
5. **Shape-periodicity checked only for sourced worlds.** They noticed growing-shape period 3 in
   sourced worlds but apparently never asked whether closed unbounded worlds have shape structure
   beyond sign alternation (normalize by the ratio and look).
6. **One overflow run.** The entire "beyond 2^40" claim domain rests on a single trajectory;
   whether the wrap-freeze is generic or a fluke of that state is unknown.
7. **The word "symmetric" is doing quiet work.** All relations are bidirectional in every
   experiment; the framework explicitly allows directed relations (the rule is stated per outgoing
   relation) and no asymmetric world was ever run.

---

## 6. MISSING OBSERVATIONS

1. **Spectral audit of the survey.** Compute λ_max of each of the 1200 graphs and tabulate against
   outcome. My explanation predicts a near-perfect three-way split at λ_max < 4 / = 4 / > 4 (with
   a friction-threshold blur near 4 at small loads). This single analysis confirms or destroys the
   whole theory.
2. **Superposition test.** Run load L at A; load L at B; load L at both. Compare the sum of the
   first two to the third, tick by tick. Deviation isolates the floor term cleanly.
3. **Second-mode measurement.** Fit the convergence rate of the ratio in (5)/(7) to extract |μ₂|;
   check against the graph's second Laplacian eigenvalue.
4. **The friction threshold.** For a fixed λ_max slightly above 4 (e.g. the twin-triad of P5),
   sweep the load and find the boundary between "floors kill it" and "growth escapes". The record
   contains no experiment at small growth rate + small load.
5. **Degree-4 lattice.** A modest 2D grid has interior λ well above 4; the theory predicts a grid
   explodes rather than diffusing. Nobody ran one, and it is the most physically suggestive case.
6. **Mixed-sign and negative initial conditions**, and **directed relations** — both permitted by
   the stated rules, both absent.
7. **Overflow census.** Many runs through the 2^63 boundary, recording whether arithmetic-glass
   freezing is the generic end state, and whether "total preserved mod 2^64" ever visibly breaks.
8. **Sourced long-run slopes.** Does each place's growth rate converge to (total production)/(place
   count) regardless of where the sources sit? The record says "grow steadily" without ever
   reporting a slope.
