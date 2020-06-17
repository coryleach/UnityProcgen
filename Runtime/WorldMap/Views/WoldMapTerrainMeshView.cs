//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

using UnityEngine;

namespace Gameframe.Procgen
{
    public class WoldMapTerrainMeshView : MonoBehaviour, IWorldMapView
    {
        [SerializeField] private MeshFilter _meshFilter = null;

        [SerializeField, Range(0,6)] private int levelOfDetail;

        [SerializeField] private float heightScale = 1;

        [SerializeField] private TerrainTable terrainTable = null;
        
        //This is here just to get the enabled checkbox in editor
        private void Start()
        {
        }

        public void DisplayMap(WorldMapData worldMapData)
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
                    x => terrainTable.GetTerrainType(x).Elevation * heightScale, 
                    x => terrainTable.GetTerrainType(x).ColorGradient.Evaluate(1));
                _meshFilter.mesh = meshData.CreateMesh();
            }
            
        }
    }
}
