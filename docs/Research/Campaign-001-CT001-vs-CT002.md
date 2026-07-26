# Campaign-001 — CT-001 versus CT-002

Status: **EXECUTED and CLOSED. Verdict: Outcome B (sealed criteria).** Sealed at commit `ab8f2e0`;
executed strictly after the seal; results in the Results section below and in **Observation-009**.
Every section between here and the Results section is immutable, exactly as sealed.

---

## The two candidates

### CT-001 — the spectral-linear candidate *(designated in DN-005; origin Obs-005/007)*

Whenever the set of firing relations is constant, the tick map is linear. **Claim: a closed world's
regime (damped / marginal / divergent) is determined by the eigenvalues of the linearised flux
operator on its topology.** For a centre-loaded n-star, the centre–leaves mode has amplification
λ = 1 − (n+1)/2 — matching the measured ratios (−0.5, −1, −1.5, −2). Corollary claims:

- **C1a** — the regime is a property of the *graph*; loads set amplitude, never regime.
- **C1b** — parity effects (Obs-005/006) are a second-order truncation perturbation riding on the
  marginal case, not first-order dynamics.
- **C1c** — growth ratios on new topologies are computable in advance from the operator.

### CT-002 — the activation-truncation candidate *(built for this trial, in earnest)*

The dynamics are fundamentally piecewise: which relations fire (`self > target`) and the integer
floor are not perturbations — they are the mechanism. **Claim: a world's regime is determined by the
trajectory of its active-relation pattern and its truncation losses; the linear spectrum describes
only episodes where the pattern has stabilised.** Corollary claims:

- **C2a** — *load placement* can change the regime on the same graph (activation differs even where
  the operator does not).
- **C2b** — truncation losses are first-order: at the marginal degree, damping time *scales with
  load* (≈ one unit lost per rebound), rather than staying flat.
- **C2c** — clean spectral ratios are asymptotic accidents of stabilised activation patterns and
  should degrade at small values, where truncation bites hardest.

CT-002 is not a strawman: the corpus already contains evidence in its favour — **dead edges**
(Obs-002: activation decides what topology even *means*) and the **parity split** (Obs-005: pure
linear theory is parity-blind).

## Deliverable 2 — explanatory audit

| Observation | CT-001 | CT-002 |
|---|---|---|
| 001 slosh → perfect ring equality | ✓ (stable modes) | ✓ (activation) |
| 002 dead edges | (must import activation as a boundary condition — a concession) | **✓ native** |
| 003 period-3 growth shape | ✗ (silent — RD-5) | ✗ (silent — RD-5) |
| 004/005 three regimes | **✓ native (λ)** | ✓ (derivable, laboriously) |
| 005 parity split at deg 3 | (perturbation addendum) | **✓ native** |
| 005 growth ratios −1.5 / −2 | **✓ native, quantitative** | (asymptotic argument only) |
| 007 max-deg-3 tree diverges | **✓ native (spectrum)** | ✓ (cascade computation) |
| 008 freeze at ≈2⁶² | ✗ (silent — RD-2) | ✗ (silent — RD-2) |

Honest summary: CT-001 owns the quantitative regime structure; CT-002 owns the discrete anomalies;
neither touches RD-2 or RD-5. The audit alone justifies the trial: their strengths are disjoint.

## Deliverable 3 — the discriminating predictions (the strongest three)

### DP-1 — Load placement (tests C1a vs C2a)

**World W1:** the deg-3 star of Obs-004, but the load of 12 placed at a *leaf*, not the centre.
Same graph, same total, different placement.

- CT-001 predicts: same graph ⇒ same marginal class ⇒ qualitatively the star behaviour of even loads
  (a persistent period-2 régime, possibly after a transient).
- CT-002 predicts: a different activation history ⇒ the world may damp to a frozen state instead.

*Filed blind: neither side has computed W1.*

### DP-2 — Load scaling at the margin (tests C1b vs C2b)

**Worlds W2a/W2b:** the deg-3 centre-loaded star with loads **1200** and **1201**.

- CT-001 predicts: the parity split persists at scale and damping (odd case) remains *fast* — a
  perturbation does not grow with amplitude.
- CT-002 predicts: the odd case's time-to-freeze grows roughly *linearly with load* (~one unit bled
  per rebound ⇒ hundreds of ticks), and the even case's period-2 persists.

Measure: exact tick of freeze, as a function of load ∈ {12, 120, 1200, 1201, 12001}.

### DP-3 — A pre-committed quantitative ratio (tests C1c vs C2c)

**World W3:** a topology neither theory has seen — two deg-4 stars sharing a single common leaf
(9 places), centre A loaded 12.

- CT-001 must **file a numeric growth ratio in advance**, computed from its operator (the filing
  slot below is part of this protocol; the campaign may not execute until it is filled).
- CT-002 predicts the measured ratio deviates from any pre-filed linear value during the early
  small-value ticks and only approaches it late, if ever.

> **CT-001 filing slot:** predicted asymptotic growth ratio for W3 = **λ = −(3+√13)/4 ≈ −1.651**
> (derived at design time from the linearised operator M = I − L/2: the symmetric centre-mode of the
> shared-leaf double star has Laplacian eigenvalue μ = (7+√13)/2; λ = 1 − μ/2. The same operator
> retrodicts every measured ratio: star-n → 1−(n+1)/2; K4 → −1.)
> **CT-002 filing slot:** predicted deviation window = **measured per-alternation ratio deviates
> > 5 % from −1.651 for the early ticks (t ≤ 15, small values), approaching it only late, if ever.**

