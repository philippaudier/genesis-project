# Campaign S1-006 — The Selected Form

Status: **SEALED — gate 7 passed; execution authorised, reduction NOT
authorised.**

Founder approval (2026-07-31), preserved verbatim:

> J’approuve la question, la paire P0/P1, les claims C0–C6, les issues A–G et
> les confiances de S1-006. J’autorise le sceau et l’implémentation, mais pas
> l’exécution.

The reviewed draft at `6beac64` is the sealed experimental contract. Everything
from the founding question through the confidence declaration is fixed. The
implementation may build P0 and P1 and inspect their construction, but no tick
of either world was authorised by the seal alone.

Execution authorisation (2026-07-31), preserved verbatim:

> J’autorise l’exécution de Campaign S1-006 : P0 et P1, 128 ticks exactement
> comme scellés, avec enregistrement complet et classification selon les issues
> A–G. Je n’autorise pas encore la réduction.

Cold review (2026-07-31): the independent re-derivation reproduced every cell
through boundary 6 and the sole first divergence after tick 7. The blind-spot
and falsifier review passed after the pre-seal revisions recorded below.

This draft is downstream of the explicitly contaminated reconnaissance in
`Lab/RD12-Probe/`. Its four-place specimen, values, forcing boundaries, and
predicted first divergence are new and have never been run.

## The resistance that opens this draft

S1-005 isolated a minimal competence boundary: under constant divisor 2,
competence 0 shuttled while competence 1 rested.

The RD-12 probe asked whether that explanation travelled. It did not travel
unchanged:

- under constant divisor 2, extended chains left the positive domain;
- under degree+1, every chain through length 9 reached a non-uniform
  complete-state fixed point, competence 0 included;
- competence still changed every final surface.

The probe therefore separated two questions that S1-005 could not:

1. **Does the law family remain in a stable domain?**
2. **Inside that domain, which spatial distribution remains?**

The existing degree-aware policy can make stability common ground. Competence
may then be tested for form selection rather than credited with rest itself.

## Founding question — candidate

> **When complete-state stability is common ground, can local
> Sediment-transport competence select a different final spatial form under
> repeated forcing?**

`Form` means only the ordered `SolidSurface` reading over the parcel. No
formation vocabulary is implied.

## Reduction before design

No kernel primitive and no production fixture are missing.

The strict pair reuses:

- S1-004's `SolidSurface = Base + Rock + Sediment`;
- its potential, Water flow, conversion, and capped Sediment transport;
- S1-005's competence guard;
- the corpus's existing degree-aware divisor `outgoing degree + 1`.

S1-005 is not a draft dependency: its competence guard was sealed at
`7b739b4` and the campaign closed at `537fc00`. S1-006 nevertheless states the
reused terms in line so its own future conformance gate does not depend on a
reader reconstructing another campaign:

```text
divisor(P) = outgoing_degree(P) + 1

if Potential(P) > Potential(Q):
    prospective = floor((Potential(P) - Potential(Q)) / divisor(P))

move Sediment P→Q only when prospective > competence
amount = min(snapshot Sediment(P) still available, prospective)
```

The competence comparison is applied to `prospective` **before** the cap by
snapshot holding. Water remains uncapped. Conversion uses the same prospective
amount and emits one `(-1 Rock,+1 Sediment)` pair when it is strictly greater
than the conversion threshold.

The campaign introduces no new law family. It composes laboratory content
already forced into existence and asks whether one local scalar changes the
global fixed reading.

## Blind Spot Audit — candidate

1. **The question is contaminated; the specimen is not.** The RD-12 probe
   revealed that degree-aware worlds can share stability while retaining
   competence-dependent forms. None of its lengths, values, forcing
   boundaries, or final surfaces may enter this campaign.
2. **Four places are extended only relative to S1-005.** A positive result is
   not a 2D terrain, basin, network, or scale law.
3. **Degree-aware is common law content, not the object on trial.** Both worlds
   receive the same policy. This campaign cannot say whether Water flow,
   conversion, Sediment transport, or their shared divisor contributes most
   to stability.
4. **Two forcing episodes are not climate.** They establish a repeated
   history, not robustness under arbitrary or continuous crossings.
5. **The final state may depend on integer scale.** Rock 12, crossing 12,
   conversion threshold 5, and competence 0/1 define one exact domain.
6. **A fixed surface is not a fixed world.** Stability requires a complete
   Base/Rock/Sediment/Water suffix with zero contributions.
