# Transformation Model — Adversarial Review

Status: **Research — adversarial review.** No decision. No repairs. No new model concepts.

> This document reviews `Transformation-Model.md` for acceptance by attempting to break it. It
> does not defend the model and it does not fix what it finds. Each weakness is *classified*, not
> repaired. The question under review is not "how do we improve this?" but "does this abstraction
> deserve to exist?"
>
> Model under review: **every world change begins as an Intent; Intents pass through one door;
> Validation accepts/denies/merges/converts them against the world and against each other; accepted
> Intents produce Transformations (applied atomic facts); nothing writes to the World directly.**
>
> Classification vocabulary: *fatal contradiction*, *architectural weakness*, *implementation
> concern*, *acceptable trade-off*, *wording problem*.

---

## Method

Eleven scenarios are pushed at the model, chosen to span agent-driven change, physical fields,
emergent transitions, and the lifecycle operations (save/load, replay, and multiplayer as a thought
experiment only). After the scenarios, the model's four load-bearing assumptions are attacked
directly. A summary table and an acceptance verdict close the document.

The reviewer's bias is deliberately hostile.

---

## Scenario attacks

### 1. Fire propagation

**Situation.** A burning cell ignites adjacent flammable cells; fire advances across a grid.

**Model behaviour.** Each burning cell, per tick, submits an *ignite* Intent scoped to its
neighbours. Validation denies where the target is non-flammable or already burning, or where a
same-tick *extinguish* Intent (water) wins. Accepted intents produce ignition Transformations. With
snapshot semantics, fire advances exactly one ring per tick.

**Where it strains.** Fire is not an actor and does not *request* anything; the rule dictates the
spread deterministically. Wrapping a physical law in "Intent → Validation" is pure ceremony *unless
contention exists*. Where fire and water contend for a cell, the pipeline earns its keep; where fire
merely spreads into empty grass, the Intent stage is a costed no-op and the word "Intent" imports an
agency that isn't there.

**Verdict.** *Acceptable trade-off* on the mechanism (uniformity has a price and here it is small),
plus a *wording problem*: "Intent" misdescribes rule-driven physics. The pipeline is not *wrong*
for fire; the vocabulary is.

### 2. Fluid / heat / pressure diffusion

**Situation.** A conserved quantity spreads across cells toward equilibrium; every cell exchanges
with every neighbour simultaneously.

**Model behaviour.** Under the model, each cell submits Intents to move quantity to neighbours,
validated against the snapshot.

**Where it breaks.** This is a genuine hit. Two problems compound:

- **No natural grain.** An Intent per unit of flux is absurd (millions per tick); an Intent per
  *field* ("diffuse everything") is just a system update wearing an Intent costume — the abstraction
  adds nothing. There is no sensible middle grain. The Intent atom has no natural size for a
  continuous field.
- **Validation becomes a solver.** Diffusion is conservation-constrained: if three neighbours each
  read cell A at 10 and each pull 4, A gives away 12 it does not have. Enforcing conservation
  requires validation to see *all* intents touching A and scale them proportionally — i.e. to
  perform quantitative constraint-solving, not accept/deny/merge arbitration. The tidy diagram's
  "Validation" box silently becomes a numerical solver.

**Verdict.** *Architectural weakness* (fundamental): the model assumes discrete, arbitrable change
and has no honest representation of conserved continuous fields. The per-flux explosion is a mere
*implementation concern*, but the absence of a natural grain and the overloading of Validation are
structural.

### 3. Plant growth

**Situation.** A plant consumes soil nutrients, water, and light over time and grows.

**Model behaviour.** The plant submits a *grow* Intent (each tick, or as a scheduled event for long
dormancy), scoped to its soil/light neighbourhood; validation checks resource availability and
resolves contention with other plants drawing the same soil.

**Where it strains.** Very little. This is the model's home turf: a quasi-agent, a resource-gated
request, discrete contention. Long dormancy (growth measured in millions of units) is handled well
by the *scheduled event* trigger rather than a per-tick no-op.

**Verdict.** *Acceptable* — the model handles this cleanly. Recorded as a survival case, not a
weakness. (It is worth noticing that the scenarios the model handles best are precisely the
agent-like ones it was designed around — see the assumption review.)

