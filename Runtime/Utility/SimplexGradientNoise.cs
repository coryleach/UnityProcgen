using UnityEngine;

namespace Gameframe.Procgen
{
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