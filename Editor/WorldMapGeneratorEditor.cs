using UnityEditor;
using UnityEngine;

namespace Gameframe.Procgen
{
    [CustomEditor(typeof(WorldMapGenController), true)]
    public class WorldMapGeneratorEditor : UnityEditor.Editor
    {
        private bool autoUpdate = false;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck() && autoUpdate)
            {
                ((WorldMapGenController) target).GenerateMap();
            }

            autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);

            if (GUILayout.Button("Generate"))
            {
                ((WorldMapGenController) target).GenerateMap();
            }
        }
    }
}
