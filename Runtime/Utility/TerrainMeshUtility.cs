using UnityEngine;

public static class TerrainMeshUtility
{
  private const int mapChunkSize = 241;
  
  public static MeshData GenerateMesh(float[] heightMap, int width, int height, int levelOfDetail)
  {
    float topLeftX = (width - 1) / -2f;
    float topLeftZ = (height - 1) / 2f;

    int lodIncrement = Mathf.Max(1, levelOfDetail * 2);
    int vertsPerLine = (width - 1) / lodIncrement + 1;
    
    var meshData = new MeshData(vertsPerLine,vertsPerLine);
    int vertIndex = 0;

    int triangleIndex = 0;
    for (int y = 0; y < height; y += lodIncrement)
    {
      for (int x = 0; x < width; x += lodIncrement)
      {
        meshData.vertices[vertIndex] = new Vector3(topLeftX + x,heightMap[y*width + x], topLeftZ - y);
        meshData.uv[vertIndex] = new Vector2(x/(float)width, y/(float)height);
        
        if (x < width - 1 && y < height - 1)
        {
          triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex, vertIndex + vertsPerLine + 1, vertIndex + vertsPerLine);
          triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex + vertsPerLine + 1, vertIndex, vertIndex + 1);
        }
        
        vertIndex++;
      }
    }

    return meshData;
  }
}

public class MeshData
{
  public readonly Vector3[] vertices;
  public readonly int[] triangles;
  public readonly Vector2[] uv;

  public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uv)
  {
    this.vertices = vertices;
    this.triangles = triangles;
    this.uv = uv;
  }
  
  public MeshData(int width, int height)
  {
    vertices = new Vector3[width * height];
    uv = new Vector2[width * height];
    triangles = new int[(width - 1) * (height - 1) * 6];
  }

  //Add the triangle and return the next index
  public int AddTriangle(int index, int a, int b, int c)
  {
    triangles[index] = a;
    triangles[index + 1] = b;
    triangles[index + 2] = c;
    return index + 3;
  }
  
  public Mesh CreateMesh()
  {
    var mesh = new Mesh
    {
      vertices = vertices, 
      triangles = triangles, 
      uv = uv
    };
    mesh.RecalculateNormals();
    return mesh;
  }
  
}