### 4. Predator / prey (mutual interaction)

**Situation.** A wolf intends to eat a rabbit; the same tick, the rabbit intends to flee the cell.

**Model behaviour.** Both submit Intents against the start-of-tick snapshot. The wolf sees the
rabbit present; the rabbit sees itself fleeing. Validation must decide which wins.

**Where it strains.** The outcome depends on *which action resolves first* — but snapshot semantics
were sold as "everyone reads start state, so there is no intra-tick ordering." That is only half
true. Snapshot removes ordering of *reads*; it does **not** remove the need for a *decision order*
among conflicting intents. Validation must contain a deterministic priority rule (initiative,
speed, a stable key). Ordering was not eliminated — it was *relocated into Validation*.

**Verdict.** *Wording problem* bordering on *architectural weakness*: the model over-sold snapshot
simplicity. Contention itself is handled well (a strength), but the claim that snapshot "forbids
intra-tick sequencing" is false — sequencing reappears as conflict-priority inside Validation, and
the model must admit it.

### 5. Cooperative / threshold construction

**Situation.** A house requires 100 wood, delivered over many ticks by many villagers. When the
100th unit arrives, the house *becomes built*.

**Model behaviour.** Each *deliver wood* is an Intent → Transformation (house stock +1). Contention
for the same tile, or the same wood, resolves cleanly.

**Where it breaks.** The individual deliveries fit perfectly. The *completion* does not. When stock
crosses 100, the house transitions from "under construction" to "built" — a change **no actor
requested**. No villager submits "house becomes complete"; it is a consequence of aggregate state
crossing a boundary. Under a model where *every change begins as an Intent*, who knocks on the door?
"The world submits the intent to itself" dilutes Intent into meaninglessness (an always-granted
request from a null actor is just a Transformation).

**Verdict.** *Architectural weakness* (fundamental, and the sharpest of the review). It is the
concrete form of the deepest assumption failure: **not every change has an originating actor.**
Threshold/emergent transitions are changes the world *notices*, not changes anyone *requests*.

### 6. Resource contention

**Situation.** Multiple agents draw from a shared resource in one tick.

**Model behaviour.** Batch validation over intents; contention resolved before any Transformation
exists.

**Where it strains.** For *discrete* contention (one rabbit, one tile, one tool) this is the model's
showcase and it survives outright. For *continuous* contention (five agents drink from a well
holding 3 units) it collapses into scenario 2: validation must proportionally ration a conserved
quantity — a solver, not an arbiter.

**Verdict.** *Acceptable / strength* for discrete resources; *architectural weakness* for continuous
ones, same root as diffusion. The model's competence is a function of whether the contested quantity
is countable or continuous.

### 7. Weather

**Situation.** Regional pressure, wind, and precipitation fields evolve and interact.

**Model behaviour.** As fields, they inherit scenario 2 wholesale.

**Where it breaks.** Everything from diffusion, plus a locality tension: a storm system spans a large
region, so its *scope* is enormous. Under the Scoped-reads model, large overlapping scopes must be
*assumed to conflict*, so weather intents serialise against each other and against everything beneath
them — the parallelism the model advertised evaporates exactly where the field is biggest.

**Verdict.** *Architectural weakness*: fields-at-scale expose both the grain problem and a
scope/parallelism failure. Weather is diffusion that also breaks Locality's convenient assumptions.

### 8. World generation

**Situation.** The initial world is created — terrain, seeded entities — from nothing.

**Model behaviour.** There is no prior World to validate against and no other intents to contend
with. Worldgen either runs *outside* the one door (writing state directly) or is a single vacuous
Intent validated against emptiness.

**Where it breaks.** Either horn dents the model. Direct writing violates "nothing writes to the
World directly." Vacuous validation makes Intent an empty ceremony. The honest reading is that the
one-door principle governs the *evolution* of a world, not its *creation*.

**Verdict.** *Acceptable trade-off* — bootstrap exceptions are normal and defensible — but it must be
stated explicitly, because it is a real exception to an absolute-sounding rule. Left unstated it
would be an *architectural weakness*; stated, it is a carve-out.

