using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/Curve Map")]
    public class CurveLayerGenerator : RandomAccessFloatGenerationLayer
    {
        [SerializeField] private XYCombinationMode xyCombinationMode = XYCombinationMode.Multiply;
        [SerializeField] private AnimationCurve xCurve = AnimationCurve.Linear(0,0,1, 1);
        [SerializeField] private AnimationCurve yCurve = AnimationCurve.Linear(0,0,1, 1);
        [SerializeField] private CutoffMode cutoff = CutoffMode.None;
        [SerializeField][Range(0,1)] private float cutoffValue = 0;

        public enum XYCombinationMode
        {
            Multiply,
            Average,
            Add
        }

        public enum CutoffMode
        {
            None,
            ZeroOrOne,
            Gradual,
        }

        private float Clamp(float v)
        {
            switch (cutoff)
            {
                case CutoffMode.None:
                    return Mathf.Clamp01(v);
                case CutoffMode.ZeroOrOne:
                    return Mathf.Floor(v + 1 - cutoffValue);
                case CutoffMode.Gradual:
                    //return Mathf.InverseLerp(cutoffValue, 1, v);
                    return v >= cutoffValue ? Mathf.Clamp01(v) : 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override float Generate(int x, int y, int width, int height, int seed)
        {
            switch (xyCombinationMode)
            {
                case XYCombinationMode.Multiply:
                    return Clamp(yCurve.Evaluate(y / (float) height) * xCurve.Evaluate(x / (float) width));
                case XYCombinationMode.Average:
                    return Clamp((yCurve.Evaluate(y / (float) height) + xCurve.Evaluate(x / (float) width)) * 0.5f);
                case XYCombinationMode.Add:
                    return Clamp(yCurve.Evaluate(y / (float) height) + xCurve.Evaluate(x / (float) width));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
