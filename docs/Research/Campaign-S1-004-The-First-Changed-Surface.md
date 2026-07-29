# Campaign S1-004 — The First Changed Surface

Status: **EXECUTED (2026-07-29). Outcome A — first changed surface, exactly as derived.**
Report: `Lab/S1-004/Runs/REPORT.md`; Observation-015. C0–C5 all held; matter invariant; no
negative value. **One World Correction, in the driver and not the world:** the first execution
scored **E** because its C5 reader conflated three things; run 1 is kept whole
(`REPORT-run-1-outcome-E.md`) and, determinism obliging, run 2 reproduced its record byte for
byte — only the reading changed. **Adjudicated by the founder: Outcome A stands, with the instrumental World Correction kept**
(see the adjudication at the end of this document). **Decision withheld: the reduction is not
authorised.**
(Sealed `b114a34`; gate 7 CONFORMING `76ccaf1`; gate 8 `76b646f`; run 1 `dc36f8c`.)

Gate 4 — the founder's approval, verbatim:

> *« J'approuve la question, la paire M0/M1, la prédiction, C5, les issues A–F et la confiance
> de 95/100. J'autorise le sceau et l'implémentation, mais pas l'exécution. »*

Gate 7 — conformance: `Lab/S1-004/CONFORMANCE.md`, **CONFORMING — no World Correction**;
eleven sealed terms verified line by line, eight calibrations passed on foreign toys, no M0 or
M1 tick executed during the review.

Gate 8 — the founder's execution authorisation, verbatim:

> *« J'autorise l'exécution de Campaign S1-004 — The First Changed Surface : M0 et M1, six ticks
> exactement comme scellés, avec enregistrement complet et classification selon les issues A–F.
> Je n'autorise pas encore la réduction. »*

Everything below this line was fixed before the seal and is not modified afterwards. Gate 7
(conformance) and gate 8 (a second explicit authorisation) still stand between this file and
any result.

Revised 2026-07-29 after cold review (gate 3): the hand derivation was re-computed
independently and holds tick for tick; four changes followed — the cross-fixture collision at
boundary 2 became an explicit claim (C5), outcome **F** was added to close a gap the falsifier
had opened, a **conformance gate** was added between implementation and execution, and the
three-place choice was justified. A second instrument review corrected one overclaim: a bad
resolution would not be silent in the cross-kind total; the global audit and local collision
witness are independent and must both scream.

## The resistance that opens this Draft

Science-001 studies **the transformation of relief by the world's laws**. After S1-003 it has
a structured hydrological observation, but still no transformed relief.

The dormant W4 candidate does not merely lack a triggering run:

- `ConversionFixture` emits `−Rock, +Sediment`;
- `SedimentTransportFixture` moves Sediment;
- flow reads only `Elevation + Water`;
- no fixture writes Elevation;
- Rock and Sediment therefore never change the surface that water reads or Unity displays.

S1-001's sealed statement that Elevation was “written only by the erosion fixture” was false of
the implementation. No conversion event occurred, so this mismatch could not affect S1-001's
record; it becomes a correction before any successor relies on W4.

The current candidate can transform a matter register. It cannot yet transform relief.

## Founding question

> **What is the smallest conserved movement of matter that makes a world's solid surface
> different from its initial surface?**

The question is deliberately prior to *erosion*, *deposition*, *river*, and *landscape*. Those
are observer words that a changed surface may or may not earn later.

## Reduction before design

No kernel primitive is missing.

At one Place, existing Kinds can represent:

- `Base` — an immutable initial substrate;
- `Rock` — erodible solid matter;
- `Sediment` — mobile solid matter;
- `Water` — admitted and transported fluid.

The candidate readings are:

```text
SolidSurface(P) = Base(P) + Rock(P) + Sediment(P)
WaterPotential(P) = SolidSurface(P) + Water(P)
```

`SolidSurface` is a reading, not a Kind and not world state. No law knows a formation.

A local conversion:

```text
Rock(P) −= 1
Sediment(P) += 1
```

conserves both total matter and the local solid surface. Conversion alone therefore cannot
change relief. A later transport:

