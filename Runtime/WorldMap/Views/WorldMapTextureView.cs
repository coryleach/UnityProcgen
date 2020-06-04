using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldMapTextureView : MonoBehaviour, IWorldMapView
{
    [SerializeField] 
    private Renderer _renderer = null;

    [SerializeField] 
    private DrawMode _drawMode = DrawMode.Color;

    [SerializeField] 
    private TerrainTable _terrainTable = null;

    [SerializeField] 
    private bool scaleRenderer = false;

    [SerializeField] private FilterMode filterMode = FilterMode.Point;
    
    [SerializeField] private bool gradiate = false;

    [SerializeField] private bool fillRegions = false;
    [SerializeField] private bool drawBorders = false;
    [SerializeField] private bool drawSpawnPoints = false;
    [SerializeField] private bool drawPoissonPoints = false;
    
    [SerializeField] private Color[] regionColors = new Color[0];
    
    public enum DrawMode
    {
        Greyscale,
        Color
    }

    public bool applyRegions = false;
    public int regions = 50;
    public int seed = 100;
    public int minimumRegionSize = 50;
    public bool remapSmallRegions = false;
    
    [Range(0f,1f)]
    public float borderAlpha = 0.5f;
    
    [Range(0,1f)]
    public float regionFillAlpha = 0.2f;
    
    public void DisplayMap(WorldMapData mapData)
    {
        if (applyRegions)
        {
            DisplayMapWithRegions(mapData);
            return;
        }
        
        var heightMap = mapData.GetLayer<HeightMapLayerData>().heightMap;
        
        if (_renderer == null)
        {
            return;
        }

        Texture2D texture = null;
        if (_drawMode == DrawMode.Greyscale || _terrainTable == null)
        {
            texture = TextureUtility.GetHeightMap(heightMap,mapData.width,mapData.height);
        }
        else
        {
            var terrainMap = _terrainTable.GetTerrainMap(heightMap);
            var colorMap = TerrainTable.GetColorMap(heightMap,terrainMap,gradiate);
            texture = TextureUtility.GetColorMap(colorMap,mapData.width,mapData.height);
        }

        texture.filterMode = filterMode;
        SetTexture(texture);

        if (scaleRenderer)
        {
            _renderer.transform.localScale = new Vector3(mapData.width,mapData.height,1);
        }
    }

    public void DisplayMapWithRegions(WorldMapData worldMapData)
    {
        var heightMapLayer = worldMapData.GetLayer<HeightMapLayerData>();
        var regionMapLayer = worldMapData.GetLayer<RegionMapLayerData>();

        var width = worldMapData.width;
        var height = worldMapData.height;

        var heightMap = heightMapLayer.heightMap;
        var regions = regionMapLayer.regions;
        var regionMap = regionMapLayer.regionMap;
        
        var terrainMap = _terrainTable.GetTerrainMap(heightMap);
        var colorMap = TerrainTable.GetColorMap(heightMap, terrainMap, gradiate);

        if (fillRegions)
        {
            for (int i = 0; i < colorMap.Length; i++)
            {
                var regionIndex = regionMap[i];
                if (regionIndex <= 0)
                {
                    continue;
                }
                var regionColor = regionColors[regionIndex - 1];
                var alpha = Mathf.Clamp01(regionColor.a * regionFillAlpha);
                colorMap[i] =  regionColor * alpha + (1 - alpha) * colorMap[i];
            }
        }
        
        if (drawBorders)
        {
            foreach (var region in regions)
            {
                foreach (var pt in region.borderPoints)
                {
                    int index = pt.y * width + pt.x;
                    var regionIndex = regionMap[index];
                    var regionColor = regionColors[regionIndex - 1];
                    var alpha = regionColor.a * borderAlpha;
                    colorMap[index] =  regionColor * alpha + (1 - alpha) * colorMap[index];
                    colorMap[index].a = 1;
                }
            }
        }
        
        if (drawSpawnPoints)
        {
            foreach (var region in regions)
            {
                var pt = region.spawnPt;
                int index = pt.y * width + pt.x;
                colorMap[index] = Color.black;
            }
        }

        if (drawPoissonPoints)
        {
            var poissonLayer = worldMapData.GetLayer<PoissonMapLayerData>();
            if (poissonLayer != null)
            {
                foreach (var pt in poissonLayer.points)
                {
                    int index = pt.y * width + pt.x;
                    colorMap[index] = Color.black;
                }
            }
        }

        var texture = TextureUtility.GetColorMap(colorMap,width,height);
        SetTexture(texture);
        
        if (scaleRenderer)
        {
            _renderer.transform.localScale = new Vector3(width,height,1);
        }
    }
    
    public void DisplayMapWithRegions(float[,] noiseMap)
    {
        if (_renderer == null)
        {
            return;
        }
        
        var width = noiseMap.GetLength(0);
        var height = noiseMap.GetLength(1);

        //Generate Regions
        var voronoiData = Voronoi.Create(width, height, regions, seed);
        var regionColors = Voronoi.GenerateColors(voronoiData.regionCount);
        
        Texture2D texture = null;
        if (_drawMode == DrawMode.Greyscale || _terrainTable == null)
        {
            texture = TextureUtility.GetHeightMap(noiseMap);
        }
        else
        {
            var terrainMap = _terrainTable.GetSingleDimensionTerrainMap(noiseMap);
            var colorMap = TerrainTable.GetColorMap(noiseMap, terrainMap, gradiate);

            //We only want non-water regions
            var regionMask = new int[terrainMap.Length];
            for (var i = 0; i < terrainMap.Length; i++)
            {
                regionMask[i] = terrainMap[i].Elevation > 0 ? 1 : 0;
            }

            if (remapSmallRegions)
            {
                for (int i = 0; i < 20; i++)
                {
                    Debug.Log($"Combine Step {i}");
                    if (CombineSmallRegions(regionMask, ref voronoiData.regionData, minimumRegionSize, width, height))
                    {
                        break;
                    }
                } 
            }
            
            Debug.Log("Colorizing Map");
            for (int i = 0; i < colorMap.Length; i++)
            {
                if (regionMask[i] != 0)
                {
                    var regionIndex = voronoiData.regionData[i];
                    colorMap[i] *= regionColors[regionIndex];
                }
            }

            texture = TextureUtility.GetColorMap(colorMap,width,height);
        }

        texture.filterMode = filterMode;
        SetTexture(texture);

        if (scaleRenderer)
        {
            _renderer.transform.localScale = new Vector3(width,height,1);
        }
    }

    [SerializeField] private bool URP = false;
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

    private void SetTexture(Texture2D texture)
    {
        if (URP)
        {
            //_renderer.sharedMaterial.SetTexture("BaseMap",texture);
            _renderer.sharedMaterial.SetTexture(BaseMap,texture);
        }
        else
        {
            _renderer.sharedMaterial.mainTexture = texture;
        }
    }
    
    private static bool CombineSmallRegions(int[] regionMask, ref int[] regionData, int minSize, int width, int height)
    {
        //Get Region Sizes
        var regionSizes = Voronoi.GetRegionSizes(regionMask, regionData);
        var adjacencies = Voronoi.GetAdjacentRegions(regionMask, regionData, width, height);

        /*foreach (var pair in regionSizes)
        {
            Debug.Log($"*{pair.Key} = {pair.Value} < {minSize}");
        }*/
        
        var smallList = new List<int>();
        foreach (var pair in regionSizes)
        {
            if (pair.Value < minSize)
            {
                smallList.Add(pair.Key);
            }
        }

        if (smallList.Count == 0)
        {
            return true;
        }

        var msg = $"{smallList.Count} small regions";
        foreach (var idx in smallList)
        {
            msg += $" {idx},";
        }
        Debug.Log(msg);

        int count = 0;
        //Remap Small Regions to larger adjacent ones
        foreach (var smallRegionIndex in smallList)
        {
            if (!adjacencies.TryGetValue(smallRegionIndex, out var adjacentList))
            {
                //Debug.Log($"{smallRegionIndex} has no adjacent regions");
                continue;
            }
            //Pick a region to remap to
            var adjacentIndex = adjacentList[0];
            //Debug.Log($"Remapping {smallRegionIndex} to {adjacentButNotSmallIndex}");
            int replaceCout = 0;
            for (int i = 0; i < regionData.Length; i++)
            {
                if (regionData[i] == smallRegionIndex)
                {
                    regionData[i] = adjacentIndex;
                    replaceCout++;
                }
            }
            
            //Debug.Log($"Replaced {smallRegionIndex} ({replaceCout} pixels) with {adjacentIndex}");

            count++;
        }
        Debug.Log($"Remapped {count}");
        
        return false;
    }

    
}
