using UnityEngine;

namespace Gameframe.Procgen
{
    public static class SimplexGradientNoise
    {
        private static readonly float SquaresToTriangles = (3f - Mathf.Sqrt(3f)) / 6f;
        private static readonly float TrianglesToSquares = (Mathf.Sqrt(3f) - 1f) / 2f;

        #region 1D

        public static NoiseSample SimplexValue1D(float pointX, uint seed, float frequency)
        {
            pointX *= frequency;
            var ix = Mathf.Floor(pointX);

            //Sample left
            var sample = _SimplexValue1DPart(pointX, ix, seed);
            //Sample right
            sample += _SimplexValue1DPart(pointX, ix + 1, seed);
            return sample;
        }

        private static NoiseSample _SimplexValue1DPart(float pointX, float ix, uint seed)
        {
            var x = pointX - ix;
            var f = 1f - x * x;
            var f2 = f * f;
            var f3 = f * f2;

            var h = Hash1D((int) ix, seed);

            return new NoiseSample
            {
                value = f3 * h
            };
        }

        private static float Hash1D(int value, uint seed)
        {
            return SquirrelEiserloh.Get1dNoiseZeroToOne(value, seed);
        }

        #endregion

        #region 2D

        public static NoiseSample SimplexValue2D(float x, float y, uint seed, float frequency)
        {
            x *= frequency;
            y *= frequency;

            //Skew
            var skew = (x + y) * TrianglesToSquares;
            var sx = x + skew;
            var sy = y + skew;

            var ix = Mathf.FloorToInt(sx);
            var iy = Mathf.FloorToInt(sy);

            var sample = _SimplexValue2DPart(x, y, ix, iy, seed);
            sample += _SimplexValue2DPart(x, y, ix + 1, iy + 1, seed);

            if (sx - ix >= sy - iy)
            {
                sample += _SimplexValue2DPart(x, y, ix + 1, iy, seed);
            }
            else
            {
                sample += _SimplexValue2DPart(x, y, ix, iy + 1, seed);
            }

            return sample * 8;
        }

        private static NoiseSample _SimplexValue2DPart(float x, float y, int ix, int iy, uint seed)
        {
            var unskew = (ix + iy) * SquaresToTriangles;

            var x2 = x - ix + unskew;
            var y2 = y - iy + unskew;
            var f = 0.5f - x2 * x2 - y2 * y2;
            var f2 = f * f;
            var f3 = f * f2;

            var sample = new NoiseSample
            {
                value = (f > 0 ? f3 : 0) * Hash2D(ix, iy, seed),
            };
            return sample;
        }

        private static float Hash2D(int x, int y, uint seed)
        {
            return SquirrelEiserloh.Get2dNoiseZeroToOne(x, y, seed);
        }

        #endregion
    }
}
