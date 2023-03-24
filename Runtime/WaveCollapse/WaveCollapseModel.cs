// Originally by Maxim Gumin
// Adapted from https://github.com/mxgmn/WaveFunctionCollapse
// Copyright (C) 2023 Cory Leach, The MIT License (MIT)

using System;

namespace Gameframe.Procgen
{
    public abstract class WaveCollapseModel
    {
        protected WaveCollapseModelData modelData;

        protected bool[][] wave;

        private int[][][] compatible;
        protected int[] observed; //This array is filled with the final output

        private (int, int)[] stack;
        private int stacksize, observedSoFar;

        protected int outputWidth, outputHeight, totalTiles;
        protected bool periodic, ground;

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

        protected WaveCollapseModel(int width, int height, bool periodic, Heuristic heuristic)
        {
            outputWidth = width;
            outputHeight = height;
            this.periodic = periodic;
            this.heuristic = heuristic;
        }

        public void SetOutputSize(int width, int height)
        {
            wave = null;
            outputWidth = width;
            outputHeight = height;
        }

        private void Init()
        {
            wave = new bool[outputWidth * outputHeight][];
            compatible = new int[wave.Length][][];
            for (var i = 0; i < wave.Length; i++)
            {
                wave[i] = new bool[totalTiles];
                compatible[i] = new int[totalTiles][];
                for (var t = 0; t < totalTiles; t++)
                {
                    compatible[i][t] = new int[4];
                }
            }

            distribution = new double[totalTiles];
            observed = new int[outputWidth * outputHeight];

            weightLogWeights = new double[totalTiles];
            sumOfWeights = 0;
            sumOfWeightLogWeights = 0;

            for (var t = 0; t < totalTiles; t++)
            {
                weightLogWeights[t] = modelData.weights[t] * Math.Log(modelData.weights[t]);
                sumOfWeights += modelData.weights[t];
                sumOfWeightLogWeights += weightLogWeights[t];
            }

            startingEntropy = Math.Log(sumOfWeights) - sumOfWeightLogWeights / sumOfWeights;

            sumsOfOnes = new int[outputWidth * outputHeight];
            sumsOfWeights = new double[outputWidth * outputHeight];
            sumsOfWeightLogWeights = new double[outputWidth * outputHeight];
            entropies = new double[outputWidth * outputHeight];

            stack = new (int, int)[wave.Length * totalTiles];
            stacksize = 0;
        }

        private RandomGenerator currentRng;

        protected bool isDone = true;
        public bool IsDone => isDone;

        public void InitRun(int seed)
        {
            if (wave == null)
            {
                Init();
            }

            Clear();

            isDone = false;
            currentRng = new RandomGenerator((uint) seed);
        }

