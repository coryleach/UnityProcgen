using UnityEngine;

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
