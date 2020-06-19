using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
  
  public class WorldMapHexMeshView : MonoBehaviour, IWorldMapView
  {
    [SerializeField] private WorldMapViewChunk _prefab = null;
    [SerializeField] private float radius = 1f;
    [SerializeField] private TerrainTable terrainTable = null;
    [SerializeField] private bool useColorGradient = false;
    [SerializeField] private int chunkWidth = 16;
    [SerializeField] private int chunkHeight = 16;
    [SerializeField] private float heightScale = 1;
    [SerializeField] private bool smooth = false;
    
    private Dictionary<Vector2, WorldMapViewChunk> _chunks = new Dictionary<Vector2, WorldMapViewChunk>();

    [SerializeField] private List<WorldMapViewChunk> chunkList = new List<WorldMapViewChunk>();

    public enum HexDirection
    {
      NE, 
      E, 
      SE, 
      SW, 
      W, 
      NW
    }
    
    private void Start()
    {
      //This is here just to show the enable checkbox in editor
    }
    
    private void OnDisable()
    {
      ClearChunks();
    }
    
    public void DisplayMap(WorldMapData mapData)
    {
      if (!enabled)
      {
        return;
      }
      
      var heightMap = mapData.GetLayer<HeightMapLayerData>().heightMap;
      
      ClearChunks();
      
      //Chunks
      var chunksWide = Mathf.CeilToInt(mapData.width / (float)chunkWidth);
      var chunksHigh = Mathf.CeilToInt(mapData.height / (float)chunkHeight);

      for (var chunkY = 0; chunkY < chunksHigh; chunkY++)
      {
        for (var chunkX = 0; chunkX < chunksWide; chunkX++)
        {
          //Create the mesh
          var startX = chunkX * chunkWidth;
          var startY = chunkY * chunkHeight;
          var chunkView = GetChunk(new Vector2Int(chunkX,chunkY));
          chunkView.transform.localPosition = new Vector3(0,0,0);
          var mesh = GenerateHexagonMesh(radius, startX, startY, chunkWidth, chunkHeight, mapData.width, mapData.height, heightMap, GetColor, GetElevation);
          chunkView.SetMesh(mesh);
        }
      }
    }

    private Color GetColor(float height)
    {
        if (terrainTable == null)
        {
          return Color.white;
        }
          
        var terrainType = terrainTable.GetTerrainType(height);
        if (!useColorGradient)
        {
          return terrainType.ColorGradient.Evaluate(0);
        }
        
        var t = Mathf.InverseLerp(terrainType.Floor, terrainType.Threshold, height);
        return terrainType.ColorGradient.Evaluate(t);
    }

    private float GetElevation(float height)
    {
      var terrainType = terrainTable.GetTerrainType(height);
      if (smooth)
      {
        var t = Mathf.InverseLerp(terrainType.Floor, terrainType.Threshold, height);
        return Mathf.Lerp(terrainType.MinElevation, terrainType.MaxElevation, t) * heightScale;
      }
      return terrainTable.GetTerrainType(height).MinElevation * heightScale;
    }

    private WorldMapViewChunk GetChunk(Vector2Int chunkPt)
    {
      WorldMapViewChunk chunk = null;
      if (_chunks.TryGetValue(chunkPt, out chunk))
      {
        return chunk;
      }

      //Create a new chunk
      chunk = Instantiate(_prefab,transform);
      _chunks.Add(chunkPt, chunk);
      chunkList.Add(chunk);
      return chunk;
    }
    
    private void ClearChunks()
    {
      foreach (var chunk in chunkList)
      {
        if (chunk == null)
        {
          continue;
        }

        if (Application.isEditor)
        {
          DestroyImmediate(chunk.gameObject);
        }
        else
        {
          Destroy(chunk.gameObject);
        }
      }

      chunkList.Clear();
      _chunks.Clear();
    }

    public static int GetNeighbor(int index, HexDirection hexDirection, int mapWidth, int mapHeight)
    {
      int y = index / mapWidth;
      int x = index - (y * mapWidth);
      
      switch (hexDirection)
      {
        case HexDirection.NE:
          //Only odd numbered rows shift a column 
          if ((x & 1) == 1)
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
          if ((x & 1) == 1)
          {
            x++;
          }
          y--;
          break;
        case HexDirection.SW:
          //Only even numbered rows shift a column 
          if ((x & 1) == 0)
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
          if ((x & 1) == 0)
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
    
    public static Mesh GenerateHexagonMesh(float outerRadius, int startX, int startY, int chunkWidth, int chunkHeight, int mapWidth, int mapHeight, float[] heightMap, Func<float,Color> colorFunction, Func<float,float> elevationFunction)
    {
      var innerRadius = outerRadius * Mathf.Sqrt(3f) * 0.5f;
      List<Vector3> vertices = new List<Vector3>(); 
      List<int> triangles = new List<int>();
      List<Color> colors = new List<Color>();
      
      Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
      };

      for (int dy = 0; dy < chunkHeight && (startY + dy) < mapHeight; dy++)
      {
        for (int dx = 0; dx < chunkWidth && (startX + dx) < mapWidth; dx++)
        {
          int x = startX + dx;
          int y = startY + dy;
          
          var index = (y * mapWidth) + x;
          float elevation = elevationFunction?.Invoke(heightMap[index]) ?? 0;
          
          var xOffset = x + y * 0.5f - (int)(y / 2);
          var center = new Vector3(xOffset*innerRadius*2,elevation,y*outerRadius*1.5f);
          for (int i = 0; i < 6; i++)
          {
            AddTriangle(vertices,triangles,colors,
              center,
              center+corners[i], 
              center+corners[(i+1) % corners.Length], colorFunction?.Invoke(heightMap[index]) ?? Color.white);
          }

          var neighbor = GetNeighbor(index, HexDirection.E, mapWidth, mapHeight);
          if (neighbor != -1)
          {
            float neighborElevation = elevationFunction?.Invoke(heightMap[neighbor]) ?? 0;
            if (!Mathf.Approximately(neighborElevation, elevation))
            {
              //Draw a quad for this side
            }
          }

        }
      }
      
      Mesh mesh = new Mesh();
      mesh.vertices = vertices.ToArray();
      mesh.triangles = triangles.ToArray();
      mesh.colors = colors.ToArray();
      mesh.RecalculateNormals();
      return mesh;
    }
    
    private static void AddTriangle(List<Vector3> vertices, List<int> triangles, List<Color> colors, Vector3 v1, Vector3 v2, Vector3 v3, Color color) {
      int vertexIndex = vertices.Count;
      
      vertices.Add(v1);
      vertices.Add(v2);
      vertices.Add(v3);
      
      colors.Add(color);
      colors.Add(color);
      colors.Add(color);
      
      triangles.Add(vertexIndex);
      triangles.Add(vertexIndex + 1);
      triangles.Add(vertexIndex + 2);
    }
    
  }

}

