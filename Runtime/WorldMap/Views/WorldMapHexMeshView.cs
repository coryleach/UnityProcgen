using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
  
  public class WorldMapHexMeshView : WorldMapView, IWorldMapView
  {
    [SerializeField] private WorldMapViewChunk _prefab = null;
    [SerializeField] private float radius = 1f;
    [SerializeField] private TerrainTable terrainTable = null;
    [SerializeField] private bool useColorGradient = false;
    [SerializeField] private int chunkWidth = 16;
    [SerializeField] private int chunkHeight = 16;
    [SerializeField] private float heightScale = 1;
    [SerializeField] private bool smooth = false;

    [SerializeField, Range(0,1f)] private float border = 0.25f;
    
    [SerializeField] private List<WorldMapViewChunk> chunkList = new List<WorldMapViewChunk>();
    private readonly Dictionary<Vector2, WorldMapViewChunk> _chunks = new Dictionary<Vector2, WorldMapViewChunk>();

    private WorldMapData _mapData = null;
    private float[] _heightMap = null;

    private void Start()
    {
      //This is here just to show the enable checkbox in editor
    }
    
    private void OnDisable()
    {
      ClearChunks();
    }

    public override Vector3 MapToWorldPosition(Vector2Int point)
    {
      var innerRadius = HexMeshUtility.GetInnerRadius(radius);
      var xOffset = point.x + point.y * 0.5f - (int)(point.y / 2);
      var pt = new Vector3
      {
        x = xOffset * innerRadius * 2,
        y = _mapData == null || _heightMap == null ? 0 : GetElevation(_heightMap[point.y * _mapData.width + point.x]),
        z = point.y * radius * 1.5f
      };
      return pt;
    }

    public override void DisplayMap(WorldMapData mapData)
    {
      if (!enabled)
      {
        return;
      }

      _mapData = mapData;
      _heightMap = _mapData.GetLayer<HeightMapLayerData>().heightMap;
      
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
          var mesh = HexMeshUtility.GenerateHexagonMesh(radius, border, startX, startY, chunkWidth, chunkHeight, mapData.width, mapData.height, _heightMap, GetColor, GetElevation);
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
    
    [ContextMenu("Clear Chunks")]
    public void ClearChunks()
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

  }

}

