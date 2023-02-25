using System;

namespace Gameframe.Procgen
{
    public static class FractalUtility
    {
        public static NoiseSample Sample1D(Func<float, uint, float, NoiseSample> action, float x, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = action(x, seed, frequency);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += action(x, seed, frequency) * amplitude;
            }
            return sum * (1 / range);
        }

        public static float Float1D(Func<float, uint, float, float> action, float x, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = action(x, seed, frequency);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += action(x, seed, frequency) * amplitude;
            }
            return sum * (1 / range);
        }

        public static NoiseSample Sample2D(Func<float, float, uint, float, NoiseSample> action, float x, float y, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = action(x, y, seed, frequency);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += action(x, y, seed, frequency) * amplitude;
            }
            return sum * (1 / range);
        }

        public static float Float2D(Func<float, float, uint, float, float> action, float x, float y, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = action(x, y, seed, frequency);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += action(x, y, seed, frequency) * amplitude;
            }
            return sum * (1 / range);
        }

        public static NoiseSample Sample3D(Func<float, float, float, uint, float, NoiseSample> action, float x, float y, float z, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = action(x, y, z, seed, frequency);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += action(x, y, z, seed, frequency) * amplitude;
            }
            return sum * (1 / range);
        }

        public static float Float3D(Func<float, float, float, uint, float, float> action, float x, float y, float z, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = action(x, y, z, seed, frequency);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += action(x, y, z, seed, frequency) * amplitude;
            }
            return sum * (1 / range);
        }
    }
}
