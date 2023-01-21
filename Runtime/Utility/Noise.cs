using UnityEngine;

namespace Gameframe.Procgen
{
  public static class Noise
  {
    /// <summary>
    /// Generate a random noise map
    /// </summary>
    /// <param name="mapWidth"></param>
    /// <param name="mapHeight"></param>
    /// <param name="seed"></param>
    /// <param name="offset"></param>
    /// <param name="scale"></param>
    /// <param name="octaves"></param>
    /// <param name="persistence"></param>
    /// <param name="lacunarity"></param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, Vector2 offset, float scale, int octaves = 4, float persistence = 0.5f, float lacunarity = 2f, float[,] falloffMap = null)
    {
      var noiseMap = new float[mapWidth, mapHeight];

      //Generate some random offsets based on the seed
      var rng = new System.Random(seed);
      var octaveOffsets = new Vector2[octaves];
      for (int i = 0; i < octaves; i++)
      {
        octaveOffsets[i] = new Vector2
        {
          x = rng.Next(-100000, 100000) + offset.x,
          y = rng.Next(-100000, 100000) + offset.y
        };
      }

      scale = Mathf.Max(scale, 0.0000001f);

      var maxNoiseHeight = float.MinValue;
      var minNoiseHeight = float.MaxValue;

      var halfWidth = mapWidth * 0.5f;
      var halfHeight = mapHeight * 0.5f;

      for (int y = 0; y < mapHeight; y++)
      {
        for (int x = 0; x < mapWidth; x++)
        {
          float amplitude = 1;
          float frequency = 1;
          float noiseHeight = 0;

          for (int i = 0; i < octaves; i++)
          {
            var sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
            var sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

            //Multiply by 2 and subtract one to create values in the range range -1 to 1
            //We can come back and normalize values later
            var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
          }

          //Track min and max values to normalize values later
          if (noiseHeight > maxNoiseHeight)
          {
            maxNoiseHeight = noiseHeight;
          }
          else if (noiseHeight < minNoiseHeight)
          {
            minNoiseHeight = noiseHeight;
          }

          noiseMap[x, y] = noiseHeight;
        }
      }

      //Normalize Noise Map so all values are in range 0 to 1
      for (int y = 0; y < mapHeight; y++)
      {
        for (int x = 0; x < mapWidth; x++)
        {
          noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
          if (falloffMap != null)
          {
            noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x,y]);
          }
        }
      }

      return noiseMap;
    }

    public static float[,] GenerateFalloffMap(int width, int height, float a = 3f, float b = 2.2f)
    {
      float[,] map = new float[width, height];

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          var valueY = (y+0.5f) / (float) height * 2 - 1;
          var valueX = (x+0.5f) / (float) width * 2 - 1;

          var value = Mathf.Max(Mathf.Abs(valueX), Mathf.Abs(valueY));
          map[x, y] = FalloffCurve(value, a, b);
        }
      }

      return map;
    }

    private static float FalloffCurve(float value, float a = 3, float b = 2.2f)
    {
      return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    public static float Noise1D_ZeroToOne(int x, uint seed)
    {
      return SquirrelEiserloh.Get1dNoiseZeroToOne(x, seed);
    }

    public static float Noise2D_ZeroToOne(Vector2Int v, uint seed)
    {
      return SquirrelEiserloh.Get2dNoiseZeroToOne(v.x, v.y, seed);
    }

    public static float Noise3D_ZeroToOne(Vector3Int v, uint seed)
    {
      return SquirrelEiserloh.Get3dNoiseZeroToOne(v.x, v.y, v.z, seed);
    }

    public static float Noise4D_ZeroToOne(int x, int y, int z, int w, uint seed)
    {
      return SquirrelEiserloh.Get4dNoiseZeroToOne(x, y, z, w, seed);
    }

    public static float Noise1D_NegOneToOne(int x, uint seed)
    {
      return SquirrelEiserloh.Get1dNoiseNegOneToOne(x, seed);
    }

    public static float Noise2D_NegOneToOne(Vector2Int v, uint seed)
    {
      return SquirrelEiserloh.Get2dNoiseNegOneToOne(v.x, v.y, seed);
    }

    public static float Noise3D_NegOneToOne(Vector3Int v, uint seed)
    {
      return SquirrelEiserloh.Get3dNoiseNegOneToOne(v.x, v.y, v.z, seed);
    }

    public static float Noise4D_NegOneToOne(int x, int y, int z, int w, uint seed)
    {
      return SquirrelEiserloh.Get4dNoiseNegOneToOne(x, y, z, w, seed);
    }

  }
}
