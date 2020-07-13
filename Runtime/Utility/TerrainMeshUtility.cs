using System;
using UnityEngine;

namespace Gameframe.Procgen
{

  public static class TerrainMeshUtility
  {
    private const int mapChunkSize = 241;

    public static MeshData GenerateMesh(float[] heightMap, int width, int height, int levelOfDetail, Func<float,float> stepFunction, Func<float,Color> colorFunction = null)
    {
      float topLeftX = (width - 1) / -2f;
      float topLeftZ = (height - 1) / 2f;

      int lodIncrement = Mathf.Max(1, levelOfDetail * 2);
      int vertsPerLine = (width - 1) / lodIncrement + 1;
      int vertsPerColumn = (height - 1) / lodIncrement + 1;

      var meshData = new MeshData(vertsPerLine, vertsPerColumn, colorFunction != null);
      int vertIndex = 0;
      int triangleIndex = 0;
      for (int i = 0; i < vertsPerColumn; i++)
      {
        for (int j = 0; j < vertsPerLine; j++)
        {
          int x = j * lodIncrement;
          int y = i * lodIncrement;

          meshData.vertices[vertIndex] = new Vector3(topLeftX + x, stepFunction.Invoke(heightMap[y * width + x]), topLeftZ - y);
          meshData.uv[vertIndex] = new Vector2(x / (float) width, y / (float) height);

          if (colorFunction != null)
          {
            meshData.colors[vertIndex] = colorFunction.Invoke(heightMap[y * width + x]);
          }//*/
          
          if (j < (vertsPerLine - 1) && i < (vertsPerColumn - 1))
          {
            triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex, vertIndex + vertsPerLine + 1,
              vertIndex + vertsPerLine);
            triangleIndex = meshData.AddTriangle(triangleIndex, vertIndex + vertsPerLine + 1, vertIndex, vertIndex + 1);
          }

          vertIndex++;
        }
      }
      
      return meshData;
    }
    
    public static MeshData GenerateMesh(float[] heightMap, int width, int height, float heightScale, int levelOfDetail)
    {
      return GenerateMesh(heightMap, width, height, levelOfDetail, x => heightScale * x);
    }
  }

  public class MeshData
  {
    public readonly Vector3[] vertices;
    public readonly int[] triangles;
    public readonly Vector2[] uv;
    public readonly Color[] colors;

    public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uv)
    {
      this.vertices = vertices;
      this.triangles = triangles;
      this.uv = uv;
    }
    
    public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uv, Color[] colors)
    {
      this.vertices = vertices;
      this.triangles = triangles;
      this.uv = uv;
      this.colors = colors;
    }

    public MeshData(int vertsWide, int vertsHigh, bool vertexColors = false)
    {
      vertices = new Vector3[vertsWide * vertsHigh];
      uv = new Vector2[vertsWide * vertsHigh];
      triangles = new int[(vertsWide - 1) * (vertsHigh - 1) * 6];
      if (vertexColors)
      {
        colors = new Color[vertsWide * vertsHigh];
      }
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

      if (colors != null)
      {
        mesh.colors = colors;
      }
      
      mesh.RecalculateNormals();
      return mesh;
    }

  }

}
