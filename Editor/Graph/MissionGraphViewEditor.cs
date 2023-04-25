using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;

namespace Gameframe.Procgen.Editor
{
    public class MissionGraphViewEditor : EditorWindow
    {
        private MissionGraphView graphView;
        private InspectorView inspectorView;

        [MenuItem("Gameframe/Graph/MissionGraphEditor")]
        public static void OpenWindow()
        {
            var window = GetWindow<MissionGraphViewEditor>();
            window.titleContent = new GUIContent("MissionGraphEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is not MissionGraph)
            {
                return false;
            }

            OpenWindow();
            return true;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.gameframe.procgen/Editor/Graph/MissionGraphView.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.gameframe.procgen/Editor/Graph/MissionGraphView.uss");
            root.styleSheets.Add(styleSheet);
            graphView = root.Q<MissionGraphView>();
            inspectorView = root.Q<InspectorView>();

            //graphView.OnNodeSelected = OnNodeSelected;
            //OnSelectionChange();
        }

        private void OnDestroy()
        {
            //graphView.OnDestroy();
        }

        // private void OnNodeSelected(MissionGraphNodeView nodeView)
        // {
        //     //inspectorView.UpdateSelection(nodeView);
        // }

        private void OnSelectionChange()
        {
            var graph = Selection.activeObject as MissionGraph;
            if (graph != null && AssetDatabase.CanOpenAssetInEditor(graph.GetInstanceID()))
            {
                //graphView.PopulateView(graph);
            }
        }
    }
}
