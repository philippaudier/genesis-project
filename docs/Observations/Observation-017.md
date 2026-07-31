# Observation-017

Date: 2026-07-31
Genesis version (commit/tag): seal `0cc83b0`, instrument `c98f6a5`,
conformance `3b5dea4`, execution gate `8395ede`, immutable record `532a5eb`
(Campaign S1-006)
World (topology, initial cells, rates): strict pair P0/P1; four-place
bidirectional chain A ↔ B ↔ C ↔ D; Base 0, Rock 12, Sediment 0, Water 0 at
each place; degree-aware divisor `outgoing degree + 1`; conversion when
prospective Water transfer is strictly greater than 5; `+12 Water` at A on
boundaries 0, 1, 10, and 11, then silence; 128 ticks. The worlds differ only
in Sediment-transport competence: P0 = 0, P1 = 1.
Reproduction seed / setup:
`dotnet run --project Lab/S1-006/S1_006.csproj -- --execute`
(deterministic; no seed).

---

## Observation

**Two complete-state fixed worlds retained different solid surfaces after the
same repeated forcing because one local Sediment transfer was admitted in P0
and refused in P1.**

P0 and P1 reproduced the sealed hand derivation through tick 6. At boundary 6,
the prospective Sediment transfer C→D was exactly one:

- P0 admitted the snapshot unit because `1 > 0`;
- P1 refused it because `1` is not `> 1`.

The first complete-state difference therefore appeared after tick 7:

```text
P0 SolidSurface [10,12,12,14]
P1 SolidSurface [10,12,13,13]
```

Both worlds then received the second identical Water episode at boundaries 10
and 11. Their complete states continued to change, and their difference
reached A and B through Water before rest. Neither world returned to the
other's trajectory.

P0 first reproduced a complete state across silent boundary 18 and remained
unchanged across 110 consecutive silent recorded transitions beginning there:

```text
Rock      [8,12,12,12]
Sediment  [0, 0, 0, 4]
Water     [18,13,11,6]
Surface   [8,12,12,16]
```

P1 first reproduced a complete state across silent boundary 21 and remained
unchanged across 107 consecutive silent recorded transitions beginning there:

```text
Rock      [8,12,12,12]
Sediment  [0, 0, 3, 1]
Water     [18,13, 8,9]
Surface   [8,12,15,13]
```

## Measurements

- Campaign classification: **Outcome A — form selected**.
- Claims C0–C6: all held.
- First complete-state difference: tick 7.
- First differing contribution: boundary 6, CompetenceTransportFixture,
  Sediment C→D, amount 1.
- Rock + Sediment: `48 → 48` in both worlds.
- Reconstruction faults: 0 in both worlds.
- Matter-audit faults: 0 in both worlds.
- Minimum value across Base, Rock, Sediment, and Water: 0 in both worlds.
- P0 first fixed transition: boundary 18; quiet recorded suffix: 110.
- P1 first fixed transition: boundary 21; quiet recorded suffix: 107.
- Final surfaces: P0 `[8,12,12,16]`; P1 `[8,12,15,13]`.
- Allocation-sensitive boundary/place records: 0 in both worlds.
- Membrane events: `+12 Water` at A on boundaries 0, 1, 10, and 11 in both
  worlds; none later.

No Kind or law is named Surface, Relief, Rest, or Form. `SolidSurface` remains
the observer's reading `Base + Rock + Sediment`.

## Hypotheses

Reduction authorised 2026-07-31:

- **F-S1-1 receives first support in S1-006's exact domain:** when
  complete-state stability was common, transport competence selected which
  conserved spatial distribution remained.
- **C-S1-1 survives with a narrower role:** competence separated two resting
  forms here; it was not necessary for rest under the shared degree-aware law.
- **S-S1-1 and T-S1-1 survive and extend** to one four-place chain, two forcing
  episodes, and a fully uncalculated suffix.
- RD-12 is paid in this first extended, repeatedly forced domain. Robustness
  across topology, scale, forcing histories, and allocation-sensitive branches
  remains unpaid as RD-13.
- No candidate fixture is promoted to a production Genesis law.

## Status

Open
