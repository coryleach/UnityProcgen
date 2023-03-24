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
    }
}
