using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.WorldMapGen
{
    [System.Serializable]
    public class PoissonMapLayerData : WorldMapLayerData
    {
        public List<Vector2Int> points = new List<Vector2Int>();
    }
}
