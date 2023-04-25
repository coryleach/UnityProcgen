using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gameframe.Procgen.Editor
{
    public class MissionGraphView : UnityEditor.Experimental.GraphView.Node
    {

        private MissionGraph graph;

        public MissionGraphView(MissionGraph graph) : base("Packages/com.gamegrame.procgen/Editor/Graph/BehaviourTreeNodeView.uxml")
        {
            this.graph = graph;
            //title = this.graph.name;
            /*this.node = node;
            title = node.DisplayName;
            viewDataKey = node.guid;
            style.left = node.position.x;
            style.top = node.position.y;

            _inspectorView = this.Q<InspectorView>();
            _inspectorView.UpdateSelection(this);

            CreateInputPorts();
            CreateOutputPorts();
            SetupStyleClasses();*/
        }

    }
}
