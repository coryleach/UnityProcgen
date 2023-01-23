using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/NoiseGenerator/PerlinNoise")]
    public class PerlinNoiseGenerator : ScriptableObject
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
}