7. **Capped transport may expose relation enumeration.** At a place with two
   downhill neighbours and insufficient Sediment for both, iteration order
   can select a receiver. Every such allocation-sensitive boundary must be
   reported. It limits generalisation even though relation order is identical
   in the strict pair. `OutgoingRelations` order is deterministic and the same
   in P0 and P1, so allocation order cannot be the source of their first
   difference; it can only amplify a difference already caused by the
   competence guard.
8. **The second forcing episode may erase the first difference.** That is an
   outcome, not a reason to preserve a preferred form.
9. **The observation window may be too short.** After the second episode, a
   fixed point is witnessed when two consecutive complete states are
   cell-for-cell identical, the intervening boundary has zero law
   contributions and zero crossings, and the future membrane is silent. Under
   determinism, that is a proof of the later trajectory. The
   instrument also reports the full quiet-suffix length, but does not require
   eight arbitrary ticks. If no such transition occurs by boundary 127, the
   run is unresolved within the window and Outcome D stands; a later fixation
   is not silently inferred.
10. **Competence may select hidden material but not surface.** Equal final
    surfaces with unequal complete states receive their own outcome.

## Objects on trial

- **C-S1-1**, supported only in S1-005's exact domain: transport competence can
  distinguish remobilised matter from resting matter.
- **S-S1-1:** a surface remains a derived reading over conserved matter.
- **T-S1-1:** conversion, transport, and remobilisation retain distinct causal
  roles.
- **F-S1-1 (candidate):** under common complete-state stability, a local
  transport competence can select which extended surface reading remains.

F-S1-1 does not exist in the Science-001 agenda and is not promoted by this
draft.

## Strict pair — candidate

Two bidirectional four-place chains:

```text
A ↔ B ↔ C ↔ D
```

They differ in exactly one scalar supplied to the same competence transport
fixture:

- **P0:** competence 0;
- **P1:** competence 1.

Common construction:

- `Base = 0`, `Rock = 12`, `Sediment = 0`, `Water = 0` at every place;
- `SolidSurface = Base + Rock + Sediment`;
- `Potential = SolidSurface + Water`;
- degree-aware divisor `outgoing degree + 1` for Water flow, conversion, and
  Sediment transport;
- strict conversion threshold `5`;
- `+12 Water` at A on boundaries 0 and 1;
- a second identical episode, `+12 Water` at A on boundaries 10 and 11;
- silence from boundary 12 onward;
- 128 ticks.

The second episode is fixed before the first difference's future is known. It
asks whether a selected distribution survives another history rather than
whether one isolated pulse can freeze.

## Hand derivation of the common prefix — candidate

Endpoint divisor: 2. Interior divisor: 3. Arrays are `[A,B,C,D]`.
Crossings enter after contributions from their boundary snapshot.

```text
initial
  Rock      [12,12,12,12]
  Sediment  [ 0, 0, 0, 0]
  Water     [ 0, 0, 0, 0]
  Surface   [12,12,12,12]

boundary 0
  no law contribution; +12 Water enters A
after tick 1
  Water     [12,0,0,0]
  Surface   [12,12,12,12]

boundary 1
  potentials [24,12,12,12]
  A→B prospective = 12/2 = 6
  6 > conversion threshold 5: A converts one pair
  no snapshot Sediment moves
  +12 Water enters A
after tick 2
  Rock      [11,12,12,12]
  Sediment  [ 1, 0, 0, 0]
  Water     [18, 6, 0, 0]
  Surface   [12,12,12,12]

boundary 2
  potentials [30,18,12,12]
  Water: A→B 6; B→C 2
  A converts a second pair
  snapshot Sediment: A→B 1 in both worlds
after tick 3
  Rock      [10,12,12,12]
  Sediment  [ 1, 1, 0, 0]
  Water     [12,10, 2, 0]
  Surface   [11,13,12,12]

boundary 3
  potentials [23,23,14,12]
  Water B→C 3
  Sediment B→C 1 in both worlds
after tick 4
  Sediment  [1,0,1,0]
  Water     [12,7,5,0]
  Surface   [11,12,13,12]

boundary 4
  potentials [23,19,18,12]
  Water A→B 2; C→D 2
  Sediment A→B 1; C→D 1 in both worlds
after tick 5
  Sediment  [0,1,0,1]
  Water     [10,9,3,2]
  Surface   [10,13,12,13]

boundary 5
  potentials [20,22,15,15]
  Water B→C 2
  Sediment B→C 1 in both worlds
after tick 6
  Sediment  [0,0,1,1]
  Water     [10,7,5,2]
  Surface   [10,12,13,13]

boundary 6
  potentials [20,19,18,15]
  Water C→D 1
  prospective Sediment C→D = 1
  P0: 1 > 0, move the snapshot unit C→D
  P1: 1 is not > 1, move nothing
after tick 7 — first predicted divergence
  Water both [10,7,4,3]
  P0 Sediment [0,0,0,2]  Surface [10,12,12,14]
  P1 Sediment [0,0,1,1]  Surface [10,12,13,13]
```

