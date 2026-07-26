# DN-004 — Declaring Constraints

Status: **Discussion.** Explores how Constraints become declared world content — without deciding
evaluation, policy, storage, scope, or kernel behaviour. Elaborates RFC-0004's requirement R3;
decides nothing R3 did not already say.

## Motivation

RFC-0004 established one thing, and one thing only: *a Constraint is a predicate over world states.*
It deliberately refused to answer when Constraints are evaluated, what happens when one fails, where
they live, and how they are realised.

One question nevertheless precedes every implementation:

> **How does a world declare that a Constraint exists?**

This note explores that question only.

## The pattern already present in Genesis

Genesis has developed a consistent habit: a world never hides its content inside behaviour. It
declares it. Places are declared. Relations are declared. Quantities are declared. Behaviour
discovers declarations — never the opposite.

**One honest exception, and it is instructive: the laws themselves.** Transitions are not declared
world content — they live in the law-set, as configuration handed to the runner. The Source
meditation confronted exactly this fork and ruled *state, not law*, for one decisive reason: what is
declared in the law-set can never be created, modified, or extinguished by the world's own dynamics.
RFC-0004's R3 already chose the same side for Constraints — *"declared world content, never
configuration hidden in the law-set."* This note elaborates that choice; the transitions' exception
shows precisely the fate it avoids.

## Constraints are world content

A world containing water should be able to declare *WaterQuantity must remain ≥ 0* — without
simultaneously declaring when this is checked, what happens otherwise, or who performs the check.
Those belong elsewhere. The declaration exists independently.

## Why the principle is forced, not chosen

Today a Constraint is silent (RFC-0004, second theorem), so its declaration could seemingly live
anywhere without consequence. But the moment Q1 — the violation policy — receives *any* answer,
Constraints will affect the world's evolution. And invariant 6 is absolute: whatever affects the
world is explicit, declared state. A Constraint declared outside world content would become hidden
state at the very instant it gained effect. Declaring Constraints as world content is therefore not
an aesthetic preference: it is **invariant 6 applied prospectively** — the only declaration that
survives every possible answer to Q1.

A quiet corollary, noted without being decided: content can, in principle, change under the world's
own dynamics. A Constraint declared as content leaves open a future in which legality itself evolves
— a river that freezes *becomes* a constraint. A Constraint declared as machinery forecloses that
future permanently. This note does not claim legality will evolve; it refuses to make its evolution
impossible.

## Declaration precedes execution

One of Genesis's oldest habits: *ontology precedes machinery.* Before asking "who evaluates
Constraints?", Genesis should first be able to answer "what Constraint has been declared?" — exactly
as it did for state, and exactly as RFC-0002 did for transformation before any runner existed.

## A declaration is inert

Perhaps the most important observation. Declaring `Quantity ≥ 0` does not change the world. It
schedules no work, allocates no system, activates no observer, registers no callback. It merely
enlarges the world's description. Execution comes later — or never.

## A declaration does not imply scope

It is tempting to write `Constraint<Cell>` or `GlobalConstraint`. That would silently answer Q4,
which RFC-0004 deliberately postponed. The declaration language must remain neutral until scope
itself is understood.

## Kinship with DN-003 — with one asymmetry

DN-003 introduced descriptive predicates; Constraint introduces normative ones. Both are predicates
over states; the distinction lies in what their truth *means* — "this state matches a recognised
pattern" versus "this state violates declared legality." But the kinship has a limit: **descriptive
predicates may live in the observers' language** (the Glossary is a document, not world content),
while **normative predicates belong to the world itself** — a world's legality is its own fact, not
a reading of it. Shared form; different residence.

## First design principle

> **A Constraint is declared exactly like any other world fact.**
> Its existence is part of the world. Its evaluation is not.

## What this note refuses to answer

Storage; runtime representation; evaluation timing; violation policy; observability; scope;
optimisation. Its only claim: **Constraint declarations belong to world content, not execution
machinery.**

## Closing thought

Genesis has repeatedly discovered the same architectural rhythm: *name, declare, observe, execute —
never the reverse.* If Constraints follow that rhythm, their declaration becomes just another piece
of the world's ontology — silent, explicit, and independent of whatever future systems choose to do
with it.

And the note marks a quiet turn in what Design Notes ask. The earlier notes answered *"how do we
build X?"* This one answers *"how does the world speak of X?"* — the first note written for the
world's language before the engine's representation. After the opening of the Age of Observation,
that is likely the right order for everything that follows.
