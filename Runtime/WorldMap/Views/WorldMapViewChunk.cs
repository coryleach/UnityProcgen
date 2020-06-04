//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

using UnityEngine;

namespace Gameframe.Procgen
{
    public class WorldMapViewChunk : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        public void SetTexture(Texture2D texture)
        {
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }

        public void SetMesh(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
        }
    }
}