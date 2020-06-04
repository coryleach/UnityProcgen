using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    [System.Serializable]
    public class RegionMapLayerData : IWorldMapLayerData
    {
        public int[] regionMap;
        public List<RegionData> regions = new List<RegionData>();
    }

    public class RegionData
    {
        public int id;
        public int size;
        public Vector2Int spawnPt;
        public List<Vector2Int> borderPoints = new List<Vector2Int>();
    }
}