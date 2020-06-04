using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.WorldMapGen
{
    public static class VoxelMeshUtility
    {
        public static VoxelMeshData CreateMeshData(float[,] noiseMap, int chunkX, int chunkY, int chunkWidth,
            int chunkHeight, TerrainTable terrainTable, bool edges = true, float edgeThickness = 1f)
        {
            var width = noiseMap.GetLength(0);
            var height = noiseMap.GetLength(1);
            var terrainMap = terrainTable.GetTerrainMap(noiseMap);

            var xOffset = chunkX * chunkWidth;
            var yOffset = chunkY * chunkHeight;

            var meshData = new VoxelMeshData(width, height, new Vector2Int(xOffset, yOffset));

            for (var y = 0; y < chunkHeight && y + yOffset < height; y++)
            {
                for (var x = 0; x < chunkWidth && x + xOffset < width; x++)
                {
                    var mapX = x + xOffset;
                    var mapY = y + yOffset;

                    var terrain = terrainMap[mapX, mapY];

                    var front = mapY + 1 < height ? terrainMap[mapX, mapY + 1] : null;
                    var back = mapY - 1 >= 0 ? terrainMap[mapX, mapY - 1] : null;
                    var right = mapX + 1 < width ? terrainMap[mapX + 1, mapY] : null;
                    var left = mapX - 1 >= 0 ? terrainMap[mapX - 1, mapY] : null;

                    meshData.AddUpQuad(x, y, terrain.Elevation);

                    if (left != null && left.Elevation < terrain.Elevation)
                    {
                        meshData.AddLeftQuad(x, y, terrain.Elevation, terrain.Elevation - left.Elevation);
                    }
                    else if (edges && mapX - 1 < 0)
                    {
                        meshData.AddLeftQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }

                    if (front != null && front.Elevation < terrain.Elevation)
                    {
                        meshData.AddFrontQuad(x, y, terrain.Elevation, terrain.Elevation - front.Elevation);
                    }
                    else if (edges && mapY + 1 >= height)
                    {
                        meshData.AddFrontQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }

                    if (right != null && right.Elevation < terrain.Elevation)
                    {
                        meshData.AddRightQuad(x, y, terrain.Elevation, terrain.Elevation - right.Elevation);
                    }
                    else if (edges && mapX + 1 >= width)
                    {
                        meshData.AddRightQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }

                    if (back != null && back.Elevation < terrain.Elevation)
                    {
                        meshData.AddBackQuad(x, y, terrain.Elevation, terrain.Elevation - back.Elevation);
                    }
                    else if (edges && mapY - 1 < 0)
                    {
                        meshData.AddBackQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }
                }
            }

            return meshData;
        }

        public static VoxelMeshData CreateMeshData(float[] heightMap, int width, int height, int chunkX, int chunkY,
            int chunkWidth, int chunkHeight, TerrainTable terrainTable, bool edges = true, float edgeThickness = 1f)
        {
            var terrainMap = terrainTable.GetTerrainMap(heightMap);

            var xOffset = chunkX * chunkWidth;
            var yOffset = chunkY * chunkHeight;

            var meshData = new VoxelMeshData(width, height, new Vector2Int(xOffset, yOffset));

            for (var y = 0; y < chunkHeight && y + yOffset < height; y++)
            {
                for (var x = 0; x < chunkWidth && x + xOffset < width; x++)
                {
                    var mapX = x + xOffset;
                    var mapY = y + yOffset;

                    var terrain = terrainMap[mapY * width + mapX];

                    var front = mapY + 1 < height ? terrainMap[(mapY + 1) * width + mapX] : null;
                    var back = mapY - 1 >= 0 ? terrainMap[(mapY - 1) * width + mapX] : null;
                    var right = mapX + 1 < width ? terrainMap[mapY * width + mapX + 1] : null;
                    var left = mapX - 1 >= 0 ? terrainMap[mapY * width + mapX - 1] : null;

                    meshData.AddUpQuad(x, y, terrain.Elevation);

                    if (left != null && left.Elevation < terrain.Elevation)
                    {
                        meshData.AddLeftQuad(x, y, terrain.Elevation, terrain.Elevation - left.Elevation);
                    }
                    else if (edges && mapX - 1 < 0)
                    {
                        meshData.AddLeftQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }

                    if (front != null && front.Elevation < terrain.Elevation)
                    {
                        meshData.AddFrontQuad(x, y, terrain.Elevation, terrain.Elevation - front.Elevation);
                    }
                    else if (edges && mapY + 1 >= height)
                    {
                        meshData.AddFrontQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }

                    if (right != null && right.Elevation < terrain.Elevation)
                    {
                        meshData.AddRightQuad(x, y, terrain.Elevation, terrain.Elevation - right.Elevation);
                    }
                    else if (edges && mapX + 1 >= width)
                    {
                        meshData.AddRightQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }

                    if (back != null && back.Elevation < terrain.Elevation)
                    {
                        meshData.AddBackQuad(x, y, terrain.Elevation, terrain.Elevation - back.Elevation);
                    }
                    else if (edges && mapY - 1 < 0)
                    {
                        meshData.AddBackQuad(x, y, terrain.Elevation, terrain.Elevation + edgeThickness);
                    }
                }
            }

            return meshData;
        }

    }

    public class VoxelMeshData
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector2> uv = new List<Vector2>();
        private List<int> triangles = new List<int>();

        private int width;
        private int height;
        private int quadCount;
        private Vector2Int offset;

        public VoxelMeshData(int width, int height, Vector2Int offset)
        {
            this.width = width;
            this.height = height;
            this.offset = offset;
            quadCount = 0;
        }

        /// <summary>
        /// Add a quad for X,Y that faces "up"
        /// A quad with normal y = 1
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="elevation">elevation of the quad</param>
        /// <param name="depth">distance from bottom edge of quad to top edge</param>
        public void AddUpQuad(int x, int y, float elevation)
        {
            var bottomLeft = new Vector3(x, elevation, y);
            var topLeft = new Vector3(x, elevation, y + 1);
            var topRight = new Vector3(x + 1, elevation, y + 1);
            var bottomRight = new Vector3(x + 1, elevation, y);

            vertices.Add(bottomLeft);
            vertices.Add(topLeft);
            vertices.Add(topRight);
            vertices.Add(bottomRight);

            AddUV(x, y);
            AddTriangles();
        }

        /// <summary>
        /// Add a quad for X,Y that faces "Left"
        /// A quad with normal x = -1
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="elevation">elevation of the quad</param>
        /// <param name="depth">distance from bottom edge of quad to top edge</param>
        public void AddLeftQuad(int x, int y, float elevation, float depth)
        {
            var bottomLeft = new Vector3(x, elevation - depth, y + 1);
            var topLeft = new Vector3(x, elevation, y + 1);
            var topRight = new Vector3(x, elevation, y);
            var bottomRight = new Vector3(x, elevation - depth, y);

            vertices.Add(bottomLeft);
            vertices.Add(topLeft);
            vertices.Add(topRight);
            vertices.Add(bottomRight);

            AddUV(x, y);
            AddTriangles();
        }

        /// <summary>
        /// Add a quad for X,Y that faces "Right"
        /// A quad with normal x = 1
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="elevation">elevation of the quad</param>
        /// <param name="depth">distance from bottom edge of quad to top edge</param>
        public void AddRightQuad(int x, int y, float elevation, float depth)
        {
            var bottomLeft = new Vector3(x + 1, elevation - depth, y);
            var topLeft = new Vector3(x + 1, elevation, y);
            var topRight = new Vector3(x + 1, elevation, y + 1);
            var bottomRight = new Vector3(x + 1, elevation - depth, y + 1);

            vertices.Add(bottomLeft);
            vertices.Add(topLeft);
            vertices.Add(topRight);
            vertices.Add(bottomRight);

            AddUV(x, y);
            AddTriangles();
        }

        /// <summary>
        /// Add a quad for X,Y that faces "Back"
        /// A quad with normal z = -1
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="elevation">elevation of the quad</param>
        /// <param name="depth">distance from bottom edge of quad to top edge</param>
        public void AddBackQuad(int x, int y, float elevation, float depth)
        {
            var bottomLeft = new Vector3(x, elevation - depth, y);
            var topLeft = new Vector3(x, elevation, y);
            var topRight = new Vector3(x + 1, elevation, y);
            var bottomRight = new Vector3(x + 1, elevation - depth, y);

            vertices.Add(bottomLeft);
            vertices.Add(topLeft);
            vertices.Add(topRight);
            vertices.Add(bottomRight);

            AddUV(x, y);
            AddTriangles();
        }

        /// <summary>
        /// Add a quad for X,Y that faces "Front"
        /// A quad with normal z = 1
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="elevation">elevation of the quad</param>
        /// <param name="depth">distance from bottom edge of quad to top edge</param>
        public void AddFrontQuad(int x, int y, float elevation, float depth)
        {
            var bottomLeft = new Vector3(x + 1, elevation - depth, y + 1);
            var topLeft = new Vector3(x + 1, elevation, y + 1);
            var topRight = new Vector3(x, elevation, y + 1);
            var bottomRight = new Vector3(x, elevation - depth, y + 1);

            vertices.Add(bottomLeft);
            vertices.Add(topLeft);
            vertices.Add(topRight);
            vertices.Add(bottomRight);

            AddUV(x, y);
            AddTriangles();
        }

        /// <summary>
        /// Add UVs for current quad
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="padding">Add a little bit of padding to prevent bleeding at edge of quads</param>
        private void AddUV(int x, int y, float padding = 0.1f)
        {
            //We need to add the offset so UVs are shifted to correct texture chunk
            var uvBottomLeft = new Vector2((x + offset.x + padding) / width, (y + offset.y + padding) / height);
            var uvTopLeft = new Vector2((x + offset.x + padding) / width, (y + offset.y + 1 - padding) / height);
            var uvTopRight = new Vector2((x + offset.x + 1 - padding) / width, (y + offset.y + 1 - padding) / height);
            var uvBottomRight = new Vector2((x + offset.x + 1 - padding) / width, (y + offset.y + padding) / height);

            uv.Add(uvBottomLeft);
            uv.Add(uvTopLeft);
            uv.Add(uvTopRight);
            uv.Add(uvBottomRight);
        }

        /// <summary>
        /// Add triangles for current quad
        /// </summary>
        private void AddTriangles()
        {
            int triangleIndex = quadCount * 4;

            triangles.Add(triangleIndex + 0);
            triangles.Add(triangleIndex + 1);
            triangles.Add(triangleIndex + 2);

            triangles.Add(triangleIndex + 0);
            triangles.Add(triangleIndex + 2);
            triangles.Add(triangleIndex + 3);

            quadCount++;
        }


        public Mesh CreateMesh()
        {
            var mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uv.ToArray()
            };
            mesh.RecalculateNormals();
            return mesh;
        }

    }
}