### 9. Save / load

**Situation.** Serialise the world; restore it later.

**Model behaviour.** If Transformations produce new state values (Q4, value semantics), the World is
a value and saving is trivial. Pending *scheduled* intents/events are part of world state and
serialise with it.

**Where it strains.** Two constraints surface rather than break. First, the **only consistent save
point is a tick boundary** — a mid-pipeline save (after validation, before application) captures an
incoherent world. Second, Intents and scheduled events must be **pure data** to be serialisable; an
Intent carrying a closure or a behaviour reference cannot be saved. Both constraints are consistent
with the model (they reinforce Intent-as-data and the tick as the atomic unit).

**Verdict.** *Implementation concern*, and a benign one. Save/load does not threaten the model; it
*constrains* it in directions the model already leans.

### 10. Replay

**Situation.** Reproduce a run from an initial state and a recorded input timeline.

**Model behaviour.** Only *external* inputs (observer/player intents) need recording; internal
intents (AI, physics) regenerate deterministically. Replay re-runs ticks, re-injecting external
intents at their recorded ticks.

**Where it strains.** Batch validation resolves conflicts over a *set* of collected intents. If that
set is gathered from many sources (potentially in parallel), the conflict outcome must not depend on
collection or thread order. The model therefore silently requires a **deterministic total order over
intents** — every intent carrying a stable sort key (origin id + sequence). Absent that, Level-1
determinism breaks and replay diverges.

**Verdict.** *Implementation concern*, but a load-bearing one that must be made explicit: batch
validation is only deterministic if intents carry a deterministic ordering key independent of how
they were collected.

### 11. Multiplayer (thought experiment only)

**Situation.** Two clients share one world. *Not a design target — a stress test.*

**Model behaviour.** Because change travels as Intents (data) and the simulation is deterministic,
only intents need cross the wire; each client re-simulates identically. This is textbook
deterministic lockstep, and the model is unusually well-suited to it. Value semantics plus
tick-boundary saves also make rollback/prediction feasible.

**Where it strains.** It does not strain the model — it *cashes a cheque the model has not yet
funded*. Lockstep demands **Level-3 (cross-platform bit-identical) determinism**, which was
explicitly deferred to a future numeric-guarantees RFC. So the model's greatest apparent strength is
contingent on a guarantee it currently declines to make.

**Verdict.** *Acceptable / out of scope.* Not a weakness of the model; a demonstration that the
model is friendly to a hard future, and that pursuing that future would force the deferred numeric
decision.

---

## Assumption attacks

### Is Intent the right abstraction?

Partly. The scenario spread is unambiguous: Intent fits **agent-originated, discrete, contended**
change beautifully (plant growth, predator/prey, discrete resource grabs, construction *deliveries*)
and fits **continuous fields** (diffusion, weather, continuous pools) and **emergent transitions**
(construction *completion*) badly or not at all. Intent is the right abstraction for *one class of
change* and a forced costume on the others. *Architectural weakness* in the claim of universality —
not in the abstraction itself.

### Does every world change truly begin as an Intent?

**No.** Two classes falsify it:

- **Field evolution** — change with no discrete requester and no natural grain (§2, §7).
- **Threshold / emergent transition** — change with no originating actor, produced by aggregate
  state crossing a boundary (§5: house completes; water freezes; a population collapses below a
  viability floor).

To keep the claim, one must let "the world submits an Intent to itself," which empties the word. The
claim as written is over-strong. This is the review's central finding. *Architectural weakness*,
fundamental.

### Are constitutive and emergent consequences a valid distinction?

Valid but **incomplete**. The distinction (a consequence *bound* to its cause in the same commit vs
a *separable* consequence that re-enters as a new Intent) is real and necessary — §5's wood delivery
is separable, whereas "wood burns ⇒ heat exists" must be inseparable. But the scenarios reveal a
*third* category the pair does not cover: transitions triggered by **aggregate state**, which are
neither an actor's intent nor the bound consequence of any single transformation. The distinction is
sound as far as it goes and does not reach far enough. *Wording / architectural clarification.*

