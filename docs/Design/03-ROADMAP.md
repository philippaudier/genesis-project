# Genesis Roadmap

How we get from here to there.

---

## Phases

Development proceeds in phases. Each phase establishes foundations for the next. No phase is rushed.

### Phase 0 — The Project Begins

**Status: Current**

Establish the foundations that everything else will build upon.

Deliverables:
- Constitution (immutable principles)
- Vision (long-term direction)
- Glossary (precise vocabulary)
- Roadmap (this document)
- Repository structure
- Development practices

Exit criteria:
- Documentation is complete and reviewed
- Team understands and agrees on principles
- Structure supports long-term development

---

### Phase 1 — The World Exists

**Status: Planned**

Create the representation of world state.

Questions to answer:
- How is state represented?
- How is space organized?
- How do we query state efficiently?
- How do we modify state safely?

Expected RFCs:
- RFC: World State Representation
- RFC: Spatial Organization
- RFC: State Queries

Exit criteria:
- World state can be created, read, and modified
- Spatial queries work correctly
- State is observable and debuggable

---

### Phase 2 — The World Changes

**Status: Planned**

Implement time and transformation.

Questions to answer:
- What is a tick?
- How are transformations ordered?
- How is determinism guaranteed?
- How do we debug transformation chains?

Expected RFCs:
- RFC: Tick System
- RFC: Transformation Pipeline
- RFC: Determinism Guarantees

Exit criteria:
- Time advances through ticks
- Transformations occur deterministically
- Same inputs produce same outputs (verified)

---

### Phase 3 — The World Grows

**Status: Planned**

Enable processes and propagation.

Questions to answer:
- How do processes track progress?
- How do effects propagate through space?
- How does complexity emerge from simple rules?

Expected RFCs:
- RFC: Process Model
- RFC: Propagation Systems
- RFC: Rule Composition

Exit criteria:
- Processes run over multiple ticks
- Effects spread through the world
- Emergent behavior is observable

---

### Phase 4 — The World Remembers

**Status: Planned**

Implement persistence and history.

Questions to answer:
- How is world state saved?
- How is causality tracked?
- How far back can we trace?
- How do we balance memory and history?

Expected RFCs:
- RFC: Persistence Format
- RFC: Causality Tracking
- RFC: History Depth

Exit criteria:
- World state saves and loads correctly
- Causality chains are traceable
- History informs present state

---

### Phase 5 — The World Adapts

**Status: Planned**

Enable feedback loops and equilibrium.

Questions to answer:
- How do systems reach equilibrium?
- How do feedback loops stabilize?
- How does the world evolve over long time scales?

Expected RFCs:
- RFC: Equilibrium Systems
- RFC: Long-term Evolution
- RFC: Stability Guarantees

Exit criteria:
- Systems find equilibrium naturally
- Long-running simulations remain stable
- Evolution is observable and meaningful

---

### Phase 6 — The Player Arrives

**Status: Planned**

Integrate observation and interaction.

Questions to answer:
- How does the player observe the world?
- How does the player affect the world?
- How do consequences flow from player actions?
- How does Lootbound integrate with Genesis?

Expected RFCs:
- RFC: Observation Model
- RFC: Interaction Model
- RFC: Player Integration

Exit criteria:
- Player can observe world state
- Player actions transform world state
- Consequences are visible and meaningful
- Lootbound development can begin

---

## Principles for Progress

1. **No phase is skipped.** Foundations matter.
2. **No phase is rushed.** Correctness over speed.
3. **RFCs before code.** Design before implementation.
4. **Questions before answers.** Understand the problem first.
5. **Exit criteria are real.** A phase ends when criteria are met, not when we're tired of it.

---

*Patience is not the absence of progress. Patience is sustainable progress.*
