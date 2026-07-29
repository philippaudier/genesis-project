# Campaign S1-003 — The First Uncalculated Parcel

Status: **SEALED — implementation authorised; execution forbidden.** Sealed 2026-07-29 by the
founder's approval of the question, U0/U1 pair, six outcomes, and confidence declaration. From
this point, the campaign text is intangible until a verdict exists.

## Founding question

> **What is the smallest structure capable of making a purely local derivation lie?**

The question was fixed in `Science-001-Research-Agenda.md` before this design existed. Here,
*lie* means something precise: a correct local reading remains correct for the present edge,
yet becomes insufficient to predict the later history of the place because structure outside
its initial neighbourhood eventually arrives through state.

This campaign does not attempt to refute Method-001. Method-001 already says where hand
derivation becomes infeasible, it stops applying. S1-003 deliberately crosses that boundary
for the first time and asks whether the laboratory can remain rigorous without knowing the
trajectory in advance.

## Why this Draft exists now

Demo-001 supplied two non-evidential resistances:

1. Under uniform rain, a visibly corrugated 32×32 relief kept every row perfectly
   column-uniform for 550 ticks. The visible troughs routed nothing.
2. In the contaminated 7×7 Routing Probe, replacing uniform rain with a point source made a
   sub-threshold corrugation causally active: flat and shaped parcels first diverged at tick 20.

An attempted 3×4 S1-003 Draft then died before filing. Two independent hand passes derived its
complete relevant trajectory (global divergence at tick 10, outlet divergence at tick 14).
It reproduced S1-002's comfort instead of threatening it.

The surviving demand is therefore not another exactly calculable pair. It is a small parcel
whose laws remain completely comprehensible while its full history does not.

## Question

> **When a locally symmetric source meets an almost-symmetric relief, where does the first
> measurable asymmetry appear — and does any asymmetry persist after the source stops?**

No river, channel, watershed, branch, winner, or preferred side is presumed. Those are possible
observer words, not campaign inputs.

## Objects on trial

- **H-S1-1:** every individual firing remains governed by the local activation threshold.
- **F-1:** local activation and global structure interact.
- **Method-001's frontier:** exact hand prediction is intentionally absent; discipline must now
  come from sealed conditions, discriminating instruments, and outcomes that can all lose.

The campaign is not a test of whether the kernel is deterministic. That is inherited and already
proven. It tests, by one-factor ablation, whether a distant one-unit relief difference becomes a
later observable difference in a structured flow.

## Blind Spot Audit

1. **Demo-001 is known:** 32×32, uniform top-row rain, three visible corrugations, constant
   divisor 5, cross-column range zero.
2. **Routing Probe is known:** 7×7 A/B pair, point rain, corrugation
   `{4, 2, 0, −2, 0, 2, 4}`, first full-state divergence at tick 20. Its exact relief and
   comparative design are permanently excluded.
3. **The dead 3×4 Draft is known:** straight versus upstream-shaped pair; hand-derived
   divergence ticks 10 and 14. Its dimensions, elevations, paired-world design, and watched
   outlet are excluded.
4. **S1-002 V4 is known:** a remote graph difference altered delivery timing despite an
   identical watched neighbourhood. S1-003 must not score merely for observing causal
   propagation.
5. **The parcel is designed with an asymmetry.** The campaign cannot claim spontaneous symmetry
   breaking. It asks whether the declared asymmetry remains local, travels, amplifies, changes
   sign, disappears, or leaves a residue.
6. **Static relief only.** No erosion occurs. This is the first structured hydrological
   observation of Science-001, not yet transformation of the relief itself.
7. **A point source is an intervention.** Legal through the membrane, recorded, and stripped
   of privilege once admitted. It cannot be offered later as an explanation of natural rainfall.

## Parcel pair — fixed candidate

Two worlds differing at exactly one elevation cell. Seven columns × seven rows; four-neighbour
bidirectional relations; constant divisor 5; additive water resolution; zero initial water.

**U0 — symmetric control**, top row first:

```text
20 18 17 20 17 18 20
17 15 14 17 14 15 17
14 12 11 14 11 12 14
11  9  8 11  8  9 11
 8  6  5  8  5  6  8
 5  3  2  5  2  3  5
 2  0 -1  2 -1  0  2
```

**U1 — perturbed subject:** identical to U0 except `(row 3, column 2)` is raised from elevation
8 to elevation 9.

Construction shared by both:

- base relief falls by 3 per row;
- lateral offsets are `+2, 0, −1, +2, −1, 0, +2`;
- every neighbouring dry elevation difference is at most 4, below divisor 5;
- therefore dry ground is inert before the first crossing.

U0 is exactly mirror-symmetric around column 3. U1's one declared perturbation is the pair's only
difference and the only broken mirror equality.

The source in both worlds is the top-centre cell `(0,3)`. Its initial left and right neighbours
are exactly equal (both elevation 17, water 0). U1's perturbation is four relations away and
cannot affect the source's first local reading.

Intervention:

- `+1 Water` at `(0,3)` once per boundary;
- rain boundaries: **0–59**;
- silence boundaries: **60–119**;
- total run: **120 ticks**.

These durations are not tuned to a result. Sixty ticks exceeds the contaminated probe's known
divergence time by a factor of three; the equal sixty-tick silent half asks whether any produced
difference survives without continued forcing.

## Measurements — fixed before execution

