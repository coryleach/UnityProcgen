// Originally by Maxim Gumin
// Adapted from https://github.com/mxgmn/WaveFunctionCollapse
// Copyright (C) 2023 Cory Leach, The MIT License (MIT)

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    public class TextureModel : WaveCollapseModel
    {
        readonly List<byte[]> _patterns;
        readonly List<Color> _colors;

        public TextureModel(Texture2D sourceTexture, int N, int width, int height, bool periodicInput, bool periodic, int symmetry, bool ground, Heuristic heuristic) : base(width, height, N, periodic, heuristic)
        {
            var bitmap = sourceTexture.GetPixels();
            var sourceWidth = sourceTexture.width;
            var sourceHeight = sourceTexture.height;

            var sample = new byte[bitmap.Length];
            _colors = new List<Color>();
            for (var i = 0; i < sample.Length; i++)
            {
                var color = bitmap[i];
                var k = 0;
                for (; k < _colors.Count; k++)
                {
                    if (_colors[k] == color)
                    {
                        break;
                    }
                }

                if (k == _colors.Count)
                {
                    _colors.Add(color);
                }

                sample[i] = (byte) k;
            }

            _patterns = new();
            Dictionary<long, int> patternIndices = new();
            List<double> weightList = new();

            var colorsCount = _colors.Count;
            var xMax = periodicInput ? sourceWidth : sourceWidth - N + 1;
            var yMax = periodicInput ? sourceHeight : sourceHeight - N + 1;

            for (var y = 0; y < yMax; y++)
            for (var x = 0; x < xMax; x++)
            {
                byte[][] ps = new byte[8][];

                ps[0] = Pattern((dx, dy) => sample[(x + dx) % sourceWidth + (y + dy) % sourceHeight * sourceWidth], N);
                ps[1] = Reflect(ps[0], N);
                ps[2] = Rotate(ps[0], N);
                ps[3] = Reflect(ps[2], N);
                ps[4] = Rotate(ps[2], N);
                ps[5] = Reflect(ps[4], N);
                ps[6] = Rotate(ps[4], N);
                ps[7] = Reflect(ps[6], N);

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
                        _patterns.Add(p);
                    }
                }
            }

            weights = weightList.ToArray();
            T = weights.Length;
            this.ground = ground;

            propagator = new int[4][][];
            for (int d = 0; d < 4; d++)
            {
                propagator[d] = new int[T][];
                for (var t = 0; t < T; t++)
                {
                    List<int> list = new();
                    for (var t2 = 0; t2 < T; t2++)
                    {
                        if (Agrees(_patterns[t], _patterns[t2], dx[d], dy[d], N))
                        {
                            list.Add(t2);
                        }
                    }

                    propagator[d][t] = new int[list.Count];
                    for (var c = 0; c < list.Count; c++)
                    {
                        propagator[d][t][c] = list[c];
                    }
                }
            }
        }

        private static byte[] Reflect(byte[] p, int N) => Pattern((x, y) => p[N - 1 - x + y * N], N);

        private static byte[] Rotate(byte[] p, int N) => Pattern((x, y) => p[N - 1 - y + x * N], N);

        private static long Hash(byte[] p, int C)
        {
            long result = 0, power = 1;
            for (var i = 0; i < p.Length; i++)
            {
                result += p[p.Length - 1 - i] * power;
                power *= C;
            }

            return result;
        }

        private static bool Agrees(byte[] p1, byte[] p2, int dx, int dy, int N)
        {
            int xMin = dx < 0 ? 0 : dx, xMax = dx < 0 ? dx + N : N, yMin = dy < 0 ? 0 : dy, yMax = dy < 0 ? dy + N : N;
            for (var y = yMin; y < yMax; y++)
            for (var x = xMin; x < xMax; x++)
            {
                if (p1[x + N * y] != p2[x - dx + N * (y - dy)])
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] Pattern(Func<int, int, byte> f, int N)
        {
            var result = new byte[N * N];
            for (var y = 0; y < N; y++)
            for (var x = 0; x < N; x++)
            {
                result[x + y * N] = f(x, y);
            }
            return result;
        }
    }
}
