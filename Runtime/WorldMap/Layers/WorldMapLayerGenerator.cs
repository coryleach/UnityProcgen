using UnityEngine;

namespace Gameframe.WorldMapGen
{
  public abstract class WorldMapLayerGenerator : ScriptableObject
  {
    public abstract void AddToMap(WorldMapData mapData);
  }
}
