using UnityEngine;

namespace Gameframe.Procgen
{
    /// <summary>
    /// Stateless and random perlin noise generation methods
    /// Perlin noise has a smoother and natural gradient appearance when visualized
    /// Includes methods for fractal generation
    /// </summary>
    public static class PerlinGradientNoise
    {
        private static readonly float Sqr2 = Mathf.Sqrt(2f);

        private const int GradientsMask1D = 1;
        private static readonly float[] Gradients1D = new[] {1f, -1f};

        private const int GradientsMask2D = 7;

        private static readonly Vector2[] Gradients2D =
        {
            new Vector2(1f, 0f),
            new Vector2(-1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0f, -1f),
            new Vector2(1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2(1f, -1f).normalized,
            new Vector2(-1f, -1f).normalized
        };

        private const int GradientsMask3D = 15;

        private static readonly Vector3[] Gradients3D =
        {
            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3(1f, -1f, 0f),
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3(1f, 0f, -1f),
            new Vector3(-1f, 0f, -1f),
            new Vector3(0f, 1f, 1f),
            new Vector3(0f, -1f, 1f),
            new Vector3(0f, 1f, -1f),
            new Vector3(0f, -1f, -1f),

            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3(0f, -1f, 1f),
            new Vector3(0f, -1f, -1f)
        };

        #region Fractal Noise

        /// <summary>
        /// 1 dimensional fractal perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Random value between 0 and 1</returns>
        public static float Fractal1D(float x, uint seed, float frequency, int octaves, float lacunarity = 2f,
            float persistence = 0.5f)
        {
            var sum = Noise1D(x * frequency, seed);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += Noise1D(x * frequency, seed) * amplitude;
            }

            return sum / range;
        }

        /// <summary>
        /// 2 dimensional fractal perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Random value between 0 and 1</returns>
        public static float Fractal2D(float x, float y, uint seed, float frequency, int octaves,
            float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = Noise2D(x * frequency, y * frequency, seed);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += Noise2D(x * frequency, y * frequency, seed) * amplitude;
            }

            return sum / range;
        }

