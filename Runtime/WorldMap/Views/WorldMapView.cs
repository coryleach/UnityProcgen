using UnityEngine;

namespace Gameframe.Procgen
{
  public abstract class WorldMapView : MonoBehaviour, IWorldMapView
  {
    public virtual Vector3 MapToWorldPosition(Vector2Int point)
    {
      return new Vector3(point.x, 0, point.y);
    }
    
    public abstract void DisplayMap(WorldMapData mapData);
  }
}