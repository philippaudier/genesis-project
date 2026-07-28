# Science-001 — Geomorphology

> Founding charter. Status: **Founded 2026-07-28.**
>
> This document describes a domain, not an implementation. **No implementation is authorised by
> this charter.** The science describes the domain; RFCs describe the laws; milestones describe
> the realisations. This register (`docs/Sciences/`) is founded with exactly this file: a science
> earns its file the day it becomes inevitable, never before (DN-005: an empty registry is a
> vocabulary-late violation).

---

> **« Une géographie posée ne peut pas devenir autobiographique. »**
>
> *A placed geography cannot become autobiographical.*

---

## Object of study

**The transformation of relief by the world's laws.**

The geomorphology of Genesis does not aim to produce beautiful terrain. It seeks to understand
how a landscape becomes what it is. A river is not placed; it appears because water found a
basin, gravity gave it a direction, and repeated passage transformed the relief.

## The founding question

> **What is the smallest collection of laws capable of transforming initial matter into
> landscape?**

As always in this house: not a result — the minimal phenomenon.

## Why this science exists — the phenomenon that made it inevitable

Per the standing rule of the workflow, every decision opens with: *which phenomenon, made
inevitable by a world, does this make observable?*

The demand did not come from an engine. It came from the narratives. **P2 — Landscape Memory**
holds the era's master prediction: *attachment forms toward the stories places accumulate —
geography becomes autobiographical.* The corpus already carries its early evidence: "**MON**
Shelter" — a place, not an object (LB-Obs-005); the Field, "véritable nœud", named by the first
human narrative while absent from the biography (LB-Obs-002); 224/307 ticks dwelt at a Shelter
before a house was tidied for no one (LB-Obs-010).

But a **placed** geography cannot become autobiographical: a placed place has no history to
tell. For geography to accumulate stories, the geography itself must be the kind of thing that
has a past. That is what this science builds the conditions for.

This is the same movement that founded RFC-L001: a world making a Genesis law inevitable.

**Evidence antecedence (the LB-002 wall):** every piece of evidence cited above predates the
seal of LB-002 — Ordinary Days. Standing rule: **no evidence from LB-002's sealed traces may
ever justify a decision of Science-001.** Its evidence is either older than the seal, or
produced in its own laboratory.

## The central definition — a manifestation is never a placement

Genesis does not place a river, a valley, a delta. Genesis provides the conditions under which
they can appear.

```text
Placement:      CreateRiver(position)

Manifestation:  relief + water + gravity + erosion + time
                → a drainage network becomes possible
```

Placement remains legal in the laboratory as an **experimental intervention** — never as an
explanation of the world. A researcher may pour water on a mountain; that crossing is declared
through the membrane, recorded, replayable — and once inside, the poured water obeys exactly the
same laws as any other water. Interventions lose all privilege at the membrane (ADR-0005,
Invariant 7).

## What this science does not know

Science-001 knows: matter, cells, flux, time. It does not know trees, animals, biomes, roads,
villages, players, or civilisations. **It does not even know what a river is.** *River*,
*valley*, *watershed* are observer words — the laboratory's language, never the world's. If one
appears, it will be a word the laboratory earns later, after observation, per the house's
vocabulary-late rule. Names are paid for, never presupposed.

## Inheritance (what transfers intact, with its proofs)

Science-001 does not start from principles it must defend. It inherits theorems:

- **The membrane / No hidden hand** (Invariant 7, RFC-L001, ADR-0005): every external influence
  — rain included — is a declared, recorded, replayable crossing, interpreted only by laws.
- **Conservation** — a theorem, not a hope: it held in every observed run, *including deep
  inside divergence*, and survives Int64 wraparound (the theorem lives in ℤ/2⁶⁴ℤ — Obs-008).
  Erosion does not destroy; it moves. Nothing appears without provenance; nothing disappears
  without destination.
- **Cell-locality (D1)**: a cell acts only on its neighbours. Mountains do not know they are
  mountains.
- **Composition, commutativity, snapshot isolation** — the kernel's proven algebra (ADR-0001,
  04-KERNEL).
- **The Reduction Test and its tariffs** (ADR-0002/0003): a law costs inevitability; a primitive
  costs irreducibility. Applied to this domain: **no manifestation ever owns a law.** There will
  never be a `CreateRiver()` — only gravity, flow, transport, deposit, and whatever proves
  irreducible beyond them.
- **Seal-then-run**: every campaign of this science is pre-registered and sealed by commit
  before execution.

What the charter declares beyond inheritance is one domain commitment:

> **No law knows the landscape.** A law knows its immediate neighbourhood. The landscape is an
> emergence — it exists only in the observer's reading.

## The science enters with a past

Geomorphology does not begin on a blank page. Read through this domain's grid, months of the
corpus are proto-geomorphology:

