using UnityEngine;

namespace Gameframe.Procgen
{
    public static class WaveCollapseExtensions
    {
        public static int RandomWeightedIndex(this double[] weights, double r)
        {
            double sum = 0;
            for (var i = 0; i < weights.Length; i++)
            {
                sum += weights[i];
            }

            var threshold = r * sum;

            double partialSum = 0;
            for (var i = 0; i < weights.Length; i++)
            {
                partialSum += weights[i];
                if (partialSum >= threshold)
                {
                    return i;
                }
            }

            return 0;
        }

        public static long ToPower(this int a, int n)
        {
            long product = 1;
            for (var i = 0; i < n; i++)
            {
                product *= a;
            }
            return product;
        }

        public static bool Agrees(int[] p1, int[] p2, int dx, int dy, int size)
        {
            var xMin = dx < 0 ? 0 : dx;
            var xMax = dx < 0 ? dx + size : size;
            var yMin = dy < 0 ? 0 : dy;
            var yMax = dy < 0 ? dy + size : size;

            for (var y = yMin; y < yMax; y++)
            {
                for (var x = xMin; x < xMax; x++)
                {
                    int i1 = x + size * y;
                    int i2 = x - dx + size * (y - dy);

                    if (p1[x + size * y] != p2[x - dx + size * (y - dy)])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
