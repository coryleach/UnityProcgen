using Gameframe.Procgen;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureWaveCollapseVisualizer))]
public class VisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var visualizer = this.target as TextureWaveCollapseVisualizer;
        if (GUILayout.Button("InitAndRun"))
        {
            visualizer.InitAndRun();
        }
        if (GUILayout.Button("InitAndRun Reseed"))
        {
            visualizer.InitAndRunReseed();
        }
        if (GUILayout.Button("Init"))
        {
            visualizer.InitOnly();
        }
        if (GUILayout.Button("Step"))
        {
            visualizer.StepNext();
        }
        if (GUILayout.Button("DrawCurrentOutput"))
        {
            visualizer.DrawCurrent();
        }
        if (GUILayout.Button("DrawRecentTile"))
        {
            visualizer.DrawStepEntry();
        }
        if (GUILayout.Button("DrawAllTiles"))
        {
            visualizer.DrawAllStepEntry();
        }
    }
}
