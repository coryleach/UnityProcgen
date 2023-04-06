// Originally by Maxim Gumin
// Adapted from https://github.com/mxgmn/WaveFunctionCollapse
// Copyright (C) 2023 Cory Leach, The MIT License (MIT)

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    public abstract class BaseWaveCollapseModel
    {
        protected BaseWaveCollapseModelData modelData;

        public struct CollapseEntry
        {
            public int node;
            public int patternIndex;
        }

        public List<CollapseEntry> collapseEntries = new List<CollapseEntry>();

        /// <summary>
        /// The wave array is a mapping of node index to an array of boolean values
        /// which indicate which patterns are still possible for the given node
        ///
        /// wave[nodeIndex][patternTileIndex]
        /// </summary>
        protected bool[][] wave;

        /// <summary>
        /// Compatibility map allows us to ask how many compatible tiles remain in each direction
        /// _combatibilityMap[nodeIndex][patternIndex][direction]
        /// </summary>
        protected List<int>[][][] _compatibilityMap;

        //This array is filled with the final output
        protected int[] observed;

        /// <summary>
        /// This stack contains the banned nodes and the banned values
        /// This is used when we propagate observed state to neighboring nodes
        /// </summary>
        private Stack<(int, int)> _stack;

        private int observedSoFar;

        protected int outputWidth;
        public int OutputWidth => outputWidth;

        protected int outputHeight;
        public int OutputHeight => outputHeight;

        protected int totalTilePatterns;

        protected bool _periodic;

        // Contains weight of a pattern multiplied by the Log of that weight
        // Not sure why we need this. I assume it relates to entropy calculation
        private double[] weightLogWeights;

        //Used to calculate the distribution
        //Essentially holds the list of weights that will be used when observing a node
        //The length of this array should be the total number of patterns
        private double[] distribution;

        // This tracks the number of patterns remaining for a given node
        protected int[] sumsPatternCount; //sumsPatternCount[nodeIndex]

        private double sumOfWeights; //Total of all weights of all patterns
        private double sumOfWeightLogWeights;
        private double startingEntropy;

        protected double[] sumsOfWeights; //sumOfWeights[nodeIndex]
        protected double[] sumsOfWeightLogWeights; //sumOfWeightLogWeights[nodeIndex]
        protected double[] entropies; //entropies[nodeIndex]

        protected static int[] dx = {-1, 0, 1, 0};
        protected static int[] dy = {0, 1, 0, -1};
        private static int[] opposite = {2, 3, 0, 1};

        public enum Heuristic
        {
            Entropy,
            MRV, //M? Remaining Value
            Scanline
        };

        private Heuristic _heuristic;

        private void _Init()
        {
            wave = new bool[outputWidth * outputHeight][];

            _compatibilityMap = new List<int>[wave.Length][][];

            for (var i = 0; i < wave.Length; i++)
            {
                wave[i] = new bool[totalTilePatterns];
                _compatibilityMap[i] = new List<int>[totalTilePatterns][];

                for (var t = 0; t < totalTilePatterns; t++)
                {
                    _compatibilityMap[i][t] = new List<int>[4];
                }
            }

            distribution = new double[totalTilePatterns];
            observed = new int[outputWidth * outputHeight];

            weightLogWeights = new double[totalTilePatterns];
            sumOfWeights = 0;
            sumOfWeightLogWeights = 0;

            for (var t = 0; t < totalTilePatterns; t++)
            {
                weightLogWeights[t] = modelData.weights[t] * Math.Log(modelData.weights[t]);
                sumOfWeights += modelData.weights[t];
                sumOfWeightLogWeights += weightLogWeights[t];
            }

            startingEntropy = Math.Log(sumOfWeights) - sumOfWeightLogWeights / sumOfWeights;

            sumsPatternCount = new int[outputWidth * outputHeight];
            sumsOfWeights = new double[outputWidth * outputHeight];
            sumsOfWeightLogWeights = new double[outputWidth * outputHeight];
            entropies = new double[outputWidth * outputHeight];

            _stack = new Stack<(int, int)>();
        }

        private RandomGenerator _currentRng;

        public void Init(int seed, int width, int height, bool periodic, Heuristic heuristic)
        {
            outputWidth = width;
            outputHeight = height;
            this._periodic = periodic;
            _heuristic = heuristic;
            _currentRng = new RandomGenerator((uint) seed);

            if (wave == null)
            {
                _Init();
            }

            Clear();
            BanEdgePatterns();
        }

        public bool Run(int limit)
        {
            for (var l = 0; l < limit || limit < 0; l++)
            {
                var result = Observe(_currentRng);
                if (result != null)
                {
                    return result.Value;
                }

                Propagate();
            }

            return false;
        }

        private int NextUnobservedNode(IRandomNumberGenerator random)
        {
            if (_heuristic == Heuristic.Scanline)
            {
                for (var i = observedSoFar; i < wave.Length; i++)
                {
                    if (OnBoundary(i))
                    {
                        continue;
                    }

                    if (sumsPatternCount[i] > 1)
                    {
                        observedSoFar = i + 1;
                        return i;
                    }

                    if (sumsPatternCount[i] == 1 && observed[i] == -1)
                    {
                        //Add to observed list
                        var w = wave[i];
                        for (var t = 0; t < w.Length; t++)
                        {
                            if (w[t])
                            {
                                CollapseNode(i, t);
                                break;
                            }
                        }
                    }
                }

                return -1;
            }

            var entropyMin = 1E+4;
            var nodeIndex = -1;
            for (var i = 0; i < wave.Length; i++)
            {
                if (OnBoundary(i))
                {
                    continue;
                }

                var remainingValues = sumsPatternCount[i];
                var entropy = _heuristic == Heuristic.Entropy ? entropies[i] : remainingValues;
                if (remainingValues > 1 && entropy <= entropyMin)
                {
                    var noise = 1E-6 * _currentRng.NextDoubleZeroToOne();
                    if (entropy + noise < entropyMin)
                    {
                        entropyMin = entropy + noise;
                        nodeIndex = i;
                    }
                }
            }

            return nodeIndex;
        }

        private bool OnBoundary(int nodeIndex)
        {
            var (x, y) = GetXY(nodeIndex);
            return OnBoundary(x, y);
        }

        private bool OnBoundary(int x, int y)
        {
            return !_periodic && (x + modelData.patternSize > outputWidth || y + modelData.patternSize > outputHeight || x < 0 || y < 0);
        }

        protected bool? Observe(IRandomNumberGenerator random)
        {
            var node = NextUnobservedNode(random);
            if (node < 0)
            {
                for (var i = 0; i < wave.Length; i++)
                for (var t = 0; t < totalTilePatterns; t++)
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

            //Select a pattern to collapse to from the current possibilities
            //All other nodes are eliminated from the possibility pool
            var patternIndex = RandomlySelectAvailablePattern(node, random);
            if (!patternIndex.HasValue)
            {
                var (x, y) = GetXY(node);
                Debug.LogError($"Failed to select pattern {node} ({x},{y})");
                return false;
            }

            CollapseNode(node, patternIndex.Value);

            return null;
        }

        protected void CollapseNode(int node, int observedPatternIndex)
        {
            collapseEntries.Add(new CollapseEntry
            {
                node = node,
                patternIndex = observedPatternIndex
            });
            var w = wave[node];
            observed[node] = observedPatternIndex;
            for (var currentPatternIndex = 0; currentPatternIndex < totalTilePatterns; currentPatternIndex++)
            {
                if (w[currentPatternIndex] != (observedPatternIndex == currentPatternIndex))
                {
                    Ban(node, currentPatternIndex);
                }
            }
        }

        private int? RandomlySelectAvailablePattern(int node, IRandomNumberGenerator random)
        {
            var candidates = new List<int>();
            var candidateWeights = new List<double>();
            var weightSum = 0.0;

            for (var patternIndex = 0; patternIndex < totalTilePatterns; patternIndex++)
            {
                //Skipping invalid patterns
                if (!wave[node][patternIndex])
                {
                    continue;
                }

                candidates.Add(patternIndex);
                var weight = modelData.weights[patternIndex];
                weightSum += weight;
                candidateWeights.Add(weight);
            }

            if (candidates.Count == 0)
            {
                return null;
            }

            var threshold = random.NextDoubleZeroToOne() * weightSum;

            double sum = 0;
            for (int i = 0; i < candidates.Count; i++)
            {
                sum += candidateWeights[i];
                if (threshold <= sum)
                {
                    return candidates[i];
                }
            }

            return null;

            // CalculateDistribution(node);
            // var r = random.NextDoubleZeroToOne();
            // return distribution.RandomWeightedIndex(r);
        }

        private void CalculateDistribution(int node)
        {
            //A list of which patterns are available
            var nodeCandidates = wave[node];

            //Calculate the distribution for the selected node
            for (var patternIndex = 0; patternIndex < totalTilePatterns; patternIndex++)
            {
                //Only potentially valid patterns should be considered
                //All patterns marked as false should be set to zero
                //All candidate patterns marked as true will use the weight assigned to that pattern
                distribution[patternIndex] = nodeCandidates[patternIndex] ? modelData.weights[patternIndex] : 0.0;
            }
        }

        protected void Propagate()
        {
            while (_stack.Count > 0)
            {
                (var nodeIndex, var bannedPatternIndex) = _stack.Pop();

                var nodeX = nodeIndex % outputWidth;
                var nodeY = nodeIndex / outputHeight;

                var log = nodeIndex == 0 && bannedPatternIndex == 1;

                for (int dir = 0; dir < 4; dir++)
                {
                    bool up = dx[dir] == 0 && dy[dir] == 1 && nodeIndex == 0;

                    int neighborX = nodeX + dx[dir];
                    int neighborY = nodeY + dy[dir];

                    if (OnBoundary(neighborX, neighborY))
                    {
                        continue;
                    }

                    //Wrap X and Y
                    (neighborX, neighborY) = WrapXY(neighborX, neighborY);

                    var neighborIndex = neighborX + neighborY * outputWidth;

                    //Get all the tiles that match the banned pattern in the given direction
                    int[] matchingPatterns = modelData.propagator[dir][bannedPatternIndex];

                    //Get compatibility map for neighbor
                    var compat = _compatibilityMap[neighborIndex];

                    for (int i = 0; i < totalTilePatterns; i++)
                    {
                        for (int d = 0; d < 4; d++)
                        {
                            if (compat[i][d].Count == 0)
                            {

                            }
                        }
                    }

                    for (int l = 0; l < matchingPatterns.Length; l++)
                    {
                        //This pattern matches the banned pattern in the g
                        int patternIndex = matchingPatterns[l];
                        var comp = compat[patternIndex];

                        if (log && neighborIndex == 16)
                        {
                            Debug.Log($"Removing: {neighborIndex} {patternIndex} from dir: {dx[dir]},{dy[dir]}");
                        }

                        comp[dir].Remove(bannedPatternIndex);
                        if (comp[dir].Count == 0)
                        {
                            // if (up)
                            // {
                            //     Debug.Log($"Banning {neighborIndex} - {patternIndex}");
                            // }

                            Ban(neighborIndex, patternIndex);
                        }
                        else
                        {
                            // if (up)
                            // {
                            //     Debug.Log($"Not Banning {neighborIndex} - {patternIndex}");
                            // }
                        }

                        // comp[dir]--;
                        // if (comp[dir] == 0)
                        // {
                        //     Ban(neighborIndex, patternIndex);
                        // }
                    }
                }
            }
        }

        private int GetNodeIndex(int x, int y)
        {
            return x + y * outputWidth;
        }

        private (int x, int y) GetXY(int nodeIndex)
        {
            return (nodeIndex % outputWidth, nodeIndex / outputWidth);
        }

        private (int, int) WrapXY(int x, int y)
        {
            if (x < 0)
            {
                x += outputWidth;
            }
            else if (x >= outputWidth)
            {
                x -= outputWidth;
            }

            if (y < 0)
            {
                y += outputHeight;
            }
            else if (y >= outputHeight)
            {
                y -= outputHeight;
            }

            return (x, y);
        }

        public bool IsContradiction()
        {
            return !(sumsPatternCount[0] > 0);
        }

        public bool? StepRun()
        {
            var result = Observe(_currentRng);
            if (result != null)
            {
                return result.Value;
            }

            Propagate();
            return null;
        }

        /// <summary>
        /// Remove a pattern/tile from consideration for a given position
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <param name="patternIndex"></param>
        private void Ban(int nodeIndex, int patternIndex)
        {
            //Do nothing if already banned
            if (!wave[nodeIndex][patternIndex])
            {
                return;
            }

            //Mark this pattern/tile as no longer available
            wave[nodeIndex][patternIndex] = false;

            //Mark it as unavailable for all directions
            var comp = _compatibilityMap[nodeIndex][patternIndex];
            for (var direction = 0; direction < 4; direction++)
            {
                comp[direction].Clear();
            }

            //Push this ban event onto the stack for later propagation
            _stack.Push((nodeIndex, patternIndex));

            double sum = sumsOfWeights[nodeIndex];
            entropies[nodeIndex] += sumsOfWeightLogWeights[nodeIndex] / sum - Math.Log(sum);

            sumsPatternCount[nodeIndex] -= 1;
            sumsOfWeights[nodeIndex] -= modelData.weights[patternIndex];
            sumsOfWeightLogWeights[nodeIndex] -= weightLogWeights[patternIndex];

            sum = sumsOfWeights[nodeIndex];
            entropies[nodeIndex] -= sumsOfWeightLogWeights[nodeIndex] / sum - Math.Log(sum);
        }

        protected virtual void Clear()
        {
            collapseEntries.Clear();

            for (int nodeIndex = 0; nodeIndex < wave.Length; nodeIndex++)
            {
                for (int patternIndex = 0; patternIndex < totalTilePatterns; patternIndex++)
                {
                    //Mark all patterns as available
                    wave[nodeIndex][patternIndex] = true;

                    //Assign the number of compatible patterns
                    for (int direction = 0; direction < 4; direction++)
                    {
                        //For each node+pattern combination get the number of valid patterns for the opposite direction
                        //We track this so we can track when a given pattern is no longer a valid option for a node
                        //Allows us to ask, how many tiles are available to match in the given direction
                        _compatibilityMap[nodeIndex][patternIndex][direction] = new List<int>(modelData.propagator[opposite[direction]][patternIndex]);
                    }
                }

                sumsPatternCount[nodeIndex] = modelData.weights.Length;
                sumsOfWeights[nodeIndex] = sumOfWeights;
                sumsOfWeightLogWeights[nodeIndex] = sumOfWeightLogWeights;
                entropies[nodeIndex] = startingEntropy;
                observed[nodeIndex] = -1;
            }

            observedSoFar = 0;
        }

        protected virtual void BanEdgePatterns()
        {
            //Look for patterns that don't have valid entries for a direction and ban them everywhere but the edge
            for (int patternIndex = 0; patternIndex < totalTilePatterns; patternIndex++)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    var matches = modelData.propagator[dir][patternIndex];
                    if (matches.Length == 0)
                    {
                        var negDir = opposite[dir];
                        //We need to ban this pattern everywhere but the edge
                        for (int y = 0; y < outputHeight; y++)
                        {
                            for (int x = 0; x < outputWidth; x++)
                            {
                                var nodeX = (x + dx[negDir]);
                                var nodeY = (y + dy[negDir]);
                                if (nodeX < 0 || nodeX >= outputWidth || nodeY < 0 || nodeY >= outputHeight)
                                {
                                    continue;
                                }
                                int node = nodeX + nodeY * outputWidth;
                                Ban(node, patternIndex);
                            }
                        }
                    }
                }
            }
            Propagate();
        }

    }
}
