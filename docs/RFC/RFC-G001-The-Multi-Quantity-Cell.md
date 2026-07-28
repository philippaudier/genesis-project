# RFC-G001 — The Multi-Quantity Cell

Status: **Draft — awaiting founder review**
Origin: Science-001 charter, Q-G1. Writing authorised by the founder 2026-07-28 — *"not because
geomorphology needs it, but because the kernel has met a limit the corpus itself revealed."*
Series note: `G` = science-demanded (as `L` = world-demanded). Filed under G because the demand
came from Science-001; its object is the kernel.

> Standing question of the workflow: **which phenomenon, made inevitable by a world, does this
> RFC make observable?**
>
> **Conversion** — matter changing register. A world used with one quantity can *move* substance;
> it cannot *transform* it. Erosion is not movement: it is rock becoming sediment. No manifestation
> of Science-001 exists without at least one conversion.

---

## The question

Not *"how do we store five values?"* but, per the founder's framing:

> **What does it mean to carry a quantity?** Do quantities live side by side, or does each
> possess its own dynamics, its own laws, its own algebraic guarantees?

## The Reduction Test (mandatory before any primitive — ADR-0002/0003)

Attempted: express a multi-quantity world — elevation, water, sediment, material, resistance —
using only the primitives in HEAD.

**The reduction succeeds. Completely.**

The investigation revealed that the question was answered on 2026-07-26, by **RFC-0003 —
State Representation**, before any science asked it:

- A **Cell is already the pair (Place, Kind)** — `State : Place × Kind → Value`. Five quantities
  at one place are five cells at one place. No new type is needed; the type exists.
- **D1** — the cell is the unit of writing and conflict: *two contributions to the same place but
  different kinds never collide.* Coexistence without collision is not achievable; it is the law.
- **D2** — resolvers attach to Kinds: *"a kind means a shared causal role; its conflict semantics
  is part of that role."* This is, verbatim, the answer to the founder's deep question: **each
  quantity already owns its algebra.** Water may resolve additively while another kind resolves
  otherwise — per-kind, by construction.
- **D3/D4** — relations connect Places; kind visibility is granted by the observing transition.
  A flow law reads Elevation and Water at discovered neighbour places because it declares those
  grants — exactly the coupling geomorphology needs, already scoped, already lawful.
- **The membrane is untouched** — external contributions already target cells = (place, kind).
  Rain is a declared crossing into Water cells; nothing new crosses.
- **Composition, commutativity, order-independence, snapshot isolation** hold componentwise: the
  runner groups by cell and commits each cell from the snapshot plus its own resolved delta;
  cells are disjoint. The single-kind world is the special case — which is why every theorem the
  physics campaigns proved was proven *at cell granularity all along*.

And the reduction is not merely possible — **it is in production**: every Lootbound world since
L-002 is a multi-kind world (Location conventions, wear, repairs — many kinds per place, resolved
per kind, replayed exactly). The "second-generation kernel" has been running for two days short
of never having existed.

RFC-0003's own closing sentence, quoted in full because it predicted this RFC:

> *"Accepted, this RFC does not introduce heterogeneous state. It merely removes the last
> architectural obstacle to expressing it."*

The drawer was pre-labelled. Q-G1 arrived at a slot the corpus had already cut for it.

## Correction record (declared, per house tradition)

The Science-001 charter stated: *"Today a cell carries essentially one quantity."* **False.**
A cell has carried (Place, Kind) since RFC-0003; the scalar world of the campaigns was a *usage*,
never a kernel limit. The reviewer's kernel-model was behind the code; the code corrected the
reviewer. The charter is amended with a dated note pointing here. The world is the only source
of truth — including about itself.

## The one new finding — conversion under conflict

The scalar corpus never exercised the case this RFC exists for. A **conversion** is a paired
emission by one law:

```text
ErosionLaw at place P:   Contribution((P, rock),     −n)
                         Contribution((P, sediment), +n)
```

