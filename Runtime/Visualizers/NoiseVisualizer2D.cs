using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Gameframe.Procgen
{
    [ExecuteAlways]
    public class NoiseVisualizer2D : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        [SerializeField] private int textureResolution = 256;

        [SerializeField] private uint seed = 100;
        [SerializeField] private float frequency = 1;
        [SerializeField] [Range(1, 8)] private int octaves = 1;
        [SerializeField] private float lacunarity = 2f;
        [SerializeField] private float persistence = 0.5f;

        [SerializeField] private FilterMode filterMode = FilterMode.Point;

        private Texture2D _texture;

        private float minValue = float.MaxValue;
        private float maxValue = float.MinValue;

        [ContextMenu("ResetMinMax")]
        public void ResetMinMax()
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;
        }

        public enum Dimension
        {
            Value1D,
            Value2D,
            Value3D,
            Perlin1D,
            Perlin2D,
            Perlin3D,
            SamplePerlin2D,
            SamplePerlin3D,
            SimplexValue1D,
            SimplexValue2D,
        }

        [SerializeField] private Dimension dimension = Dimension.Value2D;

        [SerializeField] private Vector3 offset;

        private void OnEnable()
        {
            Generate();
        }

        private void OnDisable()
        {
            ClearTexture();
        }

        private Texture2D CreateTexture()
        {
            var tex = new Texture2D(textureResolution, textureResolution)
            {
                filterMode = filterMode
            };
            return tex;
        }

        private void Update()
        {
            Generate();
        }

        [ContextMenu("Generate")]
        private void Generate()
        {
            if (_texture != null && (_texture.width != textureResolution || _texture.height != textureResolution))
            {
                ClearTexture();
            }

            if (_texture == null)
            {
                _texture = CreateTexture();
            }

            _texture.filterMode = filterMode;

            var point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
            var point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
            var point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
            var point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

            var stepSize = 1f / textureResolution;

            for (var y = 0; y < textureResolution; y++)
            {
                var point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
                var point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);

                for (var x = 0; x < textureResolution; x++)
                {
                    var point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize) + offset;
                    var v = 0f;

                    switch (dimension)
                    {
                        case Dimension.Value1D:
                            v = ValueNoise.Fractal1D(point.x * frequency, seed, frequency, octaves, lacunarity,
                                persistence);
                            break;
                        case Dimension.Value2D:
                            v = ValueNoise.Fractal2D(point * frequency, seed, frequency, octaves, lacunarity,
                                persistence);
                            break;
                        case Dimension.Value3D:
                            v = ValueNoise.Fractal3D(point * frequency, seed, frequency, octaves, lacunarity,
                                persistence);
                            break;
                        case Dimension.Perlin1D:
                            v = PerlinGradientNoise.Fractal1D(point.x, seed, frequency, octaves, lacunarity,
                                persistence);
                            break;
                        case Dimension.Perlin2D:
                            v = PerlinGradientNoise.Fractal2D(point.x, point.y, seed, frequency, octaves, lacunarity,
                                persistence);
                            break;
                        case Dimension.Perlin3D:
                            v = PerlinGradientNoise.Fractal3D(point.x, point.y, point.z, seed, frequency, octaves,
                                lacunarity, persistence);
                            break;
                        case Dimension.SamplePerlin2D:
                            v = PerlinGradientNoise.FractalSample2D(point.x, point.y, seed, frequency, octaves,
                                lacunarity, persistence).value;
                            break;
                        case Dimension.SamplePerlin3D:
                            v = PerlinGradientNoise.FractalSample3D(point.x, point.y, point.z, seed, frequency, octaves,
                                lacunarity, persistence).value;
                            break;
                        case Dimension.SimplexValue1D:
                            v = SimplexGradientNoise.SimplexValue1D(point.x, seed, frequency).value;
                            break;
                        case Dimension.SimplexValue2D:
                            v = SimplexGradientNoise.SimplexValue2D(point.x, point.y, seed, frequency).value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    minValue = Mathf.Min(v, minValue);
                    maxValue = Mathf.Max(v, maxValue);

                    _texture.SetPixel(x, y, new Color(v, v, v, 1f));
                }
            }

            Debug.Log($"Min: {minValue} Max: {maxValue}");

            _texture.Apply();

            _renderer.sharedMaterial.mainTexture = _texture;
        }

        [ContextMenu("SaveTexture")]
        private void SaveTexture()
        {
            if (_texture == null)
            {
                Debug.LogError("Texture does not exist");
                return;
            }

            TextureUtility.SaveTextureAsPNG(_texture, Application.dataPath + "GeneratedTexture.png");
        }

        private void ClearTexture()
        {
            if (_texture != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_texture);
                }
                else
                {
                    DestroyImmediate(_texture);
                }
            }
        }

        private void OnValidate()
        {
            if (_texture != null)
            {
                _texture.filterMode = filterMode;
            }
        }
    }
}