- **Obs-001 (the slosh)** was a fluid on an abstract relief: evacuation, rebound, equalisation.
- **The push-downhill law family** — studied since the Age of Observation — *is* the flow-law
  candidate: quantity moves toward lower-potential neighbours, driven by height difference.
- **Obs-005 mapped its regimes**: damped (degree ≤ 2), periodic (degree 3), **divergent
  (degree ≥ 4, unbounded)**.
- **Obs-007 sharpened the warning**: stability is *spectral*, not local-degree — a max-degree-3
  tree still diverged. The relief's spectrum decides, not its neighbourhood counts.
- **The cure is already on file**: the instability is curable in law content — a degree-aware
  divisor yields λ = 0 (the Reduction Test journal, 2026-07-26).
- **Sourced worlds** (a constant inflow — which is what rain is) were the calmest family the
  laboratory ever observed (RD-4): a small blessing on record.
- **The 2⁶² semantic frontier** (Obs-008): long-running divergent worlds freeze meaninglessly
  near the wraparound guard. Erosion runs are long runs; the frontier is on this science's map.

## The first experiment is already predicted

A 2D grid has interior degree 4; a 3D grid, degree 6. Under the flow law as studied, **every
naively meshed terrain sits in the divergent regime — CT-003 predicts the first naive watershed
will explode before it has been built.**

This is the first time a Genesis theory makes a prediction in a domain it has never met. The
first campaign of Science-001 is therefore not "make water flow" — it is **the first field test
of CT-003**, and its claims will be sealed before execution, in the house form, roughly:

1. the naive grid diverges (spectral prediction);
2. the degree-aware divisor stabilises it (λ = 0);
3. between the two, the relief's spectrum — not its local degrees — decides (Obs-007's lesson).

Science-001 does not start by asking how to make water. It starts by asking whether the corpus
was right about what water would do.

The first manifestation the laboratory will then look for is **the watershed** — chosen because
it is an observer's word by construction: a description of shared hydrological fate that no cell
needs to know. Looking for a watershed respects "no law knows the landscape" for free.

## Open questions (catalogued so none can sneak in — the RFC-0004 precedent)

These are questions, not articles. Each is a separate future decision.

- **Q-G1 — The multi-quantity cell.** This domain wants several quantities per place
  (elevation, water, sediment, material, resistance…). The question is not the list: **how do
  several quantities coexist without breaking Composition, D1, Commutativity, and the
  Membrane?** This is the true first RFC of the science — before gravity, before erosion.
  Nothing can be transported until the laboratory knows what carrying is.
  *(Correction, 2026-07-28: this question's original wording claimed "today a cell carries
  essentially one quantity" — false. A cell has been the pair (Place, Kind) since RFC-0003;
  the scalar world of the campaigns was a usage, never a kernel limit. See RFC-G001, which
  found Q-G1 pre-answered and recorded the one genuinely new condition: conservation under
  conversion.)*
- **Q-G2 — Scale coupling.** Fire in seconds, regrowth in seasons, erosion in centuries. The
  hope is that resolution may change without the ontology changing. That is a claim of theorem
  calibre — **a hoped-for theorem, not a principle.** It stays open until a reduction or a proof
  earns it.
- **Q-G3 — Record/Replay.** The corpus held this capability back: *waits for the observation
  habit to demand it.* This science's protocol (return to a tick, branch, compare futures) is
  that demand — the waiting condition is met. By determinism, step-back is replay from the
  initial state; a branch is a fork of the trace; the original history is never rewritten. An
  RFC is now legitimate; it is still an RFC.

## The laboratory (order of construction)

**Headless first.** The Constitution's promise is literal here as everywhere: the first
watershed will be born in the harness, as Obs-001–008 were. Presentation comes later, to observe
what already exists. A tiny parcel, not a world: simple initial relief, declared water crossings,
the candidate laws, the timeline.

The laboratory speaks its own language, so the words keep reminding us what this is: **Seed**
(not "new world"), **Resume Observation** (not "load"), **Replay from Tick** (not "undo"),
**Preserve Branch** (not "save" — and not *Seal*: that word is reserved, and its jurisprudence
will not be diluted).

## Death conditions for the science itself

- If the smallest law collection that produces watersheds turns out to require a law that knows
  the landscape, the founding question dies as posed — recorded, not rescued; the charter's
  central commitment is refuted, and the science must be refounded on what was actually
  observed.
- If evolved geography proves indistinguishable, in narratives, from placed geography — if
  places accumulate autobiography without needing a causal past — then P2's demand was
  misread, this science loses its founding justification, and that finding is filed exactly
  like any other. The world decides; the charter does not.

---

*The world deserves evidence before design — and for once, it already has some. This science
does not begin; it continues. Even the sciences of Genesis have a history.*
