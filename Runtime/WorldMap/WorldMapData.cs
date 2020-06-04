using System.Collections.Generic;

namespace Gameframe.Procgen
{
    [System.Serializable]
    public class WorldMapData
    {
        public int seed;
        public int width;
        public int height;
        public List<WorldMapLayerData> layers = new List<WorldMapLayerData>();

        /// <summary>
        /// Gets the first layer of the specified type
        /// </summary>
        /// <typeparam name="T">Type derived from WorldMapLayerData</typeparam>
        /// <returns>Returns the first layer of the requested type found. Otherwise null if no layer of that type exists.</returns>
        public T GetLayer<T>() where T : WorldMapLayerData
        {
            foreach (var layer in layers)
            {
                if (layer is T data)
                {
                    return data;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first layer of a specified type
        /// </summary>
        /// <param name="type">Type of layer to get</param>
        /// <returns>Layer object if found. Null if not found.</returns>
        public object GetLayer(System.Type type)
        {
            foreach (var layer in layers)
            {
                if (layer.GetType() == type)
                {
                    return layer;
                }
            }

            return null;
        }
    }
}