using UnityEngine;

namespace Gameframe.Procgen
{
  public abstract class WorldMapLayerGenerator : ScriptableObject
  {
    public abstract void AddToWorld(WorldMapData mapData);
  }
}
