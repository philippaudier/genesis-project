using System.Collections.Generic;
using Genesis.Simulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Genesis.Presentation
{
    /// <summary>
    /// The laboratory's single entry point. Drop this component onto an empty GameObject in an empty
    /// scene, press Play, and the world becomes observable: circles for places, thin lines for
    /// relations, quantity as fill, rate as halo — with keyboard transport (Space = Play/Pause,
    /// S = Step, R = Reset), right-drag pan, scroll zoom, and a facts-only panel when a place is
    /// clicked. Input goes through the Input System package; IMGUI is used for display only.
    ///
    /// The observer reads immutable snapshots and never modifies the world (Genesis-013). Remove
    /// this entire assembly and no kernel theorem changes. The interface names no world category:
    /// only facts appear; the Glossary stays in the reader's head.
    /// </summary>
    public sealed class GenesisObserver : MonoBehaviour
    {
        private SimulationHost _host;
        private WorldView _view;
        private Camera _camera;

        private bool _hasSelection;
        private Place _selected;

        private void Awake()
        {
            _host = new SimulationHost();
            _view = new WorldView();
            _view.Build(_host.World, transform);
            _camera = ConfigureCamera();
        }

        private void Update()
        {
            _host.Advance(Time.deltaTime);
            _view.Refresh(_host.Current, _host.World);
            HandleInput();
        }

        private void HandleInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.spaceKey.wasPressedThisFrame)
                {
                    _host.Playing = !_host.Playing;
                }

                if (keyboard.sKey.wasPressedThisFrame)
                {
                    _host.Step();
                }

                if (keyboard.rKey.wasPressedThisFrame)
                {
                    _host.Reset();
                    _hasSelection = false;
                }
            }

            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            // Selection: left click on a circle.
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 position = mouse.position.ReadValue();
                Ray ray = _camera.ScreenPointToRay(position);
                if (_view.TryPick(ray, out Place picked))
                {
                    _selected = picked;
                    _hasSelection = true;
                }
                else
                {
                    _hasSelection = false;
                }
            }

            // Pan: right-mouse drag.
            if (mouse.rightButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue();
                float unitsPerPixel = 2f * _camera.orthographicSize / Screen.height;
                _camera.transform.position -= new Vector3(delta.x, delta.y, 0f) * unitsPerPixel;
            }

            // Zoom: scroll wheel (sign-based; scroll units differ per platform).
            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                _camera.orthographicSize =
                    Mathf.Clamp(_camera.orthographicSize * (1f - 0.1f * Mathf.Sign(scroll)), 1.5f, 20f);
            }
        }

        private void OnGUI()
        {
            // Display only — no IMGUI input is relied upon.
            GUILayout.BeginArea(new Rect(12f, 12f, 340f, 72f), GUI.skin.box);
            GUILayout.Label($"Tick {_host.Current.CurrentTick.Value}   {(_host.Playing ? "▶ playing" : "❚❚ paused")}");
            GUILayout.Label("Space  Play/Pause    S  Step    R  Reset");
            GUILayout.EndArea();

            // Observation panel — facts only, never words.
            if (_hasSelection)
            {
                DemoWorldDefinition world = _host.World;
                SimulationState snapshot = _host.Current;

                int outgoing = world.Relations.OutgoingFrom(_selected).Count;
                int incoming = 0;
                foreach (Place place in world.Places)
                {
                    IReadOnlyList<Relation> from = world.Relations.OutgoingFrom(place);
                    for (int i = 0; i < from.Count; i++)
                    {
                        if (from[i].Target == _selected)
                        {
                            incoming++;
                        }
                    }
                }

                GUILayout.BeginArea(new Rect(12f, Screen.height - 132f, 250f, 120f), GUI.skin.box);
                GUILayout.Label($"Place {_selected.Value}");
                GUILayout.Label($"{world.KindName(world.QuantityKind)} : {snapshot.ValueAt(new Cell(_selected, world.QuantityKind))}");
                GUILayout.Label($"{world.KindName(world.RateKind)} : {snapshot.ValueAt(new Cell(_selected, world.RateKind))}");
                GUILayout.Label($"Outgoing : {outgoing}   Incoming : {incoming}");
                GUILayout.EndArea();
            }
        }

        private Camera ConfigureCamera()
        {
            Camera observerCamera = Camera.main;
            if (observerCamera == null)
            {
                var cameraObject = new GameObject("Observer Camera");
                observerCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            observerCamera.orthographic = true;
            observerCamera.orthographicSize = 4f;
            observerCamera.transform.position = new Vector3(0f, 0f, -10f);
            observerCamera.transform.rotation = Quaternion.identity;
            observerCamera.clearFlags = CameraClearFlags.SolidColor;
            observerCamera.backgroundColor = new Color(0.043f, 0.055f, 0.071f);
            return observerCamera;
        }
    }
}
