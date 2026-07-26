# ADR-0005 — The Observable Membrane

Status: **Accepted** (2026-07-26 — proposed and dictated by the founder upon reviewing RFC-L001;
generalises a decision that RFC already made once, for one crossing)

*(Numbering note: proposed as "ADR-0002"; renumbered — ADR-0002 Development Methodology,
ADR-0003 The Scientific Method and ADR-0004 Scientific Objects already exist.)*

## Context

A laboratory can only observe what crosses a defined boundary. Without a boundary: impossible to
know where a change came from; impossible to reproduce; impossible to attribute causality.

RFC-L001 gave a first answer, for one kind of crossing (agency). This ADR generalises that answer,
because the membrane is not a feature — it is an architectural decision that will likely outlive
every RFC built on it.

## Decision

> **The world has a membrane. Everything external must cross it. Nothing can appear directly in
> the world. Ever.**

**Producers.** Today: the player. Tomorrow: AI, server, weather, scripts, seasonal events, replay,
test tools. All of them are exactly one thing — **producers of external events** — and none has
any privilege over any other. A producer that writes the same values to the same cells is the same
experiment (RFC-L001, provenance-blindness).

*Ontological guard:* "Producer" was subtracted from the world's ontology during the research arc,
and it stays subtracted. It returns here only as a **boundary role** — a name for whatever stands
*outside* the membrane. There is no Producer object inside a world; a law can never see one.

**Crossings.** An external event crosses the membrane only if it is: **declared** (external Kinds,
ordinary cells), **recorded** (append-only external event trace, written before application),
**replayable** (run = initial state + relations + laws + trace), and **interpretation-free** (the
trace must survive a rewrite of the laws — RFC-L001's event/command guard).

**Consumers.** Once inside, the laws are the only consumers. No parallel system. No magic
callback. No hook. Only laws.

## Consequences

- **Invariant 7 ("No hidden hand") is this ADR's enforcement**; RFC-L001 is its first realised
  crossing. Future crossing types (weather, server, seasons…) do not need new architecture — they
  need only to name their external Kinds and cite this ADR.
- The laboratory is no longer a closed system. It is **permeable with a perfectly defined
  membrane** — which is the only kind of permeability an observatory can afford.
- Counterfactual replay (same trace, modified laws) is well-posed for every crossing type, not
  just player input, because interpretation-freedom is demanded at the membrane, not per-producer.
- **The intended fate of this document is invisibility.** The best ADRs disappear: everyone
  forgets they exist, and everyone builds on them. When, years from now, adding a "season system"
  takes one declared Kind and zero architecture discussions, this ADR will have succeeded by
  being forgotten.

## Descent

From the Constitution: *the world is the only source of truth* (a truth-source with undocumented
entrances is not one); Determinism (Art. IV); Explicitness (Art. XII). Through: ADR-0001 (the one
mechanism the membrane feeds), Genesis-003 (externally owned state — the door this membrane
disciplines), RFC-L001 (the first crossing).
