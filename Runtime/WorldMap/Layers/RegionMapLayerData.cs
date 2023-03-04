using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    [Serializable]
    public class RegionMapLayerData : WorldMapLayerData
    {
        public int[] regionMap;
        public List<RegionData> regions = new List<RegionData>();
    }

    [Serializable]
    public class RegionData
    {
        public int id;
        public int size;
        public Vector2Int spawnPt;
        public List<Vector2Int> borderPoints = new List<Vector2Int>();
    }
}