No complete trajectory after tick 7 is derived. The second forcing episode
acts on worlds already allowed to differ. This is deliberately beyond the
hand-computable prefix; the machine must reconstruct every contribution
without receiving a target final form.

## Methodological frontier — fixed before the seal

Gate 1 validates the common prefix, the integer arithmetic, and the first
competence-caused divergence. **It does not validate C3 or C4.** Common
stability and different final forms are genuine predictions carried at
72/100 and 65/100, not hand-derived confirmations.

This is a deliberate move beyond S1-004/005's fully derived trajectories and
against the agenda's standing adversary, comfort. Its cost is explicit:
conformance gate 6 can prove that the implemented laws, crossings, readers,
and reconstruction match the sealed prose throughout the suffix; it cannot
compare that suffix with an expected final trajectory that was never derived.
The outcomes, not a hidden calculation, adjudicate what happens after tick 7.

## Claims and predictions — candidate

| Claim | Prediction | Bound failure |
|---|---|---|
| C0 — common prefix | P0 and P1 match the hand derivation cell for cell through tick 6 | Outcome E or G |
| C1 — first discrimination | first complete-state difference after tick 7; only P0's unit Sediment contribution C→D differs at boundary 6 | Outcome E or F |
| C2 — accounting domain | `Rock + Sediment = 48`, zero reconstruction faults, no negative value | Outcome D or G |
| C3 — common stability | after boundary 11 and by boundary 127, both worlds exhibit two consecutive identical complete states with zero intervening law contributions and zero crossing | Outcome D |
| C4 — selected form | the two final `SolidSurface` arrays differ | Outcome B or C |
| C5 — attribution | worlds differ only in competence; all later differences reconstruct from the first guarded contribution and lawful subsequent state | Outcome F or G |
| C6 — repeated history | both second-episode crossings enter at boundaries 10 and 11 and receive no later privilege | Outcome G |

Primary prediction: **C3 and C4 hold together.** Stability is common; form is
not.

## Rival imagination

The strongest rival says degree-aware dynamics dominate the late state. It
grants the unit difference at boundary 6 but predicts that the second forcing
episode erases it:

- the final complete states become identical; or
- hidden material differs while `SolidSurface` becomes the same.

A second rival says stability was only a property of the contaminated probe:
one or both fresh worlds remain periodic, unresolved, or leave the positive
domain. Outcome D gives that rival full rights.

## Common ground

Both imaginations predict the exact shared prefix through tick 6, the unit
competence discrimination at boundary 6, conservation, declared crossings,
and the absence of any landscape-scale law.

## Measurements — candidate

Every boundary and state, both worlds:

1. all Base, Rock, Sediment, and Water cells;
2. `SolidSurface` and `Potential` per place;
3. prospective and actual Water/Sediment transfers per directed edge;
4. conversion pairs per place;
5. every contribution with fixture provenance;
6. local and global `Rock + Sediment`;
7. minimum value of every Kind;
8. complete-state and surface signatures;
9. first cross-world difference and its exact contributing cause;
10. first fixed-state transition and full zero-contribution suffix length;
11. final per-place surface and complete-state differences;
12. farthest place reached by any difference;
13. every allocation-sensitive transport boundary: more than one eligible
    outgoing edge with insufficient snapshot Sediment to satisfy all
    prospective transfers;
14. both forcing episodes as membrane events, separately witnessed.

Foreign-toy calibration must:

- distinguish surface equality from complete-state equality;
- reject a periodic state as fixed;
- accept a planted late fixed transition when its two complete states match
  and contributions are zero, while rejecting an equal surface with hidden
  state change;
- distinguish equal final surfaces / unequal hidden states from identical
  complete states;
