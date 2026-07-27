# L-004 — Regularities

*Renamed at launch, by the founder, from "Patterns" — and the rename is the milestone's whole
doctrine: the laboratory does not detect patterns; it learns to describe regularities. "Alternation
detected" is already a concept (why two passes? why that shape?). The reader computes; the
laboratory notices; observations record; debts are born from observations — never from code.
The pattern comes after. Not before.*

## Definition of Done (the founder's, exact)

The reader can produce — as facts:

- ✅ **frequencies** — `Shelter -> Field: 4`
- ✅ **distributions** — `Entries: Field x12`
- ✅ **sequences** — the raw itinerary, verbatim, from the initial place
- ✅ **repetitions** — `Field -> Tree -> Field: 4` (declared convention: length 3, count ≥ 2)
- ✅ **durations** — ticks present per place; in-hand intervals with lengths

And **nothing else**. Forbidden and now test-enforced: *loop, habit, routine, exploration,
alternation, detected* — every one of them an interpretation.

## The fourth specimen (the founder's prediction, scored — 3 for 3)

Predicted at L-003's close: RD-L6 would find a fourth specimen before a single line of Patterns
code. It did — while rereading the v2 reader to prepare this milestone: **"shelter chest."** The
world contains no chest. Its ontology holds `Location = Shelter` — the same value that says a
player stands there. The chest lived in the design document ("une caisse, c'est tout") and in the
reader — never in the world. The specimen taxonomy now spans three kinds of leak: **intention**
("refused", "voluntary") · **invented property** ("superior") · **invented object** ("chest").
All four corrected loudly in the v3 reader; the vocabulary boundary is mechanical (tests).
Two firsts were also reworded on the same ground: "stored" → "first time out of hand at the
shelter"; "retrieved" → "taken in hand at the shelter again".

## The re-reading of Run-000002 (the milestone's proof — facts only)

```
Sequence: Shelter Field Tree Field Clearing Field Clearing Field Shelter Field Station Field
          Tree Field Shelter Field Tree Field Station Field Tree Field Shelter Field Station

Transition frequencies:   Field->Tree 4 · Shelter->Field 4 · Tree->Field 4 · Field->Shelter 3 ·
                          Field->Station 3 · Clearing->Field 2 · Field->Clearing 2 · Station->Field 2
Repeated length-3:        Field->Tree->Field 4 · Field->Shelter->Field 3 · Field->Clearing->Field 2 ·
                          Field->Station->Field 2 · (four more at 2)
Ticks present:            Shelter 142 · Tree 160 · Station 56 · Clearing 150 · Field 49
In hand:                  5 intervals, 504 of 557 ticks
                          (t=25..250, t=262..267, t=269..507, t=513..517, t=525..end)
```

The reader names no shape. Two factual couples are simply left on the table, for whoever writes
the next observation: the Field is **the most-entered place (12) and the least-inhabited (49
ticks)**; and the sword was in hand **504 of 557 ticks** — including the 5-tick and 4-tick
intervals during which it was the *other* sword's turn to lie on the ground.

## Era I, restated (the founder's arc)

L-001: the world has a memory. L-002: the world has a biography. L-003: the biography has depth.
**L-004: the depth has structure.** L-005: the laboratory learns to ask *"what data do we need
now?"* — and only then, the Traveller.

The era's true product, named at this milestone's launch: **a language** — one in which the
laboratory can speak of worlds without ever inventing a property the world does not possess. The
maturity criterion, on record: *at Era I's end, two different researchers must be able to read the
same trace, with the same reader, and hold exactly the same facts before proposing different
theories.*

## The tradition

> **What became observable that wasn't before?**

*The shape of a life without its name. Frequencies, sequences, dwellings, holdings — the raw
geometry of how someone moved through a small world, stated entirely in numbers the world itself
defines. The laboratory can now see structure it is still forbidden to baptise.*

> **What illusion did the laboratory lose?**

*The illusion that describing a shape requires naming it. "Field -> Tree -> Field: 4" says
everything "loop" would say — minus the theory smuggled inside the word.*

**Exited when:** Unity EditMode green on the extended suite.
