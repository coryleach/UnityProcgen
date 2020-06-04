using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TerrainType
{
  [SerializeField]
  private int _weight = 1;
  public int Weight
  {
    get => _weight;
    set => _weight = value;
  }

  [SerializeField] private string _name = string.Empty;
  public string Name => _name;

  [SerializeField]
  private float floor = 0;

  public float Floor
  {
    get => floor;
    set => floor = value;
  }
  
  [FormerlySerializedAs("_height")] [SerializeField]
  private float _threshold;
  public float Threshold
  {
    get => _threshold;
    set => _threshold = value;
  }

  [FormerlySerializedAs("_meshHeight")] [SerializeField] 
  private float _elevation;
  public float Elevation => _elevation;
  
  [SerializeField]
  private Color _lowColor;
  public Color LowColor => _lowColor;
  
  [FormerlySerializedAs("_color")] [SerializeField]
  private Color _highColor;
  public Color HighColor => _highColor;
}

