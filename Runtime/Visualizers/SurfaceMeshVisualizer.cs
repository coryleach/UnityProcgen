using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SurfaceMeshVisualizer : MonoBehaviour
    {
        [SerializeField] private uint seed = 100;

        [SerializeField] private float strength = 1f;

        [SerializeField] private int resolution = 10;

        [SerializeField] private float frequency = 1f;

        [SerializeField] [Range(1, 8)] private int octaves = 1;

        [SerializeField] [Range(1f, 4f)] private float lacunarity = 2f;

        [SerializeField] [Range(0f, 1f)] private float persistence = 0.5f;

        [SerializeField] private Gradient coloring;

        [SerializeField] private Vector3 offset;

        [SerializeField] private bool showNormals;
        [SerializeField] private float normalSize = 0.1f;

        private Mesh mesh;
        private int currentResolution;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Color[] colors;

        private MeshFilter _meshFilter;

        public MeshFilter MeshFilter
        {
            get
            {
                if (_meshFilter == null)
                {
                    _meshFilter = GetComponent<MeshFilter>();
                }

                return _meshFilter;
            }
        }

        public enum Dimension
        {
            Value1D,
            Value2D,
            Value3D,
            Perlin1D,
            Perlin2D,
            Perlin3D,
            SimplexValue1D,
            SimplexValue2D,
            SimplexValue3D,
            SimplexGradient1D,
            SimplexGradient2D,
            SimplexGradient3D
        }

        [SerializeField] private Dimension dimension = Dimension.Value2D;

        private Vector3 prevLocation;

        private void OnEnable()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = "Surface Mesh";
                MeshFilter.mesh = mesh;
            }

            Refresh();
        }

        private void Update()
        {
            if (prevLocation != transform.position)
            {
                Refresh();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (showNormals && vertices != null)
            {
                Gizmos.color = Color.yellow;
                for (int v = 0; v < vertices.Length; v++)
                {
                    Gizmos.DrawRay( transform.TransformPoint(vertices[v]), normals[v] * normalSize);
                }
            }
        }

        float minSample = float.MaxValue;
        float maxSample = float.MinValue;

        [ContextMenu("ResetMinMax")]
        public void ResetMinMax()
        {
            minSample = float.MaxValue;
            maxSample = float.MinValue;
        }

        [ContextMenu("Refresh")]
        private void Refresh()
        {
            if (currentResolution != resolution || mesh == null || vertices == null ||
                vertices.Length != ((resolution + 1) * (resolution + 1)))
            {
                CreateMesh();
            }

            prevLocation = transform.position;
            var point00 = transform.TransformPoint(new Vector3(-0.5f, 0, -0.5f)) + offset;
            var point10 = transform.TransformPoint(new Vector3(0.5f, 0, -0.5f)) + offset;
            var point01 = transform.TransformPoint(new Vector3(-0.5f, 0, 0.5f)) + offset;
            var point11 = transform.TransformPoint(new Vector3(0.5f, 0, 0.5f)) + offset;

            var stepSize = 1f / resolution;
            for (int v = 0, y = 0; y <= resolution; y++)
            {
                var point0 = Vector3.Lerp(point00, point01, y * stepSize);
                var point1 = Vector3.Lerp(point10, point11, y * stepSize);
                for (var x = 0; x <= resolution; x++, v++)
                {
                    var point = Vector3.Lerp(point0, point1, x * stepSize);
                    var sample = Noise(point);
                    minSample = Mathf.Min(sample.value, minSample);
                    maxSample = Mathf.Max(sample.value, maxSample);
                    vertices[v].y = sample.value * strength;
                    colors[v] = coloring.Evaluate(sample.value);
                    normals[v] = new Vector3(-sample.derivative.x, 1, -sample.derivative.z).normalized;
                }
            }

            Debug.Log($"Min: {minSample} Max:{maxSample}");

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.colors = colors;
        }

        [ContextMenu("Force Refresh")]
        private void ForceRefresh()
        {
            currentResolution = 0;
            Refresh();
        }

        private NoiseSample Noise(Vector3 point)
        {
            var sample = new NoiseSample();

            switch (dimension)
            {
                case Dimension.Value1D:
                    //sample = ValueNoise.Sample1D(point.x, seed, frequency);
                    sample = ValueNoise.FractalSample1D(point.x, seed:seed, frequency:frequency, octaves:octaves, lacunarity:lacunarity, persistence:persistence);
                    //v = ValueNoise.Fractal1D(point.x * frequency, seed, frequency, octaves, lacunarity, persistence);
                    break;
                case Dimension.Value2D:
                    sample = ValueNoise.FractalSample2D(point * frequency, seed, frequency, octaves, lacunarity, persistence);
                    break;
                case Dimension.Value3D:
                    sample = ValueNoise.FractalSample3D(point * frequency, seed, frequency, octaves, lacunarity, persistence);
                    break;
                case Dimension.Perlin1D:
                    sample = PerlinGradientNoise.FractalSample1D(point.x, seed, frequency, octaves, lacunarity, persistence);
                    break;
                case Dimension.Perlin2D:
                    sample = PerlinGradientNoise.FractalSample2D(point.x, point.y, seed, frequency, octaves, lacunarity, persistence);
                    break;
                case Dimension.Perlin3D:
                    sample = PerlinGradientNoise.FractalSample3D(point.x, point.y, point.z, seed, frequency, octaves, lacunarity, persistence);
                    break;
                case Dimension.SimplexValue1D:
                    sample = SimplexGradientNoise.Value1D(point.x, seed, frequency);
                    break;
                case Dimension.SimplexValue2D:
                    sample = SimplexGradientNoise.Value2D(point.x, point.y, seed, frequency);
                    break;
                case Dimension.SimplexValue3D:
                    sample = SimplexGradientNoise.Value3D(point.x, point.y, point.z, seed, frequency);
                    break;
                case Dimension.SimplexGradient1D:
                    sample = SimplexGradientNoise.Gradient1D(point.x, seed, frequency);
                    break;
                case Dimension.SimplexGradient2D:
                    sample = SimplexGradientNoise.Gradient2D(point.x, point.y, seed, frequency);
                    break;
                case Dimension.SimplexGradient3D:
                    sample = SimplexGradientNoise.Gradient3D(point.x, point.y, point.z, seed, frequency);
                    break;
            }
            return sample;
        }

        private void CreateMesh()
        {
            Debug.Log("Create Mesh");
            currentResolution = resolution;

            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = "Surface Mesh";
                MeshFilter.mesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            vertices = new Vector3[(resolution + 1) * (resolution + 1)];
            colors = new Color[vertices.Length];
            normals = new Vector3[vertices.Length];
            var uv = new Vector2[vertices.Length];
            var stepSize = 1f / resolution;
            for (int v = 0, y = 0; y <= resolution; y++)
            {
                for (var x = 0; x <= resolution; x++, v++)
                {
                    vertices[v] = new Vector3(x * stepSize - 0.5f, 0, y * stepSize - 0.5f);
                    normals[v] = Vector3.up;
                    uv[v] = new Vector2(x * stepSize, y * stepSize);
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;

            var triangles = new int[resolution * resolution * 6];
            for (int t = 0, v = 0, y = 0; y < resolution; y++, v++)
            {
                for (var x = 0; x < resolution; x++, v++, t += 6)
                {
                    triangles[t] = v;
                    triangles[t + 1] = v + resolution + 1;
                    triangles[t + 2] = v + 1;
                    triangles[t + 3] = v + 1;
                    triangles[t + 4] = v + resolution + 1;
                    triangles[t + 5] = v + resolution + 2;
                }
            }

            mesh.triangles = triangles;
        }

        private void OnValidate()
        {
            Refresh();
        }
    }
}
