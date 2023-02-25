using UnityEngine;

namespace Gameframe.Procgen
{
    /// <summary>
    /// Noise gradients used in perlin and simplex noise
    /// Based on the tutorial by Cat-like Coding
    /// Using Random Access Noise methods by Squirrel Eiserloh
    /// </summary>
    public static class NoiseGradients
    {
        private const int GradientsMask1D = 1;
        private const int GradientsMask2D = 7;
        private const int GradientsMask3D = 15;
        private const int SimplexGradientsMask3D = 31;

        private static readonly float[] Gradients1D = new[] {1f, -1f};

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

        private static readonly Vector3[] SimplexGradients3D =
        {
            new Vector3(1f, 1f, 0f).normalized,
            new Vector3(-1f, 1f, 0f).normalized,
            new Vector3(1f, -1f, 0f).normalized,
            new Vector3(-1f, -1f, 0f).normalized,
            new Vector3(1f, 0f, 1f).normalized,
            new Vector3(-1f, 0f, 1f).normalized,
            new Vector3(1f, 0f, -1f).normalized,
            new Vector3(-1f, 0f, -1f).normalized,
            new Vector3(0f, 1f, 1f).normalized,
            new Vector3(0f, -1f, 1f).normalized,
            new Vector3(0f, 1f, -1f).normalized,
            new Vector3(0f, -1f, -1f).normalized,

            new Vector3(1f, 1f, 0f).normalized,
            new Vector3(-1f, 1f, 0f).normalized,
            new Vector3(1f, -1f, 0f).normalized,
            new Vector3(-1f, -1f, 0f).normalized,
            new Vector3(1f, 0f, 1f).normalized,
            new Vector3(-1f, 0f, 1f).normalized,
            new Vector3(1f, 0f, -1f).normalized,
            new Vector3(-1f, 0f, -1f).normalized,
            new Vector3(0f, 1f, 1f).normalized,
            new Vector3(0f, -1f, 1f).normalized,
            new Vector3(0f, 1f, -1f).normalized,
            new Vector3(0f, -1f, -1f).normalized,

            new Vector3(1f, 1f, 1f).normalized,
            new Vector3(-1f, 1f, 1f).normalized,
            new Vector3(1f, -1f, 1f).normalized,
            new Vector3(-1f, -1f, 1f).normalized,
            new Vector3(1f, 1f, -1f).normalized,
            new Vector3(-1f, 1f, -1f).normalized,
            new Vector3(1f, -1f, -1f).normalized,
            new Vector3(-1f, -1f, -1f).normalized
        };

        #region Public Methods
        public static float Gradient1D(int x, uint seed)
        {
            return HashToGradient1D(Hash1D(x, seed));
        }
        public static Vector2 Gradient2D(int x, int y, uint seed)
        {
            return HashToGradient2D(Hash2D(x, y, seed));
        }
        public static Vector3 Gradient3D(int x, int y, int z, uint seed)
        {
            return HashToGradient3D(Hash3D(x, y, z, seed));
        }
        public static Vector3 SimplexGradient3D(int x, int y, int z, uint seed)
        {
            return HashToSimplexGradient3D(Hash3D(x, y, z, seed));
        }
        public static float Dot2D(Vector2 g, float x, float y)
        {
            return g.x * x + g.y * y;
        }

        public static float Dot3D(Vector3 g, float x, float y, float z)
        {
            return g.x * x + g.y * y + g.z * z;
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
        private static Vector3 HashToSimplexGradient3D(uint value)
        {
            return SimplexGradients3D[value & SimplexGradientsMask3D];
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
    }
}
