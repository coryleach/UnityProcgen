using UnityEditor;
using UnityEngine;

namespace Gameframe.WorldMapGen
{
    [CustomEditor(typeof(WorldMapGenerator), true)]
    public class WorldMapGeneratorEditor : Editor
    {
        private bool autoUpdate = false;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck() && autoUpdate)
            {
                ((WorldMapGenerator) target).GenerateMap();
            }

            autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);

            if (GUILayout.Button("Generate"))
            {
                ((WorldMapGenerator) target).GenerateMap();
            }
        }
    }
}