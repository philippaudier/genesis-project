# Campaign S1-005 — The Competence Boundary

Status: **SEALED — implementation authorised; execution NOT authorised.**

Gate 3 — founder approval, verbatim (2026-07-30):

> *« J’approuve la question, la paire N0/N1, les claims C0–C6, les issues A–G
> et les confiances de S1-005. J’autorise le sceau et l’implémentation, mais pas
> l’exécution. »*

Gate 7 — founder execution authorisation, verbatim (2026-07-30):

> *« J’autorise le push vers origin/main des commits S1-005 nécessaires à
> l’exécution et du record produit.*
>
> *J’autorise l’exécution de Campaign S1-005 : N0 et N1, huit ticks exactement
> comme scellés, avec enregistrement complet et classification selon les issues
> A–G. Je n’autorise pas encore la réduction. »*

The approved draft is commit `3eb992f`. This seal commit changes only the
status and deposits the approval above. Everything from “The resistance that
opens this draft” through the gates below is fixed before implementation.

This draft is downstream of the explicitly contaminated reconnaissance in
`Lab/RD11-Probe/`. Its question is earned by that probe; its specimens are new
and have never been run. The probe's parcel, values, trajectories, and surfaces
are unavailable to this campaign.

## The resistance that opens this draft

S1-004 established the first changed solid surface. The later RD-11 probe did
not extend that result: it deliberately revisited a known specimen and
therefore produced no evidence. It did expose a resistance.

With the existing Sediment transport rule, the complete material state of
S1-004/M1 entered a period-2 orbit. Its surface alternated forever between a
flat reading and a non-uniform reading. A changed surface was not yet a durable
structure.

An exploratory competence sweep then exposed a three-way boundary in that
contaminated parcel:

- transport at every positive prospective amount: material shuttling;
- an intermediate minimum: non-uniform material rest;
- an excessive minimum: flat rest because construction never began.

The campaign does not inherit those results. It inherits the question they
made unavoidable.

## Founding question — candidate

> **Can a minimum Sediment-transport competence turn conserved surface
> shuttling into a durable non-uniform material state after forcing ceases,
> without preventing the surface change that precedes it?**

`Durable` has a deliberately narrow campaign meaning: the **complete material
state**, excluding only its tick label, becomes a fixed point under a silent
future membrane. A repeated surface reading is insufficient.

## Reduction before design

No kernel primitive is missing.

The campaign reuses the S1-004 law family **unchanged** (sealed `b114a34`,
`Lab/S1-004/Fixtures.cs`); only the Sediment-transport fixture gains one scalar.
The three laws are reproduced here so gate 6 can read the sealed prose line by
line. Where this text and the cited fixture disagree, the fixture governs and
the discrepancy is a World Correction.

All three fixtures read **one snapshot** and contribute simultaneously. For
every place `Potential(P) = Base + Rock + Sediment + Water`, and each fixture
iterates the place's outgoing relations with a **constant divisor `2`**.
The selected policy is not degree-aware. However, because each place here has
exactly one outgoing relation, the corpus's degree-aware policy (`degree + 1`)
would also evaluate to `2`. The campaign parcel therefore cannot distinguish
the two policies by result; a foreign branching calibration toy must.

- **Water flow (unchanged).** For each outgoing edge `P→Q` with
  `diff = Potential(P) - Potential(Q) > 0`, move `floor(diff / 2)` Water `P→Q`
  when that amount is positive. The transfer is **uncapped** — not bounded by
  `Water(P)` — so Water may go negative: a measured phenomenon, not a fault.
- **Conversion (unchanged).** For each outgoing edge `P→Q`, recompute the same
  `floor(diff / 2)` from the same snapshot; when it is **strictly greater** than
  the conversion threshold `3`, `P` emits one whole pair `(-1 Rock, +1 Sediment)`.
  Each place here has exactly one outgoing edge, so a place emits **at most one
  pair per tick**. Conversion consumes no Water.
- **Sediment transport (the one changed fixture).** Let `carried = Sediment(P)`
  in the snapshot and `prospective = floor(diff / 2)`. The existing rule moves
  `min(carried, prospective)` whenever `prospective > 0`. The candidate
  distinction inserts one guard:

```text
move Sediment only when prospective > competence
amount = min(carried, prospective)
```

At `competence = 0` the guard `prospective > 0` **is** the existing rule (which
already required a positive transfer), so N0 is extensionally the sealed S1-004
transport. At `competence = 1` a prospective transfer of exactly one no longer
moves Sediment, although Water remains governed by its unchanged law. The guard
reads `prospective` — the amount before the `min` with `carried`.

