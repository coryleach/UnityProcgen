using UnityEngine;

namespace Gameframe.Procgen
{
    public class FloatLayerVisualizer : MonoBehaviour
    {
        [SerializeField] private int seed = 0;

        [SerializeField] private Material _material;

        [SerializeField] private WorldMapLayerGenerator _layerGenerator;

        [SerializeField] private WorldMapData _mapData;

        [ContextMenu("Generate")]
        private void Generate()
        {
            _mapData.layers.Clear();
            _layerGenerator.AddToWorld(_mapData, seed);

            var layer = _mapData.GetLayer<FloatMapLayerData>();
            var texture = TextureUtility.CreateFromFloatMap(layer.FloatMap, _mapData.width, _mapData.height);
            _material.mainTexture = texture;
        }
    }
}
