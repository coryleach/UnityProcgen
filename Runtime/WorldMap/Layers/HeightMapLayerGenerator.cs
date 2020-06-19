using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/HeightMapLayerGenerator")]
    public class HeightMapLayerGenerator : WorldMapLayerGenerator
    {
        public enum MapType
        {
            Noise,
            Falloff,
            NoiseWithFalloff
        }

        [SerializeField] private bool step = false;

        [SerializeField] private int stepCount = 10;
        
        [SerializeField] private Vector2 offset = Vector2.zero;

        [SerializeField] private float noiseScale = 1;

        [SerializeField] private int octaves = 4;

        [SerializeField, Range(0f, 1f)] private float persistence = 0.5f;

        [SerializeField] private float lacunarity = 2f;

        [SerializeField] private float falloffA = 3f;

        [SerializeField] private float falloffB = 2.2f;

        [SerializeField] private MapType mapType = MapType.Noise;

        public HeightMapLayerData Generate(int width, int height, int seed)
        {
            float[,] noiseMap;

            switch (mapType)
            {
                case MapType.Noise:
                    noiseMap = Noise.GenerateNoiseMap(width, height, seed, offset, noiseScale, octaves,
                        persistence, lacunarity);
                    break;
                case MapType.Falloff:
                    noiseMap = Noise.GenerateFalloffMap(width, height, falloffA, falloffB);
                    break;
                case MapType.NoiseWithFalloff:
                    var falloffMap = Noise.GenerateFalloffMap(width, height, falloffA, falloffB);
                    noiseMap = Noise.GenerateNoiseMap(width, height, seed, offset, noiseScale, octaves,
                        persistence, lacunarity, falloffMap);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            float[] heightMap = new float[width*height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var val = noiseMap[x, y];

                    if (step)
                    {
                        val = Mathf.RoundToInt(val * stepCount) / (float)stepCount;
                    }
                    
                    heightMap[y * width + x] = val;
                }
            }
            
            return new HeightMapLayerData
            {
                heightMap = heightMap
            };
        }

        public override void AddToWorld(WorldMapData worldMapData)
        {
            worldMapData.layers.Add(Generate(worldMapData.width,worldMapData.height,worldMapData.seed));
        }

        private void OnValidate()
        {
            if (lacunarity < 1)
            {
                lacunarity = 1;
            }
            if (octaves < 0)
            {
                octaves = 0;
            }
        }
    }

}

