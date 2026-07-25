# Genesis Glossary

Words mean specific things here. This document defines them.

---

## World

The totality of simulation state. Everything that exists, exists in the world. There is no state outside the world.

The world is not a container that holds entities. The world *is* the state. Entities are patterns within that state.

## State

Data that represents what is. Position. Temperature. Ownership. Hunger. Damage.

State is always observable. There is no hidden state in Genesis.

## Transformation

A change from one state to another. Transformations are deterministic: the same input state produces the same output state.

Transformations are how the world changes. Time passes through transformations.

## Rule

A definition of when and how transformations occur. Rules are declarative: they describe what happens under certain conditions, not how to make it happen.

Example: "Flammable material adjacent to fire becomes fire."

## Process

An ongoing transformation that occurs over time. Processes are stateful: they track progress toward completion.

Example: A fire burning. Wood rotting. A creature digesting food.

## Constraint

A limit on what states are valid. Constraints are enforced by the simulation.

Example: An object cannot occupy the same space as another solid object.

## Tick

The fundamental unit of simulation time. Time advances in discrete ticks. All transformations occur at tick boundaries.

Between ticks, nothing changes. This is what makes the simulation deterministic.

## Causality

The relationship between cause and effect. Every state change has a cause. That cause can be traced.

Causality enables debugging, understanding, and emergent storytelling.

## Entity

A pattern of state that we recognize as a "thing." Entities are not fundamental to Genesis; they are a convenience for humans.

The world does not contain entities. The world contains state. We group related state and call it an entity.

## Emergence

Complex behavior arising from simple rules. Emergence is not designed; it is discovered.

Genesis relies on emergence. We do not script complex behavior. We define simple rules and observe what happens.

## Determinism

The property that identical inputs produce identical outputs. Genesis is deterministic.

Determinism is not a feature. Determinism is a requirement.

## Persistence

The property that state endures over time. Changes are permanent unless another transformation reverses them.

Persistence creates consequence. Without persistence, nothing matters.

---

## Terms We Avoid

### Agent

We do not use "agent" to describe entities in Genesis. "Agent" implies autonomy and decision-making. In Genesis, entities do not decide. The world transforms their state.

If we must refer to entities that exhibit agent-like behavior, we call them **actors**—but we remember that actors do not act. The world acts upon them.

### Behavior

We do not say entities "behave." Behavior implies intention. Instead, we say entities **change** or that the world **transforms** their state.

### AI

Genesis is not AI. There is no artificial intelligence. There are rules. Complexity emerges from rules, not from intelligence.

### Script

Rules are not scripts. Scripts dictate sequences of actions. Rules define transformations that apply when conditions are met. The difference matters.

---

*Precision in language enables precision in thought.*
