using System;
using UnityEngine;

namespace Gameframe.Procgen
{
  [Serializable]
  public class VoronoiData
  {
    public int width;
    public int height;
    public Vector2Int[] centroids;
    public int regionCount;
    public int[] regionData;
  }
}