# C# Conventions & Simulation Constraints

> Two different kinds of rule, kept deliberately separate.
>
> - **Part A — Coding conventions** are *craft*: how C# is written across the whole codebase. They
>   are about readability and consistency, and a violation is untidy, not incorrect.
> - **Part B — Simulation constraints** are *correctness*: rules that simulation code must obey to
>   keep the [invariants](invariants.md) true. A violation is a defect, not a style lapse.
>
> A rule belongs to exactly one part. Style is never mixed with simulation law.

---

## Part A — Coding conventions (all assemblies)

These apply everywhere: Core, Simulation, Presentation, Tools, Editor, Tests.

**Naming**
- Types, methods, properties, events: `PascalCase`. Local variables and parameters: `camelCase`.
- Private fields: `_camelCase`. Constants: `PascalCase`. Interfaces: `IName`.
- Names say what a thing *is* or *does*; avoid abbreviations that aren't universal.

**Files and types**
- One top-level type per file; the file is named after the type.
- Explicit access modifiers always — never rely on the default.
- Prefer `readonly` and immutability where reasonable; expose the smallest surface that works.

**Language use**
- Nullable reference types enabled; avoid `null` where a design can exclude it.
- Prefer composition over inheritance (see Constitution, Art. XIII).
- Keep methods small and single-purpose; extract when a method stops fitting in the head.
- An abstraction must be justified by present need, not anticipated need. When several designs work,
  the simplest ships first.

**Comments**
- Match the comment density of the surrounding code. Explain *why*, not *what*; the code says what.

**Formatting**
- Formatting is not argued in prose — it is delegated to `.editorconfig` and analyzers so it is
  applied, not remembered. (That file is introduced when the first code lands.)

---

## Part B — Simulation constraints (Genesis.Simulation and Genesis.Core only)

These apply **only** to simulation code and exist solely to hold invariants 4 (logical determinism)
and 6 (explicit state). They are correctness rules; treat a violation as a bug.

**Determinism (backs invariant 4)**
- **No real-time or wall-clock reads.** No `DateTime.Now`, `Stopwatch`, or elapsed-time of any kind.
  (Unity's time APIs are already blocked by the assembly boundary.)
- **Randomness is explicit, seeded, and part of world state.** No global or static RNG, no
  `System.Random` held statically. A random draw advances a seed that lives in the declared state, so
  the same run reproduces the same draws.
- **Fixed iteration order.** No logic may depend on the enumeration order of an unordered collection
  (`Dictionary`, `HashSet`). Iterate ordered structures, or sort by a stable key before acting.
- **No environment sensitivity.** No culture-dependent parsing/formatting or platform-specific
  behaviour in logic.

**Explicitness (backs invariant 6)**
- **No hidden or ambient mutable state.** Everything that affects the world is part of the declared
  world state. No static mutable fields, no singletons holding world data, no caches that change
  outcomes.
- **Transitions are side-effect free.** A rule reads state and produces the next state; it performs
  no I/O and mutates nothing outside the state it declares.

**Neutrality**
- These constraints are **representation-agnostic**. The numeric type the simulation uses
  (`float`/`double`, `Unity.Mathematics`, custom, fixed-point) is an open decision for a dedicated
  RFC; nothing in Part B assumes an answer.

> Where a Part B constraint can be made self-enforcing — an analyzer banning `DateTime.Now` in the
> Simulation assembly, the replay test catching nondeterminism, the snapshot round-trip catching
> hidden state — that enforcement is always preferred to trusting review. Until then, these are held
> by review.
