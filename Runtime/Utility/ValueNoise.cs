using System.Drawing;
using UnityEngine;

namespace Gameframe.Procgen
{
    /// <summary>
    /// Utility methods for random value noise generation
    /// This noise usually has a very blocky appearance when visualized
    /// </summary>
    public static class ValueNoise
    {
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

        public static NoiseSample FractalSample2D(Vector2 point, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalSample2D(point.x, point.y, seed, frequency, octaves, lacunarity, persistence);
        }

        public static NoiseSample FractalSample2D(float x, float y, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalUtility.Sample2D(Sample2D, x, y, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// 2 dimensional fractal perlin noise
        /// </summary>
        /// <param name="point">Vector2 position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Random value between 0 and 1</returns>
        public static float Fractal2D(Vector2 point, uint seed, float frequency, int octaves, float lacunarity = 2, float persistence = 0.5f)
        {
            return Fractal2D(point.x, point.y, seed, frequency, octaves, lacunarity, persistence);
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

        public static NoiseSample FractalSample3D(Vector3 point, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalSample3D(point.x, point.y, point.z, seed, frequency, octaves, lacunarity, persistence);
        }

        public static NoiseSample FractalSample3D(float x, float y, float z, uint seed, float frequency, int octaves, float lacunarity = 2f, float persistence = 0.5f)
        {
            return FractalUtility.Sample3D(Sample3D, x, y, z, seed, frequency, octaves, lacunarity, persistence);
        }

        /// <summary>
        /// 3 dimensional fractal perlin noise
        /// </summary>
        /// <param name="point">Vector3 position</param>
        /// <param name="seed">random number generator seed</param>
        /// <param name="frequency">How often the noise space repeats. Large numbers mean higher resolution.</param>
        /// <param name="octaves">Number of noise iterations</param>
        /// <param name="lacunarity">frequency is multiplied by this number ever octave</param>
        /// <param name="persistence">measure of how much each octave will contribute. Values gt 1 make each octave larger. Values gt 0 but lt 1 make each successive octave contribution smaller.</param>
        /// <returns>Random value between 0 and 1</returns>
        public static float Fractal3D(Vector3 point, uint seed, float frequency, int octaves, float lacunarity = 2, float persistence = 0.5f)
        {
            return Fractal3D(point.x, point.y, point.z, seed, frequency, octaves, lacunarity, persistence);
        }

        #endregion

        #region Regular Noise

        public static float Noise1D(float x, uint seed, float frequency = 1)
        {
            x *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var t = Smooth(x - x0);

            var v0 = SquirrelEiserloh.Get1dNoiseZeroToOne(x0, seed);
            var v1 = SquirrelEiserloh.Get1dNoiseZeroToOne(x1, seed);

            return Mathf.Lerp(v0, v1, t);
        }

        public static NoiseSample Sample1D(float x, uint seed, float frequency = 1)
        {
            x *= frequency;
            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var t = x - x0;
            var dt = SmoothDerivative(t);
            t = Smooth(t);

            var v0 = SquirrelEiserloh.Get1dNoiseZeroToOne(x0, seed);
            var v1 = SquirrelEiserloh.Get1dNoiseZeroToOne(x1, seed);

            var a = v0;
            var b = v1 - v0;
            var v = a + b * t; //Lerp
            var dv = b * dt; //Lerp Derivative

            var sample = new NoiseSample
            {
                value = v,
            };

            sample.derivative.x = dv;
            sample.derivative.y = 0;
            sample.derivative.z = 0;

            return sample;
        }

        public static float Noise2D(float x, float y, uint seed, float frequency = 1)
        {
            x *= frequency;
            y *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var y0 = Mathf.FloorToInt(y);
            var x1 = x0 + 1;
            var y1 = y0 + 1;

            var tx = Smooth(x - x0);
            var ty = Smooth(y - y0);

            var v00 = SquirrelEiserloh.Get2dNoiseZeroToOne(x0, y0, seed);
            var v01 = SquirrelEiserloh.Get2dNoiseZeroToOne(x0, y1, seed);

            var v10 = SquirrelEiserloh.Get2dNoiseZeroToOne(x1, y0, seed);
            var v11 = SquirrelEiserloh.Get2dNoiseZeroToOne(x1, y1, seed);

            var edge1 = Mathf.Lerp(v00, v10, tx);
            var edge2 = Mathf.Lerp(v01, v11, tx);

            return Mathf.Lerp(edge1, edge2, ty);
        }

        public static NoiseSample Sample2D(float x, float y, uint seed, float frequency = 1f)
        {
            x *= frequency;
            y *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var y0 = Mathf.FloorToInt(y);
            var x1 = x0 + 1;
            var y1 = y0 + 1;

            var tx = Smooth(x - x0);
            var ty = Smooth(y - y0);

            var dtx = SmoothDerivative(x - x0);
            var dty = SmoothDerivative(y - y0);

            var v00 = SquirrelEiserloh.Get2dNoiseZeroToOne(x0, y0, seed);
            var v01 = SquirrelEiserloh.Get2dNoiseZeroToOne(x0, y1, seed);

            var v10 = SquirrelEiserloh.Get2dNoiseZeroToOne(x1, y0, seed);
            var v11 = SquirrelEiserloh.Get2dNoiseZeroToOne(x1, y1, seed);

            var a = v00;
            var b = v10 - v00;
            var c = v01 - v00;
            var d = v11 - v01 - v10 + v00;

            var v = a + b * tx + (c + d * tx) * ty;

            var sample = new NoiseSample
            {
                value = v
            };

            sample.derivative.x = (b + d * ty) * dtx;
            sample.derivative.y = (c + d * tx) * dty;
            sample.derivative.z = 0;

            return sample;
        }

        public static float Noise2D(Vector2 v, uint seed, float frequency = 1f)
        {
            return Noise2D(v.x, v.y, seed, frequency);
        }

        public static float Noise3D(Vector3 v, uint seed, float frequency = 1f)
        {
            return Noise3D(v.x, v.y, v.z, seed, frequency);
        }

        public static float Noise3D(float x, float y, float z, uint seed, float frequency = 1f)
        {
            x *= frequency;
            y *= frequency;
            z *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var y0 = Mathf.FloorToInt(y);
            var z0 = Mathf.FloorToInt(z);

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;

            var tx = Smooth(x - x0);
            var ty = Smooth(y - y0);
            var tz = Smooth(z - z0);

            var v000 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y0, z0, seed);
            var v010 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y1, z0, seed);
            var v001 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y0, z1, seed);
            var v011 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y1, z1, seed);

