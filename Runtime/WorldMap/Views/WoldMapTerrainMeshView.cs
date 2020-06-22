//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

using UnityEngine;

namespace Gameframe.Procgen
{
    public class WoldMapTerrainMeshView : WorldMapView, IWorldMapView
    {
        [SerializeField] private MeshFilter _meshFilter = null;

        [SerializeField, Range(0,6)] private int levelOfDetail;

        [SerializeField] private float heightScale = 1;

        [SerializeField] private TerrainTable terrainTable = null;

        [SerializeField] private bool smooth = false;
        
        [SerializeField] private bool useColorGradient = false;
        
        //This is here just to get the enabled checkbox in editor
        private void Start()
        {
        }

        public override void DisplayMap(WorldMapData worldMapData)
        {
            if (!enabled)
            {
                return;
            }
            var heightMap = worldMapData.GetLayer<HeightMapLayerData>().heightMap;

            if (terrainTable == null)
            {
                var meshData = TerrainMeshUtility.GenerateMesh(heightMap,worldMapData.width,worldMapData.height,heightScale,levelOfDetail);
                _meshFilter.mesh = meshData.CreateMesh();
            }
            else
            {
                var meshData = TerrainMeshUtility.GenerateMesh(heightMap,worldMapData.width,worldMapData.height,levelOfDetail,
                    x =>
                    {
                        var terrainType = terrainTable.GetTerrainType(x);
                        if (smooth)
                        {
                            var t = Mathf.InverseLerp(terrainType.Floor, terrainType.Threshold, x);
                            return Mathf.Lerp(terrainType.MinElevation, terrainType.MaxElevation, t) * heightScale;
                        }
                        return terrainTable.GetTerrainType(x).MinElevation * heightScale;
                    }, 
                    x =>
                    {
                        var terrainType = terrainTable.GetTerrainType(x);
                        if (!useColorGradient)
                        {
                            return terrainType.ColorGradient.Evaluate(0);
                        }
                        var t = Mathf.InverseLerp(terrainType.Floor, terrainType.Threshold, x);
                        return terrainType.ColorGradient.Evaluate(t);
                    });
                _meshFilter.mesh = meshData.CreateMesh();
            }
            
        }
        
    }
}
