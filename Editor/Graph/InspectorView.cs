using UnityEngine.UIElements;

namespace Gameframe.Procgen.Editor
{
    public class InspectorView : VisualElement
    {
        public class UxmlFactor : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        private UnityEditor.Editor editor = null;

        public InspectorView() {}

        // public void UpdateSelection(MissionGraphNodeView nodeView)
        // {
        //     Clear();
        //
        //     if (editor != null)
        //     {
        //         UnityEngine.Object.DestroyImmediate(editor);
        //     }
        //
        //     editor = Editor.CreateEditor(nodeView.Node);
        //     var container = new IMGUIContainer(() =>
        //     {
        //         editor.OnInspectorGUI();
        //     });
        //     Add(container);
        // }
    }
}