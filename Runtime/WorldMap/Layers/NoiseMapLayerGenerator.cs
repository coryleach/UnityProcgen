using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/Noise Map")]
    public class NoiseMapLayerGenerator : RandomAccessFloatGenerationLayer
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

        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        public enum NoiseValueRange
        {
            ZeroToOne,
            NegOneToOne,
            AbsoluteValue
        }

        [SerializeField] private NoiseValueRange valueRange = NoiseValueRange.ZeroToOne;

        private float ApplyNoiseRange(float v)
        {
            switch (valueRange)
            {
                case NoiseValueRange.ZeroToOne:
                    return v;
                case NoiseValueRange.NegOneToOne:
                    return v * 2f - 1;
                case NoiseValueRange.AbsoluteValue:
                    return Mathf.Abs(v * 2f - 1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override float Generate(int x, int y, int width, int height, int seed)
        {
            float v = 0;
            switch (mapType)
            {
                case MapType.ValueNoise:
                    v = ValueNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                    break;
                case MapType.PerlinNoise:
                    v = PerlinGradientNoise.Fractal2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence);
                    break;
                case MapType.SimplexNoise:
                    v = SimplexGradientNoise
                        .FractalGradient2D(x, y, (uint) seed, frequency, octaves, lacunarity, persistence).value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            v = ApplyNoiseRange(v);

            if (applySmoothingCurve)
            {
                v = curve.Evaluate(v);
            }

            return Mathf.Clamp01(v);
        }

        protected override IWorldMapLayerData GenerateLayer(WorldMapData mapData, int layerSeed)
        {
            var map = GenerateMap(mapData.width, mapData.height, layerSeed);
            return new FloatMapLayerData
            {
                FloatMap = map
            };
        }
    }
}
