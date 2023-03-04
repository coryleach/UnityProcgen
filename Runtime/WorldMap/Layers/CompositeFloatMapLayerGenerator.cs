using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/Composite Float Map")]
    public class CompositeFloatMapLayerGenerator : RandomAccessFloatGenerationLayer
    {
        [SerializeField]
        public List<RandomAccessFloatGenerationLayer> generators = new List<RandomAccessFloatGenerationLayer>();

        public override float Generate(int x, int y, int width, int height, int seed)
        {
            float v = 1;
            foreach (var generator in generators)
            {
                v *= generator.Generate(x, y, width, height, seed);
            }
            return v;
        }
    }
}