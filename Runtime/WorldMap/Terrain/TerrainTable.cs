using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameframe.Procgen
{
  [CreateAssetMenu]
  public class TerrainTable : ScriptableObject
  {
    [SerializeField] private List<TerrainType> regions = new List<TerrainType>();
    public IReadOnlyList<TerrainType> Regions => regions;

    public Color GetColor(float value)
    {
      for (int i = 0; i < regions.Count; i++)
      {
        if (value <= regions[i].Threshold)
        {
          return regions[i].HighColor;
        }
      }

      return Color.clear;
    }

    private Color GetGradiatedColor(float value)
    {
      for (int i = 0; i < regions.Count; i++)
      {
        if (!(value <= regions[i].Threshold))
        {
          continue;
        }

        return GetGradiatedColor(regions[i], value);
      }

      return Color.clear;
    }

    public static Color GetGradiatedColor(TerrainType region, float value)
    {
      var intensity = Mathf.InverseLerp(region.Threshold, region.Floor, value);
      return Color.Lerp(region.HighColor, region.LowColor, intensity);
    }

    public TerrainType GetTerrainType(float value)
    {
      for (int i = 0; i < regions.Count; i++)
      {
        if (value <= regions[i].Threshold)
        {
          return regions[i];
        }
      }

      return null;
    }

    public Color[] GetColorMap(float[,] noiseMap, bool gradiate = false)
    {
      var width = noiseMap.GetLength(0);
      var height = noiseMap.GetLength(1);

      var colorMap = new Color[width * height];
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          var index = y * width + x;
          colorMap[index] = gradiate ? GetGradiatedColor(noiseMap[x, y]) : GetColor(noiseMap[x, y]);
        }
      }

      return colorMap;
    }

    public static Color[] GetColorMap(float[,] noiseMap, TerrainType[,] terrainMap, bool gradiate = false)
    {
      var width = noiseMap.GetLength(0);
      var height = noiseMap.GetLength(1);

      var colorMap = new Color[width * height];
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          var index = y * width + x;
          colorMap[index] = gradiate ? GetGradiatedColor(terrainMap[x, y], noiseMap[x, y]) : terrainMap[x, y].HighColor;
        }
      }

      return colorMap;
    }

    public static Color[] GetColorMap(float[] noiseMap, TerrainType[] terrainMap, bool gradiate = false)
    {
      var colorMap = new Color[noiseMap.Length];
      for (int i = 0; i < colorMap.Length; i++)
      {
        colorMap[i] = gradiate ? GetGradiatedColor(terrainMap[i], noiseMap[i]) : terrainMap[i].HighColor;
      }

      return colorMap;
    }

    public static Color[] GetColorMap(float[,] noiseMap, TerrainType[] terrainMap, bool gradiate = false)
    {
      var width = noiseMap.GetLength(0);
      var height = noiseMap.GetLength(1);

      var colorMap = new Color[width * height];
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          var index = y * width + x;
          colorMap[index] =
            gradiate ? GetGradiatedColor(terrainMap[index], noiseMap[x, y]) : terrainMap[index].HighColor;
        }
      }

      return colorMap;
    }

    public TerrainType[,] GetTerrainMap(float[,] noiseMap)
    {
      var width = noiseMap.GetLength(0);
      var height = noiseMap.GetLength(1);

      var valueMap = new TerrainType[width, height];
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          valueMap[x, y] = GetTerrainType(noiseMap[x, y]);
        }
      }

      return valueMap;
    }

    public TerrainType[] GetSingleDimensionTerrainMap(float[,] noiseMap)
    {
      var width = noiseMap.GetLength(0);
      var height = noiseMap.GetLength(1);

      var valueMap = new TerrainType[width * height];
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          valueMap[y * width + x] = GetTerrainType(noiseMap[x, y]);
        }
      }

      return valueMap;
    }

    public TerrainType[] GetTerrainMap(float[] heightMap)
    {
      var valueMap = new TerrainType[heightMap.Length];
      for (var i = 0; i < heightMap.Length; i++)
      {
        valueMap[i] = GetTerrainType(heightMap[i]);
      }

      return valueMap;
    }

    private void OnValidate()
    {
      var totalWeight = regions.Sum(x => x.Weight);

      //We can only do this if we have a total weight above zero
      if (totalWeight == 0)
      {
        return;
      }

      float sum = 0;
      foreach (var region in regions)
      {
        region.Weight = Mathf.Max(0, region.Weight);
        sum += region.Weight / (float) totalWeight;
        region.Threshold = sum;
      }

      for (int i = 0; i < regions.Count; i++)
      {
        if (i == 0)
        {
          regions[i].Floor = 0;
        }
        else
        {
          regions[i].Floor = regions[i - 1].Threshold;
        }
      }
    }

  }
}