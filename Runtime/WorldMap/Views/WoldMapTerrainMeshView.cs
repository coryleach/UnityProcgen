using UnityEngine;

public class WoldMapTerrainMeshView : MonoBehaviour, IWorldMapView
{
    [SerializeField] private MeshFilter _meshFilter = null;

    [SerializeField, Range(0,6)] private int levelOfDetail;
    
    void Start()
    {
    }

    public void DisplayMap(WorldMapData worldMapData)
    {
        if (!enabled)
        {
            return;
        }
        var heightMap = worldMapData.GetLayer<HeightMapLayerData>().heightMap;
        var meshData = TerrainMeshUtility.GenerateMesh(heightMap,worldMapData.width,worldMapData.height,levelOfDetail);
        _meshFilter.mesh = meshData.CreateMesh();
    }
}
