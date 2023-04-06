using Gameframe.Procgen.Towngen;
using UnityEngine;

public class TownGeneratorVisualizer : MonoBehaviour
{
    [SerializeField] private Material _material;

    private Texture2D _outputTexture;

    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;

    [SerializeField] private int minBuildings = 5;
    [SerializeField] private int maxBuildlings = 10;

    [SerializeField] private Color buildingColor = Color.cyan;
    [SerializeField] private Color doorColor = Color.black;
    [SerializeField] private Color roadColor = Color.white;
    [SerializeField] private Color grassColor = Color.green;

    [SerializeField] private bool drawBuildings = true;
    [SerializeField] private int seed = 0;

    [ContextMenu("Generate")]
    public void Generate()
    {
        _outputTexture = new Texture2D(width, height);

        var townGenerator = new TownGenerator
        {
            townWidth = width,
            townHeight = height
        };

        townGenerator.Generate(minBuildings,maxBuildlings,seed);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _outputTexture.SetPixel(x,y, grassColor);
            }
        }

        foreach (var road in townGenerator.roads)
        {
            DrawRect(road.Rect, roadColor, _outputTexture);
        }

        if (drawBuildings)
        {
            foreach (var building in townGenerator.buildings)
            {
                DrawRect(building.Rect, buildingColor, _outputTexture);
                _outputTexture.SetPixel(building.DoorPoint.X, building.DoorPoint.Y, doorColor);
            }
        }

        _outputTexture.filterMode = FilterMode.Point;
        _outputTexture.Apply();
        _material.mainTexture = _outputTexture;
    }

    private void DrawRect(Rectangle rect, Color color, Texture2D texture)
    {
        for (int y = rect.MinY; y < rect.MaxY; y++)
        {
            for (int x = rect.MinX; x < rect.MaxX; x++)
            {
                texture.SetPixel(x,y, color);
            }
        }
    }

}
