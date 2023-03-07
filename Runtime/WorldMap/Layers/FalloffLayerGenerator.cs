using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/Falloff Map")]
    public class FalloffLayerGenerator : RandomAccessFloatGenerationLayer
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

        public override float Generate(int x, int y, int width, int height, int seed)
        {
            var offset = falloffOffset;

            if (randomizeOffset)
            {
                var rng = new RandomGeneratorStruct((uint)seed);
                offset.y = rng.NextFloatRange(minOffsetY, maxOffsetY);
                offset.x = rng.NextFloatRange(minOffsetX, maxOffsetX);
            }

            var v =  Noise.GenerateFalloffPoint(x , y , width, height, falloffA, falloffB, offset);

            return Mathf.Clamp01(applyCurve ? curve.Evaluate(v) : v);
        }
    }
}
