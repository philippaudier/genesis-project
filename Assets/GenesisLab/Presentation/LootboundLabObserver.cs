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

            if (keyboard.sKey.wasPressedThisFrame)
            {
                ExportSession();
            }

            // Top row and numpad both count (AZERTY types digits on the pad).
            if (DigitPressed(keyboard, Key.Digit1, Key.Numpad1)) Go(LootboundWorld.Shelter);
            if (DigitPressed(keyboard, Key.Digit2, Key.Numpad2)) Go(LootboundWorld.Tree);
            if (DigitPressed(keyboard, Key.Digit3, Key.Numpad3)) Go(LootboundWorld.Station);
            if (DigitPressed(keyboard, Key.Digit4, Key.Numpad4)) Go(LootboundWorld.Clearing);
            if (DigitPressed(keyboard, Key.Digit5, Key.Numpad5)) Go(LootboundWorld.Field);

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

        private static bool DigitPressed(Keyboard keyboard, Key topRow, Key numpad)
        {
            return keyboard[topRow].wasPressedThisFrame || keyboard[numpad].wasPressedThisFrame;
        }

        private void Go(Place target)
        {
            Cross(new Cell(target, LootboundWorld.Go));
            _lastIntent = $"> go to {NameOf(target)} (t={_state.CurrentTick.Value})";
        }

        private void Cross(Cell cell)
        {
            _trace.Append(new ExternalEvent(_state.CurrentTick, cell, 1));
            _lastIntent = $"> {KindName(cell.Kind)} at {NameOf(cell.Place)} (t={_state.CurrentTick.Value})";
        }

        private string _lastIntent = "";

        private string ReachableFromHere()
        {
            Place here = PlayerPlace();
            if (here == LootboundWorld.Field)
            {
                return "Shelter (1) · Tree (2) · Station (3) · Clearing (4)";
            }

            return "Field (5) — every path goes through the field";
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

        /// <summary>
        /// Writes the session's four-object record (Protocol-H001) into Lootbound-Lab/Runs/:
        /// the trace verbatim, the biography, the end-state audit, and an empty Narrative section
        /// for the one question's verbatim answer. Laboratory record-keeping, presentation-side —
        /// the wall-clock date in the header is the presentation's knowledge, never the world's.
        /// </summary>
        private void ExportSession()
        {
            var text = new System.Text.StringBuilder();
            text.AppendLine("# Run-unnamed (rename me)");
            text.AppendLine();
            text.AppendLine("```");
            text.AppendLine("Producer: Human");
            text.AppendLine("World:    L-002");
            text.AppendLine($"Date:     {System.DateTime.Now:yyyy-MM-dd HH:mm}");
            text.AppendLine($"Length:   {_state.CurrentTick.Value} ticks");
            text.AppendLine("```");
            text.AppendLine();
            text.AppendLine("## Trace (verbatim — every crossing of the membrane)");
            text.AppendLine();
            text.AppendLine("```");
            for (int i = 0; i < _trace.Events.Count; i++)
            {
                ExternalEvent crossing = _trace.Events[i];
                text.AppendLine($"t={crossing.Boundary.Value,-4} {KindName(crossing.Target.Kind),-7} {NameOf(crossing.Target.Place),-10} {crossing.Amount}");
            }

            text.AppendLine("```");
            text.AppendLine();
            text.AppendLine("## Biography (the Chronicler's reading — vocabulary under study, RD-L6)");
            text.AppendLine();
            text.AppendLine("```");
            text.AppendLine(_biography.TrimEnd());
            text.AppendLine("```");
            text.AppendLine();
            text.AppendLine("## End-state audit");
            text.AppendLine();
            text.AppendLine("```");
            text.AppendLine($"Player: {NameOf(PlayerPlace())} · {SwordLine(LootboundWorld.OldSword, "Old sword")} · {SwordLine(LootboundWorld.NewSword, "Better sword")} · Wood: {_state.ValueAt(new Cell(LootboundWorld.Pack, LootboundWorld.Wood))}");
            text.AppendLine("```");
            text.AppendLine();
            text.AppendLine("## Narrative (verbatim — the one question: \"What stayed with you?\")");
            text.AppendLine();
            text.AppendLine("```");
            text.AppendLine("(paste the answer here, unedited — or record the silence)");
            text.AppendLine("```");

            string directory = System.IO.Path.Combine(Application.dataPath, "..", "Lootbound-Lab", "Runs");
            System.IO.Directory.CreateDirectory(directory);
            string path = System.IO.Path.Combine(directory, $"Run-{System.DateTime.Now:yyyyMMdd-HHmmss}.md");
            System.IO.File.WriteAllText(path, text.ToString());
            Debug.Log($"Session exported: {path}");
        }

        private static string KindName(Kind kind)
        {
            if (kind == LootboundWorld.Go) return "Go";
            if (kind == LootboundWorld.Act) return "Act";
            if (kind == LootboundWorld.Attack) return "Attack";
            return kind.ToString();
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
            GUILayout.Label($"From here you can walk to: {ReachableFromHere()}");
            GUILayout.Label("Walk: 1 Shelter · 2 Tree · 3 Station · 4 Clearing · 5 Field (top row or numpad)");
            GUILayout.Label("E interact here · F strike the tree · Space pause · R reset · S export session");
            if (_lastIntent.Length > 0)
            {
                GUILayout.Label(_lastIntent);
            }
            GUILayout.Space(10);
            GUILayout.Label(_biography);
            GUILayout.EndArea();
        }
    }
}
