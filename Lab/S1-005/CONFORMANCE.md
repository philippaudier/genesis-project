# S1-005 — Gate 6 conformance review

Date: 2026-07-30  
Seal: `7b739b4`  
Status: **CONFORMING — no World Correction**

This review compares the implementation line by line with the sealed prose.
It authorises no execution.

| Sealed term | Implementation | Verdict |
|---|---|---|
| two places `A ↔ B` | `Worlds5.Build`: places 0 and 1, exactly two directed relations | conforming |
| `Base=0`, `Rock=8`, `Sediment=0`, `Water=0` | explicit initial cells in `Worlds5.Build` | conforming |
| `SolidSurface = Base + Rock + Sediment` | reused `K4.SolidSurface` from S1-004 | conforming |
| `Potential = SolidSurface + Water` | all three fixtures reuse S1-004 `Potential.At` | conforming |
| constant divisor 2 | `Worlds5.Constant2 = degree => 2`, supplied to every fixture | conforming |
| Water flow unchanged and uncapped | literal S1-004 `SurfaceFlowFixture` source linked and instantiated | conforming |
| conversion threshold 3, strict `>` | literal S1-004 `SurfaceConversionFixture`, parameter `3` | conforming |
| conversion emits one `(-1 Rock,+1 Sediment)` pair per qualifying edge | literal S1-004 fixture; the pair has one outgoing edge per place | conforming |
| transport reads snapshot Sediment | `carried = view.Read(origin,Sediment)` before contributions exist | conforming |
| `prospective = floor(diff/2)` | positive `diff`, integer division by the supplied constant 2 | conforming |
| move only when `prospective > competence` | `if (prospective <= _competence) continue` | conforming |
| cap by snapshot holding | `Math.Min(carried, prospective)`, decrementing local `carried` only | conforming |
| N0 competence 0; N1 competence 1 | `Worlds5.N0()` / `N1()` differ only in that constructor value | conforming |
| `+8 Water` at A on boundaries 0 and 1 | two explicit membrane events, identical in N0 and N1 | conforming |
| silence after boundary 1 | no further events | conforming |
| eight recorded ticks | `Worlds5.Ticks = 8` | conforming |
| complete-state signature excludes tick | canonical Base/Rock/Sediment/Water cell sequence; tick unread | conforming |
| surface signature remains distinct | separate `SurfaceSignature` reader | conforming |

## Calibration

All obligations were executed on foreign toys:

1. equal surface / unequal hidden material distinguished;
2. repeated surface with changing Water rejected as rest;
3. true complete-state fixed point with zero contributions accepted;
4. planted complete-state period 2 measured as period 2;
5. dropped conversion half-pair caught;
6. planted negative value caught;
7. a branching origin distinguished Constant2 (`2`) from degree+1 (`3`);
8. same-tick Conversion/Transport collision proved snapshot holding;
9. competence 0 matched S1-004 transport contribution for contribution, while
   a planted competence-1 mismatch was rejected.
10. every outcome A–G and their sealed adjudication precedence were planted
    against the mechanical classifier before execution.

The sealed pair was constructed and inspected parameter by parameter. No
`TickRunner` received N0 or N1.

## Closed execution gate

`--execute` returns code `2` and states that execution is not authorised.
There is no execution driver or result writer in the S1-005 laboratory.

The compilation correction before calibration changed only the source name
`DivisorPolicies` to its actual historical name `Divisors`. No toy had run,
no sealed term changed, and no World Correction arose.
