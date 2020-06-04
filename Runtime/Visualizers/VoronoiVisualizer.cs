using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoronoiVisualizer : MonoBehaviour
{
    public int seed = 0;
    public Vector2Int imageSize;
    public int regionAmount;

    [SerializeField]
    private MeshRenderer _renderer;

    public bool URP = true;
    
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

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
        if (URP)
        {
            _renderer.sharedMaterial.SetTexture(BaseMap,texture);
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
