using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Nodes
{
    public class RootNode : Node
    {
        public RootNode(GraphView graphView) 
            : base("Assets/Plugins/Dialogs/Editor/Node/RootNode/RootNodeView.uxml", graphView)
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/Dialogs/Editor/Node/RootNode/RootNodeView.uss"));

            title = "root";
            AddPort(Direction.Output, Port.Capacity.Multi, "Dialogue");

            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
            
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(0, 0, 100, 150));
        }
    }
}