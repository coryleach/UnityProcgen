using System;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiskSampling
{
  public static List<Vector2> GeneratePoints(float radius, Vector2 size, int seed, int maxSamplesPerPoint = 30, Func<Vector2,bool> validate = null)
  {
    var rng = new System.Random(seed);
    var points = new List<Vector2>();
    var cellSize = radius / Mathf.Sqrt(2);
    var gridWidth = Mathf.CeilToInt(size.x / cellSize);
    var gridHeight = Mathf.CeilToInt(size.y / cellSize);
    //Grid values equal to zero mean there is no point in that grid cell
    var grid = new int[gridWidth,gridHeight];
    var spawnPoints = new List<Vector2>();
    
    //Pick a random point to start spawning from
    spawnPoints.Add(size/2);
    while (spawnPoints.Count > 0)
    {
      //Get a random spawn point from the list
      var spawnIndex = rng.Next(0, spawnPoints.Count);
      var spawnCenter = spawnPoints[spawnIndex];
      var accepted = false;
      
      for (int i = 0; i < maxSamplesPerPoint; i++)
      {
        //Get a random direction vector
        var angle = (float)(rng.NextDouble() * Mathf.PI * 2);
        var dir = new Vector2(Mathf.Sin(angle),Mathf.Cos(angle));
        //Get point along that direction vector between radius and 2radius distance away
        var distance = (float)(radius + rng.NextDouble() * radius * 2);
        var candidate = spawnCenter + dir * distance;

        //If point is valid we can accept it stop sampling
        if (IsValid(candidate, size, cellSize, radius, points, grid))
        {
          if (validate == null || validate.Invoke(candidate))
          {
            accepted = true;
            points.Add(candidate);
            spawnPoints.Add(candidate);
            grid[(int) (candidate.x / cellSize), (int) (candidate.y / cellSize)] = points.Count;
            break;
          }
        }
      }

      if (!accepted)
      {
        spawnPoints.RemoveAt(spawnIndex);
      }
    }
    
    return points;
  }

  public static List<Vector2Int> GenerateIntPoints(float radius, Vector2Int size, int seed, int maxSamplesPerPoint = 30, Func<Vector2Int,bool> validate = null)
  {
    var rng = new System.Random(seed);
    var points = new List<Vector2Int>();
    var cellSize = radius / Mathf.Sqrt(2);
    var gridWidth = Mathf.CeilToInt(size.x / cellSize);
    var gridHeight = Mathf.CeilToInt(size.y / cellSize);
    //Grid values equal to zero mean there is no point in that grid cell
    var grid = new int[gridWidth,gridHeight];
    var spawnPoints = new List<Vector2Int>();
    
    //Pick a random point to start spawning from
    spawnPoints.Add(new Vector2Int(size.x/2, size.y/2));
    while (spawnPoints.Count > 0)
    {
      //Get a random spawn point from the list
      var spawnIndex = rng.Next(0, spawnPoints.Count);
      var spawnCenter = spawnPoints[spawnIndex];
      var accepted = false;
      
      for (int i = 0; i < maxSamplesPerPoint; i++)
      {
        //Get a random direction vector
        var angle = (float)(rng.NextDouble() * Mathf.PI * 2);
        var dir = new Vector2(Mathf.Sin(angle),Mathf.Cos(angle));
        //Get point along that direction vector between radius and 2radius distance away
        var distance = (float)(radius + rng.NextDouble() * radius * 2);
        var pt = spawnCenter + dir * distance;
        var candidate = new Vector2Int((int)pt.x,(int)pt.y);

        //If point is valid we can accept it stop sampling
        if (IsValid(candidate, size, cellSize, radius, points, grid))
        {
          if (validate == null || validate.Invoke(candidate))
          {
            accepted = true;
            points.Add(candidate);
            spawnPoints.Add(candidate);
            grid[(int) (candidate.x / cellSize), (int) (candidate.y / cellSize)] = points.Count;
            break;
          }
        }
      }

      if (!accepted)
      {
        spawnPoints.RemoveAt(spawnIndex);
      }
    }
    
    return points;
  }
  
  private static bool IsValid(Vector2 pt, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
  {
    if (!(pt.x >= 0) || !(pt.x < sampleRegionSize.x) || !(pt.y >= 0) || !(pt.y < sampleRegionSize.y))
    {
      return false;
    }

    var cellX = (int) (pt.x / cellSize);
    var cellY = (int) (pt.y / cellSize);

    var gridWidth = grid.GetLength(0);
    var gridHeight = grid.GetLength(1);
    
    var searchStartX = Mathf.Max(0, cellX - 2);
    var searchEndX = Mathf.Min(cellX + 2, gridWidth - 1);
    
    var searchStartY = Mathf.Max(0, cellY - 2);
    var searchEndY = Mathf.Min(cellY + 2, gridHeight - 1);

    var sqrRadius = radius * radius;
    
    for (int x = searchStartX; x <= searchEndX; x++)
    {
      for (int y = searchStartY; y <= searchEndY; y++)
      {
        //Index was stored with +1 so we could avoid initializing the grid to all -1 values
        int pointIndex = grid[x, y] - 1;
        if (pointIndex != -1)
        {
          var sqrDistance = (pt - points[pointIndex]).sqrMagnitude;
          if (sqrDistance < sqrRadius)
          {
            return false;
          }
        }
      }
    }
    
    return true;
  }
  
  private static bool IsValid(Vector2 pt, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2Int> points, int[,] grid)
  {
    if (!(pt.x >= 0) || !(pt.x < sampleRegionSize.x) || !(pt.y >= 0) || !(pt.y < sampleRegionSize.y))
    {
      return false;
    }

    var cellX = (int) (pt.x / cellSize);
    var cellY = (int) (pt.y / cellSize);

    var gridWidth = grid.GetLength(0);
    var gridHeight = grid.GetLength(1);
    
    var searchStartX = Mathf.Max(0, cellX - 2);
    var searchEndX = Mathf.Min(cellX + 2, gridWidth - 1);
    
    var searchStartY = Mathf.Max(0, cellY - 2);
    var searchEndY = Mathf.Min(cellY + 2, gridHeight - 1);

    var sqrRadius = radius * radius;
    
    for (int x = searchStartX; x <= searchEndX; x++)
    {
      for (int y = searchStartY; y <= searchEndY; y++)
      {
        //Index was stored with +1 so we could avoid initializing the grid to all -1 values
        int pointIndex = grid[x, y] - 1;
        if (pointIndex != -1)
        {
          var sqrDistance = (pt - points[pointIndex]).sqrMagnitude;
          if (sqrDistance < sqrRadius)
          {
            return false;
          }
        }
      }
    }
    
    return true;
  }

}
