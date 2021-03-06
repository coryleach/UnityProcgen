﻿using UnityEngine;
//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

namespace Gameframe.Procgen
{
  public class PoissonVisualizer : MonoBehaviour
  {
    [SerializeField]
    private int seed;
    
    [SerializeField]
    private Vector2Int imageSize;
    
    [SerializeField]
    private float radius = 1f;
    
    [SerializeField]
    private int maxSamplesPerPoint = 100;

    [SerializeField] 
    private MeshRenderer _renderer;

    private void Start()
    {
      Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
      if (_renderer == null)
      {
        return;
      }

      _renderer.sharedMaterial.mainTexture = GetDiagram();
    }

    private Texture2D GetDiagram()
    {
      var points = PoissonDiskSampling.GeneratePoints(radius, imageSize, seed, maxSamplesPerPoint);
      Debug.Log($"Point Count: {points.Count}");

      var texture = new Texture2D(imageSize.x, imageSize.y);
      var colors = new Color[imageSize.x * imageSize.y];
      foreach (var point in points)
      {
        int x = (int) point.x;
        int y = (int) point.y;
        colors[y * imageSize.x + x] = Color.white;
      }

      texture.SetPixels(colors);

      texture.filterMode = FilterMode.Point;
      texture.wrapMode = TextureWrapMode.Clamp;
      texture.Apply();

      return texture;
    }

  }
}
