using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/HeightMapLayerGenerator")]
    public class HeightMapLayerGenerator : WorldMapLayerGenerator
    {
        public enum MapType
        {
            ValueNoise,
            SimplexNoise,
            PerlinNoise,
            Falloff,
            SimplexNoiseWithFalloff,
            PerlinNoiseWithFalloff,
            ValueNoiseWithFalloff
        }

        [SerializeField] private bool step = false;

        [SerializeField] private int stepCount = 10;

        [FormerlySerializedAs("noiseScale")] [SerializeField] private float frequency = 1;

        [SerializeField] private int octaves = 4;

        [SerializeField, Range(0f, 1f)] private float persistence = 0.5f;

        [SerializeField] private float lacunarity = 2f;

        [SerializeField] private float falloffA = 3f;

        [SerializeField] private float falloffB = 2.2f;

        [SerializeField] private Vector2 falloffOffset = Vector2.zero;

        [SerializeField] private MapType mapType = MapType.SimplexNoise;

        public HeightMapLayerData Generate(int width, int height, uint seed)
        {
            var heightMap = new float[width*height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = y * width + x;
                    switch (mapType)
                    {
                        case MapType.ValueNoise:
                            heightMap[i] = ValueNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                            break;
                        case MapType.PerlinNoise:
                            heightMap[i] = PerlinGradientNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                            break;
                        case MapType.SimplexNoise:
                            heightMap[i] = SimplexGradientNoise.FractalGradient2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence).value;
                            break;
                        case MapType.Falloff:
                            heightMap[i] = Noise.GenerateFalloffPoint(x , y , width, height, falloffA, falloffB, falloffOffset);
                            break;
                        case MapType.SimplexNoiseWithFalloff:
                        {
                            var falloff = Noise.GenerateFalloffPoint(x , y, width, height, falloffA, falloffB, falloffOffset);
                            heightMap[i] = SimplexGradientNoise.FractalGradient2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence).value;
                            heightMap[i] *= falloff;
                        }
                            break;
                        case MapType.PerlinNoiseWithFalloff:
                        {
                            var falloff = Noise.GenerateFalloffPoint(x, y, width, height, falloffA, falloffB, falloffOffset);
                            heightMap[i] = PerlinGradientNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                            heightMap[i] *= falloff;
                        }
                            break;
                        case MapType.ValueNoiseWithFalloff:
                        {
                            var falloff = Noise.GenerateFalloffPoint(x, y, width, height, falloffA, falloffB, falloffOffset);
                            heightMap[i] = ValueNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                            heightMap[i] *= falloff;
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (step)
                    {
                        heightMap[i] = Mathf.RoundToInt(heightMap[i] * stepCount) / (float)stepCount;
                    }
                }
            }

            return new HeightMapLayerData
            {
                heightMap = heightMap
            };
        }

        public override void AddToWorld(WorldMapData worldMapData)
        {
            worldMapData.layers.Add(Generate(worldMapData.width,worldMapData.height,(uint)worldMapData.seed));
        }

        private void OnValidate()
        {
            if (lacunarity < 1)
            {
                lacunarity = 1;
            }
            if (octaves < 0)
            {
                octaves = 0;
            }
        }
    }

}
