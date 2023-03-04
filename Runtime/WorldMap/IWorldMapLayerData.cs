using System;
using UnityEngine;

namespace Gameframe.Procgen
{
  public interface IWorldMapLayerData
  {
    string Tag { get; set; }
  }

  [Serializable]
  public abstract class WorldMapLayerData : IWorldMapLayerData
  {
    [SerializeField]
    private string tag;
    public string Tag
    {
      get => tag;
      set => tag = value;
    }
  }
}
