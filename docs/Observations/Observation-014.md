# Observation-014

Date: 2026-07-29
Genesis version (commit/tag): sealed design `6c37190`, blind instrument `79e4ab1`,
execution gate `cba37a6`, record `3f75521` (Campaign S1-003)
World (topology, initial cells, rates): strict one-factor pair U0/U1; 7×7 four-neighbour
bidirectional grids; constant divisor 5; additive Water resolution; zero initial Water; U0
mirror-symmetric relief; U1 identical except elevation `(row 3, col 2)` raised from 8 to 9;
point crossing of +1 Water at `(0,3)`, boundaries 0–59; silence 60–119.
Reproduction seed / setup:
`dotnet run --project Lab/S1-001/S1_001.csproj -- --execute-s1-003` (deterministic).

---

## Observation

**A one-unit relief difference produced a Water-state difference outside its own cell, and a
mirrored difference remained after the source had been silent for sixty ticks.**

The first U0/U1 Water difference occurred at tick 19, place 16 `(row 2, col 2)`: U0 held 1,
U1 held 2. The sole relief perturbation was place 23 `(row 3, col 2)`. At the same tick, U1's
first non-zero mirrored readings appeared at rows 2 and 3, distance 1. U0 remained exactly
mirror-symmetric for the complete record.

At tick 120, U1 still held a signed mirrored difference of −1 at row 3, distance 1. Its
left-minus-right total was also −1. U0's corresponding values remained zero.

## Reproduction

Clone at `cba37a6` or later and run the command above from the repository root. Compare the
complete records in `Runs-S1-003/U0/` and `Runs-S1-003/U1/`; the bound mechanical
classification is in `Runs-S1-003/COMPARISON.txt`.

## Measurements

- Campaign outcome: **A — confirmed, persistent**, as bound before execution.
- First cross-world Water difference: tick 19, place 16 `(2,2)`, U0 = 1, U1 = 2.
- First mirrored differences in U1: tick 19, row 2 distance 1 = +1; row 3 distance 1 = −1.
- U0 mirrored differences: zero for all 120 ticks.
- Maximum absolute left-right total difference: U0 = 0; U1 = 1, first maximum at tick 28.
- Left-right total difference at tick 60: U0 = 0; U1 = −1.
- Left-right total difference at tick 120: U0 = 0; U1 = −1.
- Flux reconstruction mismatches: 0 in both worlds.
- Conservation violations: 0 in both worlds.
- Minimum Water value: −1 in both worlds, first at tick 8, place 10.

The negative value is not unique to this campaign: S1-002 had already recorded one under the
same law family. It remains a measured fact here and is not used to classify Outcome A.

## Hypotheses

Reduction authorised 2026-07-29:

- F-1 receives its first experimental support, limited to S1-003's domain.
- H-S1-1 survives as the explanation of local activation, not as a complete trajectory
  predictor for structured parcels.
- RD-10 is paid by separating the relation-scale activation condition from the trajectory-scale
  role of delivered state and structure.
- Method-001's hand-computability frontier is confirmed, neither extended nor refuted.
- The negative Water value is excluded from this reduction.

## Status

Open
