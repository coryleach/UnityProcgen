using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
  [CreateAssetMenu(menuName = "Gameframe/Procgen/WorldMapGenerator")]
  public class WorldMapGenerator : ScriptableObject
  {
    [SerializeField] private int mapWidth = 100;
    [SerializeField] private int mapHeight = 100;
    [SerializeField] private List<WorldMapLayerGenerator> layerGenerators = new List<WorldMapLayerGenerator>();

    public WorldMapData GenerateMap(int seed)
    {
      var worldData = new WorldMapData
      {
        seed = seed,
        width = mapWidth,
        height = mapHeight
      };

      var rng = new RandomGeneratorStruct((uint)seed);
      for (int i = 0; i < layerGenerators.Count; i++)
      {
        //Generate each layer with its own seed
        layerGenerators[i].AddToWorld(worldData,rng.NextInt());
      }

      return worldData;
    }

    private void OnValidate()
    {
      if (mapWidth <= 0)
      {
        mapWidth = 1;
      }

      if (mapHeight <= 0)
      {
        mapHeight = 1;
      }
    }
  }

}
