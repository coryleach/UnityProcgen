using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
  public enum HexDirection
  {
    NE = 0, 
    E = 1, 
    SE = 2, 
    SW = 3, 
    W = 4, 
    NW = 5
  }

  public class HexMeshData
  {
    private readonly float _outerRadius;
    private readonly float _innerRadius;
    private readonly List<Vector3> _vertices;
    private readonly List<int> _triangles;
    private readonly List<Color> _colors;
    private readonly List<Vector2> _uv;
    private readonly Vector3[] _corners;
    private readonly float _border = 0.25f;
    
    public float Border => _border;
    public float Solid => 1 - _border;

    public float OuterRadius => _outerRadius;

    public float InnerRadius => _innerRadius;

    public List<Vector3> Vertices => _vertices;

    public List<int> Triangles => _triangles;

    public List<Color> Colors => _colors;

    public List<Vector2> Uv => _uv;

    public HexMeshData()
    {
    }

    public HexMeshData(float radius, float border = 0.2f)
    {
      this._border = Mathf.Clamp01(border);
      
      _outerRadius = radius;
      _innerRadius = HexMeshUtility.GetInnerRadius(_outerRadius);
      _vertices = new List<Vector3>(); 
      _triangles = new List<int>();
      _colors = new List<Color>();
      _uv = new List<Vector2>();
      _corners = new [] {
        new Vector3(0f, 0f, _outerRadius),
        new Vector3(_innerRadius, 0f, 0.5f * _outerRadius),
        new Vector3(_innerRadius, 0f, -0.5f * _outerRadius),
        new Vector3(0f, 0f, -_outerRadius),
        new Vector3(-_innerRadius, 0f, -0.5f * _outerRadius),
        new Vector3(-_innerRadius, 0f, 0.5f * _outerRadius),
        new Vector3(0f, 0f, _outerRadius)
      };
    }
    
    public Vector3 GetBridge(HexDirection direction) 
    {
      return (_corners[(int)direction] + _corners[(int)direction + 1]) * 0.5f * _border;
    }
    
    public Vector3 GetFirstCorner(HexDirection direction)
    {
      return _corners[(int) direction];
    }

    public Vector3 GetSecondCorner(HexDirection direction)
    {
      return _corners[(int) direction + 1];
    }
    
    public Vector3 GetFirstSolidCorner(HexDirection direction)
    {
      return _corners[(int) direction] * Solid;
    }

    public Vector3 GetSecondSolidCorner(HexDirection direction)
    {
      return _corners[(int) direction + 1] * Solid;
    }

    public Mesh CreateMesh()
    {
      var mesh = new Mesh
      {
        vertices = _vertices.ToArray(), 
        triangles = _triangles.ToArray(), 
        colors = _colors.ToArray(), 
        uv = _uv.ToArray()
      };
      mesh.RecalculateNormals();
      return mesh;
    }
    
  }
  
  public static class HexMeshUtility
  {
    //Calculates the inner radius from the outer radius
    public static float GetInnerRadius(float outerRadius)
    {
      return outerRadius * Mathf.Sqrt(3f) * 0.5f;
    }
    
    public static HexDirection Previous(this HexDirection direction) 
    {
      return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction) 
    {
      return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
    
    public static int GetNeighbor(int index, HexDirection hexDirection, int mapWidth, int mapHeight)
    {
      int y = index / mapWidth;
      int x = index - (y * mapWidth);
      
      switch (hexDirection)
      {
        case HexDirection.NE:
          //Only odd numbered rows shift a column 
          if ((y & 1) == 1)
          {
            x++;
          }
          y++;
          break;
        case HexDirection.E:
          x++;
          break;
        case HexDirection.SE:
          //Only odd numbered rows shift a column 
          if ((y & 1) == 1)
          {
            x++;
          }
          y--;
          break;
        case HexDirection.SW:
          //Only even numbered rows shift a column 
          if ((y & 1) == 0)
          {
            x--;
          }
          y--;
          break;
        case HexDirection.W:
          x--;
          break;
        case HexDirection.NW:
          //Only even numbered rows shift a column 
          if ((y & 1) == 0)
          {
            x--;
          }
          y++;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(hexDirection), hexDirection, null);
      }

      if (x < 0 || x >= mapWidth)
      {
        return -1;
      }

      if (y < 0 || y >= mapHeight)
      {
        return -1;
      }

      return y * mapWidth + x;
    }
    
    public static Mesh GenerateHexagonMesh(float radius, float border, int startX, int startY, int chunkWidth, int chunkHeight, int mapWidth, int mapHeight, float[] heightMap, Func<float,Color> colorFunction, Func<float,float> elevationFunction)
    {
      var meshData = new HexMeshData(radius, border);

      for (int dy = 0; dy < chunkHeight && (startY + dy) < mapHeight; dy++)
      {
        for (int dx = 0; dx < chunkWidth && (startX + dx) < mapWidth; dx++)
        {
          int x = startX + dx;
          int y = startY + dy;
          
          var index = (y * mapWidth) + x;
          
          var xOffset = x + y * 0.5f - (int)(y / 2);
          var center = new Vector3(xOffset*meshData.InnerRadius*2,0,y*meshData.OuterRadius*1.5f);
          
          for (var direction = 0; direction < 6; direction++)
          {
            var elevation = elevationFunction?.Invoke(heightMap[index]) ?? 0;
            var previousNeighborElevation = elevation;
            var neighborElevation = elevation;
            var nextNeighborElevation = elevation;
            
            var neighbor = GetNeighbor(index, (HexDirection)direction, mapWidth, mapHeight);
            if (neighbor != -1)
            {
              neighborElevation = Mathf.Min(elevation,elevationFunction?.Invoke(heightMap[neighbor]) ?? 0);
            }
            
            neighbor = GetNeighbor(index, ((HexDirection)direction).Previous(), mapWidth, mapHeight);
            if (neighbor != -1)
            {
              previousNeighborElevation = Mathf.Min(elevation,elevationFunction?.Invoke(heightMap[neighbor]) ?? 0);
            }
            
            neighbor = GetNeighbor(index, ((HexDirection)direction).Next(), mapWidth, mapHeight);
            if (neighbor != -1)
            {
              nextNeighborElevation = Mathf.Min(elevation,elevationFunction?.Invoke(heightMap[neighbor]) ?? 0);
            }

            center.y = elevation;
            
            var color = colorFunction?.Invoke(heightMap[index]) ?? Color.white;
            var uv = new Vector2(x / (float)mapWidth, y / (float)mapHeight);
            AddTriangle(meshData, center, uv, neighborElevation, previousNeighborElevation, nextNeighborElevation, (HexDirection)direction, color);
          }

        }
      }

      return meshData.CreateMesh();
    }

    private static void AddTriangle(HexMeshData meshData, Vector3 center, Vector3 uv, float neighborElevation, float previousElevation, float nextElevation, HexDirection direction, Color color)
    {
      var v1 = center;
      var v2 = center + meshData.GetFirstSolidCorner(direction);
      var v3 = center + meshData.GetSecondSolidCorner(direction);
      
      //Add inner solid triangle
      AddTriangle(meshData, v1, v2, v3, color, uv);

      //Add Quad To Fill Border Gap
      var v4 = v2 + meshData.GetBridge(direction);
      v4.y = neighborElevation;
      var v5 = v3 + meshData.GetBridge(direction);
      v5.y = neighborElevation;
      AddQuad(meshData, v2, v3, v4, v5, color, uv);
      
      //Add Triangles to fill gap on sides of quad
      var v6 = center + meshData.GetFirstCorner(direction);
      v6.y = Mathf.Min(neighborElevation,previousElevation);
      AddTriangle(meshData,v2, v6, v4, color, uv);
      
      var v7 = center + meshData.GetSecondCorner(direction);
      v7.y = Mathf.Min(neighborElevation,nextElevation);
      AddTriangle(meshData, v3, v5, v7, color, uv);
    }

    private static void AddTriangle(HexMeshData meshData, Vector3 v1, Vector3 v2, Vector3 v3, Color color, Vector3 uv)
    {
      var vertexIndex = meshData.Vertices.Count;
      
      meshData.Vertices.Add(v1);
      meshData.Vertices.Add(v2);
      meshData.Vertices.Add(v3);
      
      meshData.Colors.Add(color);
      meshData.Colors.Add(color);
      meshData.Colors.Add(color);
      
      meshData.Uv.Add(uv);
      meshData.Uv.Add(uv);
      meshData.Uv.Add(uv);

      meshData.Triangles.Add(vertexIndex);
      meshData.Triangles.Add(vertexIndex + 1);
      meshData.Triangles.Add(vertexIndex + 2);
    }
    
    private static void AddQuad(HexMeshData meshData, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color, Vector3 uv)
    {
      var vertexIndex = meshData.Vertices.Count;
      
      meshData.Vertices.Add(v1);
      meshData.Vertices.Add(v2);
      meshData.Vertices.Add(v3);
      meshData.Vertices.Add(v4);
      
      meshData.Colors.Add(color);
      meshData.Colors.Add(color);
      meshData.Colors.Add(color);
      meshData.Colors.Add(color);

      meshData.Uv.Add(uv);
      meshData.Uv.Add(uv);
      meshData.Uv.Add(uv);
      meshData.Uv.Add(uv);
      
      meshData.Triangles.Add(vertexIndex);
      meshData.Triangles.Add(vertexIndex + 2);
      meshData.Triangles.Add(vertexIndex + 1);
      
      meshData.Triangles.Add(vertexIndex + 1);
      meshData.Triangles.Add(vertexIndex + 2);
      meshData.Triangles.Add(vertexIndex + 3);
    }
    
  }
}