        public bool StepRun()
        {
            //Find the next node to look at
            //We select it using our heuristic
            var node = NextUnobservedNode(currentRng);
            if (node >= 0)
            {

                Observe(node, currentRng);
                var success = Propagate();
                if (!success)
                {
                    isDone = true;
                    return false;
                }
            }
            else
            {
                //For each "pixel" or tile loop over all the potential patterns
                for (var i = 0; i < wave.Length; i++)
                for (var t = 0; t < totalTiles; t++)
                {
                    //If this pattern is marked true we set it as the observed pattern
                    if (wave[i][t])
                    {
                        observed[i] = t;
                        isDone = true;
                        break;
                    }
                }

                return true;
            }

            return true;
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
                //Find the next node to look at
                //We select it using our heuristic
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
                    //For each "pixel" or tile loop over all the potential patterns
                    for (var i = 0; i < wave.Length; i++)
                    for (var t = 0; t < totalTiles; t++)
                    {
                        //If this pattern is marked true we set it as the observed pattern
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
                    if (!periodic && (i % outputWidth + modelData.patternSize > outputWidth || i / outputWidth + modelData.patternSize > outputHeight))
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
                if (!periodic && (i % outputWidth + modelData.patternSize > outputWidth || i / outputWidth + modelData.patternSize > outputHeight))
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
            var nodeTileCandidates = wave[node];
            //Calculate the distribution for the selected node
            for (var tileIndex = 0; tileIndex < totalTiles; tileIndex++)
            {
                //Only potentially valid patterns should be considered
                //All patterns marked as false should be set to zero
                distribution[tileIndex] = nodeTileCandidates[tileIndex] ? modelData.weights[tileIndex] : 0.0;
            }

            //Select a node to collapse to
            //All other nodes are eliminated from the possibility pool
            var randomWeightedIndex = distribution.RandomWeightedIndex(random.NextDoubleZeroToOne());
            for (var t = 0; t < totalTiles; t++)
            {
                if (nodeTileCandidates[t] != (t == randomWeightedIndex))
                {
                    Ban(node, t);
                }
            }
        }

        private bool Propagate()
        {
            while (stacksize > 0)
            {
                (var bannedNode, var bannedTileIndex) = stack[stacksize - 1];
                stacksize--;

                var bannedNodeX = bannedNode % outputWidth;
                var bannedNodeY = bannedNode / outputWidth;

                for (var directionIndex = 0; directionIndex < 4; directionIndex++)
                {
                    var neighborX = bannedNodeX + dx[directionIndex];
                    var neighborY = bannedNodeY + dy[directionIndex];

                    //Skip if neighbor is invalid and we're not "periodic"
                    //Ok so "periodic" means "wrap"
                    if (!periodic && (neighborX < 0 || neighborY < 0 || neighborX + modelData.patternSize > outputWidth || neighborY + modelData.patternSize > outputHeight))
                    {
                        continue;
                    }

                    //Wrap X
                    if (neighborX < 0)
                    {
                        neighborX += outputWidth;
                    }
                    else if (neighborX >= outputWidth)
                    {
                        neighborX -= outputWidth;
                    }

                    //Wrap Y
                    if (neighborY < 0)
                    {
                        neighborY += outputHeight;
                    }
                    else if (neighborY >= outputHeight)
                    {
                        neighborY -= outputHeight;
                    }

                    var neighborNodeIndex = neighborX + neighborY * outputWidth;
                    var prop = modelData.propagator[directionIndex][bannedTileIndex];
                    var compat = compatible[neighborNodeIndex];

                    for (var i = 0; i < prop.Length; i++)
                    {
                        var t2 = prop[i];
                        var comp = compat[t2];

                        comp[directionIndex]--;
                        if (comp[directionIndex] == 0)
                        {
                            Ban(neighborNodeIndex, t2);
                        }
                    }
                }
            }

            return sumsOfOnes[0] > 0;
        }

        /// <summary>
        /// Remove a tile from consideration for a given position
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tileIndex"></param>
        private void Ban(int node, int tileIndex)
        {
            wave[node][tileIndex] = false;

            var comp = compatible[node][tileIndex];
            for (var direction = 0; direction < 4; direction++)
            {
                comp[direction] = 0;
            }

            stack[stacksize] = (node, tileIndex);
            stacksize++;

            sumsOfOnes[node] -= 1;
            sumsOfWeights[node] -= modelData.weights[tileIndex];
            sumsOfWeightLogWeights[node] -= weightLogWeights[tileIndex];

            var sum = sumsOfWeights[node];
            entropies[node] = Math.Log(sum) - sumsOfWeightLogWeights[node] / sum;
        }

        private void Clear()
        {
            for (int i = 0; i < wave.Length; i++)
            {
                for (int t = 0; t < totalTiles; t++)
                {
                    wave[i][t] = true;
                    for (int d = 0; d < 4; d++)
                    {
                        compatible[i][t][d] = modelData.propagator[opposite[d]][t].Length;
                    }
                }

                sumsOfOnes[i] = modelData.weights.Length;
                sumsOfWeights[i] = sumOfWeights;
                sumsOfWeightLogWeights[i] = sumOfWeightLogWeights;
                entropies[i] = startingEntropy;
                observed[i] = -1;
            }

            observedSoFar = 0;

            if (ground)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    for (int t = 0; t < totalTiles - 1; t++)
                    {
                        Ban(x + (outputHeight - 1) * outputWidth, t);
                    }

                    for (int y = 0; y < outputHeight - 1; y++)
                    {
                        Ban(x + y * outputWidth, totalTiles - 1);
                    }
                }

                Propagate();
            }
        }
    }
}
