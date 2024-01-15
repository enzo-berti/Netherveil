using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Nodes
{
    public class TextNode : DialogueNode
    {
        public TextNode(GraphView graphView) 
            : base("Assets/Plugins/Dialogs/Editor/Node/DialoguesNode/TextNode/TextNodeView.uxml", graphView)
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/Dialogs/Editor/Node/DialoguesNode/TextNode/TextNodeView.uss"));

            title = "Text Node";
            AddPort(Direction.Input, Port.Capacity.Multi, "previous dialogue");
            AddPort(Direction.Output, Port.Capacity.Single, "next dialogue");

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