```text
Sediment(P) −= n
Sediment(Q) += n
```

conserves total matter while changing the two local solid-surface readings. The proposed
phenomenon reduces completely to existing cells, contributions, relations, resolvers, and
snapshot ticks.

## Objects on trial

- **D-G2:** additive resolvers preserve cross-kind accounting during conversion.
- **H1:** a paired `−Rock, +Sediment` emission remains one complete causal act.
- **S-S1-1 (candidate):** a relief need not be a writable quantity; it can be a derived reading
  of conserved matter.
- **T-S1-1 (candidate):** conversion changes matter's register; transport changes its spatial
  distribution; only the latter can change the solid-surface reading.

S-S1-1 and T-S1-1 are campaign hypotheses, not RFC decisions.

## Minimal strict pair — candidate

Two three-place bidirectional chains `A ↔ B ↔ C`, identical in initial state, relations,
crossings, flow, conversion threshold, divisors, and additive resolvers.

**Why three places and not two.** The phenomenon itself fits in `A ↔ B`. The third place earns
its cost by giving `B → C` a flux *below* the conversion threshold in the same tick in which
`A → B` is above it — **a within-world control showing that the threshold discriminates**
rather than converting wherever water moves. Without C, a conversion everywhere and a
conversion where the threshold is met would look identical.

- **M0 — conversion only:** Water flow may convert Rock to Sediment, but Sediment cannot move.
- **M1 — conversion + transport:** identical, with the candidate Sediment transport law active.

The pair differs by exactly one law fixture.

Common construction:

- `Base = 0` at A, B, C;
- `Rock = 10` at A, B, C;
- `Sediment = 0`, `Water = 0`;
- constant divisor 2 for Water flow and Sediment transport;
- conversion threshold 4, with the existing strict condition `prospective Water flux > 4`;
- point crossings of `+10 Water` at A on boundaries 0 and 1;
- silence on boundaries 2–5; total run: 6 ticks;
- flow potential reads `Base + Rock + Sediment + Water`;
- transport may move no more Sediment than the origin held in the tick snapshot.

The large crossing is an experimental intervention selected to enter the conversion domain,
not weather and not a claim about scale. Once admitted, Water has no privilege.

### Hand derivation — fixed before execution

State notation: `[A,B,C]`; `S = Base + Rock + Sediment`. External crossings are applied at the
same boundary after contributions from that boundary's snapshot, as established by the runner
and exercised in S1-002.

```text
initial        Water [ 0, 0, 0]   S [10,10,10]

boundary 0     no law fires; +10 Water enters A
after tick 1   Water [10, 0, 0]   S [10,10,10]

boundary 1     A→B Water flux = floor((20−10)/2) = 5
               5 > threshold 4, so A converts −1 Rock, +1 Sediment
               +10 Water enters A
after tick 2   Water [15, 5, 0]   S [10,10,10]
               A holds Rock 9, Sediment 1

boundary 2     A→B Water flux = floor((25−15)/2) = 5
               B→C Water flux = floor((15−10)/2) = 2
               A converts a second pair
               M1 only: the 1 Sediment present in the snapshot moves A→B
after tick 3   M0 S [10,10,10]
               M1 S [ 9,11,10]
```

Snapshot isolation is load-bearing and predicted: the Sediment emitted at boundary 1 cannot
move until boundary 2; the second emitted unit cannot move during the boundary that emits it.

**The collision at boundary 2 is the campaign's centre of gravity.** In M1, at that boundary,
one cell receives contributions from two different fixtures:

```text
ConversionFixture  → (A, Sediment, +1)
SedimentTransport  → (A, Sediment, −1)
```

Two contributions to one cell is a **conflict**: the additive resolver is invoked exactly once
and commits a delta of 0, leaving `Sediment(A) = 1` and `S(A) = 9`. **D-G2 and H1 do not merely
apply here — they meet in a single cell, for the first time in the project's history.** Were
additivity to fail, the derived reading would be wrong by exactly one unit. In this trajectory
the cross-kind `Rock + Sediment` audit should also fail by one; that global witness and the
local resolver witness are deliberately independent. The derivation depends on the collision;
so it is claimed (C5) and witnessed (measurement 13), not assumed.

