using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameframe.WorldMapGen
{

    public static class Voronoi
    {
        public static VoronoiData Create(int width, int height, int regionCount, int seed)
        {
            var data = new VoronoiData
            {
                width = width,
                height = height,
                centroids = new Vector2Int[regionCount],
                regionCount = regionCount,
                regionData = new int[width * height]
            };

            var rng = new System.Random(seed);

            for (var i = 0; i < data.regionCount; i++)
            {
                data.centroids[i] = new Vector2Int(rng.Next(0, data.width), rng.Next(0, data.height));
            }

            //Generate Region Data for each 'pixel'
            for (var y = 0; y < data.height; y++)
            {
                for (var x = 0; x < data.width; x++)
                {
                    var index = y * data.width + x;
                    var pt = new Vector2Int(x, y);
                    data.regionData[index] = GetClosestCentroidIndex(pt, data.centroids);
                }
            }

            return data;
        }

        public static Dictionary<int, int> GetRegionSizes(int[] regionMask, int[] regionData)
        {
            Dictionary<int, int> regionSizes = new Dictionary<int, int>();

            for (int i = 0; i < regionData.Length; i++)
            {
                //Don't count size pixels excluded by the mask
                if (regionMask[i] == 0)
                {
                    continue;
                }

                int regionIndex = regionData[i];
                if (regionSizes.TryGetValue(regionIndex, out var size))
                {
                    size += 1;
                }
                else
                {
                    size = 1;
                }

                regionSizes[regionIndex] = size;
            }

            return regionSizes;
        }

        public static Dictionary<int, int> GetRegionSizes(int[] regionData)
        {
            Dictionary<int, int> regionSizes = new Dictionary<int, int>();
            for (int i = 0; i < regionData.Length; i++)
            {
                int regionIndex = regionData[i];
                if (regionSizes.TryGetValue(regionIndex, out var size))
                {
                    size += 1;
                }
                else
                {
                    size = 1;
                }

                regionSizes[regionIndex] = size;
            }

            return regionSizes;
        }

        public static Dictionary<int, List<int>> GetAdjacentRegions(int[] regionMask, int[] regionData, int width,
            int height)
        {
            var regionAdjacencies = new Dictionary<int, List<int>>();

            for (var i = 0; i < regionData.Length; i++)
            {
                if (regionMask[i] == 0)
                {
                    continue;
                }

                var regionIndex = regionData[i];
                var y = i / width;
                var x = i - width * y;

                //Check Up Region
                if (y + 1 < height)
                {
                    var upIndex = (y + 1) * width + x;
                    //if (regionMask[upIndex] != 0 && regionData[upIndex] != regionIndex)
                    if (regionData[upIndex] != regionIndex)
                    {
                        //Add Adjacency
                        AddAdjacency(regionAdjacencies, regionIndex, regionData[upIndex]);
                    }
                }

                //Check Down Region
                if (y > 0)
                {
                    var downIndex = (y - 1) * width + x;
                    //if (regionMask[downIndex] != 0 && regionData[downIndex] != regionIndex)
                    if (regionData[downIndex] != regionIndex)
                    {
                        //Add Adjacency
                        AddAdjacency(regionAdjacencies, regionIndex, regionData[downIndex]);
                    }
                }

                //Check Left Region
                if (x > 0)
                {
                    var leftIndex = y * width + x - 1;
                    //if (regionMask[leftIndex] != 0 && regionData[leftIndex] != regionIndex)
                    if (regionData[leftIndex] != regionIndex)
                    {
                        //Add Adjacency
                        AddAdjacency(regionAdjacencies, regionIndex, regionData[leftIndex]);
                    }
                }

                //Check Right Region
                if (x + 1 < width)
                {
                    var rightIndex = y * width + x + 1;
                    //if (regionMask[rightIndex] != 0 && regionData[rightIndex] != regionIndex)
                    if (regionData[rightIndex] != regionIndex)
                    {
                        //Add Adjacency
                        AddAdjacency(regionAdjacencies, regionIndex, regionData[rightIndex]);
                    }
                }
            }

            return regionAdjacencies;
        }

        private static void AddAdjacency(Dictionary<int, List<int>> dict, int regionIndex, int adjacentRegionIndex)
        {
            if (!dict.TryGetValue(regionIndex, out var list))
            {
                list = new List<int>();
                dict.Add(regionIndex, list);
            }

            if (!list.Contains(adjacentRegionIndex))
            {
                list.Add(adjacentRegionIndex);
            }
        }

        public static Color[] GenerateColors(int regions)
        {
            var regionColors = new Color[regions];
            for (var i = 0; i < regions; i++)
            {
                regionColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            }

            return regionColors;
        }

        public static Texture2D CreateTexture(VoronoiData data)
        {
            //Randomly select some colors
            var regionColors = new Color[data.regionCount];
            for (var i = 0; i < data.regionCount; i++)
            {
                regionColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            }

            var pixelColors = new Color[data.width * data.height];
            for (var y = 0; y < data.height; y++)
            {
                for (var x = 0; x < data.width; x++)
                {
                    var index = y * data.width + x;
                    pixelColors[index] = regionColors[data.regionData[index]];
                }
            }

            return CreateTextureFromColorArray(pixelColors, data.width, data.height);
        }

        public static Texture2D CreateFalloffTexture(VoronoiData data)
        {
            var distances = new float[data.width * data.height];
            for (var y = 0; y < data.height; y++)
            {
                for (var x = 0; x < data.width; x++)
                {
                    var pt = new Vector2Int(x, y);
                    var index = y * data.width + x;
                    var regionIndex = data.regionData[index];
                    distances[index] = Vector2.Distance(pt, data.centroids[regionIndex]);
                }
            }

            var maxDistance = distances.Max();
            var pixelColors = new Color[data.width * data.height];
            for (var i = 0; i < distances.Length; i++)
            {
                var val = Mathf.Clamp01(distances[i] / maxDistance);
                pixelColors[i] = new Color(val, val, val, 1f);
            }

            return CreateTextureFromColorArray(pixelColors, data.width, data.height);
        }

        public static Texture2D CreateBorderTexture(VoronoiData data)
        {
            var pixelColors = new Color[data.width * data.height];
            for (var y = 0; y < data.height; y++)
            {
                for (var x = 0; x < data.width; x++)
                {
                    var pixelIndex = y * data.width + x;

                    if (x - 1 < 0 || x + 1 >= data.width)
                    {
                        pixelColors[pixelIndex] = Color.white;
                    }
                    else if (y - 1 < 0 || y + 1 >= data.height)
                    {
                        pixelColors[pixelIndex] = Color.white;
                    }
                    else
                    {
                        var up = (y + 1) * data.width + x;
                        var down = (y - 1) * data.width + x;
                        var left = y * data.width + x - 1;
                        var right = y * data.width + x + 1;

                        var thisRegion = data.regionData[pixelIndex];
                        var upRegion = data.regionData[up];
                        var downRegion = data.regionData[down];
                        var leftRegion = data.regionData[left];
                        var rightRegion = data.regionData[right];

                        if ((thisRegion == upRegion)
                            && (thisRegion == downRegion)
                            && (thisRegion == leftRegion)
                            && (thisRegion == rightRegion))
                        {
                            pixelColors[pixelIndex] = Color.black;
                        }
                        else
                        {
                            pixelColors[pixelIndex] = Color.white;
                        }
                    }
                }
            }

            return CreateTextureFromColorArray(pixelColors, data.width, data.height);
        }

        private static int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
        {
            var smallestDist = float.MaxValue;
            var index = 0;

            for (var i = 0; i < centroids.Length; i++)
            {
                if (Vector2.Distance(pixelPos, centroids[i]) < smallestDist)
                {
                    smallestDist = Vector2.Distance(pixelPos, centroids[i]);
                    index = i;
                }
            }

            return index;
        }

        private static Texture2D CreateTextureFromColorArray(Color[] pixelColors, int width, int height)
        {
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(pixelColors);
            texture.Apply();
            return texture;
        }

    }

}