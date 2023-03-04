using UnityEngine;

namespace Gameframe.Procgen
{
  public abstract class WorldMapLayerGenerator : ScriptableObject
  {
    [SerializeField]
    private string tag = string.Empty;

    protected abstract IWorldMapLayerData GenerateLayer(WorldMapData mapData, int layerSeed);

    public void AddToWorld(WorldMapData mapData, int layerSeed)
    {
      var data = GenerateLayer(mapData, layerSeed);
      data.Tag = tag;
      mapData.layers.Add(data);
    }
  }
}
