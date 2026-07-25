# Transformation Model — Exploration

Status: **Research — exploration only.** No decision. No implementation. No numbering.

> This is not an RFC. It is a precursor investigation opened because RFC-0001 (Tick System)
> revealed a more fundamental question beneath itself: *what is a transformation?* The Tick RFC
> repeatedly assumes an answer — that a transformation is enacted by a cause, applied at a tick,
> ordered against its peers — without ever defining the thing being ordered. This document
> explores that thing adversarially. It presents competing positions and their tensions and
> deliberately refuses to choose. Its only conclusion, if any, is whether the Tick genuinely
> *depends* on this model.
>
> Second pass (2026-07-24): the exploration surfaced a second concept, **Intent**, sitting
> *before* Transformation. Intent has been woven through the document below. It is a
> strengthened thesis under examination, not a decision.

---

## Why this exists

The Constitution names Transformation as more fundamental than State (Article V: "Everything is a
process. Transformation matters more than state") but never defines it. Every downstream system —
the Tick, the Scheduler, conflict resolution, causality, persistence — will inherit whatever
definition we give it, implicitly or explicitly. Getting it wrong quietly is expensive; getting it
*implicit* is worse, because Article XII forbids hidden state and an undefined atom is the largest
hidden assumption of all.

This document probes the atom before anything is built on it. It is written to be *argued with*.

## The central thesis under examination

The strongest candidate framing to emerge so far, in two parts:

> **(1) There is only one way to transform the world. There are many ways to schedule the
> evaluation of a change.**
>
> **(2) What triggers submit is not a Transformation but an *Intent*. A Transformation is what an
> accepted Intent produces — the applied, atomic fact.**

Under this thesis, a per-tick process, a scheduled event, an AI decision, and a player action are
not four kinds of change. They are four kinds of *trigger*, all of which submit **Intents** through
a single door. Nothing writes to the world directly.

```
   per-tick process ─┐
   scheduled event ──┤
   AI decision ──────┼──►  Intent  ──►  Validation  ──►  accepted Intent  ──►  Transformation  ──►  World
   player action ────┘                     │
                                           └──►  denied / merged / deferred / converted
```

Two altitudes, deliberately kept distinct:

- **Intent** — a *request* for change, in the vocabulary of the actor ("Move North", "Eat Food",
  "Ignite Tree"). It has not touched the world. It can be refused, modified, merged, delayed,
  replaced, or converted, all without consequence.
- **Transformation** — the *applied atomic fact*, in the vocabulary of world state
  ("position: x→y"). By the time it exists, the world has already changed. It is never a proposal.

Validation is the translation from the first vocabulary to the second. This split earns its keep
precisely because the two are *not the same shape*: if Intent and Transformation had identical form,
"proposed" and "applied" would be adjectives on one noun and the second concept would be waste. They
do not, so it is not.

If this holds, it buys one door, one atomicity rule, one conflict rule, one traceability path, one
validation path — and it makes **Transformation the fundamental verb of Genesis**: the World is a
noun, the Tick a rhythm, the Simulation a process, but the Transformation is the elementary act that
carries the world from one state to the next. That last claim is not new to this document; it is
Article V restated. The Constitution is not to be edited to match it — the correct outcome is for
these explorations to reveal that Article V contained it already.

This document does not assume the thesis holds. Each section below is partly a test of it.

---

## Question 1 — What *is* a transformation? (partially resolved by Intent)

The document previously hesitated among three definitions. The Intent layer reorganises them rather
than choosing arbitrarily:

- **(A) A pure function** `State × Inputs → State` and **(C) a described intent (data)** describe
  the *same* stance — change as value, not act. Under the Intent model, **(C) is the Intent**: the
  inspectable, pre-application request. Favoured by Determinism (IV) and Explicitness (XII).
- **(B) An in-place mutation** describes the *Transformation's application* — the moment the
  accepted change lands in the world.

So Q1 partially resolves into two concepts at two altitudes rather than one contested definition.
What remains genuinely open is Question 4: *how* the Transformation lands (mutating the world, or
producing a new world value). The "is it data or act?" tension dissolves; the "mutate or produce?"
tension does not.

## Question 2 — What does an intent (and its transformation) read?

An intent's validation, and the transformation it produces, must read part of the world (its
**causes**) and possibly **external inputs**. *Three* positions, not two:

- **Declared read-set.** The exact cells/components read are named up front. Strongest guarantees:
  deterministic evaluation, safe parallelism of disjoint reads, precise causality (the read-set *is*
  the cause). Cost: read-sets become large and tedious at scale, straining Article XIV.
