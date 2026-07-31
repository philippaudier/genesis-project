# S1-006 — Conformance gate

Status: **PASS — implementation and calibration only; execution remains
unauthorised.**

Compared line by line with the sealed campaign at `0cc83b0` and the independently
committed implementation at `c98f6a5`:

| Sealed term | Implementation witness |
|---|---|
| four-place bidirectional chain A↔B↔C↔D | `Worlds6.Build`, four places and six directed relations |
| P0 competence 0 / P1 competence 1; no other distinction | the single `competence` argument enters only `CompetenceTransportFixture` |
| Base 0, Rock 12, Sediment 0, Water 0 | initial cells in `Worlds6.Build` |
| SolidSurface = Base + Rock + Sediment | reused `K4.SolidSurface` |
| Potential = SolidSurface + Water | reused shared S1-004 `Potential` reader |
| divisor = outgoing degree + 1 in all three fixtures | the same `Divisors.DegreeAware` delegate is supplied three times |
| conversion only when prospective > 5 | reused `SurfaceConversionFixture`, threshold 5 |
| transport only when prospective > competence, before holding cap | reused sealed S1-005 `CompetenceTransportFixture` |
| +12 Water at A on boundaries 0, 1, 10, 11; silence after 11 | four immutable `ExternalEvent`s and no others |
| 128 ticks | `Worlds6.Ticks = 128` |
| complete-state fixed witness after boundary 11 and by boundary 127 | `Instrument6.FirstFixed`, adjacent full-state equality plus zero contributions and crossings |
| allocation sensitivity visible | `Instrument6.AllocationSensitive` |
| A–G precedence | `Instrument6.Classify`: G→F→E→D→B→C→A |

Foreign-toy calibration exercises every obligation in the sealed Measurements
section. P0 and P1 are constructed for static inspection only. No execution
path exists: `--execute` returns code 2 before calibration or world
construction, and no S1-006 source calls `TickRunner` with either parcel.

Gate 6 therefore establishes agreement between sealed prose and executable
construction/readers. It does not validate the unknown suffix, C3, or C4.