Bound first events:

- first Water flux: boundary 1, A→B, amount 5;
- first conversion: boundary 1 at A, visible after tick 2;
- first Sediment transport in M1: boundary 2, A→B, amount 1;
- first cross-fixture conflict in M1: boundary 2, cell (A, Sediment), two contributions
  (+1, −1), resolver invoked once, committed delta 0;
- first changed SolidSurface in M1: after tick 3, A = 9 and B = 11;
- M0 SolidSurface: unchanged through the full run by derivation C3.

## Measurements — candidate

Every tick, both worlds:

1. full state for Base, Rock, Sediment, and Water;
2. full `SolidSurface` reading;
3. Water flux per directed edge;
4. Sediment flux per directed edge;
5. conversion pairs per place;
6. total `Rock + Sediment`;
7. local `Rock + Sediment` per place;
8. first conversion tick;
9. first Sediment transport tick;
10. first tick any `SolidSurface(P)` differs from its initial value;
11. first tick M0 and M1 solid surfaces differ;
12. minimum value of every Kind;
13. **contributions per cell per tick** — which cells received more than one, from which
    fixtures, the resolver invocation count and inputs, and the committed delta. Fixture
    provenance is collected by the laboratory from each fixture's transitions on the same
    snapshot; actual invocation is recorded by a counting additive resolver bound into the run.
    Neither requires a kernel change. C5 is scored only when both witnesses agree.

## Derived claims

- **C0 — Dry stasis:** before crossings, every contribution is zero.
- **C1 — Matter conservation:** total Rock + Sediment remains constant in both worlds.
- **C2 — Conversion locality:** each complete conversion pair leaves local Rock + Sediment
  unchanged at its place on the tick it is emitted.
- **C3 — M0 surface invariance:** without Sediment transport, every local SolidSurface remains
  equal to its initial value, regardless of how many conversions occur.
- **C4 — Base invariance:** Base never changes.
- **C5 — Cross-fixture additivity:** at boundary 2 in M1, cell (A, Sediment) receives two
  contributions from two fixtures (+1 conversion, −1 transport); the resolver is invoked exactly
  once and commits 0. This is where D-G2 and H1 are actually exercised rather than assumed;
  its failure would falsify the derived surface and the cross-kind audit by exactly one unit.

## Risky prediction

> **M1 will produce a non-zero change in SolidSurface at at least two places, while M0's
> SolidSurface remains unchanged everywhere.**

**Confidence: 95/100 — held only with gate 7 in place.** The derivation was re-computed
independently at cold review and holds tick for tick, so its arithmetic risk is near zero. The
residual risk is not in the numbers: it is that code which does not yet exist may not realise
the prose sealed here. Without the conformance gate, 95 would be pricing that code; with it,
the number stands. The campaign exists because four-kind composition, paired emissions,
snapshot isolation, a cross-fixture collision, and two simultaneous transports have never met
in one executed world.

Falsifier: M0 changes SolidSurface anywhere, or M1 never reaches the derived `[9,11,10]` reading
after tick 3 with the declared event witnesses intact.

No prediction about the final shape, later oscillation, or observer vocabulary is permitted.

## Outcomes — candidate binding

- **A — first changed surface:** the first Water flux and conversion occur at boundary 1 as
  derived; M0 remains surface-invariant; at boundary 2 M1 transports one Sediment A→B and its
  post-tick SolidSurface is `[9,11,10]`; all audits pass.
- **B — conversion without changed surface:** conversion occurs, but M1 never transports
  Sediment or never changes SolidSurface.
- **C — no conversion:** the candidate forcing never reaches the conversion domain.
- **D — attribution failure:** M0's SolidSurface changes.
- **E — accounting or instrument failure:** a paired emission is incomplete, total matter
  changes, reconstruction fails, or a witness cannot detect a planted corruption.
