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

