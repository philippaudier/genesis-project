# Development Workflow

> **The Discovery-era motto** (adopted 2026-07-26, upon Genesis-012):
> *Never add a law because it seems useful. Wait until a world makes it inevitable.*
> Words are earned by theorems (DN-003); laws are earned by phenomena. This is the operational
> difference between building an engine and discovering a world.

> **The Age-of-Worlds rule** (adopted 2026-07-26, upon the founding of Lootbound Lab):
> every future RFC — Genesis's included — opens with one question, answered in its first paragraph:
> *Which phenomenon, made inevitable by a world, does this RFC make observable?*
> An RFC that cannot answer it is architecture for its own sake, and waits. World-demanded RFCs
> carry the `L` prefix (RFC-L001, …); their demand-side document is named in that first paragraph.

> How Genesis is developed. This is **not** a Git guide — branching, commits, and the mechanics of
> version control are assumed known and are not explained here. This describes the loop specific to
> Genesis: how a change travels from a need to done, and where it must answer to the Constitution, the
> [invariants](invariants.md), and the RFCs.

## Before

- **Understand the need.** What must the world do, or be? If it cannot be stated plainly, it is not
  ready to build.
- **Check the RFCs.** No production simulation code exists without an accepted RFC that says why it
  exists. If the change needs a decision that has not been made, the RFC comes first — not the code.
- **Check the invariants.** Which of the invariants does this touch? A change that would *alter* an
  invariant is Constitutional-weight and stops here until that is faced deliberately, never in passing.
- **Define the milestone.** One thin vertical slice with a clear Definition of Done. If you cannot
  name the moment it is finished, it is too big — split it.

## During

- **One responsibility.** A milestone does one thing. When a second thing appears, it is the *next*
  milestone, not this one.
- **Know your layer.** Simulation and Core obey Part B (correctness); Presentation, Tools, and Editor
  obey Part A (craft). A rule that binds one layer does not bind another.
- **Compile often.** The assembly boundary and the type system are enforcement — let them work by
  building frequently, not once at the end.
- **Small steps.** Each commit is a coherent step. The point is granularity, not ceremony.
- **Stop at the Definition of Done.** The moment the slice meets its DoD, the work is finished.

## After

- **Review** — against the invariants and the governing RFC, not against taste. Does simulation code
  still hold Part B? Did the change drift from the RFC's intent?
- **Test** — determinism/replay wherever the change touches the simulation. An invariant is only real
  once a test guards it.
- **Document** — only if a *durable* fact changed, and only in the one document that owns it. Never
  state a fact twice. Most milestones need no documentation change at all.
- **Journal** — if the milestone taught something, record what happened and why. Chronology, not state.
- **Context** — update `.claude/current.md` so "where we are" stays true.

## Definition of Done

A milestone is done when, and only when:

- it compiles;
- the invariants it touches still hold — and are tested where a test is possible;
- its stated Definition of Done is met;
- nothing beyond its single responsibility was changed.

## Stop when Done

> When the milestone is complete, stop.
> The next improvement belongs to the next milestone.

The pull of *"while we're here…"* is how a one-file change becomes a three-day refactor. An
improvement that is genuinely worth making is still worth making next milestone — where it gets its
own need, its own Definition of Done, and its own review. Finishing is a feature, not a pause.
