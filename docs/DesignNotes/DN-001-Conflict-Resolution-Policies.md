# DN-001 — Conflict Resolution Policies

Status: **Design Note — analysis only.** Lists the candidate policies and determines which respect
Genesis's invariants. It does **not** choose one. Genesis-006 chooses a single policy and proves it.

## Purpose

Genesis-005 proved that *independent* transitions — those writing disjoint state — compose
deterministically and order-independently. Genesis-006 introduces the case Genesis-005 excluded: two
or more transitions writing **the same** state in the same tick. ADR-0001 named a deterministic
conflict rule as *required* but left the rule itself open. This note enumerates the rules and asks a
single question of each:

> **Which policies satisfy the new invariant — "for any conflicting writes, the model always
> produces exactly one deterministic result" — without breaking an invariant Genesis already holds?**

## Definitions

- **Conflict.** Two or more transitions, in the same tick, write the same piece of state.
- **Conflict policy.** The rule that turns several competing writes into exactly one committed value.

The invariants a policy must respect, in priority order (Genesis's stated values):

1. **Determinism** (Art. IV) — the same conflicting writes always yield the same committed value.
2. **Order-independence** — *this is the decisive one.* Genesis-005 established that reordering
   transitions must not change the result, and Genesis-006's own test suite includes
   `Enumeration_Order_Does_Not_Change_Conflict_Result`. So the conflict result must be independent of
   the order the transitions were enumerated in.
3. **Comprehensibility** (Art. XIV), **Emergence** (Art. VIII), **Extensibility** — quality criteria
   used to separate the policies that survive (1) and (2).

## The distinction the whole note turns on

There are two different properties often both called "deterministic":

- **Deterministic given a fixed order** — the same inputs *in the same order* give the same result.
- **Order-independent** — the result is the same *regardless of order*.

Genesis requires the **stronger** one. A policy can be perfectly deterministic-given-order and still
fail Genesis, because reordering the transitions would change the answer. This single distinction
disqualifies two of the five candidates below.

Order-independence has a precise consequence: **an order-independent conflict policy must be a
commutative operation over the competing writes.** Anything non-commutative (like "the last one
enumerated wins") depends on order and cannot satisfy invariant (2).

## Candidate policies

### A — Last-Writer-Wins (by enumeration)

The last transition to write, in enumeration order, sets the value.

- Deterministic given order: **yes**. Order-independent: **no** — reorder the transitions and a
  different write is "last".
- **Disqualified.** It fails invariant (2) and would fail `Enumeration_Order_Does_Not_Change_Conflict_Result`.
  (Marking it simply "deterministic" hides that it is order-*dependent*.) A "last-writer" variant where
  "last" is decided by an *intrinsic* key rather than enumeration is not this policy — it is a Custom
  Resolver (E).

### B — First-Writer-Wins (by enumeration)

The first transition to write sets the value; later writes are ignored.

- The mirror of A. Order-independent: **no**. **Disqualified** for the same reason.

### C — Reject

Two writes to the same state is an error; the conflict is refused (and recorded), not resolved.

- Order-independent: **yes** — a conflict is an error regardless of order.
- Comprehensibility: **very high** — "conflicts are illegal" is the simplest possible rule.
- Emergence: **poor** — a world where interaction *halts* rather than *resolves* cannot let two
  wolves contend for one rabbit; conflict becomes a bug to author around, not a phenomenon to model.
- Expressiveness: works for any state kind (it resolves none of them).
- **Survives the invariants.** Safe and simple, but treats conflict as failure — a strong stance
  against emergence.

### D — Accumulate (commutative reduction)

Competing writes are combined by a commutative reduction: sum, min, max, logical or/and, …

- Order-independent: **yes**, *provided the reduction is commutative* (sum/min/max are; "subtract" is
  not).
- Comprehensibility: **moderate** — one must know *which* reduction applies, and it only reads
  naturally for quantities.
- Emergence: **good** — additive contributions combine, which is exactly right for `damage`, `force`,
  `heat`, `momentum`; many small effects sum into a large one.
- Expressiveness: **partial** — there is no meaningful sum of `name`, `owner`, or `species`. It fits
  reducible state and is meaningless for categorical state.
- **Survives the invariants for reducible state only.**

### E — Custom Resolver (per state kind)

Each kind of state carries its own resolver: `health` sums damage, `position` resolves by some rule,
`owner` by another. Priority/arbitration schemes are a sub-case (the resolver picks by an intrinsic
priority, with a commutative tie-break).

- Order-independent: **yes, if and only if every resolver is commutative** (an intrinsic-priority
  winner with a commutative tie-break qualifies; "last enumerated" does not).
- Comprehensibility: **lower** — many resolvers, each its own small rule.
- Emergence: **highest** — every kind of state resolves contention in a way that means something,
  which is where believable interaction lives.
- Expressiveness: **highest** — additive kinds accumulate, categorical kinds arbitrate, each by its
  own rule.
- **Survives the invariants**, conditional on each resolver being commutative. Prior analysis (the
  research arc and ADR-0001) already leaned toward *per-state-kind* resolution — recorded here as
  input, not a decision.

*Noted sub-variants:* seeded-random selection is deterministic but order-independent only if the key
is intrinsic, and it is opaque (poor comprehensibility); min/max/sum are all instances of D. None
changes the analysis.

## Evaluation

The decisive column is **order-independence**, not raw determinism.

| Policy | Deterministic (fixed order) | Order-independent | Comprehensible | Emergence | All state kinds |
|---|---|---|---|---|---|
| A Last-Writer (enum) | ✅ | ❌ | ✅ | ⚠️ | ✅ |
| B First-Writer (enum) | ✅ | ❌ | ✅ | ⚠️ | ✅ |
| C Reject | ✅ | ✅ | ✅✅ | ❌ | ✅ |
| D Accumulate (commutative) | ✅ | ✅ | ⚠️ | ✅ | ❌ (reducible only) |
| E Custom Resolver (commutative) | ✅ | ✅* | ⚠️ | ✅✅ | ✅✅ |

\* if and only if every resolver is commutative.

## Conclusion (no policy is chosen)

- **The policies that respect Genesis's invariants are exactly the order-independent (commutative)
  ones: C (Reject), D (commutative Accumulate), and E (per-kind Custom Resolver, each resolver
  commutative).**
- **A and B (Last/First-Writer by enumeration) are disqualified** — deterministic, yet order-*dependent*,
  which contradicts the order-independence Genesis committed to in Genesis-005 and re-asserts in
  Genesis-006's own test. This refines the intuition that "Last-Writer is deterministic ✅": it is,
  but not in the sense Genesis requires.
- Among the survivors there is a clean trade-off along Genesis's own priorities: **Reject** is the
  simplest and safest but is *anti-emergence* (it halts interaction); **Accumulate** is
  emergence-friendly but only for reducible quantities; **Custom Resolver** is the most expressive and
  most emergence-friendly but the most complex, and is where prior analysis leaned. "Prefer the
  simpler" points at Reject; "serve emergence and all state kinds" points at Custom. That tension is
  Genesis-006's to resolve — this note only maps it.

## A prerequisite Genesis-006 must face first

Before *any* policy can be applied, conflicts must be **detected**, and detection requires the runner
to know *which state each transition wrote*. The current model — `Apply(snapshot, next)` returning a
whole next-state — hides that: a transition hands back a full state, and the runner cannot tell which
field the transition *meant* to write from the fields it merely carried over. So every conflict policy
presupposes a representation of writes as **identifiable contributions** (which state, what value),
not opaque full-state returns. Evolving that representation is a precondition of Genesis-006, logically
prior to choosing a policy at all.

## The proof Genesis-006 must pass

Whichever policy is chosen, it is proven by four tests (from the Genesis-006 plan):

- `Conflicting_Writes_Are_Deterministic` — two transitions, same field, always the same result.
- `Enumeration_Order_Does_Not_Change_Conflict_Result` — the discriminating test; it is exactly what
  disqualifies the order-dependent policies above.
- `Non_Conflicting_Writes_Are_Unaffected` — Genesis-005's composition must not regress.
- `Conflict_Policy_Is_Applied_Exactly_Once` — the resolver runs once per conflicting field per tick;
  this is a correctness property of the implementation, not a discriminator between policies.

## Open question carried into Genesis-006

Which surviving policy — Reject, commutative Accumulate, or per-kind Custom Resolver — and under what
representation of contributions? This note deliberately leaves it open. It has done its job if
Genesis-006 begins from *these three* rather than from all five, and knows why the other two were
never really candidates.
