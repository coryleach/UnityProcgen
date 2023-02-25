using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/NoiseGenerator/BasicNoise")]
    public class NoiseGenerator : ScriptableObject
    {
        [SerializeField]
        private uint seed;

        [SerializeField]
        private float frequency = 1.0f;

        public uint Seed
        {
            get => seed;
            set => seed = value;
        }

        public float Frequency
        {
            get => frequency;
            set => frequency = value;
        }

        public float Value1D(float x)
        {
            return ValueNoise.Noise1D(x*frequency, seed);
        }

        public float Value2D(float x, float y)
        {
            return ValueNoise.Noise2D(x*frequency, y*frequency, seed);
        }

        public float Value3D(float x, float y, float z)
        {
            return ValueNoise.Noise3D(x*frequency, y*frequency, z*frequency, seed);
        }

        public float Value2D(Vector2 v)
        {
            return Value2D(v.x, v.y);
        }

        public float Value3D(Vector3 v)
        {
            return Value3D(v.x, v.y, v.z);
        }

    }

    public class SimplexNoiseGenerator : ScriptableObject
    {

        /// <summary>
        /// Map coordinate to position in a 1d simplex lattice
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static float SkewCoordinate1d(float x)
        {
            const float f = 0.41421356237f; // (Mathf.Sqrt(1 + 1) - 1) / 1;
            var skewed = 0f;
            skewed = x + x * f;
            return skewed;
        }

        /// <summary>
        /// Map coordinate to position in a 2d simplex lattice
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static Vector2 SkewCoordinate2d(Vector2 v)
        {
            const float f = 0.36602540378f; // (Mathf.Sqrt(2 + 1) - 1) / 2;
            var skewed = Vector2.zero;
            skewed.x = v.x + (v.x + v.y) * f;
            skewed.y = v.y + (v.x + v.y) * f;
            return skewed;
        }

        /// <summary>
        /// Map coordinate to position in 3d simplex lattice
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static Vector3 SkewCoordinate3d(Vector3 v)
        {
            const float f = 0.33333333333f; // (Mathf.Sqrt(3 + 1) - 1) / 3;
            var skewed = Vector3.zero;
            var sum = (v.x + v.y + v.z) * f;
            skewed.x = v.x + sum;
            skewed.y = v.y + sum;
            skewed.z = v.z + sum;
            return skewed;
        }

    }
}