The kernel commits cells independently (D1). Under conflict, each side of the pair resolves by
*its own kind's* resolver (D2). Therefore:

> **The pairing — and with it cross-kind accounting — survives conflict if and only if the
> resolvers of both kinds are additive** (resolve(amounts) = Σ amounts). Under additive
> resolvers, each kind's total change equals the sum of all emitted deltas, so laws that emit
> zero-sum *across* kinds conserve globally, conversions included. A non-additive resolver may
> commit one side of a pair and not the other: substance created or destroyed at the resolver.

This is the conservation theorem of the corpus meeting its first genuinely new condition — not
broken, *transferred*: conservation-with-conversion is a property of resolver algebra plus law
content, never of the kernel. It is stated here as a **theorem candidate with a test
obligation**: it must be demonstrated in the harness before any Science-001 campaign relies on
a conversion.

## Decisions

- **D-G1 — No new primitive. No kernel change is authorised by this RFC.** Multi-quantity worlds
  are D1–D6 usage: several Kinds per Place, declared as world content at construction, exactly
  as any world fact. Kind identifiers remain opaque to the kernel — **the kernel will never
  contain a type named Water.** This is the type-level face of the charter's commitment: no law
  knows the landscape, and no kernel type knows a substance.
- **D-G2 — Conversion is law content**: one law, paired contributions across kinds at one place.
  The additivity condition above is recorded as its conservation criterion; worlds that want
  accountable conversions must attach additive resolvers to the convertible kinds — a
  world-content obligation, enforced by the conservation audit, invisible to the kernel.
- **D-G3 — The corpus transfers per kind.** CT-003's stability map (regimes, spectra, the
  degree-aware cure) applies to each (kind, law-family) independently. **Coupling through laws
  is new physics with no corpus behind it** — the laboratory should expect coupled-dynamics
  surprises and will open the debt when an observation produces it, not before.

## Deliberately not decided (catalogued so nothing sneaks in)

- **Per-kind legality** (may Water be negative where Elevation may not?) — the constraint layer:
  RFC-0004's territory, Q1–Q6 all still open. Nothing here advances them.
- **Numeric heterogeneity** — all values remain `long`; the numeric-guarantees RFC stays
  separate (RFC-0003 Non-Goal, unchanged).
- **Kind lifecycle** — kinds stay closed, explicit, few. Note for scale: geomorphology multiplies
  **places**, not kinds; five kinds over ten thousand places honours the Non-Goal.
- **Commit-path performance** — `WithValue` copies state per committed cell; a grid parcel will
  feel this. By D6 the layout is an implementation detail: optimising it is semantics-free
  engineering for the first laboratory milestone, not RFC matter.
- **Measurement instruments** — per-kind ledgers and the conversion audit are laboratory
  tooling; the first campaign will demand them (instrument before world, as always).

## Kill criteria

- Exhibit a multi-quantity phenomenon that cannot be expressed as (place, kind) cells + per-kind
  resolvers + paired law emissions without breaking a proven theorem — then the reduction dies,
  and a true kernel generalisation becomes legitimate.
- Exhibit a conversion that satisfies the additivity condition and still fails the conservation
  audit — then D-G2's criterion is wrong and must be re-derived.

## Consequences for Science-001

The first campaign requires **zero kernel work**. Its sealed slots, when authorised, are:

1. the spectral predictions of the charter (naive grid diverges; degree-aware divisor cures;
   the relief's spectrum decides);
2. the conversion-conservation demonstration (D-G2's test obligation);
3. and the standing surprise slot: coupled dynamics the per-kind corpus cannot predict.

## Decision Record

Decision: **Draft — awaiting founder review.**
Proposed rationale for acceptance, if the review sustains it: not because the design is good,
but because the demonstration could not be avoided — the primitive already exists, the theorems
already cover it, and the only honest act left was to record the one condition (additivity under
conversion) that no scalar world could ever have revealed.
