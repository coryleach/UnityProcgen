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
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, uint seed, Vector2 offset, float frequency, int octaves = 4, float persistence = 0.5f, float lacunarity = 2f, float[,] falloffMap = null)
    {
      var noiseMap = new float[mapWidth, mapHeight];

      //Generate some random offsets based on the seed
      for (int y = 0; y < mapHeight; y++)
      {
        for (int x = 0; x < mapWidth; x++)
        {
          var sample = SimplexGradientNoise.FractalGradient2D(x + offset.x, y + offset.y, seed, frequency, octaves, lacunarity, persistence);
          noiseMap[x, y] = sample.value * (1f - falloffMap[x, y]);
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

    public static float GenerateFalloffPoint(int x, int y, int width, int height, float a = 3f, float b = 2.2f, Vector2 offset = default)
    {
      x += Mathf.RoundToInt(width * offset.x);
      y += Mathf.RoundToInt(height * offset.y);

      var valueY = (y + 0.5f) / (float) height * 2 - 1;
      var valueX = (x + 0.5f) / (float) width * 2 - 1;
      var value = Mathf.Max(Mathf.Abs(valueX), Mathf.Abs(valueY));
      return 1 - FalloffCurve(value, a, b);
    }

    public static float FalloffCurve(float value, float a = 3, float b = 2.2f)
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