- **F — the surface changes, but not as derived:** M1 transports and its SolidSurface does
  change, at a different tick, place, or magnitude than `[9,11,10]` after tick 3. The
  phenomenon exists; the reading of the mechanism does not. Adjudication, bound now: re-derive
  by hand first — an arithmetic error is a **World Correction**; a *correct* derivation that
  still mismatches the record means the mechanism-reading is incomplete, which is evidence, not
  embarrassment. (Outcome C of S1-002, transposed. Without this letter the declared falsifier
  had no home.)

Later surface states are recorded but do not subdivide A. Persistence is not the first question
and may not be invented as a scored result after seeing the record.

## Instrument obligations before execution

On foreign toys only:

1. the SolidSurface reader equals Base + Rock + Sediment exactly;
2. a conversion pair changes neither local nor global SolidSurface;
3. a Sediment transfer changes source and target readings with opposite equal deltas;
4. the cross-kind ledger detects a dropped half-pair;
5. the transport reconstruction detects an incorrect edge amount;
6. identical toy records produce no cross-world surface difference;
7. a counting additive resolver receives a planted `(+1, −1)` conflict exactly once and records
   committed delta 0;
8. a planted provenance record that omits either fixture contribution disagrees with the
   resolver inputs and makes the collision witness fail.

## Kill criteria before seal

This Draft dies or is rewritten before seal if:

- the surface reading requires a kernel primitive;
- the strict pair differs in more than the Sediment transport fixture;
- M0 can change SolidSurface under the declared laws;
- M1's predicted surface change is caused directly by an external Rock or Sediment crossing;
- any law writes a landscape-scale object or formation;
- the first conversion and first transport cannot be independently witnessed;
- positivity is silently assumed rather than measured;
- the candidate law makes matter conservation depend on execution order;
- the exact minimal trajectory cannot be derived or bounded well enough to seal a losing
  outcome.

## Gates

1. Correct the W4 implementation description on the historical record without rewriting its
   sealed design.
2. Fill the exact parcel and hand derivation.
3. Cold-review the surface reduction, strict pair, outcomes, and instruments.
4. Founder approves the question, exact pair, predictions, outcomes, and confidence.
5. Seal.
6. Only then may implementation begin.
7. **Conformance.** Before any execution, the implemented fixtures are read against the prose
   sealed here, line by line: potential terms, threshold comparison, divisor, transport cap.
   Any divergence is a **World Correction** recorded before the run — never a silent
   adjustment, in either direction. *This gate exists because of an inversion worth naming: in
   S1-002 the hand derived from committed code; here it derives from prose, since the fixtures
   do not yet exist. The gate is what keeps Method-001 honest under that inversion.*
8. Execution requires a second explicit authorisation.

Until those gates are crossed, Genesis still has no law-earned changed relief.

---

# Post-execution — the founder's adjudication (2026-07-29)

> **Outcome A — first changed surface, exactly as derived.**
> **World Correction — the first classifier misread C5 and initially reported E.**

Reasoning of record, the founder's: C5's criterion existed **before the world** and designated
without ambiguity — boundary 2, `(A, Sediment)`, two fixtures, contributions `[+1,−1]`, one
resolver invocation, committed delta `0`. The first classifier did not measure that criterion
and obtain an unfavourable result; **it measured a different one** — every kind, several
boundaries, all resolvers aggregated. Its E is therefore not a refutation of C5 but a **scope
error in the transcription of the claim**.

The requalification is admissible because: the sealed text did not change; the derivation did
not change; the traces did not change; the first record already contained both required
witnesses; the correction follows entirely from C5, with no threshold adjusted and no exception
added; the second run produced no new evidence; and the false E remains readable at `dc36f8c`.

**The discriminating rule, stated by the founder and kept here:**

> *Had the initial reader applied C5 correctly and the two witnesses diverged, E would have had
> to stand.* That is not what happened.

(Whether this rule earns a place in `docs/Methods/` is a reduction-time question, not one this
campaign may settle for itself.)

Deposited by the founder on the phenomenon itself: *Genesis did not move an elevation. It moved
one unit of conserved matter — and the surface, which exists only as a reading, became other.*

**The reduction remains a separate act. The phenomenon is acquired.**