The distinction adds no Kind, no mutable Elevation, no kernel state, no memory,
and no privileged observer object. It is a candidate fixture parameter, not a
Genesis law.

**Snapshot isolation is load-bearing.** At boundary 2 the same cell
`(A, Sediment)` is written by Conversion (`+1`) and read-then-decremented by
Sediment transport (`-1`), both from the one snapshot. Transport hands over
`min(Sediment(A) in the snapshot = 1, prospective)` — the unit that existed
**before** Conversion's emission, never the freshly-converted unit (which would
make the amount two). The sealed result depends on this; an implementation that
let one fixture read another's within-tick output computes a different amount
and fails gate 6.

## Blind Spot Audit — candidate

1. **The law family is contaminated; the specimens are not.** The competence
   distinction was chosen after the RD-11 sweep. This campaign may test it but
   cannot claim it arose blind. The new two-place parcel, values, crossings,
   prediction, and outcome bindings must be sealed before any code ticks it.
2. **Two places are not an extended landscape.** A positive result would
   establish a minimal durable structure, not spatial scaling, drainage,
   terrain, or a general mechanism of deposition.
3. **Rest is tested only after one finite forcing history.** The campaign does
   not test robustness under renewed crossings, noise, changing relations, or
   another law.
4. **A fixed point is stronger than a repeated reading but narrower than
   geological durability.** It means the same complete state returns under
   the same silent future, not that a named formation exists.
5. **Integer arithmetic is load-bearing.** The distinction occurs exactly at
   a prospective transfer of one. Another divisor or scale may move the
   boundary.
6. **The threshold might merely suppress transport.** The strict pair therefore
   requires both worlds to produce the same first changed surface before they
   diverge. A candidate that never constructs cannot score the intended
   outcome.
7. **The finite window could mistake a long transient for rest.** Fixed-point
   status is not inferred from visual quiet: two consecutive complete states
   must be cell-for-cell equal in all four Kinds, and the instrument must also
   show zero contributions.
8. **The candidate may duplicate an existing concept.** Conversion already has
   a threshold, but its causal role differs. No shared abstraction is promoted
   before a result forces one.
9. **Snapshot isolation is a conformance-critical invariant, not a background
   assumption.** The divergence turns on integer amounts of exactly one, and
   Conversion and Sediment transport touch `(A, Sediment)` in the same tick. The
   derivation is correct only if transport reads the snapshot's Sediment (one),
   not Conversion's within-tick emission (two). A calibration toy is added for
   exactly this; the S1-004 Rock+Sediment audit and the per-fixture provenance
   remain the two independent witnesses.
10. **The campaign topology cannot identify its divisor policy.** With one
    outgoing relation, both `Constant2` and the existing `degree + 1` policy
    return two. The sealed construction still requires the `Constant2` object,
    but result agreement cannot prove that choice. A foreign place with two
    outgoing relations must make the two policies disagree (`2` versus `3`)
    before execution.

## Objects on trial

- **S-S1-1**, supported representation hypothesis: the surface remains the
  reading `Base + Rock + Sediment`.
- **T-S1-1**, supported mechanism hypothesis: spatial transport, not local
  conversion alone, changes that reading.
- **C-S1-1 (candidate):** a transport competence can distinguish mobilised
  Sediment from Sediment that remains at rest, allowing a conserved changed
  surface to become a complete-state fixed point.

C-S1-1 does not yet exist in the Science-001 agenda and is not promoted by this
draft.

## Minimal strict pair — candidate

Two bidirectional two-place parcels `A ↔ B`, identical in every respect except
one scalar supplied to the same candidate Sediment-transport fixture:

- **N0 — competence 0:** every positive prospective transfer may move
  Sediment; this must conform extensionally to S1-004 transport.
- **N1 — competence 1:** a prospective transfer must be strictly greater than
  one to move Sediment.

Common construction:

- `Base = 0` at A and B;
- `Rock = 8` at A and B;
- `Sediment = 0`, `Water = 0`;
- `SolidSurface = Base + Rock + Sediment`;
- `Potential = SolidSurface + Water`;
- one relation each way, `A → B` and `B → A`;
- constant divisor `2` for Water and Sediment;
- conversion threshold `3`, strict: prospective Water flux must be `> 3`;
- `+8 Water` at A on boundaries 0 and 1;
- silent membrane from boundary 2 onward;
- eight ticks recorded.

