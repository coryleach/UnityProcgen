using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
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

                    meshData.AddUpQuad(x, y, terrain.MinElevation);

                    if (left != null && left.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddLeftQuad(x, y, terrain.MinElevation, terrain.MinElevation - left.MinElevation);
                    }
                    else if (edges && mapX - 1 < 0)
                    {
                        meshData.AddLeftQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
                    }

                    if (front != null && front.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddFrontQuad(x, y, terrain.MinElevation, terrain.MinElevation - front.MinElevation);
                    }
                    else if (edges && mapY + 1 >= height)
                    {
                        meshData.AddFrontQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
                    }

                    if (right != null && right.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddRightQuad(x, y, terrain.MinElevation, terrain.MinElevation - right.MinElevation);
                    }
                    else if (edges && mapX + 1 >= width)
                    {
                        meshData.AddRightQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
                    }

                    if (back != null && back.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddBackQuad(x, y, terrain.MinElevation, terrain.MinElevation - back.MinElevation);
                    }
                    else if (edges && mapY - 1 < 0)
                    {
                        meshData.AddBackQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
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

                    meshData.AddUpQuad(x, y, terrain.MinElevation);

                    if (left != null && left.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddLeftQuad(x, y, terrain.MinElevation, terrain.MinElevation - left.MinElevation);
                    }
                    else if (edges && mapX - 1 < 0)
                    {
                        meshData.AddLeftQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
                    }

                    if (front != null && front.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddFrontQuad(x, y, terrain.MinElevation, terrain.MinElevation - front.MinElevation);
                    }
                    else if (edges && mapY + 1 >= height)
                    {
                        meshData.AddFrontQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
                    }

                    if (right != null && right.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddRightQuad(x, y, terrain.MinElevation, terrain.MinElevation - right.MinElevation);
                    }
                    else if (edges && mapX + 1 >= width)
                    {
                        meshData.AddRightQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
                    }

                    if (back != null && back.MinElevation < terrain.MinElevation)
                    {
                        meshData.AddBackQuad(x, y, terrain.MinElevation, terrain.MinElevation - back.MinElevation);
                    }
                    else if (edges && mapY - 1 < 0)
                    {
                        meshData.AddBackQuad(x, y, terrain.MinElevation, terrain.MinElevation + edgeThickness);
                    }
                }
            }

            return meshData;
        }

    }

    public class VoxelMeshData
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector2> _uv = new List<Vector2>();
        private readonly List<int> _triangles = new List<int>();

        private readonly int _width;
        private readonly int _height;
        private int _quadCount;
        private readonly Vector2Int _offset;

        public VoxelMeshData(int width, int height, Vector2Int offset)
        {
            _width = width;
            _height = height;
            _offset = offset;
            _quadCount = 0;
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

            _vertices.Add(bottomLeft);
            _vertices.Add(topLeft);
            _vertices.Add(topRight);
            _vertices.Add(bottomRight);

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

            _vertices.Add(bottomLeft);
            _vertices.Add(topLeft);
            _vertices.Add(topRight);
            _vertices.Add(bottomRight);

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

            _vertices.Add(bottomLeft);
            _vertices.Add(topLeft);
            _vertices.Add(topRight);
            _vertices.Add(bottomRight);

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

            _vertices.Add(bottomLeft);
            _vertices.Add(topLeft);
            _vertices.Add(topRight);
            _vertices.Add(bottomRight);

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

            _vertices.Add(bottomLeft);
            _vertices.Add(topLeft);
            _vertices.Add(topRight);
            _vertices.Add(bottomRight);

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
            var uvBottomLeft = new Vector2((x + _offset.x + padding) / _width, (y + _offset.y + padding) / _height);
            var uvTopLeft = new Vector2((x + _offset.x + padding) / _width, (y + _offset.y + 1 - padding) / _height);
            var uvTopRight = new Vector2((x + _offset.x + 1 - padding) / _width, (y + _offset.y + 1 - padding) / _height);
            var uvBottomRight = new Vector2((x + _offset.x + 1 - padding) / _width, (y + _offset.y + padding) / _height);

            _uv.Add(uvBottomLeft);
            _uv.Add(uvTopLeft);
            _uv.Add(uvTopRight);
            _uv.Add(uvBottomRight);
        }

        /// <summary>
        /// Add triangles for current quad
        /// </summary>
        private void AddTriangles()
        {
            int triangleIndex = _quadCount * 4;

            _triangles.Add(triangleIndex + 0);
            _triangles.Add(triangleIndex + 1);
            _triangles.Add(triangleIndex + 2);

            _triangles.Add(triangleIndex + 0);
            _triangles.Add(triangleIndex + 2);
            _triangles.Add(triangleIndex + 3);

            _quadCount++;
        }


        public Mesh CreateMesh()
        {
            var mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uv.ToArray()
            };
            mesh.RecalculateNormals();
            return mesh;
        }

    }
}