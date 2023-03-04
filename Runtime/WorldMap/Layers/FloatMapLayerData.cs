using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    [Serializable]
    public class FloatMapLayerData : WorldMapLayerData, IFloatMapLayerData
    {
        [SerializeField]
        private float[] floatMap;
        public float[] FloatMap
        {
            get => floatMap;
            set => floatMap = value;
        }
    }

    public interface IFloatMapLayerData : IWorldMapLayerData
    {
        public float[] FloatMap { get; }
    }
}
