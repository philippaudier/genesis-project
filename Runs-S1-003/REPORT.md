# Campaign S1-003 — Execution Report

Date: 2026-07-29. Sealed design: `6c37190`. Blind instrument: `79e4ab1`. Execution gate:
`cba37a6`. U0 and U1 ran exactly as sealed for 120 ticks each. This report binds the record to
the campaign's predeclared outcomes; it performs no reduction.

## Witnesses first

| Witness | U0 | U1 |
|---|---:|---:|
| Flux reconstruction mismatches | 0 | 0 |
| Conservation violations | 0 | 0 |
| Mirror violation during the run | no | yes |
| Maximum absolute left-right total difference | 0 | 1 (tick 28) |
| Left-right total difference at tick 60 | 0 | −1 |
| Left-right total difference at tick 120 | 0 | −1 |

Both records contain a minimum Water value of −1, first at tick 8, place 10. This is recorded
without interpretation; it does not break reconstruction or conservation.

## Bound outcome

**A — P-S1-003 confirmed, persistent.**

- The first U0/U1 Water difference occurs at tick 19, place 16: `(row 2, col 2)`.
  U0 holds 1 and U1 holds 2.
- Place 16 is outside the perturbed elevation cell, place 23: `(row 3, col 2)`.
- U0 has no mirrored Water difference at any recorded tick.
- U1's first mirrored differences occur at tick 19.
- At tick 120, U1 still has a signed mirrored difference of −1 at row 3, distance 1.
- The instruments report no failure.

These facts satisfy Outcome A exactly as it was bound before execution: a difference outside
the perturbed cell exists, and a mirrored U1 difference remains non-zero at tick 120.

## Traces

For each world under `Runs-S1-003/U0/` and `Runs-S1-003/U1/`:

- `states.csv` — full elevation and Water state for every cell and tick;
- `flux.csv` — directed per-edge flux for every firing edge and tick;
- `first-wet.csv` — first positive-Water tick per cell;
- `mirror.csv` — every signed mirrored difference;
- `regions.csv` — left, centre, right, signed difference, and full total per tick;
- `witnesses.txt` — reconstruction, conservation, negativity, and declared summary readings.

`COMPARISON.txt` contains the mechanical cross-world classification.

## Decision

Withheld. Reduction is a separate act.
