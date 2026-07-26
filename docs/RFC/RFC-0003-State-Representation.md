# RFC-0003 — State Representation

Status: **Accepted** — 2026-07-26. D1–D6 ratified on review; the finding (the candidates are curried
projections of one semantic object) stood unrefuted.

## Purpose

This RFC decides how the world addresses **multiple natures of state belonging to the same place
without confusing them**. It is the single decision standing between the frozen kernel and the next
milestone.

This document is deliberately **domain-blind**. No world concept appears in it: natures of state are
opaque identities, exactly as Genesis-008 made locations opaque identities. The motivating fact — the
next milestone requires two values of *distinct causal roles* to coexist at one address — is recorded
in the Journal (2026-07-26); its vocabulary is deliberately kept out of this RFC, so that no future
word can influence a kernel representation.

A prediction was recorded before this RFC was written: *that the two candidates would turn out to be
two projections of the same deeper representation.* This RFC treats that prediction as a hypothesis to
test, not a conclusion to reach.

## Context

Genesis-008 separated **identity** from **kind** and, having only one kind of value ("counter"), made
kind implicit: `CounterAddress` sufficed to designate one value. Every kernel surface inherited that
assumption — `Contribution` targets an address; `ReadScope` declares addresses; resolvers register per
address; views map addresses to values; state equality ranges over address/value pairs.

The moment a second nature of state exists at the same address, the assumption breaks: **an address
alone no longer designates a unique value.** Genesis-008's recorded limitation ("not generic
heterogeneous state") and DN-001's per-*kind* resolver language both anticipated this moment.

## Problem Statement

Define the addressing model under which:

- one *place* may hold several values of different *natures*;
- reads, writes, conflicts, resolvers, views, scopes, and equality are all unambiguous;
- the kernel's proven guarantees (snapshot semantics, order-independence, scoped observation,
  deterministic conflict resolution) survive unchanged;
- no more machinery is introduced than the empirical fact requires.

## Terminology

- **Place** — the stable identity of a location (the role `CounterAddress` has played since 008).
- **Kind** — the identity of a *nature* of value. Opaque, closed, explicitly declared per world; a
  Kind carries a causal role, not a value type (all values remain integers; the numeric-representation
  RFC stays separate — two integer values do not have homogeneous roles).
- **Cell** — one value-holding location: the pair (Place, Kind).
- **Value** — the integer held by a cell.

## Candidates

### Candidate A — Place → StructuredState

Each place maps to a structured value holding one field per kind:

```
A ├─ Kind₁ → v₁
  └─ Kind₂ → v₂
```

*Mechanics.* State: `Place → (Kind → Value)`. Reads naturally per place; per-kind access goes through
the structure. Contributions must still name (place, kind) to be unambiguous — a contribution to a
whole structure has no defined conflict semantics.

*Strengths:* place-centric reading is primary; adding a kind is a structure change, not an addressing
change; everything at a place is co-located.
*Weaknesses:* kind is second-class (reached *through* a place); per-kind views, per-kind resolvers,
and homogeneous iteration over one kind cut against the grain; scope precision requires field-level
declarations anyway.

### Candidate B — (Place, Kind) → Value

The state is a flat map over composite keys:

```
(A, Kind₁) → v₁
(A, Kind₂) → v₂
(B, Kind₁) → v₃
```

*Mechanics.* State: `(Place × Kind) → Value`. Every kernel surface re-keys from Place to Cell.

*Strengths:* kind is an explicit dimension; per-kind resolvers, homogeneous views, and cell-grained
scopes are primary; the 008 pattern (opaque identity, order-independent equality over key/value pairs)
generalises directly.
*Weaknesses:* richer addressing everywhere; more logical identities; "everything at this place" is a
derived query, not a lookup; conceptually heavier at first contact.

## Finding — the candidates are projections

The comparison collapses under a standard observation: **A and B are the curried and uncurried forms
of the same function.**

```
A :  Place → (Kind → Value)
B :  (Place × Kind) → Value
```

These are canonically isomorphic. Any world expressible in one is expressible in the other with
identical observable behaviour: the same cells, the same values, the same equality (a set of
(place, kind, value) triples in both cases), the same conflicts, the same views. Neither candidate
adds or removes an expressible semantics; they differ only in which lookup is primary. The recorded
prediction is confirmed — not as intuition, but as currying.

