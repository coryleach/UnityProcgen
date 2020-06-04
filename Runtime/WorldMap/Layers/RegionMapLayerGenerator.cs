using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
  [CreateAssetMenu]
  public class RegionMapLayerGenerator : ScriptableObject
  {
    public int regionCount = 5;
    public float waterLevel = 0.2380952f;

    public void AddToWorld(WorldMapData mapData)
    {
      var rng = new System.Random(mapData.seed);
      var regionLayer = new RegionMapLayerData();

      var heightMapLayer = mapData.GetLayer<HeightMapLayerData>();
      if (heightMapLayer == null)
      {
        throw new Exception("Cannot generate region layer without a height map");
      }

      var heightMap = heightMapLayer.heightMap;

      //Generate stuff
      var regionMap = new int[mapData.width * mapData.height];

      int emptyTileCount = 0;
      //Initialize map such that negative values are 'water'
      for (int i = 0; i < regionMap.Length; i++)
      {
        if (heightMap[i] > waterLevel)
        {
          emptyTileCount++;
          regionMap[i] = 0;
        }
        else
        {
          regionMap[i] = -1;
        }
      }

      List<RegionData> regions = new List<RegionData>();

      for (int i = 0; i < 100 && regions.Count < regionCount; i++)
      {
        //Pick a random point
        Vector2Int pt = new Vector2Int(rng.Next(0, mapData.width), rng.Next(0, mapData.height));
        int index = pt.y * mapData.width + pt.x;

        //If point is not empty try again
        if (regionMap[index] != 0)
        {
          continue;
        }

        //Regions are assigned id starting from 1
        var region = new RegionData
        {
          id = regions.Count + 1,
          spawnPt = pt
        };

        AddPointToRegion(region, regionMap, mapData.width, mapData.height, pt);
        emptyTileCount -= 1;

        regions.Add(region);
      }

      if (regions.Count != regionCount)
      {
        throw new Exception("Failed to create enough regions");
      }

      var neighbors = new List<Vector2Int>();

      int maxIterations = mapData.width * mapData.height;
      int count = 0;
      int emptyCount = 0;
      //Grow each region until there are no more empty tiles remaining
      while (emptyTileCount > 0 && emptyCount != regions.Count && count < maxIterations)
      {
        emptyCount = 0;
        foreach (var region in regions)
        {
          if (region.borderPoints.Count == 0)
          {
            //Count the number of regions that can no longer grow
            //Then we can early out if all regions are maxed out
            emptyCount++;
            continue;
          }

          if (TryExpandRegion(region, regionMap, mapData.width, mapData.height, rng, neighbors))
          {
            emptyTileCount--;
          }
        }

        count++;
      }


      count = 0;
      while (emptyTileCount != 0 && count < maxIterations)
      {
        count++;

        //We may have islands so lets find one and expand the closest region into it
        int index = FindUnassignedRegion(regionMap);
        int y = index / mapData.width;
        int x = index - (y * mapData.width);
        var pt = new Vector2Int(x, y);

        //Find the region with spawn point closest to pt
        var closestRegion = FindClosestRegion(regions, pt);
        AddPointToRegion(closestRegion, regionMap, mapData.width, mapData.height, pt);
        emptyTileCount--;

        //Expand region until it has no border points left
        while (closestRegion.borderPoints.Count > 0)
        {
          if (TryExpandRegion(closestRegion, regionMap, mapData.width, mapData.height, rng, neighbors))
          {
            emptyTileCount--;
          }
        }
      }

      if (emptyTileCount != 0)
      {
        Debug.LogError($"Failed to fill all tiles {emptyTileCount} (MaxIterations? {count >= maxIterations})");
      }

      //Validate
      for (int i = 0; i < regionMap.Length; i++)
      {
        if (regionMap[i] == 0)
        {
          Debug.LogError("Found a zero. TODO: Handle Islands");
          break;
        }
      }

      //Clear Border Points
      foreach (var region in regions)
      {
        region.borderPoints.Clear();
      }

      //Find actual border points
      for (int y = 0; y < mapData.height; y++)
      {
        for (int x = 0; x < mapData.width; x++)
        {
          var index = y * mapData.width + x;

          if (regionMap[index] <= 0)
          {
            continue;
          }

          var region = regions[regionMap[index] - 1];
          var pt = new Vector2Int(x, y);
          if (IsBorder(regionMap, mapData.width, mapData.height, pt))
          {
            region.borderPoints.Add(pt);
          }
        }
      }

      regionLayer.regionMap = regionMap;
      regionLayer.regions = regions;

      mapData.layers.Add(regionLayer);
    }


    private static bool TryExpandRegion(RegionData region, int[] regionMap, int width, int height, System.Random rng,
      List<Vector2Int> neighbors)
    {
      var borderPt = region.borderPoints[rng.Next(0, region.borderPoints.Count)];
      GetEmptyNeighbors(regionMap, width, height, borderPt, neighbors);

      if (neighbors.Count == 0)
      {
        region.borderPoints.Remove(borderPt);
        return false;
      }

      var neighbor = neighbors[rng.Next(0, neighbors.Count)];
      AddPointToRegion(region, regionMap, width, height, neighbor);

      if (neighbors.Count == 1)
      {
        region.borderPoints.Remove(borderPt);
      }

      return true;
    }

    public static int FindUnassignedRegion(int[] regionMap)
    {
      for (int i = 0; i < regionMap.Length; i++)
      {
        if (regionMap[i] == 0)
        {
          return i;
        }
      }

      return -1;
    }

    public static RegionData FindClosestRegion(List<RegionData> regions, Vector2Int pt)
    {
      float minDistance = float.MaxValue;
      RegionData closestRegion = null;

      foreach (var region in regions)
      {
        var dist = Vector2.Distance(region.spawnPt, pt);
        if (dist < minDistance)
        {
          minDistance = dist;
          closestRegion = region;
        }
      }

      return closestRegion;
    }

    public static void AddPointToRegion(RegionData region, int[] regionMap, int width, int height, Vector2Int pt)
    {
      regionMap[(pt.y * width) + pt.x] = region.id;
      region.size += 1;
      region.borderPoints.Add(pt);
    }

    public static void GetEmptyNeighbors(int[] regionMap, int width, int height, Vector2Int pt,
      List<Vector2Int> neighbors)
    {
      var up = new Vector2Int(pt.x, pt.y + 1);
      var down = new Vector2Int(pt.x, pt.y - 1);
      var left = new Vector2Int(pt.x - 1, pt.y);
      var right = new Vector2Int(pt.x + 1, pt.y);

      //Make sure the list is empty before we start adding things
      neighbors.Clear();

      if (up.y < height)
      {
        //Check Up
        if (regionMap[(up.y * width) + up.x] == 0)
        {
          neighbors.Add(up);
        }
      }

      if (down.y >= 0)
      {
        //Check Down
        if (regionMap[down.y * width + down.x] == 0)
        {
          neighbors.Add(down);
        }
      }

      if (left.x >= 0)
      {
        //Check Left
        if (regionMap[left.y * width + left.x] == 0)
        {
          neighbors.Add(left);
        }
      }

      if (right.x < width)
      {
        //Check Right
        if (regionMap[right.y * width + right.x] == 0)
        {
          neighbors.Add(right);
        }
      }
    }

    public static bool IsBorder(int[] regionMap, int width, int height, Vector2Int pt)
    {
      var up = new Vector2Int(pt.x, pt.y + 1);
      var down = new Vector2Int(pt.x, pt.y - 1);
      var left = new Vector2Int(pt.x - 1, pt.y);
      var right = new Vector2Int(pt.x + 1, pt.y);

      var id = regionMap[pt.y * width + pt.x];

      if (up.y < height)
      {
        //Check Up
        if (regionMap[(up.y * width) + up.x] != id)
        {
          return true;
        }
      }

      if (down.y >= 0)
      {
        //Check Down
        if (regionMap[down.y * width + down.x] != id)
        {
          return true;
        }
      }

      if (left.x >= 0)
      {
        //Check Left
        if (regionMap[left.y * width + left.x] != id)
        {
          return true;
        }
      }

      if (right.x < width)
      {
        //Check Right
        if (regionMap[right.y * width + right.x] != id)
        {
          return true;
        }
      }

      return false;
    }


  }
}