- detect a wrong first-divergence boundary or fixture;
- detect a dropped contribution, matter fault, negative value, or crossing;
- expose a planted allocation-sensitive transition;
- distinguish degree-aware from Constant2 on a branching toy;
- and plant every outcome A–G plus adjudication precedence.

No calibration world may use P0 or P1.

## Outcomes — candidate

- **A — form selected:** both worlds meet the complete-state fixed-suffix
  criterion and their final `SolidSurface` arrays differ.
- **B — hidden selection only:** both worlds fix; final surfaces are equal but
  complete material states differ.
- **C — selection erased:** both worlds fix to the same complete material
  state.
- **D — stability is not common within the window:** either world has no
  witnessed fixed transition by boundary 127, remains periodic, or leaves the
  non-negative domain. A clean fixed transition anywhere in the window,
  including after tick 120, satisfies stability; no arbitrary eight-tick
  tariff is applied. F-S1-1 receives no adjudication.
- **E — mechanism incomplete:** the first difference is still caused solely by
  a correctly applied competence guard, but not at the hand-derived boundary,
  place, amount, or tick. Re-derive first; arithmetic error is a World
  Correction, otherwise the causal reading is incomplete.
- **F — strict pair broken:** the worlds first differ through anything other
  than the competence guard, or another implementation term differs.
- **G — invalid evidence:** accounting, provenance, crossings, determinism,
  calibration, or the instrument fails.

Adjudication precedence: `G → F → E → D → B → C → A`. Between B and C, B
applies only when complete states differ; C only when they are identical.

## Expectations — informal, sealed if approved

- The second forcing episode will alter both candidate forms rather than merely
  replay the first.
- At least one difference will propagate from the initially contested C–D
  edge back into A or B before rest.
- Allocation sensitivity will remain absent in the derived prefix but may
  appear after divergence.

These expectations are non-scored.

## Confidence declaration — candidate

- C0 and C1: **95/100**, conditional on post-implementation conformance. The
  common prefix has been derived from prose; no S1-006 implementation exists.
- C2: conservation **98/100**; positivity **70/100** because the specimen is
  fresh and coupled.
- C3, common stability: **72/100**. The contaminated degree-aware matrix
  motivates but cannot prove it.
- C4, different final surfaces: **65/100**. Every contaminated stable pair
  differed, but the second episode may erase this fresh difference.
- C5 and C6: **90/100**, conditional on calibrated provenance and membrane
  readers.
- F-S1-1 beyond this exact pair: **30/100**.

## Kill criteria before seal

Rewrite or kill this draft if:

- the strict pair differs in more than competence;
- the common prefix does not survive independent hand derivation;
- degree-aware is not supplied identically to all three fixtures;
- the second episode is chosen after seeing either future;
- the instrument cannot distinguish surface equality from complete-state
  equality;
- allocation sensitivity is left invisible;
- final-form equality has no bound outcome;
- or execution can occur before an independently versioned conformance gate.

## Cold review record

- **Gate 1 — PASS, within its declared reach.** Independent arithmetic found
  exactly two conversions (boundaries 1 and 2), `Rock + Sediment = 48`,
  Water total 24 after the first episode, no negative value through tick 7,
  and the sole first difference at boundary 6 / C→D / amount 1.
- **Gate 2 — PASS after revision.** No allocation-sensitive transition exists
  in boundaries 0–6. The review forced the self-contained law formulas, the
  explicit methodological frontier, the logical fixed-point witness, and the
  deterministic-order protection into this draft before any seal.

## Gates — 1 through 7 passed

1. **passed** — cold independent re-derivation of boundaries 0–6;
2. **passed** — blind-spot, allocation, and falsifier review;
3. **passed** — founder approval of question, pair, claims, outcomes, and
   confidence, verbatim above;
4. **passed** — sealed against reviewed draft `6beac64`;
5. **passed** — implementation `c98f6a5`; ten foreign-toy and static-pair
   obligations pass, with P0/P1 constructed but never ticked;
6. **passed** — `Lab/S1-006/CONFORMANCE.md`, line by line against seal
   `0cc83b0` and independently committed implementation `c98f6a5`;
7. **passed** — separate founder execution authorisation, verbatim above;
8. immutable record and mechanical classification;
9. separate founder reduction authorisation.

This seal and the separate gate-7 decision authorise execution and immutable
recording only. They do not authorise reduction.
