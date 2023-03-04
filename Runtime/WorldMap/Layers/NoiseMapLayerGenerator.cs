using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/Noise Map")]
    public class NoiseMapLayerGenerator : WorldMapLayerGenerator
    {
        public enum MapType
        {
            ValueNoise,
            SimplexNoise,
            PerlinNoise,
        }

        [SerializeField] private MapType mapType = MapType.SimplexNoise;

        [SerializeField] private float frequency = 1;

        [SerializeField] private int octaves = 4;

        [SerializeField, Range(0f, 1f)] private float persistence = 0.5f;

        [SerializeField] private float lacunarity = 2f;

        [SerializeField] private bool applySmoothingCurve = false;

        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0,0,1, 1);

        private float[] Generate(int width, int height, uint seed)
        {
            var floatMap = new float[width*height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = y * width + x;
                    switch (mapType)
                    {
                        case MapType.ValueNoise:
                            floatMap[i] = ValueNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                            break;
                        case MapType.PerlinNoise:
                            floatMap[i] = PerlinGradientNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                            break;
                        case MapType.SimplexNoise:
                            floatMap[i] = SimplexGradientNoise.FractalGradient2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence).value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (applySmoothingCurve)
                    {
                        floatMap[i] = curve.Evaluate(floatMap[i]);
                    }
                }
            }

            return floatMap;
        }

        protected override IWorldMapLayerData GenerateLayer(WorldMapData mapData, int layerSeed)
        {
            var map = Generate(mapData.width,mapData.height, (uint)layerSeed);
            return new FloatMapLayerData
            {
                FloatMap = map
            };
        }
    }
}
