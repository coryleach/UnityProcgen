// Originally by Maxim Gumin
// Adapted from https://github.com/mxgmn/WaveFunctionCollapse
// Copyright (C) 2023 Cory Leach, The MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameframe.Procgen
{
    [Serializable]
    public class ColorWaveCollapseModel : WaveCollapseModelData
    {
        public List<Color> colors;
    }

    public class TextureWaveCollapseModel : WaveCollapseModel
    {
        private readonly ColorWaveCollapseModel colorModelData;
        public ColorWaveCollapseModel ModelData => colorModelData;

        /// <summary>
        ///
        /// </summary>
        /// <param name="sourceTexture"></param>
        /// <param name="adjacentPixelDistance">How many pixels out from the start pixel should we look</param>
        /// <param name="outputWidth"></param>
        /// <param name="outputHeight"></param>
        /// <param name="periodicInput"></param>
        /// <param name="periodic"></param>
        /// <param name="symmetry">1 to 8 representing how many directions of symmetry</param>
        /// <param name="ground"></param>
        /// <param name="heuristic"></param>
        public TextureWaveCollapseModel(Texture2D sourceTexture, int adjacentPixelDistance, int outputWidth, int outputHeight, bool periodicInput, bool periodic, int symmetry, bool ground, Heuristic heuristic) : base(outputWidth, outputHeight, periodic, heuristic)
        {
            var bitmap = sourceTexture.GetPixels();
            var sourceWidth = sourceTexture.width;
            var sourceHeight = sourceTexture.height;

            colorModelData = new ColorWaveCollapseModel
            {
                colors = new List<Color>(),
                patterns = new List<WaveCollapseModelData.PatternList>(),
                patternSize = 1 + adjacentPixelDistance * 2,
            };

            modelData = colorModelData;

            //This array will map each position in bitmap to an index in _colors
            var sample = new int[bitmap.Length];

            //Identify each unique color and build a list of colors
            //Also build our sample map
            for (var i = 0; i < sample.Length; i++)
            {
                var color = bitmap[i];
                var k = 0;
                for (; k < colorModelData.colors.Count; k++)
                {
                    if (colorModelData.colors[k] == color)
                    {
                        break;
                    }
                }

                if (k == colorModelData.colors.Count)
                {
                    colorModelData.colors.Add(color);
                }

                sample[i] = (byte) k;
            }

            Dictionary<long, int> patternIndices = new();
            List<double> weightList = new();

            var colorsCount = colorModelData.colors.Count;
            var xMax = periodicInput ? sourceWidth : sourceWidth - modelData.patternSize + 1;
            var yMax = periodicInput ? sourceHeight : sourceHeight - modelData.patternSize + 1;

            // Build a list of different patterns
            // Each time we encounter the same pattern we add weight to it
            for (var y = 0; y < yMax; y++)
            for (var x = 0; x < xMax; x++)
            {
                int[][] ps = new int[8][];

                ps[0] = Pattern((dx, dy) =>
                {
                    // var x2 = x + (dx - adjacentPixelDistance);
                    // var y2 = y + (dy - adjacentPixelDistance);
                    // if (x2 < 0 || y2 < 0 || x2 >= sourceWidth || y2 >= sourceHeight)
                    // {
                    //     return -1;
                    // }
                    //
                    // var index = x2 + y2 * sourceWidth;
                    // return sample[index];

                    // This version wraps around the texture
                    return sample[(x + dx) % sourceWidth + (y + dy) % sourceHeight * sourceWidth];
                }, modelData.patternSize);
                ps[1] = Reflect(ps[0], modelData.patternSize);
                ps[2] = Rotate(ps[0], modelData.patternSize);
                ps[3] = Reflect(ps[2], modelData.patternSize);
                ps[4] = Rotate(ps[2], modelData.patternSize);
                ps[5] = Reflect(ps[4], modelData.patternSize);
                ps[6] = Rotate(ps[4], modelData.patternSize);
                ps[7] = Reflect(ps[6], modelData.patternSize);

                for (var k = 0; k < symmetry; k++)
                {
                    var p = ps[k];
                    var h = Hash(p, colorsCount);
                    if (patternIndices.TryGetValue(h, out int index))
                    {
                        weightList[index] = weightList[index] + 1;
                    }
                    else
                    {
                        patternIndices.Add(h, weightList.Count);
                        weightList.Add(1.0);
                        modelData.patterns.Add(new WaveCollapseModelData.PatternList(p));
                    }
                }
            }

            modelData.weights = weightList.ToArray();

            totalTiles = modelData.weights.Length;

            this.ground = ground;

            modelData.propagator = new WaveCollapseModelData.Propagator();

            for (var directionIndex = 0; directionIndex < 4; directionIndex++)
            {
                modelData.propagator[directionIndex] = new WaveCollapseModelData.PropagatorDirection(totalTiles);

                for (var tileIndex = 0; tileIndex < totalTiles; tileIndex++)
                {
                    //Build a list of patterns that match this tile for the given direction
                    List<int> tempList = new();
                    for (var tileIndex2 = 0; tileIndex2 < totalTiles; tileIndex2++)
                    {
                        if (Agrees(modelData.patterns[tileIndex].array, modelData.patterns[tileIndex2].array, dx[directionIndex], dy[directionIndex], modelData.patternSize))
                        {
                            tempList.Add(tileIndex2);
                        }
                    }

                    modelData.propagator[directionIndex][tileIndex] = tempList.ToArray();
                }
            }
        }

        public TextureWaveCollapseModel(WaveCollapseModelData modelData, int outputWidth, int outputHeight, bool periodic, Heuristic heuristic) : base(outputWidth, outputHeight, periodic, heuristic)
        {
            this.modelData = modelData;
        }

        public IEnumerable<Texture2D> GetPatternsAsTextures()
        {
            foreach (var pattern in modelData.patterns)
            {
                //Get Colors
                var colors = pattern.array.Select(x => colorModelData.colors[x]).ToArray();
                var tex = new Texture2D(modelData.patternSize, modelData.patternSize);
                tex.filterMode = FilterMode.Point;
                tex.SetPixels(colors);
                tex.Apply();
                yield return tex;
            }
        }

        public void Save(Texture2D texture2D)
        {
            var bitmap = new Color[outputWidth * outputHeight];
            Save(bitmap);
            texture2D.SetPixels(bitmap);
            texture2D.Apply(false);
        }

        public void Save(Color[] bitmap)
        {
            if (bitmap.Length < (outputWidth * outputHeight))
            {
                throw new Exception("Array not correct size");
            }

            if (observed[0] >= 0)
            {
                for (var y = 0; y < outputHeight; y++)
                {
                    var dy = y < outputHeight - modelData.patternSize + 1 ? 0 : modelData.patternSize - 1;
                    for (var x = 0; x < outputWidth; x++)
                    {
                        var dx = x < outputWidth - modelData.patternSize + 1 ? 0 : modelData.patternSize - 1;
                        var obs = observed[x - dx + (y - dy) * outputWidth];
                        var something = modelData.patterns[obs][dx + dy * modelData.patternSize];
                        bitmap[x + y * outputWidth] = colorModelData.colors[something];
                    }
                }
            }
            else
            {
                for (var i = 0; i < wave.Length; i++)
                {
                    int contributors = 0;
                    float r = 0, g = 0, b = 0;
                    int x = i % outputWidth, y = i / outputWidth;
                    for (var dy = 0; dy < modelData.patternSize; dy++)
                    for (int dx = 0; dx < modelData.patternSize; dx++)
                    {
                        var sx = x - dx;
                        if (sx < 0)
                        {
                            sx += outputWidth;
                        }

                        var sy = y - dy;
                        if (sy < 0)
                        {
                            sy += outputHeight;
                        }

                        int s = sx + sy * outputWidth;
                        if (!periodic && (sx + modelData.patternSize > outputWidth || sy + modelData.patternSize > outputHeight || sx < 0 || sy < 0))
                        {
                            continue;
                        }

                        for (int t = 0; t < totalTiles; t++)
                        {
                            if (wave[s][t])
                            {
                                contributors++;
                                var argb = colorModelData.colors[modelData.patterns[t][dx + dy * modelData.patternSize]];
                                r += argb.r;
                                g += argb.g;
                                b += argb.b;
                            }
                        }
                    }

                    if (contributors == 0)
                    {
                        bitmap[i] = Color.red;
                    }
                    else
                    {
                        bitmap[i] = new Color(r / contributors, g / contributors, b / contributors);
                    }
                }
            }
        }

        private static int[] Reflect(int[] p, int N) => Pattern((x, y) => p[N - 1 - x + y * N], N);

        private static int[] Rotate(int[] p, int N) => Pattern((x, y) => p[N - 1 - y + x * N], N);

        private static long Hash(int[] p, int C)
        {
            long result = 0, power = 1;
            for (var i = 0; i < p.Length; i++)
            {
                result += p[p.Length - 1 - i] * power;
                power *= C;
            }

            return result;
        }

        private static bool Agrees(int[] p1, int[] p2, int dx, int dy, int size)
        {
            var xMin = dx < 0 ? 0 : dx;
            var xMax = dx < 0 ? dx + size : size;
            var yMin = dy < 0 ? 0 : dy;
            var yMax = dy < 0 ? dy + size : size;

            for (var y = yMin; y < yMax; y++)
            for (var x = xMin; x < xMax; x++)
            {
                if (p1[x + size * y] != p2[x - dx + size * (y - dy)])
                {
                    return false;
                }
            }

            return true;
        }

        private static int[] Pattern(Func<int, int, int> f, int size)
        {
            var result = new int[size * size];
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                result[x + y * size] = f(x, y);
            }

            return result;
        }
    }
}
