using UnityEngine;
using UnityEngine.InputSystem;
using Genesis.Simulation;
using Genesis.Simulation.Lootbound;

namespace Genesis.Presentation
{
    /// <summary>
    /// L-002 — the human producer's window into the first living world. Presentation only: it
    /// renders text and turns key presses into external events on the trace — nothing else. Every
    /// gesture crosses the membrane exactly as a bot's would (provenance-blindness); the simulation
    /// never learns a human is here. Deliberately boring by protocol: no models, no animation — a
    /// psychology-experiment window whose one job is to let a biography happen.
    /// </summary>
    public sealed class LootboundLabObserver : MonoBehaviour
    {
        [Tooltip("Real seconds per logical tick. Presentation knowledge only; the world knows ticks.")]
        public float SecondsPerTick = 0.4f;

        private SimulationState _state;
        private RelationSet _relations;
        private System.Collections.Generic.IReadOnlyList<ITransition> _laws;
        private TickRunner _runner;
        private ExternalEventTrace _trace;
        private float _accumulator;
        private bool _paused;
        private string _biography = "";

        private void Start()
        {
            ResetWorld();
        }

        private void ResetWorld()
        {
            _state = LootboundWorld.BuildInitialState();
            _relations = LootboundWorld.BuildRelations(_state);
            _laws = LootboundWorld.BuildLaws();
            _runner = LootboundWorld.BuildRunner();
            _trace = new ExternalEventTrace(LootboundWorld.BuildMembrane());
            _accumulator = 0f;
            _biography = "";
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                _paused = !_paused;
            }

            if (keyboard.rKey.wasPressedThisFrame)
            {
                ResetWorld();
                return;
            }

            if (keyboard.digit1Key.wasPressedThisFrame) Go(LootboundWorld.Shelter);
            if (keyboard.digit2Key.wasPressedThisFrame) Go(LootboundWorld.Tree);
            if (keyboard.digit3Key.wasPressedThisFrame) Go(LootboundWorld.Station);
            if (keyboard.digit4Key.wasPressedThisFrame) Go(LootboundWorld.Clearing);
            if (keyboard.digit5Key.wasPressedThisFrame) Go(LootboundWorld.Field);

            if (keyboard.eKey.wasPressedThisFrame)
            {
                Place here = PlayerPlace();
                // Only places with an Act cell may receive an Act event (the cells are the world's).
                if (here == LootboundWorld.Shelter || here == LootboundWorld.Station || here == LootboundWorld.Clearing)
                {
                    Cross(new Cell(here, LootboundWorld.Act));
                }
            }

            if (keyboard.fKey.wasPressedThisFrame)
            {
                Cross(new Cell(LootboundWorld.Tree, LootboundWorld.Attack));
            }

            if (_paused)
            {
                return;
            }

            _accumulator += Time.deltaTime;
            while (_accumulator >= SecondsPerTick)
            {
                _accumulator -= SecondsPerTick;
                _state = _runner.Run(_state, _relations, _laws, _trace, 1);
                _biography = BiographyChronicler.Chronicle(
                    _trace, _state.CurrentTick.Value, LootboundWorld.OldSword, "Run-live", "Sword-1000");
            }
        }

        private void Go(Place target)
        {
            Cross(new Cell(target, LootboundWorld.Go));
        }

        private void Cross(Cell cell)
        {
            _trace.Append(new ExternalEvent(_state.CurrentTick, cell, 1));
        }

        private Place PlayerPlace()
        {
            foreach (Place place in LootboundWorld.Spatial)
            {
                if (_state.ValueAt(new Cell(place, LootboundWorld.PlayerAt)) == 1)
                {
                    return place;
                }
            }

            return LootboundWorld.Field;
        }

        private static string NameOf(Place place)
        {
            if (place == LootboundWorld.Shelter) return "Shelter";
            if (place == LootboundWorld.Tree) return "Tree";
            if (place == LootboundWorld.Station) return "Repair station";
            if (place == LootboundWorld.Clearing) return "Clearing";
            return "Field";
        }

        private string SwordLine(Place sword, string label)
        {
            long loc = _state.ValueAt(new Cell(sword, LootboundWorld.Location));
            string where = loc == 0 ? "in hand" : NameOf(new Place((int)loc));
            long wear = _state.ValueAt(new Cell(sword, LootboundWorld.Wear));
            long repairs = _state.ValueAt(new Cell(sword, LootboundWorld.Repairs));
            return $"{label}: {where}   wear {wear}   repairs {repairs}";
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12, 12, 640, 720));
            GUILayout.Label($"LOOTBOUND LAB — the first living world      tick {_state.CurrentTick.Value}{(_paused ? "   [paused]" : "")}");
            GUILayout.Space(6);
            GUILayout.Label($"You are at: {NameOf(PlayerPlace())}");
            GUILayout.Label(SwordLine(LootboundWorld.OldSword, "Old sword"));
            GUILayout.Label(SwordLine(LootboundWorld.NewSword, "Better sword"));
            GUILayout.Label($"Wood: {_state.ValueAt(new Cell(LootboundWorld.Pack, LootboundWorld.Wood))}");
            GUILayout.Space(6);
            GUILayout.Label("Walk: 1 Shelter · 2 Tree · 3 Station · 4 Clearing · 5 Field (paths go through the field)");
            GUILayout.Label("E interact here · F strike the tree · Space pause · R reset");
            GUILayout.Space(10);
            GUILayout.Label(_biography);
            GUILayout.EndArea();
        }
    }
}
