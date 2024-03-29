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
        public static float Fractal1D(float x, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalUtility.Float1D(Noise1D, x, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// Sample 1 dimensional fractal perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Noise sample which includes value in range 0 to 1 and derivative of noise function at the sampled location</returns>
        public static NoiseSample FractalSample1D(float x, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalUtility.Sample1D(Sample1D, x, seed, frequency, octaves, lacunarity, persistence);
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
        public static float Fractal2D(float x, float y, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalUtility.Float2D(Noise2D, x, y, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// Sample 2 dimensional fractal perlin noise
        /// </summary>
        /// <param name="point">2D Position (x,y) coordinates</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Noise sample which includes value in range 0 to 1 and derivative of noise function at the sampled location</returns>
        public static NoiseSample FractalSample2D(Vector2 point, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalSample2D(point.x, point.y, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// Sample 2 dimensional fractal perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Noise sample which includes value in range 0 to 1 and derivative of noise function at the sampled location</returns>
        public static NoiseSample FractalSample2D(float x, float y, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalUtility.Sample2D(Sample2D, x, y, seed, frequency, octaves, lacunarity, persistence);
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
        public static float Fractal2D(Vector2 position, uint seed, float frequency, int octaves, float lacunarity = 2, float persistence = 0.5f)
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
            return FractalUtility.Float3D(Noise3D, x, y, z, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// Sample 3 dimensional fractal perlin noise
        /// </summary>
        /// <param name="point">3D Position (x,y,z) coordinates</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Noise sample which includes value in range 0 to 1 and derivative of noise function at the sampled location</returns>
        public static NoiseSample FractalSample3D(Vector3 point, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalSample3D(point.x, point.y, point.z, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// Sample 3 dimensional fractal perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="z">z position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Noise sample which includes value in range 0 to 1 and derivative of noise function at the sampled location</returns>
        public static NoiseSample FractalSample3D(float x, float y, float z, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalUtility.Sample3D(Sample3D, x, y, z, seed, frequency, octaves, lacunarity, persistence);
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
        public static float Fractal3D(Vector3 position, uint seed, float frequency, int octaves, float lacunarity = 2, float persistence = 0.5f)
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
        public static float Noise1D(float x, uint seed, float frequency = 1f)
        {
            x *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var t0 = x - x0;
            var t1 = t0 - 1f;

            var g0 = NoiseGradients.Gradient1D(x0, seed);
            var g1 = NoiseGradients.Gradient1D(x1, seed);

            var v0 = g0 * t0;
            var v1 = g1 * t1;

            var t = Smooth(t0);
            var v = Mathf.Lerp(v0, v1, t) * 2f;
            return v * 0.5f + 0.5f; //Remap from -1 to 1, to 0 to 1
        }

        /// <summary>
        /// Sample 1 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="seed">rng seed</param>
        /// <param name="frequency">frequency at which to sample the noise</param>
        /// <returns>NoiseSample containing value between 0 and 1 (inclusive) and derivative</returns>
        public static NoiseSample Sample1D(float x, uint seed, float frequency = 1)
        {
            x *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var t0 = x - x0;
            var t1 = t0 - 1f;

            var g0 = NoiseGradients.Gradient1D(x0, seed);
            var g1 = NoiseGradients.Gradient1D(x1, seed);

            var v0 = g0 * t0;
            var v1 = g1 * t1;

            var t = Smooth(t0);
            var dt = SmoothDerivative(t0);

            var a = v0;
            var b = v1 - v0;

            var da = g0;
            var db = g1 - g0; //Because b = v1 - v0 = g0 * t0 - g1 * t1

            var v = a + b * t;
            var dv = da + db * t + b * dt;

            var sample = new NoiseSample
            {
                value = v + 0.5f, //Remap to 0 - 1
            };

            sample.derivative.x = dv;
            sample.derivative.y = 0;
            sample.derivative.z = 0;

            return sample;
        }


        /// <summary>
        /// 2 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise2D(float x, float y, uint seed, float frequency = 1f)
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

            var g00 = NoiseGradients.Gradient2D(x0, y0, seed);
            var g10 = NoiseGradients.Gradient2D(x1, y0, seed);
            var g01 = NoiseGradients.Gradient2D(x0, y1, seed);
            var g11 = NoiseGradients.Gradient2D(x1, y1, seed);

            var v00 = NoiseGradients.Dot2D(g00, tx0, ty0);
            var v10 = NoiseGradients.Dot2D(g10, tx1, ty0);
            var v01 = NoiseGradients.Dot2D(g01, tx0, ty1);
            var v11 = NoiseGradients.Dot2D(g11, tx1, ty1);

            var tx = Smooth(tx0);
            var ty = Smooth(ty0);

            var v0 = Mathf.Lerp(v00, v10, tx);
            var v1 = Mathf.Lerp(v01, v11, tx);
            var v = Mathf.Lerp(v0, v1, ty) * Sqr2;
            return v * 0.5f + 0.5f; //Remap from -1 to 1, to 0 to 1
        }

        /// <summary>
        /// Sample 2 dimensional perlin noise
        /// </summary>
        /// <param name="point">2D Position</param>
        /// <param name="seed">rng seed</param>
        /// <param name="frequency">frequency at which to sample the noise</param>
        /// <returns>NoiseSample containing value between 0 and 1 (inclusive) and derivative</returns>
        public static NoiseSample Sample2D(Vector2 point, uint seed, float frequency = 1f)
        {
            return Sample2D(point.x, point.y, seed, frequency);
        }

        /// <summary>
        /// Sample 2 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="seed">rng seed</param>
        /// <param name="frequency">frequency at which to sample the noise</param>
        /// <returns>NoiseSample containing value between 0 and 1 (inclusive) and derivative</returns>
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

            var g00 = NoiseGradients.Gradient2D(x0, y0, seed);
            var g10 = NoiseGradients.Gradient2D(x1, y0, seed);
            var g01 = NoiseGradients.Gradient2D(x0, y1, seed);
            var g11 = NoiseGradients.Gradient2D(x1, y1, seed);

            var v00 = NoiseGradients.Dot2D(g00, tx0, ty0);
            var v10 = NoiseGradients.Dot2D(g10, tx1, ty0);
            var v01 = NoiseGradients.Dot2D(g01, tx0, ty1);
            var v11 = NoiseGradients.Dot2D(g11, tx1, ty1);

            var tx = Smooth(tx0);
            var ty = Smooth(ty0);

            float dtx = SmoothDerivative(tx0);
            float dty = SmoothDerivative(ty0);

            float a = v00;
            float b = v10 - v00;
            float c = v01 - v00;
            float d = v11 - v01 - v10 + v00;

            var da = g00;
            var db = g10 - g00;
            var dc = g01 - g00;
            var dd = g11 - g01 - g10 + g00;

            var sample = new NoiseSample
            {
                value = a + b * tx + (c + d * tx) * ty,
            };

            sample.value *= Sqr2;

            sample.derivative = da + db * tx + (dc + dd * tx) * ty;
            sample.derivative.x += (b + d * ty) * dtx;
            sample.derivative.y += (c + d * tx) * dty;
            sample.derivative.z = 0f;

            sample.value = Mathf.InverseLerp(-1, 1, sample.value);

            return sample;
        }

        /// <summary>
        /// 2 dimensional perlin noise
        /// </summary>
        /// <param name="position">Vector2 position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise2D(Vector2 position, uint seed, float frequency = 1f)
        {
            return Noise2D(position.x, position.y, seed, frequency);
        }

        /// <summary>
        /// 3 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="z">z position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise3D(float x, float y, float z, uint seed, float frequency = 1f)
        {
            x *= frequency;
            y *= frequency;
            z *= frequency;

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

            var g000 = NoiseGradients.Gradient3D(x0, y0, z0, seed);
            var g100 = NoiseGradients.Gradient3D(x1, y0, z0, seed);
            var g010 = NoiseGradients.Gradient3D(x0, y1, z0, seed);
            var g110 = NoiseGradients.Gradient3D(x1, y1, z0, seed);
            var g001 = NoiseGradients.Gradient3D(x0, y0, z1, seed);
            var g101 = NoiseGradients.Gradient3D(x1, y0, z1, seed);
            var g011 = NoiseGradients.Gradient3D(x0, y1, z1, seed);
            var g111 = NoiseGradients.Gradient3D(x1, y1, z1, seed);

            var v000 = NoiseGradients.Dot3D(g000, tx0, ty0, tz0);
            var v100 = NoiseGradients.Dot3D(g100, tx1, ty0, tz0);
            var v010 = NoiseGradients.Dot3D(g010, tx0, ty1, tz0);
            var v110 = NoiseGradients.Dot3D(g110, tx1, ty1, tz0);
            var v001 = NoiseGradients.Dot3D(g001, tx0, ty0, tz1);
            var v101 = NoiseGradients.Dot3D(g101, tx1, ty0, tz1);
            var v011 = NoiseGradients.Dot3D(g011, tx0, ty1, tz1);
            var v111 = NoiseGradients.Dot3D(g111, tx1, ty1, tz1);

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
        /// Sample 3 dimensional perlin noise
        /// </summary>
        /// <param name="point">3D Position</param>
        /// <param name="seed">rng seed</param>
        /// <param name="frequency">frequency at which to sample the noise</param>
        /// <returns>NoiseSample containing value between 0 and 1 (inclusive) and derivative</returns>
        public static NoiseSample Sample3D(Vector3 point, uint seed, float frequency = 1)
        {
            return Sample3D(point.x, point.y, point.z, seed, frequency);
        }

        /// <summary>
        /// Sample 3 dimensional perlin noise
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="z">z position/param>
        /// <param name="seed">rng seed</param>
        /// <param name="frequency">frequency at which to sample the noise</param>
        /// <returns>NoiseSample containing value between 0 and 1 (inclusive) and derivative</returns>
        public static NoiseSample Sample3D(float x, float y, float z, uint seed, float frequency = 1)
        {
            x *= frequency;
            y *= frequency;
            z *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var y0 = Mathf.FloorToInt(y);
            var z0 = Mathf.FloorToInt(z);

            var tx0 = x - x0;
            var ty0 = y - y0;
            var tz0 = z - z0;

            var tx1 = tx0 - 1f;
            var ty1 = ty0 - 1f;
            var tz1 = tz0 - 1f;

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;

            var g000 = NoiseGradients.Gradient3D(x0, y0, z0, seed);
            var g100 = NoiseGradients.Gradient3D(x1, y0, z0, seed);
            var g010 = NoiseGradients.Gradient3D(x0, y1, z0, seed);
            var g110 = NoiseGradients.Gradient3D(x1, y1, z0, seed);
            var g001 = NoiseGradients.Gradient3D(x0, y0, z1, seed);
            var g101 = NoiseGradients.Gradient3D(x1, y0, z1, seed);
            var g011 = NoiseGradients.Gradient3D(x0, y1, z1, seed);
            var g111 = NoiseGradients.Gradient3D(x1, y1, z1, seed);

            var v000 = NoiseGradients.Dot3D(g000, tx0, ty0, tz0);
            var v100 = NoiseGradients.Dot3D(g100, tx1, ty0, tz0);
            var v010 = NoiseGradients.Dot3D(g010, tx0, ty1, tz0);
            var v110 = NoiseGradients.Dot3D(g110, tx1, ty1, tz0);
            var v001 = NoiseGradients.Dot3D(g001, tx0, ty0, tz1);
            var v101 = NoiseGradients.Dot3D(g101, tx1, ty0, tz1);
            var v011 = NoiseGradients.Dot3D(g011, tx0, ty1, tz1);
            var v111 = NoiseGradients.Dot3D(g111, tx1, ty1, tz1);

            var dtx = SmoothDerivative(tx0);
            var dty = SmoothDerivative(ty0);
            var dtz = SmoothDerivative(tz0);

            var tx = Smooth(tx0);
            var ty = Smooth(ty0);
            var tz = Smooth(tz0);

            var a = v000;
            var b = v100 - v000;
            var c = v010 - v000;
            var d = v001 - v000;
            var e = v110 - v010 - v100 + v000;
            var f = v101 - v001 - v100 + v000;
            var g = v011 - v001 - v010 + v000;
            var h = v111 - v011 - v101 + v001 - v110 + v010 + v100 - v000;

            var da = g000;
            var db = g100 - g000;
            var dc = g010 - g000;
            var dd = g001 - g000;
            var de = g110 - g010 - g100 + g000;
            var df = g101 - g001 - g100 + g000;
            var dg = g011 - g001 - g010 + g000;
            var dh = g111 - g011 - g101 + g001 - g110 + g010 + g100 - g000;

            var sample = new NoiseSample
            {
                value = a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz,
                derivative = da + db * tx + (dc + de * tx) * ty + (dd + df * tx + (dg + dh * tx) * ty) * tz
            };

            sample.derivative.x += (b + e * ty + (f + h * ty) * tz) * dtx;
            sample.derivative.y += (c + e * tx + (g + h * tx) * tz) * dty;
            sample.derivative.z += (d + f * tx + (g + h * tx) * ty) * dtz;

            sample.value = Mathf.InverseLerp(-1, 1, sample.value);

            return sample;
        }

        /// <summary>
        /// 3 dimensional perlin noise
        /// </summary>
        /// <param name="position">Vector3 position</param>
        /// <param name="seed">rng seed</param>
        /// <returns>value between 0 and 1 (inclusive)</returns>
        public static float Noise3D(Vector3 position, uint seed, float frequency = 1f)
        {
            return Noise3D(position.x, position.y, position.z, seed, frequency);
        }

        #endregion

        private static float Smooth(float t)
        {
            return SmoothStep.Degree5(t);
        }

        private static float SmoothDerivative(float t)
        {
            return SmoothStep.Degree5Derivative(t);
        }
    }
}
