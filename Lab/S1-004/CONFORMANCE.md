# S1-004 — Gate 7 Conformance Record

Date: 2026-07-29
Sealed prose: `b114a34`
Implementation reviewed: `75636a5`
Final pre-seal instrument specification: `f677972`.
Result: **CONFORMING — no World Correction.**

No M0 or M1 tick was executed during this review.

## Line-by-line obligations

| Sealed term | Implementation | Verdict |
|---|---|---|
| `SolidSurface = Base + Rock + Sediment` | `K4.SolidSurface`; shared `Potential.Terms` | conforms |
| `WaterPotential = SolidSurface + Water` | `K4.WaterPotential`; `Potential.At` | conforms |
| constant divisor 2 | `Worlds4.Constant2`, passed to all three fixtures | conforms |
| conversion iff prospective Water flux is strictly greater than 4 | `diff / divisor > _threshold`; `Threshold = 4` | conforms |
| conversion emits one complete `−1 Rock, +1 Sediment` pair per qualifying edge | `SurfaceConversionFixture` returns both contributions in one transition result | conforms |
| transport reads the same potential | all three fixtures call the shared `Potential.At` reader | conforms |
| transport cannot exceed Sediment held in the snapshot | `carried` is read once from the declared view; each transfer is capped and decrements it | conforms |
| Base is immutable | no fixture emits a contribution targeting `K4.Base` | conforms |
| only Water crosses the membrane | two `+10 Water` events at A, boundaries 0 and 1 | conforms |
| silence on boundaries 2–5; six ticks total | no later events; `Worlds4.Ticks = 6` | conforms |
| M0/M1 differ by exactly the Sediment transport fixture | M0 has flow + conversion; M1 has those plus transport | conforms |

## Collision witnesses

The two witnesses are independent:

1. `SpyResolver` wraps the actual additive resolver bound into the runner. It records invocation
   amounts and committed delta.
2. `Provenance.Collect` invokes each fixture's transitions separately through the same declared
   relational views, retaining fixture identity and target cell.

The resolver interface does not expose the target cell. Therefore neither witness is sufficient
alone: C5 may be scored only when resolver inputs and provenance agree, as sealed.

## Independent calibration

Command:

```text
dotnet run --project Lab/S1-004/S1_004.csproj -- --calibrate
```

Result: all eight sealed obligations passed, plus static construction of M0/M1. Calibration
worlds were foreign two-place toys with Rock 5, preloaded Sediment, and initial Water; they used
neither campaign parcel nor campaign crossings.

The calibration confirmed:

1. exact surface reading;
2. conversion surface invariance;
3. equal-and-opposite surface change under transport;
4. detection of a dropped conversion half-pair;
5. reconstruction failure under a corrupted edge amount and under omitted conversion input;
6. equality of identical toy worlds;
7. one resolver invocation for a planted `(+1, −1)` conflict, committing 0;
8. disagreement when fixture provenance loses one contribution.

## Gate result

Gate 7 is closed with agreement. Gate 8 remains closed: execution still requires the founder's
second explicit authorisation.
