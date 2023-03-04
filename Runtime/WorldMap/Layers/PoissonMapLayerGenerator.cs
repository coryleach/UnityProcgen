using UnityEngine;

namespace Gameframe.Procgen
{
  [CreateAssetMenu(menuName = "Gameframe/Procgen/Layers/Poisson Map")]
  public class PoissonMapLayerGenerator : WorldMapLayerGenerator
  {
    [SerializeField]
    private float radius = 10;

    [SerializeField]
    private int maxSamplesPerPoint = 30;

    [SerializeField]
    private bool useRegions = true;

    [SerializeField]
    private int edgeAvoidance;

    protected override IWorldMapLayerData GenerateLayer(WorldMapData mapData, int layerSeed)
    {
      return useRegions ? AddToWorldUsingRegions(mapData,layerSeed) : AddToWorldDefault(mapData,layerSeed);
    }

    private IWorldMapLayerData AddToWorldUsingRegions(WorldMapData mapData, int layerSeed)
    {
      var regionLayer = mapData.GetLayer<RegionMapLayerData>();
      var regionMap = regionLayer.regionMap;

      //Only select points in valid regions
      var points = PoissonDiskSampling.GenerateIntPoints(radius, new Vector2Int(mapData.width, mapData.height), layerSeed, maxSamplesPerPoint,
        (pt) =>
        {
          if (regionMap[pt.y * mapData.width + pt.x] <= 0)
          {
            return false;
          }

          if (edgeAvoidance == 0)
          {
            return true;
          }

          //Check any points in radius for overlap with bad region
          var searchStartX = Mathf.Max(0, pt.x - edgeAvoidance);
          var searchEndX = Mathf.Min(pt.x + edgeAvoidance, mapData.width - 1);

          var searchStartY = Mathf.Max(0, pt.y - edgeAvoidance);
          var searchEndY = Mathf.Min(pt.y + edgeAvoidance, mapData.height - 1);

          for (var x = searchStartX; x <= searchEndX; x++)
          {
            for (var y = searchStartY; y <= searchEndY; y++)
            {
              //Index was stored with +1 so we could avoid initializing the grid to all -1 values
              int regionIndex = regionMap[y * mapData.width + x];
              if (regionIndex <= 0)
              {
                return false;
              }
            }
          }

          return true;
        });

      var layer = new PoissonMapLayerData
      {
        points = points
      };

      return layer;
    }

    private IWorldMapLayerData AddToWorldDefault(WorldMapData mapData, int layerSeed)
    {
      var points = PoissonDiskSampling.GenerateIntPoints(radius, new Vector2Int(mapData.width, mapData.height), layerSeed, maxSamplesPerPoint);
      var layer = new PoissonMapLayerData
      {
        points = points
      };
      return layer;
    }

  }
}
