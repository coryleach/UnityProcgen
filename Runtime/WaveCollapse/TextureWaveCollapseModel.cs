// Originally by Maxim Gumin
// Adapted from https://github.com/mxgmn/WaveFunctionCollapse
// Copyright (C) 2023 Cory Leach, The MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameframe.Procgen
{

    public class TextureWaveCollapseModel : GenericWaveCollapseModel<Color>
    {
        private readonly ColorWaveCollapseModelData colorModelData;
        public ColorWaveCollapseModelData ColorModelData => colorModelData;

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
        public TextureWaveCollapseModel(Texture2D sourceTexture, int adjacentPixelDistance, int outputWidth, int outputHeight, bool periodicInput, bool periodic, int symmetry, Heuristic heuristic) : base(outputWidth, outputHeight, periodic, heuristic)
        {
            colorModelData = new ColorWaveCollapseModelData();
            BuildModel(colorModelData, sourceTexture.GetPixels(), sourceTexture.width, sourceTexture.height, adjacentPixelDistance, symmetry, periodicInput);
        }

        public TextureWaveCollapseModel(ColorWaveCollapseModelData modelData, int outputWidth, int outputHeight, bool periodic, Heuristic heuristic) : base(outputWidth, outputHeight, periodic, heuristic)
        {
            SetModel(modelData);
        }

        public IEnumerable<Texture2D> GetPatternsAsTextures()
        {
            foreach (var pattern in modelData.patterns)
            {
                //Get Colors
                var colors = pattern.array.Select(x => colorModelData.values[x]).ToArray();
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
                        bitmap[x + y * outputWidth] = colorModelData.values[something];
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
                                var argb = colorModelData.values[modelData.patterns[t][dx + dy * modelData.patternSize]];
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

    }
}