## SEALED DECLARATIONS (filed blind, before any execution)

*Both advocates may cite only already-published observations. No world in this protocol has been
run or mentally simulated beyond the qualitative level. Confidence measures the advocate, not the
theory (per the calibration principle: not only "was it right?" but "how much did it know it could
be wrong?").*

**CT-001 declares:**

| | Prediction | Confidence |
|---|---|---|
| DP-1 | Leaf-loaded deg-3 star remains in the marginal class: persistent period-2 (after a possible transient) | **55 %** |
| DP-2 | Even loads: period-2 persists with exactly scaled amplitude (centre extreme = −load/2). Odd loads: damping stays *fast* (freeze-time roughly flat in load) | **30 %** *(the advocate concedes: S2/S5 already show freeze-time growing with load — 13→t6, 15→t8)* |
| DP-3 | Asymptotic ratio −(3+√13)/4 ≈ −1.651, within 3 % by late ticks | **75 %** |

**CT-002 declares:**

| | Prediction | Confidence |
|---|---|---|
| DP-1 | Activation from a leaf differs from centre-activation: the world damps to frozen instead of oscillating | **60 %** |
| DP-2 | Odd-load freeze-time grows ~linearly with load (~one unit bled per rebound ⇒ load 1201 freezes in hundreds-to-thousands of ticks, not single digits) | **85 %** |
| DP-3 | Early ratios (t ≤ 15) deviate > 5 % from any pre-filed linear value; late convergence possible | **55 %** |

**The files are now immutable. The seal is the commit that contains this text; its hash timestamps
the blind filing. Execution may begin only after this commit exists.**

## Deliverable 4 — campaign design

Worlds: W1 (leaf-loaded deg-3 star) · W2a/b + scaling series (deg-3 star, loads 12→12001) · W3
(shared-leaf double star). All closed (no production), all headless, 2 000-tick horizon, standard
classifiers (frozen / periodic-with-period / divergent), plus per-tick freeze-time and ratio
measurements. Every world reproducible from its stated configuration alone — no seeds needed, no
randomness exists.

## Deliverable 5 — outcome criteria (bound now)

| Result pattern | Verdict |
|---|---|
| CT-001 right on DP-1 *and* DP-3; CT-002 wrong on ≥2 | **Outcome A** — CT-001 strongly reinforced |
| CT-002 right on DP-1 *and* DP-2; CT-001 wrong on ≥2 | **Outcome B** — CT-002 preferable |
| Each right where the other is silent; no head-on loss | **Outcome C** — compatible; RD-1 refined, not paid. Includes the merger reading: CT-002 as the first-order truncation layer of CT-001's linear skeleton — one theory at two orders |
| Either fails its own native ground (CT-001 wrong on a ratio; CT-002 wrong on a truncation effect) on top of losing a DP | **Outcome D** — both wounded; a new candidate is required |

No outcome names a theory. Naming remains DN-005's separate, unanswered question — this campaign can
at most produce the *evidence* a future naming would cite.

## What execution will require

A separate authorisation; the two filing slots completed; and nothing else. The harness exists; the
worlds are three configurations away. The design is deliberately finished *before* anyone is allowed
to peek — that is the entire point of writing it down.

---

# RESULTS (written after the seal; nothing above this line was modified)

Execution: 2026-07-26, headless harness, worlds exactly as pre-registered. Full data and analysis in
**Observation-009**; summary here.

- **DP-1** — leaf-loaded star **damps and freezes** at (3,3,3,3), t=6, no negativity ever; the
  centre-loaded control oscillates period-2 with violations. **CT-002 right; CT-001 wrong.**
- **DP-2** — odd-load freeze time is **exactly t = (load+1)/2** (13→7, 121→61, 1201→601,
  12001→6001): damping time scales linearly with load. Even loads: period-2 with centre extreme
  exactly −load/2. **CT-002 right; CT-001 wrong** (its even-scaling sub-claim held).
- **DP-3** — measured ratio at t=60: **−1.65137** against the blind-filed −(3+√13)/4 ≈ −1.65139 —
  five decimal places on an unseen topology. Ratio within 5 % of the filed value from t=10, so the
  sealed CT-002 window ("> 5 % through t ≤ 15") is false. **CT-001 right; CT-002 wrong.**

## Verdict

The sealed table row *"CT-002 right on DP-1 and DP-2; CT-001 wrong on ≥ 2"* matched:
**Outcome B — CT-002 preferable.** The verdict is bound and recorded.

The discussion (Obs-009) records what the criteria were too coarse to say: the same run delivered
CT-001's strongest confirmation ever (the five-decimal blind ratio), and the full evidence pattern
is the sealed merger reading of Outcome C — activation/truncation as the first-order layer, the
linear spectrum exact over stabilised episodes. The lesson filed against future campaigns: outcome
rows scored *per-prediction wins*; they should also have scored *per-claim ground* (C1c was
confirmed spectacularly while C1a/C1b fell). Criteria bind; they must therefore be written finer.

## Calibration record

CT-001's advocate: 55/30/75 → Brier 0.152. CT-002's advocate: 60/85/55 → Brier 0.162. The losing
theory was the better-calibrated defence; the concession on DP-2 (30 %, forced by public S2/S5 data)
is the record's first example of an advocate scoring against its own theory — which is exactly what
the confidence field was added to detect.
