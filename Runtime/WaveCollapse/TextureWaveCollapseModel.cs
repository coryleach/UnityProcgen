// Originally by Maxim Gumin
// Adapted from https://github.com/mxgmn/WaveFunctionCollapse
// Copyright (C) 2023 Cory Leach, The MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gameframe.Procgen
{
    public static class PixelUtility
    {
        public static void FlipVertical(this Color[] pixels, int width, int height)
        {
            for (int i = 0; i < (pixels.Length/2); i++)
            {
                var x = i % width;
                var y = i / width;
                var y2 = height - 1 - y;
                var i2 = x + y2 * width;
                (pixels[i2], pixels[i]) = (pixels[i], pixels[i2]);
            }
        }
    }

    [Serializable]
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
        public TextureWaveCollapseModel(Texture2D sourceTexture, int adjacentPixelDistance, bool periodicInput, int symmetry)
        {
            colorModelData = new ColorWaveCollapseModelData();

            //Change to bitmap format
            var pixels = sourceTexture.GetPixels();
            //pixels.FlipVertical(sourceTexture.width, sourceTexture.height);
            BuildModel(colorModelData, pixels, sourceTexture.width, sourceTexture.height, adjacentPixelDistance, symmetry, periodicInput);
        }

        // public TextureWaveCollapseModel(ColorWaveCollapseModelData modelData, int outputWidth, int outputHeight, bool ground, bool periodic, Heuristic heuristic)
        // {
        //     SetModel(modelData);
        // }

        public IEnumerable<Texture2D> GetPatternsAsTextures()
        {
            for (int patternIndex = 0; patternIndex < modelData.patterns.Count; patternIndex++)
            {
                yield return PatternToTexture(patternIndex);
            }
        }

        public Color[] PatternToPixels(int index)
        {
            return modelData.patterns[index].array.Select(x => colorModelData.values[x]).ToArray();
        }

        public Texture2D PatternToTexture(int index)
        {
            var colors = PatternToPixels(index);
            var tex = new Texture2D(modelData.patternSize, modelData.patternSize)
            {
                filterMode = FilterMode.Point
            };
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        public Texture2D GetOrCreatePatternTexture(Dictionary<int, Texture2D> dict, int patternIndex)
        {
            if (dict.TryGetValue(patternIndex, out var tex))
            {
                return tex;
            }
            tex = PatternToTexture(patternIndex);
            dict.Add(patternIndex, tex);
            return tex;
        }

        [Serializable]
        public class TextureWaveCollapseTextureData
        {
            [Serializable]
            public class PatternData
            {
                public int index;
                public Texture2D texture;
                public List<MatchingPatternData> matchingPatterns = new List<MatchingPatternData>();
            }

            [Serializable]
            public class MatchingPatternData
            {
                public Vector2Int direction;
                public List<MatchedPatternEntry> entries = new List<MatchedPatternEntry>();
            }

            [Serializable]
            public class MatchedPatternEntry
            {
                public int index;
                public Texture2D texture;
            }

            public List<PatternData> patterns = new List<PatternData>();
        }

        public TextureWaveCollapseTextureData GetPropagatorData()
        {
            var textureData = new TextureWaveCollapseTextureData();
            var textureDict = new Dictionary<int, Texture2D>();

            for (var patternIndex = 0; patternIndex < modelData.patterns.Count; patternIndex++)
            {
                var patternData = new TextureWaveCollapseTextureData.PatternData
                {
                    index = patternIndex,
                    texture = GetOrCreatePatternTexture(textureDict, patternIndex)
                };

                textureData.patterns.Add(patternData);

                for (var directionIndex = 0; directionIndex < 4; directionIndex++)
                {
                    var matchingPatterns = modelData.propagator[directionIndex].tiles[patternIndex];
                    var directionData = new TextureWaveCollapseTextureData.MatchingPatternData
                    {
                        direction = modelData.propagator[directionIndex].Dir
                    };

                    foreach ( var matchingPatternIndex in matchingPatterns.patterns)
                    {
                        var entry = new TextureWaveCollapseTextureData.MatchedPatternEntry();
                        entry.index = matchingPatternIndex;
                        entry.texture = GetOrCreatePatternTexture(textureDict, matchingPatternIndex);
                        directionData.entries.Add(entry);
                    }

                    patternData.matchingPatterns.Add(directionData);
                }
            }

            return textureData;
        }

        public void GetCollapseData()
        {
            //this._compatibilityMap[]
        }

        [Serializable]
        public class TextureWaveEntry
        {
            public List<Texture2D> patterns = new List<Texture2D>();
        }

        public TextureWaveEntry[] GetWave()
        {
            var dict = new Dictionary<int, Texture2D>();
            var waveList = new TextureWaveEntry[wave.Length];
            for (var i = 0; i < wave.Length; i++)
            {
                waveList[i] = new TextureWaveEntry();

                for (int j = 0; j < wave[i].Length; j++)
                {
                    if (wave[i][j])
                    {
                        if (!dict.TryGetValue(j, out var tex))
                        {
                            tex = PatternToTexture(j);
                            dict[j] = tex;
                        }
                        waveList[i].patterns.Add(tex);
                    }
                }
            }
            return waveList;
        }

        public void Save(Texture2D texture2D)
        {
            var bitmap = new Color[outputWidth * outputHeight];
            Save(bitmap);
            //bitmap.FlipVertical(outputWidth, outputHeight);
            texture2D.SetPixels(bitmap);
            texture2D.Apply(false);
        }

        public void Save(Color[] bitmap)
        {
            if (bitmap.Length < (outputWidth * outputHeight))
            {
                throw new Exception("Array not correct size");
            }

            for (var y = 0; y < outputHeight; y++)
            {
                var patternY = Mathf.Clamp(modelData.patternSize - (outputHeight - y), 0, int.MaxValue); // y < (outputHeight - modelData.patternSize + 1) ? 0 : modelData.patternSize - 1;
                for (var x = 0; x < outputWidth; x++)
                {
                    var patternX = Mathf.Clamp(modelData.patternSize - (outputWidth - x), 0, int.MaxValue);
                    var patternIndex = observed[x - patternX + (y - patternY) * outputWidth];
                    if (patternIndex < 0 || patternIndex >= modelData.patterns.Count)
                    {
                        bitmap[x + y * outputWidth] = Color.black;
                    }
                    else
                    {
                        var colorIndex = modelData.patterns[patternIndex][patternX + patternY * modelData.patternSize];
                        bitmap[x + y * outputWidth] = colorModelData.values[colorIndex];
                    }
                }
            }

            // if (observed[0] >= 0)
            // {
            //     for (var y = 0; y < outputHeight; y++)
            //     {
            //         var dy = y < outputHeight - modelData.patternSize + 1 ? 0 : modelData.patternSize - 1;
            //         for (var x = 0; x < outputWidth; x++)
            //         {
            //             var dx = x < outputWidth - modelData.patternSize + 1 ? 0 : modelData.patternSize - 1;
            //             var obs = observed[x - dx + (y - dy) * outputWidth];
            //             if (obs < 0 || obs >= modelData.patterns.Count)
            //             {
            //                 bitmap[x + y * outputWidth] = Color.black;
            //             }
            //             else
            //             {
            //                 var something = modelData.patterns[obs][dx + dy * modelData.patternSize];
            //                 bitmap[x + y * outputWidth] = colorModelData.values[something];
            //             }
            //         }
            //     }
            // }
            // else
            // {
            //     for (var i = 0; i < wave.Length; i++)
            //     {
            //         int contributors = 0;
            //         float r = 0, g = 0, b = 0;
            //         int x = i % outputWidth, y = i / outputWidth;
            //         for (var dy = 0; dy < modelData.patternSize; dy++)
            //         for (int dx = 0; dx < modelData.patternSize; dx++)
            //         {
            //             var sx = x - dx;
            //             if (sx < 0)
            //             {
            //                 sx += outputWidth;
            //             }
            //
            //             var sy = y - dy;
            //             if (sy < 0)
            //             {
            //                 sy += outputHeight;
            //             }
            //
            //             int s = sx + sy * outputWidth;
            //             if (!_periodic && (sx + modelData.patternSize > outputWidth || sy + modelData.patternSize > outputHeight || sx < 0 || sy < 0))
            //             {
            //                 continue;
            //             }
            //
            //             for (int t = 0; t < totalTilePatterns; t++)
            //             {
            //                 if (wave[s][t])
            //                 {
            //                     contributors++;
            //                     var argb = colorModelData.values[modelData.patterns[t][dx + dy * modelData.patternSize]];
            //                     r += argb.r;
            //                     g += argb.g;
            //                     b += argb.b;
            //                 }
            //             }
            //         }
            //
            //         if (contributors == 0)
            //         {
            //             bitmap[i] = Color.red;
            //         }
            //         else
            //         {
            //             bitmap[i] = new Color(r / contributors, g / contributors, b / contributors);
            //         }
            //     }
            // }
        }

        public void Ground(int groundPatternIndex = 0, int groundY = 0)
        {
            CollapseNode(0 + groundY * outputWidth, groundPatternIndex);
            Propagate();
        }

    }
}
