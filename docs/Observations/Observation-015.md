# Observation-015

Date: 2026-07-29
Genesis version (commit/tag): sealed design `b114a34`, conformance `76ccaf1`, gate 8 `76b646f`
World (topology, initial cells, rates): Campaign S1-004's strict pair — two three-place
bidirectional chains A ↔ B ↔ C, identical in every respect but one law fixture. Base 0, Rock 10,
Sediment 0, Water 0 at each place; constant divisor 2; conversion where the prospective water
flux strictly exceeds 4; two crossings of +10 Water at A, boundaries 0 and 1, then silence;
six ticks. M0 may convert but not transport Sediment; M1 may do both.
Reproduction seed / setup: `Lab/S1-004$ dotnet run -- --execute` (deterministic; no seed).

---

## Observation

**A world's solid surface became different from its initial surface, by conserved movement of
matter alone.** Reading `SolidSurface(P) = Base + Rock + Sediment`, M1 stood at `[10,10,10]`
after ticks 1 and 2 and at **`[9,11,10]` after tick 3**. M0 — the same world minus one fixture —
read `[10,10,10]` at every one of its seven recorded states.

No quantity called elevation exists in either world. Nothing wrote a surface: the surface is a
reading over three conserved kinds, and it moved because one unit of Sediment crossed one
relation.

Every bound event occurred where it had been derived by hand and sealed before execution: the
first water flux at boundary 1 (A→B, 5); the first conversion at boundary 1 at A; the first
Sediment transport at boundary 2 (A→B, 1); the first changed surface after tick 3.

At boundary 2, one cell was written by two different laws: `(A, Sediment)` received **+1** from
the conversion fixture and **−1** from the transport fixture. The two witnesses agree: the
contribution record shows both fixtures at that cell, and the resolver's own record shows
`kind(4) [1,-1] → 0` — invoked exactly once in the entire run, committing zero.

## Reproduction

Clone at `dc36f8c` or later. `cd Lab/S1-004 && dotnet run -- --execute`. Determinism makes the
record identical to the digit: a second execution reproduced `states.csv`, `readings.txt` and
`resolver.txt` byte for byte.

## Measurements

- SolidSurface, M1, after ticks 0–6: `[10,10,10] · [10,10,10] · [10,10,10] · [9,11,10] ·
  [9,10,11] · [8,11,11] · [9,10,11]`. States after tick 3 are recorded, not scored.
- SolidSurface, M0, after ticks 0–6: `[10,10,10]` throughout.
- First tick the two worlds differ: 3.
- Rock + Sediment: 30 → 30 in both worlds; audit faults 0.
- Minima over the whole run, both worlds: Base 0, Rock 8, Sediment 0, Water 0 — no negative
  value occurred.
- Contested cells: exactly one cross-fixture collision in the run, `(A, Sediment)` at
  boundary 2.

## Hypotheses

Withheld — the reduction of Campaign S1-004 is not authorised. The campaign's candidates
(S-S1-1: a relief need not be a writable quantity; T-S1-1: only transport can change the
surface reading) are recorded in the sealed design and are not adjudicated here.

## Status

Open
