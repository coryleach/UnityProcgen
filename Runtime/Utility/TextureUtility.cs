using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    public static class TextureUtility
    {
        /// <summary>
        /// Creates a texture from a 2 dimensional array of floats
        /// </summary>
        /// <param name="floatMap">2 dimensional array of floats</param>
        /// <param name="a">left hand side of texture color lerp</param>
        /// <param name="b">right hand side of texture color lerp</param>
        /// <returns>Texture2D filled with colors visualizing the 2d array</returns>
        public static Texture2D CreateFrom2dFloatMap(float[,] floatMap, Color a, Color b)
        {
            var width = floatMap.GetLength(0);
            var height = floatMap.GetLength(1);

            var texture = new Texture2D(width, height);
            var colorMap = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorMap[y * width + x] = Color.Lerp(a, b, floatMap[x, y]);
                }
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Creates a texture from a 2 dimensional array of floats
        /// Maps float values 0->1 onto a texture be lerping pixel color from black and white
        /// </summary>
        /// <param name="floatMap">2 dimensional array of floats</param>]
        /// <returns>Texture2D filled with colors visualizing the 2d array</returns>
        public static Texture2D CreateFrom2dFloatMap(float[,] floatMap)
        {
            return CreateFrom2dFloatMap(floatMap, Color.black, Color.white);
        }

        /// <summary>
        /// Create texture from a float map
        /// Maps float values 0->1 onto a texture be lerping pixel color from black and white
        /// </summary>
        /// <param name="floatMap">array of floats</param>
        /// <param name="width">width of map/texture</param>
        /// <param name="height">height of map/texture</param>
        /// <returns>a Texture2D filled with colors representing the map</returns>
        public static Texture2D CreateFromFloatMap(float[] floatMap, int width, int height)
        {
            return CreateFromFloatMap(floatMap, width, height, Color.black, Color.white);
        }

        /// <summary>
        /// Create texture from a float map
        /// Maps float values 0->1 onto a texture be lerping pixel color from color 'a' to color 'b'
        /// </summary>
        /// <param name="floatMap">array of floats</param>
        /// <param name="width">width of map/texture</param>
        /// <param name="height">height of map/texture</param>
        /// <param name="a">Color that maps to 0</param>
        /// <param name="b">Color that maps to 1</param>
        /// <returns>a Texture2D filled with colors representing the map</returns>
        public static Texture2D CreateFromFloatMap(float[] floatMap, int width, int height, Color a, Color b)
        {
            var texture = new Texture2D(width, height);
            var colorMap = new Color[width * height];

            if (floatMap.Length < colorMap.Length)
            {
                throw new Exception("Height size does not match texture size");
            }

            for (var i = 0; i < floatMap.Length; i++)
            {
                colorMap[i] = Color.Lerp(a, b, floatMap[i]);
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Creates a texture from an array of colors
        /// </summary>
        /// <param name="colorMap">Array of colors</param>
        /// <param name="width">width of color map and texture</param>
        /// <param name="height">height of color map and texture</param>
        /// <returns>Texture2D filled with the color map</returns>
        public static Texture2D CreateFromColorMap(Color[] colorMap, int width, int height)
        {
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Writes the texture to disk in png format
        /// </summary>
        /// <param name="tex2d">Texture to write to disk</param>
        /// <param name="fullPath">Full path to write file to</param>
        public static void SaveTextureAsPNG(Texture2D tex2d, string fullPath)
        {
            var bytes = tex2d.EncodeToPNG();
            System.IO.File.WriteAllBytes(fullPath, bytes);
            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + fullPath);
        }
    }

}
