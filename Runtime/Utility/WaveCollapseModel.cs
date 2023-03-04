// Originally by Maxim Gumin
// Adapted from https://github.com/mxgmn/WaveFunctionCollapse
// Copyright (C) 2023 Cory Leach, The MIT License (MIT)

using System;

namespace Gameframe.Procgen
{
    public abstract class WaveCollapseModel
    {
        protected bool[][] wave;

        protected int[][][] propagator;
        private int[][][] compatible;
        protected int[] observed;

        private (int, int)[] stack;
        private int stacksize, observedSoFar;

        protected int MX, MY, T, N;
        protected bool periodic, ground;

        protected double[] weights;
        private double[] weightLogWeights, distribution;

        protected int[] sumsOfOnes;
        private double sumOfWeights, sumOfWeightLogWeights, startingEntropy;
        protected double[] sumsOfWeights, sumsOfWeightLogWeights, entropies;

        protected static int[] dx = {-1, 0, 1, 0};
        protected static int[] dy = {0, 1, 0, -1};
        private static int[] opposite = {2, 3, 0, 1};

        public enum Heuristic
        {
            Entropy,
            MRV,
            Scanline
        };

        private readonly Heuristic heuristic;

        protected WaveCollapseModel(int width, int height, int N, bool periodic, Heuristic heuristic)
        {
            MX = width;
            MY = height;
            this.N = N;
            this.periodic = periodic;
            this.heuristic = heuristic;
        }

        private void Init()
        {
            wave = new bool[MX * MY][];
            compatible = new int[wave.Length][][];
            for (var i = 0; i < wave.Length; i++)
            {
                wave[i] = new bool[T];
                compatible[i] = new int[T][];
                for (var t = 0; t < T; t++)
                {
                    compatible[i][t] = new int[4];
                }
            }

            distribution = new double[T];
            observed = new int[MX * MY];

            weightLogWeights = new double[T];
            sumOfWeights = 0;
            sumOfWeightLogWeights = 0;

            for (var t = 0; t < T; t++)
            {
                weightLogWeights[t] = weights[t] * Math.Log(weights[t]);
                sumOfWeights += weights[t];
                sumOfWeightLogWeights += weightLogWeights[t];
            }

            startingEntropy = Math.Log(sumOfWeights) - sumOfWeightLogWeights / sumOfWeights;

            sumsOfOnes = new int[MX * MY];
            sumsOfWeights = new double[MX * MY];
            sumsOfWeightLogWeights = new double[MX * MY];
            entropies = new double[MX * MY];

            stack = new (int, int)[wave.Length * T];
            stacksize = 0;
        }

        public bool Run(int seed, int limit)
        {
            if (wave == null)
            {
                Init();
            }

            Clear();

            var random = new RandomGenerator((uint) seed);

            for (var l = 0; l < limit || limit < 0; l++)
            {
                var node = NextUnobservedNode(random);
                if (node >= 0)
                {
                    Observe(node, random);
                    var success = Propagate();
                    if (!success)
                    {
                        return false;
                    }
                }
                else
                {
                    for (var i = 0; i < wave.Length; i++)
                    for (var t = 0; t < T; t++)
                    {
                        if (wave[i][t])
                        {
                            observed[i] = t;
                            break;
                        }
                    }

                    return true;
                }
            }

            return true;
        }

        private int NextUnobservedNode(IRandomNumberGenerator random)
        {
            if (heuristic == Heuristic.Scanline)
            {
                for (var i = observedSoFar; i < wave.Length; i++)
                {
                    if (!periodic && (i % MX + N > MX || i / MX + N > MY))
                    {
                        continue;
                    }

                    if (sumsOfOnes[i] > 1)
                    {
                        observedSoFar = i + 1;
                        return i;
                    }
                }

                return -1;
            }

            var min = 1E+4;
            var argmin = -1;
            for (var i = 0; i < wave.Length; i++)
            {
                if (!periodic && (i % MX + N > MX || i / MX + N > MY))
                {
                    continue;
                }

                var remainingValues = sumsOfOnes[i];
                var entropy = heuristic == Heuristic.Entropy ? entropies[i] : remainingValues;
                if (remainingValues <= 1 || !(entropy <= min))
                {
                    continue;
                }

                var noise = 1E-6 * random.NextDoubleZeroToOne();
                if (!(entropy + noise < min))
                {
                    continue;
                }

                min = entropy + noise;
                argmin = i;
            }

            return argmin;
        }

        private void Observe(int node, IRandomNumberGenerator random)
        {
            var w = wave[node];
            for (var t = 0; t < T; t++)
            {
                distribution[t] = w[t] ? weights[t] : 0.0;
            }

            var r = distribution.Random(random.NextDoubleZeroToOne());
            for (var t = 0; t < T; t++)
            {
                if (w[t] != (t == r))
                {
                    Ban(node, t);
                }
            }
        }

        private bool Propagate()
        {
            while (stacksize > 0)
            {
                (var i1, var t1) = stack[stacksize - 1];
                stacksize--;

                var x1 = i1 % MX;
                var y1 = i1 / MX;

                for (var d = 0; d < 4; d++)
                {
                    var x2 = x1 + dx[d];
                    var y2 = y1 + dy[d];
                    if (!periodic && (x2 < 0 || y2 < 0 || x2 + N > MX || y2 + N > MY))
                    {
                        continue;
                    }

                    if (x2 < 0)
                    {
                        x2 += MX;
                    }
                    else if (x2 >= MX)
                    {
                        x2 -= MX;
                    }

                    if (y2 < 0)
                    {
                        y2 += MY;
                    }
                    else if (y2 >= MY)
                    {
                        y2 -= MY;
                    }

                    var i2 = x2 + y2 * MX;
                    var p = propagator[d][t1];
                    var compat = compatible[i2];

                    for (var l = 0; l < p.Length; l++)
                    {
                        var t2 = p[l];
                        var comp = compat[t2];

                        comp[d]--;
                        if (comp[d] == 0)
                        {
                            Ban(i2, t2);
                        }
                    }
                }
            }

            return sumsOfOnes[0] > 0;
        }

        private void Ban(int i, int t)
        {
            wave[i][t] = false;

            var comp = compatible[i][t];
            for (var d = 0; d < 4; d++)
            {
                comp[d] = 0;
            }

            stack[stacksize] = (i, t);
            stacksize++;

            sumsOfOnes[i] -= 1;
            sumsOfWeights[i] -= weights[t];
            sumsOfWeightLogWeights[i] -= weightLogWeights[t];

            var sum = sumsOfWeights[i];
            entropies[i] = Math.Log(sum) - sumsOfWeightLogWeights[i] / sum;
        }

        private void Clear()
        {
            for (int i = 0; i < wave.Length; i++)
            {
                for (int t = 0; t < T; t++)
                {
                    wave[i][t] = true;
                    for (int d = 0; d < 4; d++)
                    {
                        compatible[i][t][d] = propagator[opposite[d]][t].Length;
                    }
                }

                sumsOfOnes[i] = weights.Length;
                sumsOfWeights[i] = sumOfWeights;
                sumsOfWeightLogWeights[i] = sumOfWeightLogWeights;
                entropies[i] = startingEntropy;
                observed[i] = -1;
            }

            observedSoFar = 0;

            if (ground)
            {
                for (int x = 0; x < MX; x++)
                {
                    for (int t = 0; t < T - 1; t++)
                    {
                        Ban(x + (MY - 1) * MX, t);
                    }

                    for (int y = 0; y < MY - 1; y++)
                    {
                        Ban(x + y * MX, T - 1);
                    }
                }

                Propagate();
            }
        }
    }
}
