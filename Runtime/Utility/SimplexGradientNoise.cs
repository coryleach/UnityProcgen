using UnityEngine;

namespace Gameframe.Procgen
{
    public static class NoiseGradients
    {
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
        private const int SimplexGradientsMask3D = 31;

        private static readonly Vector3[] Gradients3D =
        {
            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 1f,-1f, 0f),
            new Vector3(-1f,-1f, 0f),
            new Vector3( 1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3( 1f, 0f,-1f),
            new Vector3(-1f, 0f,-1f),
            new Vector3( 0f, 1f, 1f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f, 1f,-1f),
            new Vector3( 0f,-1f,-1f),

            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f,-1f,-1f)
        };

        private static readonly Vector3[] SimplexGradients3D = {
            new Vector3( 1f, 1f, 0f).normalized,
            new Vector3(-1f, 1f, 0f).normalized,
            new Vector3( 1f,-1f, 0f).normalized,
            new Vector3(-1f,-1f, 0f).normalized,
            new Vector3( 1f, 0f, 1f).normalized,
            new Vector3(-1f, 0f, 1f).normalized,
            new Vector3( 1f, 0f,-1f).normalized,
            new Vector3(-1f, 0f,-1f).normalized,
            new Vector3( 0f, 1f, 1f).normalized,
            new Vector3( 0f,-1f, 1f).normalized,
            new Vector3( 0f, 1f,-1f).normalized,
            new Vector3( 0f,-1f,-1f).normalized,

            new Vector3( 1f, 1f, 0f).normalized,
            new Vector3(-1f, 1f, 0f).normalized,
            new Vector3( 1f,-1f, 0f).normalized,
            new Vector3(-1f,-1f, 0f).normalized,
            new Vector3( 1f, 0f, 1f).normalized,
            new Vector3(-1f, 0f, 1f).normalized,
            new Vector3( 1f, 0f,-1f).normalized,
            new Vector3(-1f, 0f,-1f).normalized,
            new Vector3( 0f, 1f, 1f).normalized,
            new Vector3( 0f,-1f, 1f).normalized,
            new Vector3( 0f, 1f,-1f).normalized,
            new Vector3( 0f,-1f,-1f).normalized,

            new Vector3( 1f, 1f, 1f).normalized,
            new Vector3(-1f, 1f, 1f).normalized,
            new Vector3( 1f,-1f, 1f).normalized,
            new Vector3(-1f,-1f, 1f).normalized,
            new Vector3( 1f, 1f,-1f).normalized,
            new Vector3(-1f, 1f,-1f).normalized,
            new Vector3( 1f,-1f,-1f).normalized,
            new Vector3(-1f,-1f,-1f).normalized
        };

        private static float HashToGradient1D(uint value)
        {
            return Gradients1D[value & GradientsMask1D];
        }

        public static float HashToGradient1D(int x, uint seed)
        {
            return HashToGradient1D(Hash1D(x, seed));
        }

        private static Vector2 HashToGradient2D(uint value)
        {
            return Gradients2D[value & GradientsMask2D];
        }

        public static Vector2 HashToGradient2D(int x, int y, uint seed)
        {
            return HashToGradient2D(Hash2D(x,y,seed));
        }

        private static Vector3 HashToGradient3D(uint value)
        {
            return Gradients3D[value & GradientsMask3D];
        }

        private static Vector3 HashToSimplexGradient3D(uint value)
        {
            return SimplexGradients3D[value & SimplexGradientsMask3D];
        }

        public static Vector3 HashToGradient3D(int x, int y, int z, uint seed)
        {
            return HashToGradient3D(Hash3D(x, y, z, seed));
        }

        public static Vector3 HashToSimplexGradient3D(int x, int y, int z, uint seed)
        {
            return HashToSimplexGradient3D(Hash3D(x, y, z, seed));
        }

        public static uint Hash1D(int value, uint seed)
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

        public static float Dot2D(Vector2 g, float x, float y)
        {
            return g.x * x + g.y * y;
        }

