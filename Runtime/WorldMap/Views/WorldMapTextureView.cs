using UnityEngine;

namespace Gameframe.Procgen
{
    public class WorldMapTextureView : MonoBehaviour, IWorldMapView
    {
        [SerializeField] private Material _material = null;
        [SerializeField] private bool mainTexture = false;
        [SerializeField] private string texturePropertyName = "_BaseMap";

        [SerializeField] private TerrainTable _terrainTable = null;

        [SerializeField] private bool gradiate = false;

        [SerializeField] private bool fillRegions = false;
        [SerializeField] private bool drawBorders = false;
        [SerializeField] private bool drawSpawnPoints = false;
        [SerializeField] private bool drawPoissonPoints = false;

        [SerializeField, Range(0f, 1f)] private float borderAlpha = 0.5f;
        [SerializeField, Range(0, 1f)] private float regionFillAlpha = 0.2f;
        [SerializeField] private Color[] regionColors = new Color[0];
        [SerializeField] private FilterMode filterMode = FilterMode.Point;

        [SerializeField] private bool scaleTexture = false;
        [SerializeField] private float textureScale = 2f;

        private void Start()
        {
            //This is here just to show the enabled checkbox in the unity inspector
        }

        public async void DisplayMap(WorldMapData worldMapData)
        {
            if (!enabled)
            {
                return;
            }

            var heightMapLayer = worldMapData.GetLayer<IFloatMapLayerData>();
            var regionMapLayer = worldMapData.GetLayer<RegionMapLayerData>();

            var width = worldMapData.width;
            var height = worldMapData.height;

            var heightMap = heightMapLayer.FloatMap;
            var regions = regionMapLayer.regions;
            var regionMap = regionMapLayer.regionMap;

            var terrainMap = _terrainTable.GetTerrainMap(heightMap);
            var colorMap = TerrainTable.GetColorMap(heightMap, terrainMap, gradiate);

            if (fillRegions && regionColors.Length > 0)
            {
                for (int i = 0; i < colorMap.Length; i++)
                {
                    var regionIndex = regionMap[i];
                    if (regionIndex <= 0)
                    {
                        continue;
                    }

                    var regionColor = regionColors[(regionIndex - 1) % regionColors.Length];
                    var alpha = Mathf.Clamp01(regionColor.a * regionFillAlpha);
                    colorMap[i] = regionColor * alpha + (1 - alpha) * colorMap[i];
                }
            }

            if (drawBorders && regionColors.Length > 0)
            {
                foreach (var region in regions)
                {
                    foreach (var pt in region.borderPoints)
                    {
                        int index = pt.y * width + pt.x;
                        var regionIndex = regionMap[index];
                        var regionColor = regionColors[regionIndex - 1];
                        var alpha = regionColor.a * borderAlpha;
                        colorMap[index] = regionColor * alpha + (1 - alpha) * colorMap[index];
                        colorMap[index].a = 1;
                    }
                }
            }

            if (drawSpawnPoints)
            {
                foreach (var region in regions)
                {
                    var pt = region.spawnPt;
                    int index = pt.y * width + pt.x;
                    colorMap[index] = Color.black;
                }
            }

            if (drawPoissonPoints)
            {
                var poissonLayer = worldMapData.GetLayer<PoissonMapLayerData>();
                if (poissonLayer != null)
                {
                    foreach (var pt in poissonLayer.points)
                    {
                        int index = pt.y * width + pt.x;
                        colorMap[index] = Color.black;
                    }
                }
            }

            var texture = TextureUtility.CreateFromColorMap(colorMap, width, height);
            texture.filterMode = filterMode;
            SetTexture(texture);

            if (scaleTexture)
            {
                await TextureScale.BilinearAsync(texture, Mathf.Min(2048,Mathf.RoundToInt(width * textureScale)), Mathf.Min(2048,Mathf.RoundToInt(height * textureScale)));
            }
        }

        private void SetTexture(Texture2D texture)
        {
            if (!mainTexture)
            {
                _material.SetTexture(texturePropertyName, texture);
            }
            else
            {
                _material.mainTexture = texture;
            }
        }

    }
}