- **Scoped read.** A declared *region* rather than an enumerated set — "the neighbourhood around
  entity X." Respects Locality (Article VII) and Explicitness (XII) while staying compact enough to
  survive thousands of entities. Honest caveat: a scope is a *weaker* parallelism guarantee than a
  read-set — you know the region but not the exact cells, so two scoped transformations over
  overlapping regions must be *assumed* to conflict. It trades parallelism precision for authoring
  sanity and bounded reads.
- **Ambient reads.** Read anything at evaluation time. Simplest to author; catastrophic for the
  above. An undeclared read is a hidden dependency (forbidden by Article XII) and makes a cause
  unknowable without execution.

**Left open**, but the field has narrowed: Scoped is the likely default, Declared the option where
precision is worth its cost, Ambient the position to justify against rather than reach for.

## Question 3 — What does an accepted intent produce?

Candidates, not mutually exclusive: a **new state / delta**; a set of **proposed writes**; or
**further change** (a cascade — fire → "wood burns" → "heat rises"…).

The Intent layer reframes the cascade sharply. A consequence can re-enter as either:

- **a new Intent** — it passes through validation again, and (naturally) lands next tick. This
  bounds per-tick work and makes a tick a *quantum of causal depth*: one tick per causal hop. Clean.
- **a further Transformation** — inseparable from its cause, applied in the same commit, bypassing
  re-validation.

The re-entry-as-Intent model is elegant but suspicious for being *too* uniform: some consequences
must be inseparable from their cause. You cannot have "wood burns" accepted while its "heat exists"
consequence is independently denied. So the model likely needs *both* paths — re-entrant Intents for
separable consequences, bound Transformations for inseparable ones — and the criterion dividing them
is itself an open question. **Left open**, and flagged as a primary reason to suspect the Tick
depends on this document.

## Question 4 — Does a transformation mutate state, or produce new state?

The deepest surviving fork; it governs everything after it. (Q1's "data or act" collapsed into
Intent-vs-Transformation, but *this* remains.)

- **Mutation-in-place.** The applied transformation alters the live world. Cheap in memory; but it
  creates read-after-write hazards within a tick, makes atomicity hard (a half-applied tick is a
  corrupt world), and complicates replay and snapshotting.
- **Produce-new-state (value semantics).** Each tick reads a stable prior world and produces the
  next; the old world is never mutated. Atomicity is free (the next world exists whole or not at
  all), determinism is natural (everyone reads one snapshot), a saved world is just a value. Cost:
  duplication — mitigable by structural sharing, which this document must not prejudge.

Constitutional pull is toward value semantics (IV, XII, and the atomicity Causality implicitly
needs). Counter-pressure is practical cost and Article XIV. **Left open**, but note it is now tightly
coupled to Question 6: value semantics makes the batch/snapshot model below almost free.

## Question 5 — The life of an Intent: proposition, validation, application

Reframed. These are the stages of an **Intent**, not a Transformation — because an applied
Transformation is already a fact and has no "proposal" stage to speak of.

1. **Proposition.** A trigger submits an Intent. The single door; the pipeline thesis lives here.
2. **Validation.** The Intent is checked against the world's rules *and against the other Intents
   proposed this tick*. It may be accepted, denied, modified, merged, deferred, or converted.
3. **Application.** Accepted Intents produce Transformations, committed atomically in a defined order.

Open questions the Intent lifecycle raises:

- **Cardinality (new).** An accepted Intent may yield **zero** Transformations (a denied or fully
  merged-away request), **one**, or **many** ("Eat Food" → food decreases, energy rises, a scent is
  emitted). The Intent→Transformation relation is therefore not 1:1, and the model must say what it
  is.
- **Trace of the denied (Causality).** Does a *rejected* Intent leave a causal record? It never
  touched the world, but "Wolf B tried and lost" may be exactly the kind of thing history wants.
- **Legitimate bypasses.** Does anything enter *not* as an Intent? Scrutinise two: external input
  injection (does a player action arrive as an Intent, or by a side door?) and initial world
  construction (is genesis-of-the-world an Intent, or a privileged act outside the pipeline?). If
  either must bypass, "only one door" needs qualification.

**Left open.** The lifecycle is attractive because it unifies every trigger; it is suspicious for
the same reason — unifying frames often hide a special case.

## Question 6 — Atomicity and simultaneity (now pressured by Intent)