        public static float Dot3D(Vector3 g, float x, float y, float z)
        {
            return g.x * x + g.y * y + g.z * z;
        }
    }

    public static class SimplexGradientNoise
    {
        private static readonly float SquaresToTriangles = (3f - Mathf.Sqrt(3f)) / 6f;
        private static readonly float TrianglesToSquares = (Mathf.Sqrt(3f) - 1f) / 2f;
        private static readonly float Sqr2 = Mathf.Sqrt(2f);
        private static readonly float SimplexScale2D = 2916f * Sqr2 / 125f;
        private static readonly float SimplexScale1D = 64f / 27f;

        #region 1D

        public static NoiseSample SampleValue1D(float pointX, uint seed, float frequency)
        {
            pointX *= frequency;

            var ix = Mathf.FloorToInt(pointX);

            //Sample left
            var sample = _Value1DPart(pointX, ix, seed);
            //Sample right
            sample += _Value1DPart(pointX, ix + 1, seed);

            sample.derivative *= frequency;

            return sample;
        }

        private static NoiseSample _Value1DPart(float pointX, int ix, uint seed)
        {
            var x = pointX - ix;
            var f = 1f - x * x;
            var f2 = f * f;
            var f3 = f * f2;

            var h = Hash1D(ix, seed);

            return new NoiseSample
            {
                value = f3 * h,
                derivative = new Vector3
                {
                    x = -6f * h * x * f2
                }
            };
        }

        public static NoiseSample SampleGradient1D(float pointX, uint seed, float frequency)
        {
            pointX *= frequency;

            var ix = Mathf.FloorToInt(pointX);

            //Sample left
            var sample = _Gradient1DPart(pointX, ix, seed);
            //Sample right
            sample += _Gradient1DPart(pointX, ix + 1, seed);

            sample.derivative *= frequency;
            sample *= SimplexScale1D;
            sample.value = (sample.value + 1) * 0.5f;
            return sample;
        }

        private static NoiseSample _Gradient1DPart(float pointX, int ix, uint seed)
        {
            var x = pointX - ix;
            var f = 1f - x * x;
            var f2 = f * f;
            var f3 = f * f2;

            var g = NoiseGradients.HashToGradient1D(ix, seed);
            var v = g * x;

            return new NoiseSample
            {
                value = f3 * v,
                derivative = new Vector3
                {
                    x = g * f3 - 6f * v * x * f2,
                }
            };
        }

        private static float Hash1D(int value, uint seed)
        {
            return SquirrelEiserloh.Get1dNoiseZeroToOne(value, seed);
        }

        #endregion

        #region 2D

        public static NoiseSample SampleValue2D(float x, float y, uint seed, float frequency)
        {
            x *= frequency;
            y *= frequency;

            //Skew
            var skew = (x + y) * TrianglesToSquares;
            var sx = x + skew;
            var sy = y + skew;

            var ix = Mathf.FloorToInt(sx);
            var iy = Mathf.FloorToInt(sy);

            var sample = _Value2DPart(x, y, ix, iy, seed);
            sample += _Value2DPart(x, y, ix + 1, iy + 1, seed);

            if (sx - ix >= sy - iy)
            {
                sample += _Value2DPart(x, y, ix + 1, iy, seed);
            }
            else
            {
                sample += _Value2DPart(x, y, ix, iy + 1, seed);
            }

            sample.derivative *= frequency;

            return sample * 8;
        }

        private static NoiseSample _Value2DPart(float x, float y, int ix, int iy, uint seed)
        {
            var unskew = (ix + iy) * SquaresToTriangles;

            var x2 = x - ix + unskew;
            var y2 = y - iy + unskew;
            var f = 0.5f - x2 * x2 - y2 * y2;
            var f2 = f * f;
            var f3 = f * f2;

            if (f > 0)
            {
                var h = Hash2D(ix, iy, seed);
                var h6f2 = -6f * h * f2;

                return new NoiseSample
                {
                    value = f3 * h,
                    derivative = new Vector3
                    {
                        x = h6f2 * x,
                        y = h6f2 * y,
                    }
                };
            }

            return new NoiseSample();
        }

        public static NoiseSample SampleGradient2D(float x, float y, uint seed, float frequency)
        {
            x *= frequency;
            y *= frequency;

            //Skew
            var skew = (x + y) * TrianglesToSquares;
            var sx = x + skew;
            var sy = y + skew;

            var ix = Mathf.FloorToInt(sx);
            var iy = Mathf.FloorToInt(sy);

            var sample = _Gradient2DPart(x, y, ix, iy, seed);
            sample += _Gradient2DPart(x, y, ix + 1, iy + 1, seed);

            if (sx - ix >= sy - iy)
            {
                sample += _Gradient2DPart(x, y, ix + 1, iy, seed);
            }
            else
            {
                sample += _Gradient2DPart(x, y, ix, iy + 1, seed);
            }

            sample.derivative *= frequency;

            sample.value *= SimplexScale2D;
            sample.value += 1;
            sample.value *= 0.5f;

            return sample;
        }

        private static NoiseSample _Gradient2DPart(float pointX, float pointY, int ix, int iy, uint seed)
        {
            var unskew = (ix + iy) * SquaresToTriangles;

            var x2 = pointX - ix + unskew;
            var y2 = pointY - iy + unskew;
            var f = 0.5f - x2 * x2 - y2 * y2;

            if (f > 0)
            {
                var f2 = f * f;
                var f3 = f * f2;
                var g = NoiseGradients.HashToGradient2D(ix, iy, seed);
                var v = NoiseGradients.Dot2D(g, x2, y2);
                var v6f2 = -6f * v * f2;

                return new NoiseSample
                {
                    value = f3 * v,
                    derivative = new Vector3
                    {
                        x = g.x * f3 + v6f2 * pointX,
                        y = g.y * f3 + v6f2 * pointY,
                    }
                };
            }

            return new NoiseSample();
        }

        private static float Hash2D(int x, int y, uint seed)
        {
            return SquirrelEiserloh.Get2dNoiseZeroToOne(x, y, seed);
        }

        #endregion

        #region 3D

        private static NoiseSample _Value3DPart(float x, float y, float z, int ix, int iy, int iz, uint seed)
        {
            var unskew = (ix + iy + iz) * (1f / 6f);
            var x2 = x - ix + unskew;
            var y2 = y - iy + unskew;
            var z2 = z - iz + unskew;
            var f = 0.5f - x2 * x2 - y2 * y2 - z2 * z2;
            var sample = new NoiseSample();

            if (f > 0f)
            {
                var h = Hash3D(ix, iy, iz, seed);
                var f2 = f * f;
                var f3 = f * f2;
                sample.value = f3 * h;

                var h6f2 = -6f * h * f2;
                sample.derivative = new Vector3
                {
                    x = x * h6f2,
                    y = y * h6f2,
                    z = z * h6f2,
                };
            }

            return sample;
        }

        public static NoiseSample SampleValue3D(float x, float y, float z, uint seed, float frequency)
        {
            x *= frequency;
            y *= frequency;
            z *= frequency;

            var skew = (x + y + z) * (1f / 3f);
            var sx = x + skew;
            var sy = y + skew;
            var sz = z + skew;

            var ix = Mathf.FloorToInt(sx);
            var iy = Mathf.FloorToInt(sy);
            var iz = Mathf.FloorToInt(sz);

            var sample = _Value3DPart(x, y, z, ix, iy, iz, seed);

            sample += _Value3DPart(x, y, z, ix + 1, iy + 1, iz + 1, seed);

            var tetX = sx - ix;
            var tetY = sy - iy;
            var tetZ = sz - iz;

            if (tetX >= tetY)
            {
                if (tetX >= tetZ)
                {
                    sample += _Value3DPart(x, y, z, ix + 1, iy, iz, seed);
                    if (tetY >= tetZ)
                    {
                        sample += _Value3DPart(x, y, z, ix + 1, iy + 1, iz, seed);
                    }
                    else
                    {
                        sample += _Value3DPart(x, y, z, ix + 1, iy, iz + 1, seed);
                    }
                }
                else
                {
                    sample += _Value3DPart(x, y, z, ix, iy, iz + 1, seed);
                    sample += _Value3DPart(x, y, z, ix + 1, iy, iz + 1, seed);
                }
            }
            else
            {
                if (tetY >= tetZ)
                {
                    sample += _Value3DPart(x, y, z, ix, iy + 1, iz, seed);
                    if (tetX >= tetZ)
                    {
                        sample += _Value3DPart(x, y, z, ix + 1, iy + 1, iz, seed);
                    }
                    else
                    {
                        sample += _Value3DPart(x, y, z, ix, iy + 1, iz + 1, seed);
                    }
                }
                else
                {
                    sample += _Value3DPart(x, y, z, ix, iy, iz + 1, seed);
                    sample += _Value3DPart(x, y, z, ix, iy + 1, iz + 1, seed);
                }
            }

            sample.derivative *= frequency;

            return sample * 8f;
        }

        private static NoiseSample _Gradient3DPart(float x, float y, float z, int ix, int iy, int iz, uint seed)
        {
            var unskew = (ix + iy + iz) * (1f / 6f);
            var x2 = x - ix + unskew;
            var y2 = y - iy + unskew;
            var z2 = z - iz + unskew;
            var f = 0.5f - x2 * x2 - y2 * y2 - z2 * z2;
            var sample = new NoiseSample();

            if (f > 0f)
            {
                var h = Hash3D(ix, iy, iz, seed);
                var f2 = f * f;
                var f3 = f * f2;
                sample.value = f3 * h;

                var h6f2 = -6f * h * f2;
                sample.derivative = new Vector3
                {
                    x = x * h6f2,
                    y = y * h6f2,
                    z = z * h6f2,
                };
            }

            return sample;
        }

        public static NoiseSample SampleGradient3D(float x, float y, float z, uint seed, float frequency)
        {
            x *= frequency;
            y *= frequency;
            z *= frequency;

            var skew = (x + y + z) * (1f / 3f);
            var sx = x + skew;
            var sy = y + skew;
            var sz = z + skew;

            var ix = Mathf.FloorToInt(sx);
            var iy = Mathf.FloorToInt(sy);
            var iz = Mathf.FloorToInt(sz);

            var sample = _Gradient3DPart(x, y, z, ix, iy, iz, seed);

            sample += _Gradient3DPart(x, y, z, ix + 1, iy + 1, iz + 1, seed);

            var tetX = sx - ix;
            var tetY = sy - iy;
            var tetZ = sz - iz;

            if (tetX >= tetY)
            {
                if (tetX >= tetZ)
                {
                    sample += _Gradient3DPart(x, y, z, ix + 1, iy, iz, seed);
                    if (tetY >= tetZ)
                    {
                        sample += _Gradient3DPart(x, y, z, ix + 1, iy + 1, iz, seed);
                    }
                    else
                    {
                        sample += _Gradient3DPart(x, y, z, ix + 1, iy, iz + 1, seed);
                    }
                }
                else
                {
                    sample += _Gradient3DPart(x, y, z, ix, iy, iz + 1, seed);
                    sample += _Gradient3DPart(x, y, z, ix + 1, iy, iz + 1, seed);
                }
            }
            else
            {
                if (tetY >= tetZ)
                {
                    sample += _Gradient3DPart(x, y, z, ix, iy + 1, iz, seed);
                    if (tetX >= tetZ)
                    {
                        sample += _Gradient3DPart(x, y, z, ix + 1, iy + 1, iz, seed);
                    }
                    else
                    {
                        sample += _Gradient3DPart(x, y, z, ix, iy + 1, iz + 1, seed);
                    }
                }
                else
                {
                    sample += _Gradient3DPart(x, y, z, ix, iy, iz + 1, seed);
                    sample += _Gradient3DPart(x, y, z, ix, iy + 1, iz + 1, seed);
                }
            }

            sample.derivative *= frequency;

            return sample * 8f;
        }

        private static float Hash3D(int x, int y, int z, uint seed)
        {
            return SquirrelEiserloh.Get3dNoiseZeroToOne(x, y, z, seed);
        }

        #endregion
    }
}
