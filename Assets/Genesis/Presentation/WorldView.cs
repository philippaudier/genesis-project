using System.Collections.Generic;
using Genesis.Simulation;
using UnityEngine;

namespace Genesis.Presentation
{
    /// <summary>
    /// The light table. Builds the minimal visual vocabulary — a circle per place, a thin line per
    /// relation — and refreshes it every frame from the current snapshot. Everything visible is
    /// derivable (Genesis-013, Invariant 3): the fill is the quantity normalised by the maximum
    /// currently observed; the halo exists iff a positive rate exists; positions derive from place
    /// identity order. No sprites, no meshes beyond primitives, no effects.
    /// </summary>
    public sealed class WorldView
    {
        private static readonly Color BackgroundFill = new Color(0.10f, 0.12f, 0.16f);
        private static readonly Color HaloColor = new Color(1.00f, 0.72f, 0.30f);
        private static readonly Color LineColor = new Color(0.25f, 0.28f, 0.34f);

        private readonly Dictionary<Place, GameObject> _circles = new Dictionary<Place, GameObject>();
        private readonly Dictionary<Place, GameObject> _halos = new Dictionary<Place, GameObject>();
        private readonly Dictionary<Place, Material> _fillMaterials = new Dictionary<Place, Material>();
        private readonly Dictionary<Collider, Place> _pickables = new Dictionary<Collider, Place>();
        private readonly Dictionary<Place, Vector3> _positions = new Dictionary<Place, Vector3>();

        private Shader _unlit;

        public void Build(DemoWorldDefinition world, Transform root)
        {
            _unlit = Shader.Find("Universal Render Pipeline/Unlit");

            // Positions derive deterministically from place identity order: evenly spaced on X.
            var ordered = new List<Place>(world.Places);
            ordered.Sort();
            for (int i = 0; i < ordered.Count; i++)
            {
                float x = (i - (ordered.Count - 1) * 0.5f) * 3f;
                _positions[ordered[i]] = new Vector3(x, 0f, 0f);
            }

            foreach (Place place in ordered)
            {
                Vector3 position = _positions[place];

                GameObject halo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                halo.name = $"Halo {place.Value}";
                halo.transform.SetParent(root, false);
                halo.transform.position = position + new Vector3(0f, 0f, 0.5f);
                Object.Destroy(halo.GetComponent<Collider>());
                halo.GetComponent<Renderer>().material = NewUnlit(HaloColor);
                halo.SetActive(false);
                _halos[place] = halo;

                GameObject circle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                circle.name = $"Place {place.Value}";
                circle.transform.SetParent(root, false);
                circle.transform.position = position;
                Material fill = NewUnlit(BackgroundFill);
                circle.GetComponent<Renderer>().material = fill;
                _circles[place] = circle;
                _fillMaterials[place] = fill;
                _pickables[circle.GetComponent<Collider>()] = place;
            }

            // One thin line per directed relation; a symmetric pair overlaps into one quiet stroke.
            foreach (Place place in ordered)
            {
                IReadOnlyList<Relation> outgoing = world.Relations.OutgoingFrom(place);
                for (int i = 0; i < outgoing.Count; i++)
                {
                    var lineObject = new GameObject($"Relation {outgoing[i].Source.Value}-{outgoing[i].Target.Value}");
                    lineObject.transform.SetParent(root, false);
                    var line = lineObject.AddComponent<LineRenderer>();
                    line.material = NewUnlit(LineColor);
                    line.widthMultiplier = 0.035f;
                    line.positionCount = 2;
                    line.SetPosition(0, _positions[outgoing[i].Source] + new Vector3(0f, 0f, 1f));
                    line.SetPosition(1, _positions[outgoing[i].Target] + new Vector3(0f, 0f, 1f));
                }
            }
        }

        public void Refresh(SimulationState snapshot, DemoWorldDefinition world)
        {
            long max = 1;
            foreach (Place place in world.Places)
            {
                long value = snapshot.ValueAt(new Cell(place, world.QuantityKind));
                if (value > max)
                {
                    max = value;
                }
            }

            foreach (Place place in world.Places)
            {
                long quantity = snapshot.ValueAt(new Cell(place, world.QuantityKind));
                float t = Mathf.Clamp01(quantity / (float)max);
                _fillMaterials[place].color = Color.Lerp(BackgroundFill, Color.white, t);

                long rate = snapshot.ValueAt(new Cell(place, world.RateKind));
                GameObject halo = _halos[place];
                halo.SetActive(rate > 0);
                if (rate > 0)
                {
                    float scale = 1.35f + 0.12f * Mathf.Min(rate, 8);
                    halo.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }

        public bool TryPick(Ray ray, out Place place)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 200f) && _pickables.TryGetValue(hit.collider, out place))
            {
                return true;
            }

            place = default;
            return false;
        }

        private Material NewUnlit(Color color)
        {
            var material = new Material(_unlit);
            material.color = color;
            return material;
        }
    }
}