Every measurement is applied identically to U0 and U1; none names a formation.

1. Full water state, every cell, every tick, per world.
2. Per-edge directed flux, every tick, per world.
3. First wet tick of every cell (`Water > 0`), per world.
4. First tick at which mirrored cells differ:
   `(row, 3−d)` versus `(row, 3+d)`, for `d = 1..3`.
5. Signed mirrored difference through time:
   `Water(left) − Water(right)` for every mirrored pair, per world.
6. Three declared totals:
   - left: columns 0–2;
   - centre: column 3;
   - right: columns 4–6.
7. First tick and cell at which U0 and U1 water states differ.
8. Maximum absolute left-right total difference, with tick, per world.
9. Left-right total difference at ticks 60 and 120, per world.
10. Conservation ledger: admitted water versus water present, per world.
11. Most negative value and its first tick, if any, per world.

The geometric partition is a declared reading convention, not world state. Reversing its sign
must reverse every signed result and change no magnitude.

## Claims and predictions

### Derived claims

- **C0 — Dry stasis.** In both worlds, with zero water and before crossings are applied, every
  law contribution is zero.
- **C1 — Conservation.** In both worlds, at every tick, total water equals the number of admitted
  crossings.
- **C2 — Control symmetry.** Every mirrored U0 water value remains exactly equal for the entire
  run.
- **C3 — Initial local symmetry.** Before state arriving from outside the source's initial
  neighbourhood can distinguish its sides, U1's first lateral source contributions are equal.

No exact tick beyond C0 is hand-derived. That absence is the subject, not a missing slot.

### Risky prediction P-S1-003

> **U1's one-unit perturbation will produce a water-state difference from U0 outside the
> perturbed cell before tick 120.**

The prediction deliberately does **not** state:

- which side will hold more water;
- where the first mirrored difference will appear;
- when it will appear;
- whether it will persist after tick 60;
- whether its sign will remain stable.

Those are the possible findings.

Confidence: **65/100.** The Routing Probe supports causal reach, but the parcel, relief, metric,
and controlled pair are independent; thresholding may trap the perturbation locally or erase
its effect after the source stops.

## Instrument obligations

Before execution, on foreign toy sequences only:

1. mirrored-difference reader returns zero for perfectly mirrored states;
2. swapping left/right reverses signs and preserves magnitudes;
3. an injected one-cell difference is located at its exact first tick;
4. left + centre + right equals the full-state total;
5. flux reconstruction matches `next = previous + crossings + inflow − outflow`;
6. conservation witness proves it can fail on a deliberately corrupted toy record.
7. identical toy worlds produce no reported cross-world difference.

An instrument that cannot scream cannot testify.

## Outcomes — bound before execution

- **A — P-S1-003 confirmed, persistent.** U1 differs from U0 outside the perturbed cell and a
  mirrored U1 difference remains non-zero at tick 120.
- **B — P-S1-003 confirmed, transient.** U1 differs from U0 outside the perturbed cell, but every
  mirrored U1 difference returns to zero by tick 120.
- **C — P-S1-003 refuted, confined.** U1 differs from U0 at the perturbed cell's water value, but
  at no water cell outside it.
- **D — P-S1-003 refuted, silent.** U0 and U1 never differ in water state.
- **E — Control failure.** U0 develops a mirrored difference. The claimed one-factor attribution
  is unavailable; no scientific verdict.
- **F — Instrument failure.** A witness disagrees with direct state comparison, reconstruction,
  or conservation. No scientific verdict; repair the laboratory first.

The sign may change any number of times under A or B. Sign history is recorded, never used to
rescue or reclassify the prediction.

## What would be learned — limits fixed in advance

At most:

- whether a remote sub-threshold relief difference can become a non-local water-state difference;
- where and when the first measured asymmetry appears;
- whether the difference persists after forcing stops;
- whether the perturbed world departs from an observed symmetric control;
- whether Science-001 can conduct a rigorous campaign beyond exact hand-computability.

Not:

- that a watershed, river, channel, drainage network, or erosion process exists;
- that the chosen relief is realistic;
- that point rain models weather;
- that asymmetry generally amplifies;
- that F-1 is a theory;
- that Method-001 failed.

## Kill criteria before seal

This Draft dies if review shows:

- any dry neighbouring elevation difference is 5 or more;
- the source's initial left/right neighbourhood is not symmetric;
- the perturbation is fewer than four relations from the source;
- U0 is not exactly mirror-symmetric;
- U0 and U1 differ anywhere except elevation `(3,2)`;
- a complete trajectory is easily hand-derivable, recreating the comfort this campaign must
  leave;
- the measurements cannot distinguish persistent, transient, confined, and silent outcomes;
- the parcel's expected behaviour was already learned from Demo-001 or the Routing Probe;
- the single perturbation makes the predicted direction tautological rather than risky.

## Gates

Before a seal:

1. cold review of the grid and all distances;
2. review that U0/U1 form a strict one-factor pair;
3. final instrument specification;
4. founder approval of the question, parcel, outcomes, and confidence.

Only the seal commit may authorise implementation. Implementation still may not execute the
parcel. A second explicit authorisation must open the execution gate.

### Gate record

Founder, 2026-07-29:

> *« fais ce que tu penses être le mieux, mais j'approuve. »*

Interpretation fixed at seal: approval covers the question, parcel pair, outcomes, and confidence;
it authorises blind implementation. It does **not** authorise execution.