When many intents are proposed in one tick, do their transformations see each other?

- **Snapshot / simultaneous.** All intents are validated against, and all transformations read, the
  *same* start-of-tick state; writes land in the next state. Deterministic, parallelisable, a clean
  tick boundary. Forbids intra-tick sequencing and forces same-tick conflicts into an explicit rule.
- **Sequential.** Transformations apply in a defined order, each seeing prior writes. More
  expressive; but every outcome depends on a total, explicit, stable order, or determinism dies. And
  it is inherently serial.

**The Intent layer applies pressure here, and this must be stated openly.** Conflict resolution over
intents (Question 7) requires *collecting all intents proposed this tick before applying any* — you
cannot know Wolf A and Wolf B contend for the rabbit until you can see both. But "collect all, then
resolve, then apply" *is* the snapshot/simultaneous model. So adopting Intent-based conflict
resolution is not free of consequence for Q6; it leans the fork toward snapshot. This is flagged as a
hidden coupling, not decided.

Also open: the **unit of atomicity** — one transformation, one phase, one whole tick? Snapshot makes
the *tick* atomic; sequential makes each *transformation* atomic and the tick their sum.

## Question 7 — Conflict

Two intents this tick lead to writes on the same piece of the world. Resolution must be explicit
(XII) and deterministic (IV); the Constitution names no rule. The Intent layer improves the framing:
conflict is resolved *between Intents, before any Transformation exists* — so the world is never
touched by a losing party.

> Two wolves intend to eat the same rabbit. Both submit Intents. Validation sees both, applies a
> deterministic rule, accepts Wolf A, denies Wolf B. **One** Transformation is produced. The world
> never briefly held two claims on one rabbit.

This is cleaner than resolving conflicts *after* transformations exist — but it depends entirely on
batch validation (Q6) and on a deterministic rule. Candidate rules: last-writer by defined order;
accumulate/merge (where a state has a meaningful merge); reject the conflicting set; a dedicated
arbitration phase. The right answer is probably **per-state-kind**, not global — itself a design
decision with Constitutional weight (it must stay explicit and uniform enough to remain
comprehensible). **Left open.**

## Question 8 — Does the Tick actually depend on this model?

The fork that opened this document. The Tick RFC cannot answer, on its own:

- **Q3** — does a consequence re-enter as an Intent (next tick) or bind as a Transformation (same
  commit)? Is a tick a quantum of time or of causal depth?
- **Q6** — do a tick's transformations share one snapshot or apply sequentially? This *defines*
  "simultaneous within a tick", which the Tick RFC assumes.
- **Q5** — at what point does application happen relative to proposition and validation?

Each is a property the Tick RFC leaned on and left unstated. **Tentative finding (not a decision),
strengthened this pass:** the dependency appears real. With Intent in view, the Tick is plainly *the
clock that decides when accepted Intents may be committed as Transformations* — the scheduling and
ordering of an atom this document is still defining. If a full reading confirms this, the Tick RFC
should declare a dependency on a Transformation model rather than silently embedding one.

Offered for challenge, not adoption.

---

## Open questions, collected

- Q1 — *(partially resolved)* Intent = change-as-data; Transformation = applied fact. Remaining
  tension moved to Q4.
- Q2 — Declared, Scoped, or Ambient reads? (Scoped the likely default.)
- Q3 — Do consequences re-enter as new Intents (next tick) or bind as Transformations (same commit)?
  What criterion divides separable from inseparable consequences?
- Q4 — Does an applied transformation mutate the world, or produce a new world value?
- Q5 — Is the Intent→Transformation relation 0/1/many? Do denied Intents leave a causal trace? Are
  there legitimate pipeline bypasses (input injection, world construction)?
- Q6 — Snapshot or sequential within a tick? (Intent-based conflict pressures this toward snapshot.)
- Q7 — How are same-tick conflicts resolved, and is the rule global or per-state-kind?
- Q8 — Does the Tick System depend on the answers to Q3/Q5/Q6?

## What this document is not

It is not a decision, not an RFC, not numbered, and contains no implementation. It does not close
RFC-0001, and it does not touch the Constitution — Article V already carries the intuition it
explores. When (and if) this matures, three paths remain open, none forced now:

1. promote this into an RFC and renumber the Tick behind it;
2. keep the Tick first and fold in only a *minimal* definition of Transformation (and Intent);
3. create a subordinate Transformation RFC without renumbering.

The choice among them should follow the evidence, not precede it.