**Consequence:** "A versus B" is not a semantic decision. It is a *storage and API-ergonomics*
decision, and it is not even permanent — an implementation may change layout without changing any
observable behaviour. The semantic decisions are elsewhere, and the projections merely made them
visible from two angles. They are the actual subject of this RFC:

## The real decisions

- **D1 — The Cell is the unit of writing and conflict.** A `Contribution` targets a cell. Conflicts
  group by cell. Two contributions to the same place but different kinds do not conflict.
- **D2 — Resolvers attach to Kinds, not cells.** A kind *means* a shared causal role; its conflict
  semantics is part of that role and is uniform wherever the kind occurs. (This realises DN-001's
  per-kind language literally, and replaces 008's per-address demo registration.) A conflict on a cell
  resolves by the resolver of its kind; a kind without a resolver rejects, as always.
- **D3 — Relations connect Places, not cells.** Topology is about location, not nature. A relation
  `A → B` asserts a connection between places; which kinds a transition reads across it is the
  transition's declaration, not the relation's property. **The relation discovers places only; kind
  visibility is granted entirely by the observing transition.**
- **D4 — Scopes declare at cell precision.** `ReadScope` declares cells. Relational observation
  declares origin *places* plus the kinds the transition may read at discovered targets. Nothing is
  observable at a coarser grain than it is declared.
- **D5 — Place existence is derived.** A place exists iff at least one cell is declared at it. No
  separate place registry; relation validation ranges over the places the cells define.
- **D6 — Storage layout is an implementation detail.** Curried, uncurried, or otherwise — invisible
  to semantics, free to change, never load-bearing.

## Comparison by concern (under the finding)

Each kernel concern has a *natural unit*; the projections differ only in which units they make
primary. No row decides A against B — which is the point.

| Concern | Natural unit | Note |
|---|---|---|
| Locality (Art. VII) | Place | neighbourhoods are places; D3 |
| Conflict & resolvers | Cell / Kind | D1, D2 |
| Read scoping | Cell | D4 |
| Relational views | Place origins + kind grants | D4 |
| Equality / determinism | Triple set | identical in both projections |
| Serialization (future) | Triple set | layout-independent by D6 |
| Write scoping (parked) | Cell | the asymmetry, when addressed, will be cell-grained |
| Constraint layer (future) | Kind or Cell | per-kind invariants mirror per-kind resolvers |
| Rules-as-data (Reserved) | Kind | rule content will reference kinds, not layouts |
| Pattern recognition (DN-003) | Predicate over cells | unaffected by layout |

## Kill criteria

- **The finding is falsifiable:** if any *observable* difference between the two projections is
  exhibited — a world expressible in one and not the other, or differing behaviour under identical
  cells — the isomorphism claim dies and A-versus-B becomes a real semantic fork to be decided the
  hard way.
- If cell-grained scoping (D4) proves too heavy in practice for the smallest witness, the grain — not
  the model — is revisited.

## Non-Goals

No generic component system; no dynamic type registry; no open-ended kind sets (kinds are closed,
explicit, few); no value-type heterogeneity (numeric RFC separate); no address lifecycle; no domain
vocabulary of any sort. The next milestone's needs are two kinds at three places — the model must
carry exactly that, and merely not obstruct more.

## Consequences

Adopting D1–D6: `Contribution`, `ReadScope`, view interfaces, and resolver registration re-key from
Place to Cell/Kind; `RelationSet` and `RelationScope` stay place-keyed (unchanged); state equality
generalises to triples (the 008 commutative-aggregation pattern, one dimension wider). The kernel's
proofs re-run under the new keys; no guarantee is weakened, and every existing test's *meaning* is
preserved under the single-kind special case (a one-kind world must behave exactly as today's).

## Decision Record

Decision: **Accepted — D1–D6 ratified**
Date: 2026-07-26
Rationale: The comparison was carried out and dissolved: the candidates are curried projections of
one semantic object, `State : Place × Kind → Value`. The decision was therefore not A-versus-B but
the semantic commitments D1–D6, ratified on review with no surviving counter-example against the
decisions themselves. The storage layout remains free (D6).

> **Accepted, this RFC does not introduce heterogeneous state. It merely removes the last
> architectural obstacle to expressing it.**
