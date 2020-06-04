using UnityEngine;

public static class TerrainMeshUtility
{
  private const int mapChunkSize = 241;
  
  public static MeshData GenerateMesh(float[] heightMap, int width, int height, int levelOfDetail)
  {
    float topLeftX = (width - 1) / -2f;
    float topLeftZ = (height - 1) / 2f;

    int lodIncrement = Mathf.Max(1, levelOfDetail * 2);
    int vertsPerLine = (width-1) / lodIncrement + 1;
    int vertsPerColumn = (height-1) / lodIncrement + 1;
    
    var meshData = new MeshData(vertsPerLine,vertsPerColumn);
    int vertIndex = 0;
    int triangleIndex = 0;
    for (int i = 0; i < vertsPerColumn; i++)
    {
      for (int j = 0; j < vertsPerLine; j++)
      {
        int x = j * lodIncrement;
        int y = i * lodIncrement;
        
        meshData.vertices[vertIndex] = new Vector3(topLeftX + x,heightMap[y*width + x], topLeftZ - y);
        meshData.uv[vertIndex] = new Vector2(x/(float)width, y/(float)height);

        if (j < (vertsPerLine-1) && i < (vertsPerColumn-1))
        {
          triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex, vertIndex + vertsPerLine + 1, vertIndex + vertsPerLine);
          triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex + vertsPerLine + 1, vertIndex, vertIndex + 1);
        }
        
        vertIndex++;
      }
    }
    
    /*for (int y = 0; y < height; y += lodIncrement)
    {
      for (int x = 0; x < width; x += lodIncrement)
      {
        if (x < width - 1 && y < height - 1)
        {
          //triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex, vertIndex + vertsPerLine, vertIndex + 1);
          //triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex + 1, vertIndex + vertsPerLine + 1, vertIndex + vertsPerLine);
          triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex, vertIndex + vertsPerLine + 1, vertIndex + vertsPerLine);
          triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex + vertsPerLine + 1, vertIndex, vertIndex + 1);
        }
      }
    }*/

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
  
  public MeshData(int vertsWide, int vertsHigh)
  {
    vertices = new Vector3[vertsWide * vertsHigh];
    uv = new Vector2[vertsWide * vertsHigh];
    triangles = new int[(vertsWide-1) * (vertsHigh-1) * 6];
    
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