//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

using UnityEngine;

namespace Gameframe.WorldMapGen
{
    public class WoldMapTerrainMeshView : MonoBehaviour, IWorldMapView
    {
        [SerializeField] private MeshFilter _meshFilter = null;

        [SerializeField, Range(0,6)] private int levelOfDetail;
        
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
            var meshData = TerrainMeshUtility.GenerateMesh(heightMap,worldMapData.width,worldMapData.height,levelOfDetail);
            _meshFilter.mesh = meshData.CreateMesh();
        }
    }
}
