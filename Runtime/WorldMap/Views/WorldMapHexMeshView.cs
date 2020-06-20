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
    
    [SerializeField] private List<WorldMapViewChunk> chunkList = new List<WorldMapViewChunk>();
    private Dictionary<Vector2, WorldMapViewChunk> _chunks = new Dictionary<Vector2, WorldMapViewChunk>();

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
          var mesh = HexMeshUtility.GenerateHexagonMesh(radius, startX, startY, chunkWidth, chunkHeight, mapData.width, mapData.height, heightMap, GetColor, GetElevation);
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

  }

}

