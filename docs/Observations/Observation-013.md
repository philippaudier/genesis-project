# Observation-013

Date: 2026-07-29
Genesis version (commit/tag): sealed design `4885d04`, gate `4dcf41b` (Lab/S1-001, Campaign S1-002)
World (topology, initial cells, rates): twelve minimal parcels — 2-cell pairs (flat and sloped
d = 0…4), stars (k = 1…3, centre kind(1) = 1), a 5-chain, and a forked six-place graph — naive
or degree-aware divisor as sealed; point crossings of +1 into kind(2), boundaries 0–29.
Reproduction seed / setup: `Lab/S1-001$ dotnet run -- --execute-s1-002` (deterministic).

---

## Observation

**The first transfers in Science-001's history occurred** — earliest during tick 0 (2-cell
worlds with kind(1) difference ≥ 2), and at ticks 1, 2, 3, 5, 9, 13, 14 across the other
parcels. Every first-transfer tick, in every world, on every edge, **equals the value derived
by hand from the fixture text before execution and sealed at `4885d04`**: the threshold sweep
gave 2, 1, 0, 0, 0; the stars gave k exactly; the cascade gave 2, 5, 9, 14 (hop delays 3, 4, 5,
increasing); the locality pair gave 14 and 13 — the forked world's watched edge fired one tick
earlier than the single-path world's, with identical 1-hop neighbourhoods. The per-edge flux
counter's consistency check reported zero mismatches across all twelve worlds; the
conservation audit, zero violations. One sealed informal expectation (constant cascade delay)
failed against the record — and the sealed hand derivation had already contradicted it before
execution; the record agreed with the derivation.

## Reproduction

Clone at `4dcf41b` or later. `cd Lab/S1-001 && dotnet run -- --execute-s1-002`. Compare
`Runs-S1-002/*/summary.txt` against the claims table sealed in
`docs/Research/Campaign-S1-002-The-First-Flow.md`.

## Measurements

- Claims C0–C5: six of six confirmed, all exact (see the campaign report).
- Consistency mismatches: 0 across 12 worlds; conservation violations: 0 across 12 worlds.
- First negative kind(2) value: tick 1, place(0), in V1-d4 only — as the sealed informal
  expectation had computed.
- Unclaimed rhythm facts on record, unread: stars fire with period k+1; V1-d3 fires at ticks
  0 and 1 then alternates; V3 upstream edges fire irregularly under downstream interaction.

## Hypotheses

Withheld — the reduction of Campaign S1-002 is not authorised. (H-S1-1's promotion and F-1's
evaluation are explicitly outside the execution authorisation.)

## Status

Open
