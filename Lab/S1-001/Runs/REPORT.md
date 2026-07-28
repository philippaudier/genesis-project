# Campaign S1-001 — Execution Report

Date: 2026-07-28. Sealed design: `7a4814e`. Gate opened: `f54958c`. Worlds run exactly as
sealed: W0, W1, W2, W3, W4, W4-B3. No abort guard fired. This report contains claims, statuses,
and record facts — no interpretation. The reduction is not authorised and is not here.

Status label used below: **NOT EXERCISED** = the conditions the claim addresses never arose in
the record; the claim is neither confirmed nor refuted.

## Claims

| # | Claim (as sealed) | Status | Record fact |
|---|---|---|---|
| A0 | W0 flat control reproduces corpus behaviour | **CONFIRMED** | Uniform accumulation to 20 during rain; zero movement; frozen from tick 20; gradients 0 throughout |
| A1 | Naive flow on interior-degree-4 grid (W1) diverges | **REFUTED** | W1 froze at tick 20; no value ever exceeded 20; max driving gradient over the whole run: 1; zero transfers occurred |
| A2 | Degree-aware divisor (W2) stabilises the same parcel | **NOT EXERCISED** | W2 was stable (frozen at tick 20) — but the contrast the claim was built for never arose: the naive parcel did not diverge |
| A3 | Drainage pattern locks early; volumes evolve slowly inside it | **NOT EXERCISED** | Descent pattern locked at tick 0 in every world and never changed (1 pattern per run); no flow-driven volume evolution existed |
| A4 | Freeze ⇔ all driving gradients ≤ 1 (W2/W3) | **REFUTED** (⇐ direction) | W2: frozen, max gradient 1 — consistent. **W3: frozen from tick 20 while holding a driving gradient of 2** (ridge column against its neighbours) |
| A5 | Locus of first negativity (deposit's overshoot pump) | **NOT EXERCISED** | No negative value occurred in any world |
| B1 | Per-kind conservation, crossings accounted | **CONFIRMED** | kind(2) audit: 0 violations in all six worlds, rain accounted tick for tick |
| B2 | Conversion conserves under additive resolvers (H1) | **NOT EXERCISED** | Zero conversion events occurred in W4; kind(3)+kind(4) invariant trivially (no edge ever exceeded the threshold; no edge ever fired at all) |
| B3 | Negative control: the audit must detect the non-additive leak | **CONFIRMED** | kind(5) audit: **150 violations** (leak of exactly +1 per tick, every tick); W4-B3 is the only world that never froze |

Common Ground note (fact, not reading): A1 was a shared prediction of both imaginations. It
failed for both columns simultaneously. No discriminating claim was exercised.

## Traces (E2 — the first artifact)

Per world under `Runs/<name>/`: `states.csv` (full record: tick, place, kind, value),
`ledger.csv` (per-kind totals per tick), `audit.txt`, `pattern.txt` (descent-pattern change
ticks, declared convention), `negativity.txt`, `partition.txt` (terminal-minimum partition of
the final state, declared conventions, vocabulary gate applied — no trip), `summary.txt`.

## Observations filed

- **Observation-011** — the stasis: five worlds, twenty ticks of rain, and no unit of any kind
  ever crossed a relation.
- **Observation-012** — W3 froze and stayed frozen while holding a driving gradient of 2.

(E3 honoured: the two facts are filed separately, unnamed, un-unified.)

## Unexpected Manifestations

```text
Unexpected phenomenon: the stasis (→ Observation-011)
```

Neither sealed column predicted it; no sealed expectation imagined it.

## Failed Expectations (scored only against the sealed lists)

All six sealed informal expectations failed:

1. Basin-ward convergence within O(diameter) ticks — **failed**: no water ever moved.
2. Ridge line carries near-zero flux (dead-edges echo) — **failed as meant**: flux was zero
   everywhere; the contrast the expectation relied on never existed.
3. Two sides fill independently — **failed as meant**: both filled by rain identically; no flow
   on either side.
4. Local minima trap water — **failed as meant**: no water was in motion to be trapped.
5. Period-3 breathing shape reappears — **failed**: no periodic behaviour of any kind appeared
   in W0–W4 (the only non-frozen world is W4-B3, whose motion is the sacrificial pair).
6. Sediment accumulates where capacity drops — **failed**: no sediment ever moved; no
   conversion ever fired.

## World Corrections

1. **A4's wording carried a threshold from one family to another.** The executor noticed, from
   the law text and before execution, that the corrected family's transfer is zero whenever
   diff ≤ degree — so freezing can hold gradients up to the local degree, not 1. Per E1,
   nothing was changed; the run then produced the refuting fact itself (Observation-012).
   Timing declared: noticed pre-run, recorded here, acted on never.
2. **The sealed instrument spec included per-edge flux counts; the counter was not implemented
   before execution.** The record shows zero threshold crossings — there was no flux to count —
   but the gap is recorded, not excused.
3. Cross-reference (filing fact, no causal claim): the sealed Blind Spot Audit, item 3,
   declared distributed sources untested — corpus sources were single-point.

## Decision

Withheld. The reduction is a separate authorisation.
