# Current

> Live working state only. **Not** history (→ `docs/Journal/`), **not** cross-session memory, **not**
> philosophy (→ `CLAUDE.md`). Keep it short. Update as things change; delete what is no longer current.

- **Milestone:** Genesis-013 **First Observation** — implemented, awaiting *human* validation (a first for the project: the instrument is validated by using it, not by EditMode tests). Vision doc: `docs/Milestones/Genesis-013-First-Observation.md` (new `Milestones/` module, registered in CLAUDE.md). Method inverted for the first time: Observation → Questions → Tests → Architecture (if necessary).
- **What was built** (`Genesis.Presentation`, 4 files; the assembly marker retired):
  - `DemoWorld` — the Genesis-012 witness world (3 places, Quantity+Rate kinds, symmetric chain, rate=2 at one place) + the two law fixtures (duplicated from tests; a shared world-content home is deliberately deferred per the motto).
  - `SimulationHost` — decides *when* ticks run, never what they do; publishes immutable snapshots; fixed 0.5 s/tick playback; deterministic Reset.
  - `WorldView` — the light table: circle per place, thin line per relation, fill = quantity normalised by observed max (derivable), halo iff rate > 0; positions derived from place-identity order; URP Unlit materials, no custom shaders.
  - `GenesisObserver` — the single entry point (MonoBehaviour): Play/Pause/Step/Reset + tick (OnGUI), right-drag pan, scroll zoom, click → facts-only panel (Place N, kind values, out/in counts — **no category words in the interface**).
- **Invariants held structurally:** observer can't write (SimulationState immutable; `WithTickAdvanced` internal to Simulation); removing the assembly changes no theorem (one-way asmdef; the 70 kernel tests never reference Presentation); everything visible derivable from the snapshot.
- **Config change:** `ProjectSettings` activeInputHandler 1 → **2 (Both)** — required for OnGUI/legacy input; Input System package untouched. **Unity may need a restart to apply.**
- **How to run (the moment):** open Unity → New Scene (empty) → empty GameObject → Add Component → **Genesis Observer** → Play. Expected: three circles on a dark table, thin lines between them, an amber halo on the left one; press Play — the lit place brightens then the light *spreads right*, tick by tick. Also confirm: 70/70 EditMode tests still green (Invariant 4).
- **Planned from the start, not built:** Record/Replay (snapshot recording, shareable exact observations). Success criterion of a new kind: **zero discoveries** — 013 ends with an instrument, not a finding.
- **Standing opens:** unchanged (constraint layer · numeric RFC · write-scoping parked · address lifecycle · type-level enforcement · resolver-algebra conjecture · world-content home · Sink/through-flow).
- **Active RFCs:** RFC-0001 Accepted · RFC-0002 → ADR-0001 · RFC-0003 Accepted.

**Milestone naming convention:** `Genesis-NNN Name`.
