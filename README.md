# Genesis

**Simulate the world. Let stories emerge.**

---

Genesis is built around one fundamental belief:

> **The world is the only source of truth.**

And one discovery made while building it:

> **The world is not a collection of things. It is a succession of transformations.**

Genesis is a long-term deterministic world simulation framework. It is not a game — games are
consumers of Genesis. The first planned game is [Lootbound](docs/Design/lootbound.md).

## What Genesis Is

A framework in which a world advances by law, not by script. Every tick, every transition reads an
immutable snapshot of the world and contributes to the next one; conflicts resolve by explicit,
deterministic rules; nothing changes without a cause, and nothing observes more than it has declared.

Genesis is not a game engine. It does not render, play sound, or read input — Unity does, strictly as
a presentation layer. The simulation is the product; everything else is a window onto it. Genesis
always runs headless.

## Core Principles

1. **World over objects.** The world is primary; entities are patterns within its state.
2. **Transformation over state.** What matters is not what the world *is*, but how one moment
   becomes the next. State is transformation caught mid-sentence.
3. **Determinism over randomness.** Identical beginnings produce identical worlds — always.
4. **Causality over coincidence.** Every change has a traceable cause.
5. **Emergence over scripting.** Rules shape possibility; they never dictate outcomes.
6. **Explicitness over convenience.** If it affects the world, it is declared, inspectable state.

The full principles live in the [Constitution](docs/Constitution/00-CONSTITUTION.md) — the document
everything else in this repository answers to.

## Current Status

**The computational kernel is complete and frozen** (tag `genesis-core-kernel`): snapshot →
scoped reads → transitions → contributions → conflict resolution → commit → next snapshot. Seven
milestones, each proving exactly one property, every proof executable and still running. See the
[Kernel Completion Record](docs/Design/04-KERNEL.md).

There is deliberately no world yet — no creatures, terrain, or gameplay. Genesis built its laws
first; the world comes next.

## Documentation

Documentation is organised like code: each folder is a documentary module.

| Document | Purpose |
|----------|---------|
| [Constitution](docs/Constitution/00-CONSTITUTION.md) | The rules that do not change |
| [Vision](docs/Design/01-VISION.md) | Where Genesis is going |
| [Glossary](docs/Design/02-GLOSSARY.md) | What words mean here |
| [Roadmap](docs/Design/03-ROADMAP.md) | How we get there |
| [Kernel Record](docs/Design/04-KERNEL.md) | What has been proven |
| [Decisions](docs/Decisions/) | Accepted architecture (ADRs) |
| [RFCs](docs/RFC/) | The reasoning behind decisions |
| [Engineering](docs/Engineering/) | Invariants, conventions, workflow |
| [Research](docs/Research/) | Explorations and adversarial reviews |
| [Journal](docs/Journal/) | The story as it happened |

Development follows an RFC-first workflow and the Proof Rule: every milestone exists to prove exactly
one property of the engine ([ADR-0002](docs/Decisions/ADR-0002-Development-Methodology.md)).

## Architecture

The Simulation/Presentation boundary is enforced by the compiler, not by discipline:
`Genesis.Simulation` and `Genesis.Core` are Unity-free assemblies (`noEngineReferences`) — any use of
a Unity API in simulation code fails to build. Details in
[docs/Engineering/unity.md](docs/Engineering/unity.md).

## Technology

- **Runtime:** Unity 6 (URP)
- **Language:** C#

Technology choices prioritise stability over novelty — and the Constitution outlives the technology:
if Genesis is ever rewritten on another stack, the principles survive the rewrite unchanged.

## Contributing

Genesis is in closed development. Principles for future contribution:

- Understand the Constitution before proposing changes
- Small, correct changes over large, ambitious ones
- No production code without an accepted reason for existing
- Questions are welcome; assumptions are not

## License

MIT License

Copyright (c) 2025 Genesis Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

*The laws came first. The world is next.*
