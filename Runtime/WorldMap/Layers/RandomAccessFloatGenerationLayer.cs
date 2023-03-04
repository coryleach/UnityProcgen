namespace Gameframe.Procgen
{
    public abstract class RandomAccessFloatGenerationLayer : WorldMapLayerGenerator
    {
        public abstract float Generate(int x, int y, int width, int height, int seed);

        protected float[] GenerateMap(int width, int height, int seed)
        {
            var floatMap = new float[width * height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = y * width + x;
                    floatMap[i] = Generate(x, y, width, height, seed);
                }
            }
            return floatMap;
        }

        protected override IWorldMapLayerData GenerateLayer(WorldMapData mapData, int layerSeed)
        {
            var map = GenerateMap(mapData.width, mapData.height, layerSeed);
            return new FloatMapLayerData
            {
                FloatMap = map
            };
        }
    }

}
