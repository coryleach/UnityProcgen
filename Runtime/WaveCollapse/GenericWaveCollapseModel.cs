using System;
using System.Collections.Generic;

namespace Gameframe.Procgen
{
    public class GenericWaveCollapseModelData<TTiles> : WaveCollapseModelData
    {
        public List<TTiles> values;
    }

    public abstract class GenericWaveCollapseModel<TTile> : WaveCollapseModel
    {
        public GenericWaveCollapseModelData<TTile> ModelData { get; private set; }

        protected GenericWaveCollapseModel(int width, int height, bool periodic, Heuristic heuristic) : base(width, height, periodic, heuristic)
        {

        }

        protected void BuildModel(GenericWaveCollapseModelData<TTile> model, TTile[] tilemap, int width, int height, int adjacentDistance, int symmetry, bool periodicInput)
        {
            var sourceWidth = width;
            var sourceHeight = height;

            // ReSharper disable once VirtualMemberCallInConstructor
            ModelData = model;
            modelData = model;

            ModelData.values = new List<TTile>();
            ModelData.patterns = new List<WaveCollapseModelData.PatternList>();
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
