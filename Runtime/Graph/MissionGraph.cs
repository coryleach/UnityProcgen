using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Graphs/MissionGraph")]
    public class MissionGraph : ScriptableObject
    {
        public List<MissionNode> nodes = new List<MissionNode>();
    }
}
