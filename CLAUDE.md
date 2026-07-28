# Genesis — AI Project Bootstrap

> This document is the entry point for any AI assistant joining the Genesis project.
>
> Read this document before answering any question related to Genesis.
> It describes the project's philosophy, workflow, current state, and expected collaboration model.
>
> This document intentionally avoids implementation details.
> Those belong to the documentation and RFCs.

---

# What is Genesis?

Genesis is **not a game**.

Genesis is a long-term deterministic world simulation framework.

Its purpose is to explore how believable worlds can emerge from a very small set of simple, deterministic rules.

Games are consumers of Genesis.

The first planned game is **Lootbound**.

---

# Fundamental Philosophy

Genesis is built around one central belief:

> **The world is the only source of truth.**

Everything derives from this principle.

The simulation is the product.

Rendering, UI, audio and player interaction are only ways to observe or influence the simulation.

Unity is therefore **only the presentation layer**.

Genesis must always remain able to run headless.

---

# Current State

The live state of the project — current milestone, objective, blockers, and active RFC — lives in
`.claude/current.md`, not here.

This document holds only what is stable across milestones. Anything that changes as work progresses
belongs in `current.md`.

---

# Documentation Hierarchy

Documents have different levels of authority.

Highest authority:

1. Constitution

Defines immutable principles.

Very rarely changes.

↓

Vision

Defines long-term objectives.

↓

Roadmap

Defines development phases.

↓

RFCs

Define individual engineering decisions.

↓

Implementation

Code implements accepted RFCs.

Never the opposite.

---

# Current Repository Structure

```
docs/

Constitution/
Design/
RFC/
Decisions/
DesignNotes/
Engineering/
Milestones/
Observations/
Research/
Sciences/
Journal/

Assets/

Genesis/

Core/
Simulation/
Presentation/
Tools/
Editor/
Tests/
Systems/   (legacy placeholder — pending an RFC)
World/     (legacy placeholder — to be dissolved into Space/State/Rules/Resources by a future RFC)
```

Documentation is organised like code.

Each folder is a documentary module.

> Note: `Systems/` and `World/` are pre-Constitution placeholders retained for now.
> They will be reorganised or removed through a dedicated RFC, not ad hoc.

---

# Current Documents

Read these in order.

1.
README.md

2.
docs/Constitution/00-CONSTITUTION.md

3.
docs/Design/01-VISION.md

4.
docs/Design/02-GLOSSARY.md

5.
docs/Design/03-ROADMAP.md

6.
docs/Decisions/ADR-0001-Simulation-Transformation-Model.md  (the accepted architecture — the Snapshot Transition Model)

7.
docs/Design/04-KERNEL.md  (the Kernel Completion Record — what has been proven)

8.
docs/Decisions/ADR-0002-Development-Methodology.md  (how Genesis builds — the Proof Rule)

8b.
docs/Decisions/ADR-0003-The-Scientific-Method.md  (how Genesis learns — the discovery pipeline)

9.
docs/Engineering/ — invariants, conventions, unity, workflow (how Genesis is built)

RFCs (docs/RFC/) and Design Notes (docs/DesignNotes/) hold the reasoning behind the decisions.

Only then begin discussing implementation.

---

# Development Workflow

Genesis follows an RFC-first workflow.

Every major architectural decision follows this order:

Problem

↓

Discussion

↓

RFC

↓

Review

↓

Acceptance

↓

Implementation

↓

Testing

No production code should exist without an accepted reason for existing.

---

# Collaboration Expectations

When helping with Genesis:

Do NOT immediately generate code.

Instead:

Understand the problem.

Challenge assumptions.

Identify trade-offs.

Compare alternatives.

Only then suggest an implementation.

Question architecture when necessary.

Protect long-term consistency over short-term convenience.

If an idea contradicts the Constitution, explain why.

---

# Engineering Principles

Always favour:

Determinism

Explicit state

Traceable causality

Simple systems

Emergent complexity

Composition over inheritance

Simulation before gameplay

Presentation separated from simulation

Readable architecture

Long-term maintainability

Avoid:

Hidden state

Magic behaviour

Special cases

Premature optimisation

Architecture driven by content

Player-centric simulation

Scripted outcomes

---

# Communication Style

When discussing Genesis:

Be precise.

Avoid hype.

Avoid unnecessary abstraction.

Prefer engineering reasoning over opinion.

If multiple solutions exist:

Explain each.

Explain trade-offs.

Do not force a conclusion unless evidence supports it.

---

# Long-Term Goal

Genesis should become a simulation framework capable of producing believable worlds whose stories emerge naturally from deterministic rules.

The first game built on Genesis will be Lootbound.

Genesis is expected to evolve over many years.

Design every decision as though it will still matter in ten years.

---

# Final Rule

If you are uncertain whether something belongs in Genesis, ask this question:

> Does this make the world itself better?

If the answer is no, reconsider the proposal before implementing it.

# About the Creator

Genesis is being developed as a long-term engineering project.

The goal is not to finish quickly.

The goal is to build something coherent enough that its architecture remains understandable many years from now.

Every important decision should favour longevity over immediate convenience.

The project values thoughtful design, documentation and understanding above rapid feature development.
