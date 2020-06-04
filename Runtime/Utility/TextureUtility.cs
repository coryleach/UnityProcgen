using System;
using UnityEngine;

namespace Gameframe.Procgen
{

    public static class TextureUtility
    {
        public static Texture2D GetHeightMap(float[,] heightMap)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);

            var texture = new Texture2D(width, height);
            var colorMap = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D GetHeightMap(float[] heightMap, int width, int height)
        {
            var texture = new Texture2D(width, height);
            var colorMap = new Color[width * height];

            if (heightMap.Length < colorMap.Length)
            {
                throw new Exception("Height size does not match texture size");
            }

            for (int i = 0; i < heightMap.Length; i++)
            {
                colorMap[i] = Color.Lerp(Color.black, Color.white, heightMap[i]);
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D GetColorMap(Color[] colorMap, int width, int height)
        {
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }
    }

}