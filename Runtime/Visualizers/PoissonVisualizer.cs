using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonVisualizer : MonoBehaviour
{
  public int seed = 0;
  public Vector2Int imageSize;
  public float radius = 1f;
  public int maxSamplesPerPoint = 100;

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
    
    var texture = new Texture2D(imageSize.x,imageSize.y);
    var colors = new Color[imageSize.x * imageSize.y];
    foreach (var point in points)
    {
      int x = (int)point.x;
      int y = (int)point.y;
      colors[y * imageSize.x + x] = Color.white;
    }
    texture.SetPixels(colors);

    texture.filterMode = FilterMode.Point;
    texture.wrapMode = TextureWrapMode.Clamp;
    texture.Apply();
    
    return texture;
  }
  
}
