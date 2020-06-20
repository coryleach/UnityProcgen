//Ignore those dumb 'never assigned to' warnings cuz this is Unity and our fields are serialized 
#pragma warning disable CS0649

using UnityEngine;
using UnityEngine.Serialization;

namespace Gameframe.Procgen
{
  [System.Serializable]
  public class TerrainType
  {
    [SerializeField] private int _weight = 1;
    public int Weight
    {
      get => _weight;
      set => _weight = value;
    }

    [SerializeField] private string _name = string.Empty;
    public string Name => _name;

    [SerializeField] private float floor = 0;
    public float Floor
    {
      get => floor;
      set => floor = value;
    }

    [FormerlySerializedAs("_height")] [SerializeField]
    private float _threshold = 0.5f;
    public float Threshold
    {
      get => _threshold;
      set => _threshold = value;
    }

    [FormerlySerializedAs("_elevation")] [SerializeField]
    private float _minElevation;
    public float MinElevation
    {
      get => _minElevation;
      set => _minElevation = value;
    }

    [SerializeField]
    private float _maxElevation;
    public float MaxElevation
    {
      get => _maxElevation;
      set => _maxElevation = value;
    }

    [SerializeField]
    private Gradient gradient;
    public Gradient ColorGradient => gradient;
  }
}