### Does the one-door principle survive every scenario?

**No — it survives the *evolution* of the world, not its edges.** Two documented breaches: world
generation (§8, creation precedes the door) and threshold transitions (§5, no actor to reach the
door). Both are defensible as explicit carve-outs; neither is defensible as silence under an
absolute rule. *Architectural weakness* if unstated; *acceptable trade-off* if the carve-outs are
declared.

### Does the model introduce hidden complexity somewhere else?

**Yes — and this is the most important structural finding.** Nearly every hard scenario discharges
its difficulty into one box: **Validation.** Validation is asked to be, variously, a discrete
arbiter (§4, §6-discrete), a quantitative conservation solver (§2, §6-continuous, §7), a
deterministic priority ordering (§4), a threshold detector (§5 — if it lives anywhere), and a
deterministic intent sorter (§10). The elegance at the top of the diagram — one door, one atom — is
purchased by a Validation stage that is not one mechanism but many, and the model currently describes
it as a single tidy box. *Architectural weakness.* The complexity was not removed by the model; it
was relocated and hidden.

---

## Verdict — does the model deserve to exist?

**It survives. There is no fatal contradiction.** Nothing in the scenarios makes the model logically
impossible or self-refuting. Its core motion — requests are separated from facts, facts are the only
things that touch the world, contention is resolved before the world changes — is coherent, and for
agent-originated discrete change it is genuinely excellent (§3, §4, §6-discrete), save/load-friendly
(§9), replay-friendly (§10), and network-friendly (§11).

**But it does not survive as a *universal* model, and its current statement over-claims.** Three
architectural weaknesses bound it:

- **W1 — No natural grain for continuous fields** (§2, §6-continuous, §7). Intent is either absurdly
  fine or degenerately coarse for conserved fields.
- **W2 — No originator for emergent/threshold change** (§5). "Every change begins as an Intent"
  fails for transitions the world *notices* rather than any actor *requests*.
- **W3 — Validation is overloaded and underspecified.** The one-door elegance hides a validation
  stage that is several distinct mechanisms.

Plus two lesser findings — the snapshot-ordering wording problem (§4) and the one-door bootstrap
carve-out (§8) — and one benign hard requirement — deterministic intent ordering (§10).

**The honest conclusion:** the model deserves to exist, but not as stated. What deserves acceptance
is the *narrower* claim — Intent is the abstraction for agent-originated, contended, discrete change
— together with an explicit admission that the world also contains rule-driven field change and
aggregate-triggered transition that this abstraction does not naturally own. Whether those enter the
same pipeline by a different grain, or are honestly a second mechanism, is **not resolved here** and
must not be resolved here. This review's job was to determine whether the abstraction earns its
place. It does — with its universality claim struck.

---

## Summary

| # | Scenario / assumption | Classification | Fundamental? |
|---|---|---|---|
| 1 | Fire propagation | acceptable trade-off + wording problem | no |
| 2 | Fluid/heat diffusion | **architectural weakness** (+ impl. concern) | yes |
| 3 | Plant growth | acceptable (survival case) | — |
| 4 | Predator/prey mutual | wording problem → architectural weakness | partly |
| 5 | Threshold construction | **architectural weakness** | yes |
| 6 | Resource contention | acceptable (discrete) / weakness (continuous) | mixed |
| 7 | Weather | **architectural weakness** | yes |
| 8 | World generation | acceptable trade-off (if carve-out stated) | edge |
| 9 | Save / load | implementation concern | no |
| 10 | Replay | implementation concern (load-bearing) | no |
| 11 | Multiplayer (thought exp.) | acceptable / out of scope | no |
| A | Intent as right abstraction | architectural weakness (universality) | yes |
| B | Every change begins as Intent | **architectural weakness** (central) | yes |
| C | Constitutive vs emergent | wording / incomplete | partly |
| D | One-door survives all | architectural weakness / carve-out | yes |
| E | Hidden complexity (Validation) | **architectural weakness** | yes |

**No fatal contradictions. Three fundamental architectural weaknesses (W1 fields, W2 emergent
transitions, W3 validation overload). Model survives with its universality claim removed.**
