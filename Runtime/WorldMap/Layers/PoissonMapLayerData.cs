using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    [System.Serializable]
    public class PoissonMapLayerData : IWorldMapLayerData
    {
        public List<Vector2Int> points = new List<Vector2Int>();
    }
}
