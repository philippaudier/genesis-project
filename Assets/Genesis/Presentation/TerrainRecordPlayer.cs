using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Genesis.Presentation
{
    /// <summary>
    /// Replays a world the simulation already lived — nothing more. This component holds no law,
    /// no transition, no state: it reads a record written headless by a laboratory and shows it.
    /// Presentation observes what already exists (Invariant 1/2; the Science-001 charter's
    /// "headless first"). It is also the presentation half of the Record/Replay question the
    /// corpus has open (Q-G3) — arriving as a viewer, not as a decision.
    ///
    /// Usage: put this on an empty GameObject in any scene and press Play. The record is produced
    /// by <c>Lab/Demo-001$ dotnet run</c>.
    /// </summary>
    [AddComponentMenu("Genesis/Terrain Record Player")]
    public sealed class TerrainRecordPlayer : MonoBehaviour
    {
        [Header("Record")]
        [Tooltip("Path to the record, relative to the project folder (the parent of Assets/).")]
        public string RecordPath = "Lab/Demo-001/Record/demo-001.record";

        [Header("Look")]
        public float HeightScale = 0.14f;
        [Tooltip("Water depth that reads as fully deep.")]
        public float DeepAt = 25f;
        public bool ShowWaterInHeight = true;

        [Header("Playback")]
        public float TicksPerSecond = 24f;
        public bool Playing = true;
        public bool Loop = true;

        private int _width;
        private int _height;
        private int _tickCount;
        private long[] _elevation;
        private readonly List<long[]> _water = new List<long[]>();
        private float[] _shade;

        private Mesh _mesh;
        private Vector3[] _vertices;
        private Texture2D _texture;
        private Color32[] _pixels;

        private int _tick;
        private float _accumulator;
        private string _error;

        private static readonly Color DryLow = new Color(0.38f, 0.34f, 0.26f);
        private static readonly Color DryHigh = new Color(0.72f, 0.70f, 0.64f);
        private static readonly Color WaterShallow = new Color(0.35f, 0.72f, 0.78f);
        private static readonly Color WaterDeep = new Color(0.06f, 0.20f, 0.48f);
        private static readonly Color Negative = new Color(0.85f, 0.15f, 0.20f);

        private void Start()
        {
            try
            {
                Load(Path.GetFullPath(Path.Combine(Application.dataPath, "..", RecordPath)));
            }
            catch (Exception exception)
            {
                _error = exception.Message;
                return;
            }

            MeasureElevation();
            BuildShade();
            BuildMesh();
            BuildTexture();
            EnsureCamera();
            Apply(0);
        }

        private void Update()
        {
            if (_error != null)
            {
                return;
            }

            ReadKeys();

            if (Playing && _tickCount > 1)
            {
                _accumulator += Time.deltaTime * Mathf.Max(0.01f, TicksPerSecond);
                while (_accumulator >= 1f)
                {
                    _accumulator -= 1f;
                    int next = _tick + 1;
                    if (next >= _tickCount)
                    {
                        if (!Loop)
                        {
                            Playing = false;
                            break;
                        }

                        next = 0;
                    }

                    Apply(next);
                }
            }
        }

        private void ReadKeys()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                Playing = !Playing;
            }

            if (keyboard.rKey.wasPressedThisFrame)
            {
                Apply(0);
            }

            if (keyboard.rightArrowKey.wasPressedThisFrame)
            {
                Playing = false;
                Apply(Mathf.Min(_tick + 1, _tickCount - 1));
            }

            if (keyboard.leftArrowKey.wasPressedThisFrame)
            {
                Playing = false;
                Apply(Mathf.Max(_tick - 1, 0));
            }
        }

        // --- record ------------------------------------------------------------------------

        private void Load(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"No record at {fullPath}");
            }

            _water.Clear();
            foreach (string line in File.ReadLines(fullPath))
            {
                if (line.Length == 0)
                {
                    continue;
                }

                if (line.StartsWith("grid ", StringComparison.Ordinal))
                {
                    string[] parts = line.Split(' ');
                    _width = int.Parse(parts[1], CultureInfo.InvariantCulture);
                    _height = int.Parse(parts[2], CultureInfo.InvariantCulture);
                }
                else if (line.StartsWith("elevation ", StringComparison.Ordinal))
                {
                    _elevation = ParseValues(line, 1);
                }
                else if (line.StartsWith("w ", StringComparison.Ordinal))
                {
                    _water.Add(ParseValues(line, 2));
                }
            }

            if (_width <= 0 || _elevation == null || _water.Count == 0)
            {
                throw new InvalidDataException("The record is incomplete.");
            }

            _tickCount = _water.Count;
        }

        private static long[] ParseValues(string line, int skip)
        {
            string[] parts = line.Split(' ');
            var values = new long[parts.Length - skip];
            for (int i = skip; i < parts.Length; i++)
            {
                values[i - skip] = long.Parse(parts[i], CultureInfo.InvariantCulture);
            }

            return values;
        }

        // --- building ----------------------------------------------------------------------

        /// <summary>A slope shade baked from elevation alone, so relief reads without any light.</summary>
        private void BuildShade()
        {
            _shade = new float[_width * _height];
            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    int index = row * _width + col;
                    long left = _elevation[row * _width + Mathf.Max(col - 1, 0)];
                    long right = _elevation[row * _width + Mathf.Min(col + 1, _width - 1)];
                    long up = _elevation[Mathf.Max(row - 1, 0) * _width + col];
                    long down = _elevation[Mathf.Min(row + 1, _height - 1) * _width + col];
                    var normal = new Vector3(-(right - left) * HeightScale, 2f, -(down - up) * HeightScale).normalized;
                    var sun = new Vector3(-0.4f, 0.82f, -0.4f).normalized;
                    _shade[index] = Mathf.Clamp01(0.35f + 0.65f * Vector3.Dot(normal, sun));
                }
            }
        }

        private void BuildMesh()
        {
            _mesh = new Mesh { name = "GenesisTerrain" };
            _vertices = new Vector3[_width * _height];
            var uvs = new Vector2[_width * _height];
            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    int index = row * _width + col;
                    _vertices[index] = new Vector3(col, _elevation[index] * HeightScale, -row);
                    uvs[index] = new Vector2((col + 0.5f) / _width, 1f - (row + 0.5f) / _height);
                }
            }

            var triangles = new int[(_width - 1) * (_height - 1) * 6];
            int t = 0;
            for (int row = 0; row < _height - 1; row++)
            {
                for (int col = 0; col < _width - 1; col++)
                {
                    int a = row * _width + col;
                    int b = a + 1;
                    int c = a + _width;
                    int d = c + 1;
                    // Wound so the front faces point up: with x = col and z = -row, the order
                    // (a, b, c) gives a +Y normal. The reverse hides the world under itself.
                    triangles[t++] = a; triangles[t++] = b; triangles[t++] = c;
                    triangles[t++] = b; triangles[t++] = d; triangles[t++] = c;
                }
            }

            _mesh.vertices = _vertices;
            _mesh.uv = uvs;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();

            MeshFilter filter = gameObject.GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = _mesh;
            renderer.sharedMaterial = new Material(FindShader());
        }

        private static Shader FindShader()
        {
            string[] candidates =
            {
                "Universal Render Pipeline/Unlit",
                "Unlit/Texture",
                "Sprites/Default",
            };

            foreach (string name in candidates)
            {
                Shader shader = Shader.Find(name);
                if (shader != null)
                {
                    return shader;
                }
            }

            return Shader.Find("Standard");
        }

        private void BuildTexture()
        {
            _texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };
            _pixels = new Color32[_width * _height];
        }

        private void EnsureCamera()
        {
            if (Camera.main != null)
            {
                return;
            }

            var holder = new GameObject("Genesis Observer Camera");
            Camera camera = holder.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.09f, 0.11f);
            holder.transform.position = new Vector3(_width * 0.5f, _width * 0.85f, _height * 0.75f);
            holder.transform.LookAt(new Vector3(_width * 0.5f, 0f, -_height * 0.5f));
        }

        // --- one tick ----------------------------------------------------------------------

        private void Apply(int tick)
        {
            _tick = Mathf.Clamp(tick, 0, _tickCount - 1);
            long[] water = _water[_tick];

            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    int index = row * _width + col;
                    long elevation = _elevation[index];
                    long depth = index < water.Length ? water[index] : 0;

                    if (ShowWaterInHeight)
                    {
                        _vertices[index].y = (elevation + (depth > 0 ? depth : 0)) * HeightScale;
                    }

                    // Texture rows run bottom-up; the mesh's row 0 is the far edge.
                    _pixels[(_height - 1 - row) * _width + col] = Colour(elevation, depth, _shade[index]);
                }
            }

            _mesh.vertices = _vertices;
            _mesh.RecalculateNormals();
            _texture.SetPixels32(_pixels);
            _texture.Apply(false);
            GetComponent<MeshRenderer>().sharedMaterial.mainTexture = _texture;
        }

        private float _elevationMin;
        private float _elevationRange = 1f;

        private void MeasureElevation()
        {
            long min = long.MaxValue, max = long.MinValue;
            foreach (long value in _elevation)
            {
                if (value < min) min = value;
                if (value > max) max = value;
            }

            _elevationMin = min;
            _elevationRange = Mathf.Max(1f, max - min);
        }

        private Color32 Colour(long elevation, long depth, float shade)
        {
            Color colour;
            if (depth < 0)
            {
                // Positivity is not a theorem here (Obs-004). An instrument never hides what it knows.
                colour = Color.Lerp(Negative * 0.6f, Negative, Mathf.Clamp01(-depth / 10f));
            }
            else if (depth == 0)
            {
                float high = Mathf.Clamp01((elevation - _elevationMin) / _elevationRange);
                colour = Color.Lerp(DryLow, DryHigh, high) * shade;
            }
            else
            {
                Color water = Color.Lerp(WaterShallow, WaterDeep, Mathf.Clamp01(depth / Mathf.Max(1f, DeepAt)));
                colour = water * (0.65f + 0.35f * shade);
            }

            colour.a = 1f;
            return colour;
        }

        // --- the panel ----------------------------------------------------------------------

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 13;
            if (_error != null)
            {
                GUI.Label(new Rect(14, 12, 900, 120),
                    $"No record to replay.\n{_error}\n\nGenerate one:  cd Lab/Demo-001  &&  dotnet run");
                return;
            }

            long[] water = _water[_tick];
            long total = 0, deepest = 0, lowest = 0;
            int wet = 0, negative = 0;
            foreach (long value in water)
            {
                total += value;
                if (value > 0) wet++;
                if (value < 0) negative++;
                if (value > deepest) deepest = value;
                if (value < lowest) lowest = value;
            }

            GUI.Label(new Rect(14, 10, 620, 22), $"tick {_tick} / {_tickCount - 1}      {(Playing ? "playing" : "paused")}");
            GUI.Label(new Rect(14, 30, 620, 22), $"water present {total}      wet cells {wet} / {water.Length}      deepest {deepest}");
            GUI.Label(new Rect(14, 50, 620, 22), negative > 0
                ? $"negative cells {negative}  (lowest {lowest}) — shown in red"
                : "no negative value anywhere");
            GUI.Label(new Rect(14, 74, 620, 22), "space play/pause     ← → step     R restart");
        }
    }
}
