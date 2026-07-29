# RD-11 probe result — a changed surface that does not stay

Date: 2026-07-29  
Status: **exploratory, contaminated, non-evidential**

The already-known S1-004/M1 parcel was extended from its sealed six ticks to
500 ticks without changing its state, relations, crossings, or fixtures.

## Output

- first changed surface: tick 3, `[9,11,10]`;
- first repeated surface after change: tick 4 again at tick 6;
- first repeated full material state: tick 10 again at tick 12;
- full-state period: 2;
- surfaces on that material cycle:
  - tick 10: `[10,10,10]`;
  - tick 11: `[8,12,10]`;
- Rock + Sediment: `30 -> 30`;
- minimum value across Base, Rock, Sediment, and Water: `0`;
- at tick 500 the same surface alternation is still present.

The full signatures at the first material recurrence were:

```text
t10 [10,10,10] | 0,8,2,9, 0,10,0,4, 0,10,0,7
t11 [8,12,10]  | 0,8,0,7, 0,10,2,7, 0,10,0,6
t12 = t10 (tick number excluded)
```

The kernel is deterministic. Once the complete material state at tick 10
returned at tick 12 under the same silent future membrane, the parcel's later
period-2 orbit was fixed.

## The resistance exposed

S1-004 established that conserved transport can change a surface. This probe
shows where that result does not yet pay RD-11: in this parcel, change does not
accumulate into a durable structure. The transported matter keeps shuttling,
and the apparently flat surface is one phase of the orbit rather than a final
rest.

This is not a result of Science-001. It is a reason to design the next question.
The next campaign must not ask merely whether a surface changes again. It must
discriminate **a lasting non-uniform material structure after forcing has
ceased** from:

- a repeated reading whose hidden material state still changes;
- a periodic material orbit;
- a flat fixed point;
- and an observation window too short to decide among them.

No law, hypothesis, campaign, or Observation is promoted by this probe.

## Exploratory competence sweep

The resistance suggested one minimal distinction: the existing transport moves
Sediment for every positive prospective transfer. An exploratory fixture
therefore required the prospective transfer to be **strictly greater** than a
competence threshold. Nothing else changed. Threshold `0` reproduces the
existing rule.

```text
threshold  recurrence       cycle surface(s)           classification
0          t10 -> t12 (p2)  [10,10,10] / [8,12,10]    material orbit
1          t6  -> t7  (p1)  [8,11,11]                 non-uniform fixed point
2          t5  -> t6  (p1)  [9,10,11]                 non-uniform fixed point
3          t7  -> t8  (p1)  [9,11,10]                 non-uniform fixed point
4          t7  -> t8  (p1)  [9,11,10]                 non-uniform fixed point
5          t7  -> t8  (p1)  [10,10,10]                flat fixed point
6          t7  -> t8  (p1)  [10,10,10]                flat fixed point
```

All seven variants conserved `Rock + Sediment = 30` and kept every value
non-negative.

This sweep exposes a qualitative three-way boundary in the contaminated
parcel:

1. no competence distinction: matter keeps shuttling;
2. an intermediate distinction: matter moves, then rests in a non-uniform
   complete state;
3. an excessive distinction: no durable relief is constructed.

That boundary earns a candidate question; it does not answer it. The values,
parcel, and resulting surfaces are now contaminated and unavailable to a
future campaign.