The parcel is smaller than S1-004's because no within-world threshold control
is needed here. The strict pair itself is the control, and the decisive
prospective amount is one.

## Hand derivation — candidate, before implementation

Notation: arrays are `[A,B]`; `S = Base + Rock + Sediment`. Crossings enter
after the boundary's snapshot contributions, as in the established runner.

```text
initial        Rock [8,8]  Sediment [0,0]  Water [0,0]  S [8,8]

boundary 0     no contribution; +8 Water enters A
after tick 1   Rock [8,8]  Sediment [0,0]  Water [8,0]  S [8,8]

boundary 1     potentials [16,8]
               A→B prospective Water = floor(8/2) = 4
               4 > conversion threshold 3: A converts one pair
               no Sediment existed in the snapshot
               +8 Water enters A
after tick 2   Rock [7,8]  Sediment [1,0]  Water [12,4] S [8,8]

boundary 2     potentials [20,12]
               A→B prospective Water = 4
               A converts a second pair
               both N0 and N1 move the one snapshot Sediment A→B
after tick 3   Rock [6,8]  Sediment [1,1]  Water [8,8]  S [7,9]

boundary 3     potentials [15,17]
               B→A prospective Water = 1; no conversion
               N0: 1 > 0, so one Sediment returns B→A
               N1: 1 is not > 1, so Sediment does not move
after tick 4
  N0           Rock [6,8]  Sediment [2,0]  Water [9,7]  S [8,8]
  N1           Rock [6,8]  Sediment [1,1]  Water [9,7]  S [7,9]

boundary 4
  N0           potentials [17,15]: Water and one Sediment move A→B
  N1           potentials [16,16]: no fixture contributes
after tick 5
  N0           equals its complete state after tick 3: period 2 established
  N1           equals its complete state after tick 4: fixed point established
```

Under a silent future membrane, determinism binds the remaining states:

```text
N0: tick 3 = tick 5 = tick 7; tick 4 = tick 6 = tick 8
N1: tick 4 = tick 5 = tick 6 = tick 7 = tick 8
```

The two worlds therefore share their construction history through tick 3.
They diverge only when the prospective Sediment transfer becomes exactly one.

## Claims and predictions — candidate

| Claim | N0 prediction | N1 prediction | Bound failure |
|---|---|---|---|
| C0 — conservation and positivity | `Rock + Sediment = 16`; no negative value | same | Outcome G |
| C1 — construction precedes discrimination | first changed surface after tick 3, `[7,9]` | exactly the same | Outcome B if a correctly applied competence guard prevents N1 from constructing |
| C2 — the decisive boundary is one | boundary 3 moves one Sediment B→A | boundary 3 moves none | Outcome B or E if the guard alone discriminates at another boundary; Outcome F if another difference appears |
| C3 — shuttle control | full state tick 3 = 5 = 7; tick 4 = 6 = 8; the two phases differ | — | Outcome C |
| C4 — durable candidate | — | full state tick 4 = 5 = 6 = 7 = 8; zero contributions after boundary 3; surface `[7,9]` | Outcome D or E |
| C5 — rest is not a surface-only reading | complete state and surface both alternate | every Base, Rock, Sediment, and Water cell is identical across the fixed suffix | Outcome D, E, or G |
| C6 — threshold-0 conformance | contribution-for-contribution identical to the S1-004 transport rule on foreign calibration toys | not applicable | implementation cannot pass gate 6 |

### Rival imagination

The rival reading is that competence does not create durability; it merely
prevents motion. It predicts one of two useful defeats:

- N1 remains flat because its threshold suppresses the only constructive
  transfer; or
- N1 changes once but Water or conversion keeps the full state moving, so the
  apparent resting surface is not material rest.

The equal first trajectory through tick 3 is the rival's strongest protection:
N1 must first do the same constructive work as N0.

### Common ground

Both readings predict conservation, non-negativity, two conversions at A, one
initial Sediment transfer A→B, and the first changed surface `[7,9]` after tick
3. Disagreement begins only at the unit prospective transfer on boundary 3.

## Measurements — candidate

Every boundary and resulting state, both worlds:

1. every Base, Rock, Sediment, and Water cell;
2. `SolidSurface` and `Potential` per place;
3. Water and Sediment prospective amounts per directed edge;
4. actual Water and Sediment contributions per directed edge;
5. conversion pairs per place;
6. total and local `Rock + Sediment`;
7. minimum value of every Kind;
8. complete-state signature excluding tick;
9. surface-only signature, kept visibly distinct;
10. first repeated complete state and return distance;
11. contribution count after the proposed fixed point;
12. first boundary at which N0 and N1 differ.

