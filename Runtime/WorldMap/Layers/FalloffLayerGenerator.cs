using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/Falloff Map")]
    public class FalloffLayerGenerator : WorldMapLayerGenerator
    {
        [SerializeField] private bool randomizeOffset = false;

        [SerializeField] private float minOffsetY = 0f;
        [SerializeField] private float minOffsetX = 0f;

        [SerializeField] private float maxOffsetY = 1f;
        [SerializeField] private float maxOffsetX = 1f;

        [SerializeField] private Vector2 falloffOffset = Vector2.zero;

        [SerializeField] private float falloffA = 3f;

        [SerializeField] private float falloffB = 2.2f;

        [SerializeField] private bool applyCurve = false;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0,0,1, 1);

        private float[] Generate(int width, int height, uint seed)
        {
            var offset = falloffOffset;

            if (randomizeOffset)
            {
                var rng = new RandomGeneratorStruct(seed);
                offset.y = rng.NextFloatRange(minOffsetY, maxOffsetY);
                offset.x = rng.NextFloatRange(minOffsetX, maxOffsetX);
            }

            var floatMap = new float[width*height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = y * width + x;
                    floatMap[i] = Noise.GenerateFalloffPoint(x , y , width, height, falloffA, falloffB, offset);
                    if (applyCurve)
                    {
                        floatMap[i] = curve.Evaluate(floatMap[i]);
                    }
                }
            }

            return floatMap;
        }

        protected override IWorldMapLayerData GenerateLayer(WorldMapData mapData, int layerSeed)
        {
            var map = Generate(mapData.width,mapData.height, (uint)layerSeed);
            return new FloatMapLayerData
            {
                FloatMap = map
            };
        }
    }
}
