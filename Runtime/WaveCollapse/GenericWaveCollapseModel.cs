using System;
using System.Collections.Generic;

namespace Gameframe.Procgen
{
    public abstract class GenericWaveCollapseModel<TTile> : BaseWaveCollapseModel
    {
        public GenericWaveCollapseModelData<TTile> ModelData { get; private set; }

        protected void BuildModel(GenericWaveCollapseModelData<TTile> model, TTile[] tilemap, int width, int height, int adjacentDistance, int symmetry, bool periodicInput)
        {
            var sourceWidth = width;
            var sourceHeight = height;

            ModelData = model;
            modelData = model;

            ModelData.values = new List<TTile>();
            ModelData.patterns = new List<BaseWaveCollapseModelData.PatternList>();
            ModelData.patternSize = 1 + adjacentDistance * 2;

            //This array will map each position in bitmap to an index in _colors
            var sample = new int[tilemap.Length];

            //Identify each unique color and build a list of colors
            //Also build our sample map
            for (var i = 0; i < sample.Length; i++)
            {
                var tileValue = tilemap[i];
                var k = 0;
                for (; k < ModelData.values.Count; k++)
                {
                    if (ModelData.values[k].Equals(tileValue))
                    {
                        break;
                    }
                }

                if (k == ModelData.values.Count)
                {
                    ModelData.values.Add(tileValue);
                }

                sample[i] = (byte) k;
            }

            Dictionary<long, int> patternIndices = new();
            List<double> weightList = new();

            var valueCount = ModelData.values.Count;
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
                    var h = Hash(p, valueCount);
                    if (patternIndices.TryGetValue(h, out int index))
                    {
                        //Increase weight of pattern we've already seen
                        weightList[index] += 1.0;
                    }
                    else
                    {
                        //New pattern
                        patternIndices.Add(h, weightList.Count);
                        weightList.Add(1.0);
                        modelData.patterns.Add(new BaseWaveCollapseModelData.PatternList(p));
                    }
                }
            }

            modelData.weights = weightList.ToArray();

            totalTilePatterns = modelData.weights.Length;

            modelData.propagator = new BaseWaveCollapseModelData.Propagator();

            for (var dir = 0; dir < 4; dir++)
            {
                modelData.propagator[dir] = new BaseWaveCollapseModelData.PropagatorDirection(totalTilePatterns, dx[dir], dy[dir]);

                for (var patternIndex = 0; patternIndex < totalTilePatterns; patternIndex++)
                {
                    //Build a list of patterns that match this pattern in the given direction
                    List<int> tempList = new();
                    for (var patternIndex2 = 0; patternIndex2 < totalTilePatterns; patternIndex2++)
                    {
                        if (Agrees(modelData.patterns[patternIndex].array, modelData.patterns[patternIndex2].array, dx[dir], dy[dir], modelData.patternSize))
                        {
                            tempList.Add(patternIndex2);
                        }
                    }

                    modelData.propagator[dir][patternIndex] = tempList.ToArray();
                }
            }
        }

        protected void SetModel(GenericWaveCollapseModelData<TTile> modelData)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            ModelData = modelData;
            base.modelData = modelData;
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
            return WaveCollapseExtensions.Agrees(p1, p2, dx, dy, size);
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