# Unity Project Practices

> How the Unity project is structured so the [technical invariants](invariants.md) hold — above all,
> the Simulation/Presentation boundary. Unity is the presentation runtime, not the simulation.
> Everything here exists to keep that true by construction rather than by discipline.

## The assembly boundary (the load-bearing decision)

The Simulation/Presentation split is enforced by **assembly definitions** (`.asmdef`), not by
convention. The mechanism is a single asmdef flag:

> `"noEngineReferences": true` makes an assembly compile **without** `UnityEngine` / `UnityEditor`.
> Any use of a Unity type in that assembly then fails to build.

Two assemblies carry that flag and are therefore Unity-free by construction. This is what makes
invariants 1, 2, and 5 compile-time facts.

### Reference graph

Dependencies point one way only. An assembly may reference those *below* it, never above.

```
Genesis.Core          noEngineReferences   — Unity-free foundational types shared below the simulation
      ▲
Genesis.Simulation    noEngineReferences   — the world: state, rules, transitions (the product)
      ▲            ▲            ▲
      │            │            │
Genesis.Presentation  Genesis.Tools   Genesis.Tests
(UnityEngine, URP)   (UnityEngine)    (headless)
      ▲
Genesis.Editor        Editor platform  — authoring-time tooling (UnityEditor)
```

- **Genesis.Core** — Unity-free foundational types shared by the simulation. No Unity.
- **Genesis.Simulation** — the world and its rules. References Core only. **No Unity.** Runs headless.
- **Genesis.Presentation** — observes the Simulation and renders it. References Simulation (and Core as
  needed) plus `UnityEngine`/URP. Nothing in Simulation references it.
- **Genesis.Tools** — visualisation, debug, inspection, replay. Observes the Simulation; may use Unity.
  Presentation-side.
- **Genesis.Editor** — editor-only tooling; an Editor-platform assembly (`UnityEditor`).
- **Genesis.Tests** — exercises the Simulation; references Simulation and Core plus the Unity Test
  Framework (the harness). The *subjects* under test stay Unity-free; the harness necessarily is not.

If a reference ever needs to point *upward* (Simulation → Presentation), the design is wrong — that is
invariant 2 refusing to compile, working as intended.

## Folder → assembly map

| Folder | Assembly | Unity allowed? |
|---|---|---|
| `Assets/Genesis/Core/` | `Genesis.Core` | No (`noEngineReferences`) |
| `Assets/Genesis/Simulation/` | `Genesis.Simulation` | No (`noEngineReferences`) |
| `Assets/Genesis/Presentation/` | `Genesis.Presentation` | Yes |
| `Assets/Genesis/Tools/` | `Genesis.Tools` | Yes |
| `Assets/Genesis/Editor/` | `Genesis.Editor` | Yes (Editor only) |
| `Assets/Genesis/Tests/` | `Genesis.Tests` | Harness only (see wiring below) |
| `Assets/Genesis/Systems/`, `World/` | *unassigned* | — pending the RFC that dissolves them |

`Systems/` and `World/` are pre-Constitution placeholders and get **no** assembly until an RFC decides
their fate; leaving them unassigned keeps them out of the compiled graph.

## Test & editor harness wiring

The wiring left unspecified in the original draft (found during Genesis-001, folded back here):

- **Genesis.Tests** is an **Editor-platform, EditMode** test assembly: `includePlatforms: ["Editor"]`,
  references `Genesis.Simulation`, `Genesis.Core`, `UnityEngine.TestRunner`, `UnityEditor.TestRunner`;
  `overrideReferences: true` with `nunit.framework.dll` precompiled; `defineConstraints:
  ["UNITY_INCLUDE_TESTS"]`; `autoReferenced: false`. Requires the `com.unity.test-framework` package.
- **Genesis.Editor** is an Editor-platform assembly (`includePlatforms: ["Editor"]`) for
  authoring-time tooling; full Unity allowed.

## What may use Unity, and what may not

- **Simulation and Core:** no Unity, ever. This includes the obvious traps — `Time.deltaTime`,
  `UnityEngine.Random`, `MonoBehaviour`, `Physics`, `Vector3` — all of which are `UnityEngine` types
  and will not compile there.
- **Presentation, Tools, Editor:** full Unity. This is where rendering, input, and authoring live.

> **Numeric types are an open decision — this document takes no position.** What Simulation uses in
> place of Unity's math types (`float`/`double`, `Unity.Mathematics`, a custom type, fixed-point, …)
> is deliberately left undefined and will be decided by a dedicated RFC. `Unity.Mathematics` is not
> `UnityEngine`, so the boundary does not by itself exclude it. The only architectural decision made
> here is that Simulation must not depend on `UnityEngine`; nothing here commits Genesis to a custom
> math library.

## Unity subsystems are presentation, never simulation

Unity's physics, animation, particles, audio, and input are **presentation and interaction**, not
world truth. The Simulation never delegates a world rule to them (nor could it — they are
`UnityEngine`). A rendered position, a played sound, or a spawned particle is an *observation* of
simulation state, produced downstream; the simulation would reach the same state with none of them
present.

## Scenes and bootstrapping

A Unity scene is a presentation artifact — a way to observe a run — not where the world lives. The
Simulation is plain C# objects with no scene dependency. A presentation-side bootstrapper constructs
the world, then decides *when* to advance ticks; **it never decides what a tick does.** Tick timing is
a playback rate (a presentation concern); tick content is logical and frame-independent (invariant 5).

## URP

URP is a rendering choice inside Presentation and has no reach into the Simulation. Render pipeline,
quality settings, and platform targets may change freely without altering a single simulation result —
that replaceability is invariant 3.
