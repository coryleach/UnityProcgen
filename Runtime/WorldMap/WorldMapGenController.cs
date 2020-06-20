//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameframe.Procgen
{
    public class WorldMapGenController : MonoBehaviour
    {
        [SerializeField] private WorldMapGenerator _generator = null;
        
        [SerializeField] private int seed = 100;

        [SerializeField] private bool randomizeSeed = false;

        [SerializeField, HideInInspector] private WorldMapData _mapData;

        private void Start()
        {
            GenerateMap();
        }

        [ContextMenu("GenerateMap")]
        public void GenerateMap()
        {
            if (randomizeSeed)
            {
                seed = Random.Range(int.MinValue, Int32.MaxValue);
            }
            _mapData = _generator.GenerateMap(seed);
            DisplayMap(_mapData);
        }

        private void DisplayMap(WorldMapData mapData)
        {
            var mapViews = GetComponents<IWorldMapView>();
            foreach (var view in mapViews)
            {
                view.DisplayMap(mapData);
            }
        }

    }

}