Instrument calibration must plant:

- two states with equal surface but unequal hidden material values;
- a false fixed point whose surface repeats while Water changes;
- a true complete-state fixed point with zero contributions;
- a period-2 complete-state orbit;
- a half-pair conservation fault;
- a negative value;
- a branching origin with two outgoing relations, proving the selected
  constant divisor returns `2` where the existing degree-aware policy returns
  `3`;
- a single tick in which a conversion emission and a sediment transport touch
  the same `(place, Sediment)` cell, proving the transport amount is computed
  from the pre-conversion snapshot holding and never from the converted value;
- and a threshold-0 mismatch against the existing S1-004 transport fixture.

No calibration world may use N0 or N1.

## Cold review note — pre-seal

The first review strengthened the fixture prose and made snapshot isolation an
explicit calibration obligation. It also introduced one false statement: that
the corpus's degree-aware divisor would return `1` for a place with one
outgoing relation. The code defines it as `degree + 1`, so it returns `2`.

The hand derivation is unchanged because the campaign requires a constant
divisor of `2`. What changed is the epistemic claim: this two-place result
cannot itself prove which of the two policies was supplied. Blind spot 10 and
the foreign branching calibration above preserve that limitation before the
seal. This is a draft correction, not a World Correction; no world has run.

## Outcomes — candidate

- **A — discriminating support:** C0–C6 hold. Both worlds first construct
  `[7,9]`; N0 enters the derived period-2 material shuttle; N1 enters the
  derived non-uniform complete-state fixed point.
- **B — suppression, not durable construction:** N0 constructs while N1 does
  not, and the first difference is correctly attributable solely to the
  competence guard acting earlier than derived. Re-derive first. If the
  arithmetic was wrong, record a World Correction; if it was right, the
  mechanism is incomplete. C-S1-1 is not supported by this specimen.
- **C — the control does not shuttle as derived:** N0 fixes, follows another
  period, or otherwise fails its hand-derived orbit. Re-derive first; arithmetic
  error is a World Correction, correct derivation plus divergence means the
  mechanism is incomplete.
- **D — the candidate does not rest durably:** N1 changes surface but fails the
  complete-state fixed-point criterion, fixes flat, or only repeats its surface
  while hidden material continues. C-S1-1 is refuted or narrowed.
- **E — durable, but the mechanism is incomplete:** N1 reaches a non-uniform
  complete-state fixed point, but not at the derived tick and state; the first
  difference remains attributable solely to a correctly applied competence
  guard. Re-derive first; arithmetic error is a World Correction, while a
  correct derivation that diverges means the causal reading is incomplete.
- **F — causal comparison broken:** the worlds first differ through anything
  other than the competence guard, or that guard is applied to the wrong
  quantity or inequality. The strict pair was not realised; no scientific
  adjudication of C-S1-1.
- **G — invalid evidence:** conservation, positivity, witness completeness,
  threshold-0 conformance, determinism, or the instrument fails. No scientific
  adjudication.

Adjudication precedence is fixed: `G` (record invalid) → `F` (strict pair
broken) → `C` (control trajectory wrong) → `B` (construction suppressed) →
`D` (no durable non-uniform rest) → `E` (durable rest, wrong derivation) → `A`.
The first applicable outcome stands.

## Expectations — informal, sealed if this draft is approved

- The fixed-point witness will be harder to satisfy than the surface reader.
- Threshold-0 conformance will be the most important implementation guard.
- No new kernel primitive will be requested.

These expectations are non-scored except in the future Failed Expectations
section.

## Confidence declaration — candidate

- C0, C1, C2, C3, C4: **95/100**, conditional on an independent conformance
  gate after implementation. The arithmetic has been derived from the prose;
  the code does not exist.
- C5: **90/100**; the distinction is exact, but the instrument must prove it
  can reject surface-only false rest.
- C6: no confidence score before implementation; it is a gate, not a world
  prediction.
- C-S1-1 beyond this exact pair: **35/100**. A positive minimal result would
  justify another scale, not a general law.

## Gates — not yet opened

1. cold independent re-derivation against this text;
2. blind-spot and falsifier review;
3. founder approval of the question, pair, claims, outcomes, and confidence;
4. seal commit;
5. implementation and foreign-toy calibration;
6. line-by-line conformance against the sealed prose;
7. separate founder authorisation to execute;
8. immutable raw record and classification;
9. separate founder authorisation to reduce.

This draft authorises none of them.
