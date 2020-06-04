using Gameframe.WorldMapGen;
using UnityEngine;

public class WorldMapGenerator : MonoBehaviour
{
    [SerializeField]
    private int mapWidth = 100;
    
    [SerializeField]
    private int mapHeight = 100;
    
    [SerializeField] 
    private HeightMapLayerGenerator heightMapGenerator;

    [SerializeField] 
    private RegionMapLayerGenerator regionMapGenerator;

    [SerializeField] 
    private PoissonMapLayerGenerator poissonMapGenerator;

    [SerializeField] private int seed = 100;
    
    private void Start()
    {
        GenerateMap();
    }
    
    [ContextMenu("GenerateMap")]
    public void GenerateMap()
    {
        var worldData = new WorldMapData
        {
            seed = seed,
            width = mapWidth, 
            height = mapHeight
        };
        heightMapGenerator.AddToWorld(worldData);
        regionMapGenerator.AddToWorld(worldData);
        poissonMapGenerator.AddToWorld(worldData);
        DisplayMap(worldData);
    }

    private void DisplayMap(WorldMapData mapData)
    {
        var mapViews = GetComponents<IWorldMapView>();
        foreach (var view in mapViews)
        {
            view.DisplayMap(mapData);
        }
    }

    private void OnValidate()
    {
        if (mapWidth <= 0)
        {
            mapWidth = 1;
        }
        if (mapHeight <= 0)
        {
            mapHeight = 1;
        }
    }

}