        /// <summary>
        /// 2 dimensional fractal perlin noise
        /// </summary>
        /// <param name="position">Vector2 position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Random value between 0 and 1</returns>
        public static float Fractal2D(Vector2 position, uint seed, float frequency, int octaves, float lacunarity = 2,
            float persistence = 0.5f)
        {
            return Fractal2D(position.x, position.y, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// 3 dimensional fractal perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="z">z position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Random value between 0 and 1</returns>
        public static float Fractal3D(float x, float y, float z, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            var sum = Noise3D(x * frequency, y * frequency, z * frequency, seed);
            var amplitude = 1f;
            var range = 1f;
            for (var i = 1; i < octaves; i++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += Noise3D(x * frequency, y * frequency, z * frequency, seed) * amplitude;
            }

            return sum / range;
        }

        /// <summary>
        /// 3 dimensional fractal perlin noise
        /// </summary>
        /// <param name="position">Vector3 position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Random value between 0 and 1</returns>
        public static float Fractal3D(Vector3 position, uint seed, float frequency, int octaves, float lacunarity = 2,
            float persistence = 0.5f)
        {
            return Fractal3D(position.x, position.y, position.z, seed, frequency, octaves, lacunarity, persistence);
        }

        #endregion

        #region Gradient Noise

        /// <summary>
        /// 1 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise1D(float x, uint seed)
        {
            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var t0 = x - x0;
            var t1 = t0 - 1f;

            var g0 = HashToGradient1D(Hash1D(x0, seed));
            var g1 = HashToGradient1D(Hash1D(x1, seed));

            var v0 = g0 * t0;
            var v1 = g1 * t1;

            var t = Smooth(t0);
            var v = Mathf.Lerp(v0, v1, t) * 2f;
            return v * 0.5f + 0.5f; //Remap from -1 to 1, to 0 to 1
        }

        public static NoiseSample Sample1D(float x, uint seed, float frequency = 1)
        {
            var x0 = Mathf.FloorToInt(x * frequency);
            var x1 = x0 + 1;

            var t0 = x - x0;
            var t1 = t0 - 1f;

            var g0 = HashToGradient1D(Hash1D(x0, seed));
            var g1 = HashToGradient1D(Hash1D(x1, seed));

            var v0 = g0 * t0;
            var v1 = g1 * t1;

            var t = Smooth(t0);
            var dt = SmoothDerivative(t0);

            var a = v0;
            var b = v1 - v0;

            var v = a + b * t;
            var dv = b * t;

            var sample = new NoiseSample
            {
                value = v * 2f,
            };

            sample.derivative.x = dv;
            sample.derivative.y = 0;
            sample.derivative.z = 0;

            sample.derivative *= frequency;

            return sample;
        }


        /// <summary>
        /// 2 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise2D(float x, float y, uint seed)
        {
            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var y0 = Mathf.FloorToInt(y);
            var y1 = y0 + 1;

            var tx0 = x - x0;
            var tx1 = tx0 - 1f;

            var ty0 = y - y0;
            var ty1 = ty0 - 1f;

            var g00 = HashToGradient2D(Hash2D(x0, y0, seed));
            var g10 = HashToGradient2D(Hash2D(x1, y0, seed));
            var g01 = HashToGradient2D(Hash2D(x0, y1, seed));
            var g11 = HashToGradient2D(Hash2D(x1, y1, seed));

            var v00 = Dot2D(g00, tx0, ty0);
            var v10 = Dot2D(g10, tx1, ty0);
            var v01 = Dot2D(g01, tx0, ty1);
            var v11 = Dot2D(g11, tx1, ty1);

            var tx = Smooth(tx0);
            var ty = Smooth(ty0);

            var v0 = Mathf.Lerp(v00, v10, tx);
            var v1 = Mathf.Lerp(v01, v11, tx);
            var v = Mathf.Lerp(v0, v1, ty) * Sqr2;
            return v * 0.5f + 0.5f; //Remap from -1 to 1, to 0 to 1
        }

        public static NoiseSample Sample2D(float x, float y, uint seed, float frequency = 1f)
        {
            x *= frequency;
            y *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var y0 = Mathf.FloorToInt(y);
            var y1 = y0 + 1;

            var tx0 = x - x0;
            var tx1 = tx0 - 1f;

            var ty0 = y - y0;
            var ty1 = ty0 - 1f;

            var g00 = HashToGradient2D(Hash2D(x0, y0, seed));
            var g10 = HashToGradient2D(Hash2D(x1, y0, seed));
            var g01 = HashToGradient2D(Hash2D(x0, y1, seed));
            var g11 = HashToGradient2D(Hash2D(x1, y1, seed));

            var v00 = Dot2D(g00, tx0, ty0);
            var v10 = Dot2D(g10, tx1, ty0);
            var v01 = Dot2D(g01, tx0, ty1);
            var v11 = Dot2D(g11, tx1, ty1);

            var tx = Smooth(tx0);
            var ty = Smooth(ty0);

            var a = v00;
            var b = v10 - v00;

            var v0 = Mathf.Lerp(v00, v10, tx);
            var v1 = Mathf.Lerp(v01, v11, tx);
            var v = Mathf.Lerp(v0, v1, ty) * Sqr2;

            var sample = new NoiseSample
            {

            };

            return sample;
        }


        /// <summary>
        /// 2 dimensional perlin noise
        /// </summary>
        /// <param name="position">Vector2 position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise2D(Vector2 position, uint seed)
        {
            return Noise2D(position.x, position.y, seed);
        }

        /// <summary>
        /// 3 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="z">z position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise3D(float x, float y, float z, uint seed)
        {
            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var y0 = Mathf.FloorToInt(y);
            var y1 = y0 + 1;

            var z0 = Mathf.FloorToInt(z);
            var z1 = z0 + 1;

            var tx0 = x - x0;
            var tx1 = tx0 - 1f;

            var ty0 = y - y0;
            var ty1 = ty0 - 1f;

            var tz0 = z - z0;
            var tz1 = tz0 - 1f;

            var h000 = Hash3D(x0, y0, z0, seed);
            var h010 = Hash3D(x0, y1, z0, seed);
            var h001 = Hash3D(x0, y0, z1, seed);
            var h011 = Hash3D(x0, y1, z1, seed);
            var h100 = Hash3D(x1, y0, z0, seed);
            var h110 = Hash3D(x1, y1, z0, seed);
            var h101 = Hash3D(x1, y0, z1, seed);
            var h111 = Hash3D(x1, y1, z1, seed);

            var g000 = HashToGradient3D(h000);
            var g100 = HashToGradient3D(h100);
            var g010 = HashToGradient3D(h010);
            var g110 = HashToGradient3D(h110);
            var g001 = HashToGradient3D(h001);
            var g101 = HashToGradient3D(h101);
            var g011 = HashToGradient3D(h011);
            var g111 = HashToGradient3D(h111);

            var v000 = Dot3D(g000, tx0, ty0, tz0);
            var v100 = Dot3D(g100, tx1, ty0, tz0);
            var v010 = Dot3D(g010, tx0, ty1, tz0);
            var v110 = Dot3D(g110, tx1, ty1, tz0);
            var v001 = Dot3D(g001, tx0, ty0, tz1);
            var v101 = Dot3D(g101, tx1, ty0, tz1);
            var v011 = Dot3D(g011, tx0, ty1, tz1);
            var v111 = Dot3D(g111, tx1, ty1, tz1);

            var tx = Smooth(tx0);
            var ty = Smooth(ty0);
            var tz = Smooth(tz0);

            var vx0 = Mathf.Lerp(v000, v100, tx);
            var vx1 = Mathf.Lerp(v010, v110, tx);

            var vx2 = Mathf.Lerp(v001, v101, tx);
            var vx3 = Mathf.Lerp(v011, v111, tx);

            var vy0 = Mathf.Lerp(vx0, vx1, ty);
            var vy1 = Mathf.Lerp(vx2, vx3, ty);

            var v = Mathf.Lerp(vy0, vy1, tz);
            return v * 0.5f + 0.5f; //Remap from -1 to 1, to 0 to 1
        }

        /// <summary>
        /// 3 dimensional perlin noise
        /// </summary>
        /// <param name="position">Vector3 position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise3D(Vector3 position, uint seed)
        {
            return Noise3D(position.x, position.y, position.z, seed);
        }

        #endregion

        private static float HashToGradient1D(uint value)
        {
            return Gradients1D[value & GradientsMask1D];
        }

        private static Vector2 HashToGradient2D(uint value)
        {
            return Gradients2D[value & GradientsMask2D];
        }

        private static Vector3 HashToGradient3D(uint value)
        {
            return Gradients3D[value & GradientsMask3D];
        }

        private static uint Hash1D(int value, uint seed)
        {
            return SquirrelEiserloh.Get1dNoiseUint(value, seed);
        }

        private static uint Hash2D(int x, int y, uint seed)
        {
            return SquirrelEiserloh.Get2dNoiseUint(x, y, seed);
        }

        private static uint Hash3D(int x, int y, int z, uint seed)
        {
            return SquirrelEiserloh.Get3dNoiseUint(x, y, z, seed);
        }

        private static float Dot2D(Vector2 g, float x, float y)
        {
            return g.x * x + g.y * y;
        }

        private static float Dot3D(Vector3 g, float x, float y, float z)
        {
            return g.x * x + g.y * y + g.z * z;
        }

        private static float Smooth(float t)
        {
            return SmoothStep.Degree5(t);
        }

        private static float SmoothDerivative(float t)
        {
            return SmoothStep.Degree5Derivative(t);
        }
    }

    public static class SimplexGradientNoise
    {
        public static float Noise1D(float x, uint seed)
        {
            var x0 = Mathf.FloorToInt(x);
            var x1 = x - x0;

            return 0;
        }
    }
}