            var v100 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y0, z0, seed);
            var v110 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y1, z0, seed);
            var v101 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y0, z1, seed);
            var v111 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y1, z1, seed);

            //Lerp along all edges along the x axis
            var xEdge1 = Mathf.Lerp(v000, v100, tx);
            var xEdge2 = Mathf.Lerp(v010, v110, tx);
            var xEdge3 = Mathf.Lerp(v001, v101, tx);
            var xEdge4 = Mathf.Lerp(v011, v111, tx);

            //Lerp over y
            var yEdge1 = Mathf.Lerp(xEdge1, xEdge2, ty);
            var yEdge2 = Mathf.Lerp(xEdge3, xEdge4, ty);

            //Finally lerp over z
            return Mathf.Lerp(yEdge1, yEdge2, tz);
        }

        public static NoiseSample Sample3D(float x, float y, float z, uint seed, float frequency = 1f)
        {
            x *= frequency;
            y *= frequency;
            z *= frequency;

            var x0 = Mathf.FloorToInt(x);
            var y0 = Mathf.FloorToInt(y);
            var z0 = Mathf.FloorToInt(z);

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;

            var tx = x - x0;
            var ty = y - y0;
            var tz = z - z0;

            var dtx = SmoothDerivative(tx);
            var dty = SmoothDerivative(ty);
            var dtz = SmoothDerivative(tz);

            tx = Smooth(tx);
            ty = Smooth(ty);
            tz = Smooth(tz);

            var v000 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y0, z0, seed);
            var v010 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y1, z0, seed);
            var v001 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y0, z1, seed);
            var v011 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y1, z1, seed);

            var v100 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y0, z0, seed);
            var v110 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y1, z0, seed);
            var v101 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y0, z1, seed);
            var v111 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y1, z1, seed);

            float a = v000;
            float b = v100 - v000;
            float c = v010 - v000;
            float d = v001 - v000;
            float e = v110 - v010 - v100 + v000;
            float f = v101 - v001 - v100 + v000;
            float g = v011 - v001 - v010 + v000;
            float h = v111 - v011 - v101 + v001 - v110 + v010 + v100 - v000;

            var sample = new NoiseSample
            {
                value = a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz
            };

            sample.derivative.x = (b + e * ty + (f + h * ty) * tz) * dtx;
            sample.derivative.y = (c + e * tx + (g + h * tx) * tz) * dty;
            sample.derivative.z = (d + f * tx + (g + h * tx) * ty) * dtz;

            return sample;
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
