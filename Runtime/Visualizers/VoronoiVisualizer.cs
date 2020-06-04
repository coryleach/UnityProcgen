//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

using UnityEngine;

namespace Gameframe.Procgen
{
  public class VoronoiVisualizer : MonoBehaviour
  {
    public int seed = 0;
    public Vector2Int imageSize;
    public int regionAmount;

    [SerializeField]
    private MeshRenderer _renderer;

    public bool UseMainTexture = true;
    public string texturePropertyName = "_BaseMap";
    
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
      SetTexture(GetDiagram());
    }

    private void SetTexture(Texture2D texture)
    {
      if (!UseMainTexture)
      {
        _renderer.sharedMaterial.SetTexture(texturePropertyName,texture);
      }
      else
      {
        _renderer.sharedMaterial.mainTexture = texture;
      }
    }
    
    [ContextMenu("GenerateFalloff")]
    public void GenerateFalloff()
    {
      if (_renderer == null)
      {
        return;
      }
      SetTexture(GetFalloffDiagram());
    }
    
    [ContextMenu("GenerateFalloffV2")]
    public void GenerateFalloff_V2()
    {
      if (_renderer == null)
      {
        return;
      }
      SetTexture(GetFalloffDiagram_V2());
    }

    //Each random position is called a 'centroid'
    private Texture2D GetDiagram()
    {
      var data = Voronoi.Create(imageSize.x, imageSize.y, regionAmount, seed);
      return Voronoi.CreateTexture(data);
    }
    
    private Texture2D GetFalloffDiagram()
    {
      var data = Voronoi.Create(imageSize.x, imageSize.y, regionAmount, seed);
      return Voronoi.CreateFalloffTexture(data);
    }
    
    private Texture2D GetFalloffDiagram_V2()
    {
      var data = Voronoi.Create(imageSize.x, imageSize.y, regionAmount, seed);
      return Voronoi.CreateBorderTexture(data);
    }
    
  }
}
