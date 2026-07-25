# Genesis

**Simulate the world. Let stories emerge.**

---

Genesis is built around one fundamental belief:

> **The world is the only source of truth.**

Everything flows from this. State exists in the world. Change happens to the world. Stories emerge from the world. Not from scripts. Not from designers. Not from agents making decisions in isolation.

The world is not a backdrop. The world is the simulation.

## What Genesis Is

Genesis is a deterministic simulation framework. It models world state, processes, and transformations. It tracks causality. It enforces constraints. It lets complexity emerge from simple rules applied consistently.

Genesis is not a game. It is the foundation that games will be built upon. [Lootbound](docs/Design/lootbound.md) is the first.

## What Genesis Is Not

Genesis is not a game engine. It does not render. It does not handle input. It does not play sound. Those responsibilities belong to Unity, which serves as the runtime.

Genesis is not an AI system. It does not simulate agents. Agents may eventually exist within Genesis, but they are consequences of world simulation—not the purpose of it.

## Mission

To build infrastructure for worlds that are coherent, persistent, and alive—without scripted behavior or predetermined narrative.

## Philosophy

Most simulations model entities that act upon a world. Genesis inverts this. The world acts. Entities are patterns within it.

A fire does not decide to spread. The world's rules dictate that fire spreads to adjacent flammable material. A creature does not decide to be hungry. The world's rules transform energy over time. The creature's hunger is a world state, not a creature decision.

This inversion has consequences:

- **No hidden state.** If it matters, it exists in the world.
- **No special cases.** Rules apply uniformly.
- **No narrative privilege.** The player is subject to the same physics as everything else.

## Core Principles

1. **World over agents.** The world is primary. Entities are emergent.
2. **State over behavior.** Model what *is*, not what things *do*.
3. **Determinism over randomness.** Same inputs produce same outputs.
4. **Causality over coincidence.** Every change has a traceable cause.
5. **Constraints over scripts.** Rules shape possibility; they don't dictate action.

## Current Status

Genesis is in Phase 0: establishing foundations.

## Project Phases

| Phase | Focus |
|-------|-------|
| 0 | **The project begins.** Structure, principles, constitution. |
| 1 | **The world exists.** State representation, spatial foundation. |
| 2 | **The world changes.** Time, ticks, deterministic transformation. |
| 3 | **The world grows.** Processes, propagation, emergent complexity. |
| 4 | **The world remembers.** Persistence, history, causality chains. |
| 5 | **The world adapts.** Feedback loops, equilibrium, evolution. |
| 6 | **The player arrives.** Observation, interaction, consequence. |

## Repository Structure

```
genesis-project/
├── Assets/
│   └── Genesis/
│       ├── Core/            # Foundational systems
│       ├── Simulation/      # The world's rules — the product itself
│       ├── Presentation/    # Observation layer (Unity lives here)
│       ├── Tools/           # Visualisation, debug, inspection, replay
│       ├── Editor/          # Authoring-time tooling
│       └── Tests/           # Verification
├── docs/
│   ├── Constitution/        # Immutable principles
│   ├── Design/             # Vision, glossary, roadmap, game designs
│   ├── RFC/                # Technical proposals
│   ├── Decisions/          # Recorded architectural decisions
│   ├── Journal/            # Development log
│   └── Research/           # Notes and investigations
└── README.md
```

Documentation is organised like code: each folder is a documentary module, not a
dump of loose `.md` files. Every module in `Assets/Genesis/` maps to a concern the
[Constitution](docs/Constitution/00-CONSTITUTION.md) names — most importantly, the
separation of **Simulation** (the world) from **Presentation** (any window onto it).

## Documentation

| Document | Purpose |
|----------|---------|
| [Constitution](docs/Constitution/00-CONSTITUTION.md) | The rules that do not change |
| [Vision](docs/Design/01-VISION.md) | Where Genesis is going |
| [Glossary](docs/Design/02-GLOSSARY.md) | What words mean here |
| [Roadmap](docs/Design/03-ROADMAP.md) | How we get there |

Technical decisions are proposed and discussed through RFCs in `docs/RFC/`. The first is [RFC-0001 — Tick System](docs/RFC/RFC-0001-Tick-System.md).

## Technology

- **Runtime:** Unity 2022 LTS
- **Language:** C#

Technology choices prioritize stability over novelty.

## Contributing

Genesis is in closed development. Principles for future contribution:

- Understand the constitution before proposing changes
- Small, correct changes over large, ambitious ones
- Questions are welcome; assumptions are not

## License

MIT License

Copyright (c) 2025 Genesis Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

*Day zero of a ten-year project.*
