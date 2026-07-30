# Observation-016

Date: 2026-07-30
Genesis version (commit/tag): seal `7b739b4`, instrument `acf17b8`,
execution gate `cf1f2fd`, record `3fc47a2` (Campaign S1-005)
World (topology, initial cells, rates): strict pair N0/N1; two-place
bidirectional relation A ↔ B; Base 0, Rock 8, Sediment 0, Water 0 at each
place; constant divisor 2; conversion when prospective Water transfer is
strictly greater than 3; `+8 Water` at A on boundaries 0 and 1, then silence;
eight ticks. The worlds differ only in Sediment-transport competence: N0 = 0,
N1 = 1.
Reproduction seed / setup:
`dotnet run --project Lab/S1-005/S1_005.csproj -- --execute`
(deterministic; no seed).

---

## Observation

**The same conserved surface change entered a period-2 material orbit without
competence and a non-uniform complete-state fixed point with competence one.**

Both worlds shared every complete state through tick 3. Their first changed
surface was `[7,9]` after tick 3. At boundary 3 the prospective transfer from
B to A was exactly one:

- N0 admitted the Sediment transfer because `1 > 0`;
- N1 refused it because `1` is not `> 1`.

That was the first difference between the worlds.

N0 then alternated between two complete material states:

```text
tick 3 = tick 5 = tick 7
  A: Base 0, Rock 6, Sediment 1, Water 8
  B: Base 0, Rock 8, Sediment 1, Water 8
  SolidSurface [7,9]

tick 4 = tick 6 = tick 8
  A: Base 0, Rock 6, Sediment 2, Water 9
  B: Base 0, Rock 8, Sediment 0, Water 7
  SolidSurface [8,8]
```

N1 reached this complete state after tick 4 and reproduced it through tick 8:

```text
A: Base 0, Rock 6, Sediment 1, Water 9, Potential 16
B: Base 0, Rock 8, Sediment 1, Water 7, Potential 16
SolidSurface [7,9]
```

No fixture contributed in N1 on boundaries 4–7. Its first complete-state
recurrence was tick 4 at tick 5, return distance 1. N0's first complete-state
recurrence was tick 3 at tick 5, return distance 2.

## Measurements

- Campaign classification: **Outcome A — discriminating support**.
- Claims C0–C6: all held.
- First changed surface, both worlds: tick 3, `[7,9]`.
- First complete-state difference: tick 4.
- Rock + Sediment: `16 → 16` in both worlds.
- Reconstruction faults: 0 in both worlds.
- Matter-audit faults: 0 in both worlds.
- Minimum value across Base, Rock, Sediment, and Water: 0 in both worlds.
- N0 complete-state period: 2.
- N1 complete-state period: 1.
- N1 contributions after boundary 3: 0.

No Kind or law is named Surface, Relief, Rest, or Formation. `SolidSurface`
remains the observer's reading `Base + Rock + Sediment`.

## Hypotheses

Withheld. Reduction of Campaign S1-005 is not authorised. C-S1-1 is neither
promoted nor entered into the Science-001 agenda by this Observation.

## Status

Open

