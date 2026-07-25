# Current

> Live working state only. **Not** history (→ `docs/Journal/`), **not** cross-session memory, **not**
> philosophy (→ `CLAUDE.md`). Keep it short. Update as things change; delete what is no longer current.

- **Genesis-008 Addressable State: COMPLETE** — 36/36 EditMode tests green. State locations have stable, explicit identity.
- **Milestone:** Genesis-009 Explicit Relations (Sprint 1) — the *proof* that addressed locations can be connected through explicit directed relations without spatial semantics and without weakening determinism. Things exist (008); now they may be related (009).
- **New invariant:** *Relations between state addresses are explicit, stable, and independent of storage or enumeration order.*
- **Chosen representation:** `Relation` — a readonly struct `(CounterAddress Source → CounterAddress Target)`. Structural identity over the pair; **direction matters** (asymmetric hash, A→B ≠ B→A). No kind, weight, coordinate, distance, or lifecycle — a connection, nothing more.
- **Storage & placement (deliberate, recorded as open):** `RelationSet` — an immutable set built **beside** `SimulationState`, validated against the state's addresses at construction (unknown source/target → `ArgumentException`); duplicates collapse. **Whether topology ultimately belongs *inside* SimulationState — mutable by transitions like any other state — is NOT decided here.** For this milestone relations are a separate immutable structure because nothing yet needs to read or change them during a tick; deciding their mutability now would silently answer a question that belongs to a future milestone/RFC (same discipline as the TickClock episode).
- **Equality/hashing semantics:** set equality (`SetEquals`) over relations; hash = commutative aggregation (sum) of relation hashes — neither can depend on insertion/enumeration order. Same pattern as SimulationState (008).
- **The proof (`RelationTests`, 9 tests):** Relation_Connects_Two_Known_Addresses · Reverse_Relation_Is_Distinct · Duplicate_Relations_Do_Not_Change_The_Set · Relation_Equality_Is_Independent_Of_Insertion_Order · Two_Equivalent_Relation_Sets_Have_Equal_Hash_Codes · Relation_To_Unknown_Source/Target_Is_Rejected · Relations_Do_Not_Modify_Simulation_State · Existing_Computational_Kernel_Remains_Deterministic. All 004–008 regression suites untouched and still run.
- **Honest limitations:** *Genesis-009 proves explicit directed relations between known addresses. It does not yet prove neighbour-scoped reads, traversal, topology mutation, spatial meaning, relation kinds, or propagation.* Also open: relation placement/mutability (above); validation is construction-time only (a RelationSet does not track later states — no lifecycle exists to need it yet).
- **Blockers:** run EditMode tests in Unity's Test Runner to confirm green (I cannot). RFC-0001 still Draft.
- **Proof statement (pending green):** *Genesis-009 proved that addressed state locations can be connected through explicit relations without introducing spatial semantics or weakening determinism.*
- **Active RFCs:** RFC-0001 — Tick System (Draft) · RFC-0002 — Resolved by ADR-0001.

**Milestone naming convention:** `Genesis-NNN Name`.
