using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Nodes
{
    public class ChoicesNode : DialogueNode
    {
        public ChoicesNode(GraphView graphView) 
            : base("Assets/Plugins/Dialogs/Editor/Node/DialoguesNode/ChoicesNode/ChoicesNodeView.uxml", graphView)
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/Dialogs/Editor/Node/DialoguesNode/ChoicesNode/ChoicesNodeView.uss"));

            title = "Choices Node";
            AddPort(Direction.Input, Port.Capacity.Multi, "previous dialogue");

            var button = this.Q<Button>("button-new-choice");
            button.clickable.clicked += () => AddChoicePort();

            RefreshExpandedState();
            RefreshPorts();
        }

        public void AddChoicePort(string overridenPortName = "")
        {
            var generatedPort = AddPort(Direction.Output);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            var outputPortCount = outputContainer.Query("connector").ToList().Count;
            generatedPort.portName = $"Choice {outputPortCount}";

            var choicePortName = string.IsNullOrEmpty(overridenPortName)
                ? $"Choice {outputPortCount}"
                : overridenPortName;

            var textField = new TextField
            {
                name = string.Empty,
                value = choicePortName,
            };
            textField.style.minWidth = 60;
            textField.style.maxWidth = 100;
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(graphView, generatedPort))
            {
                text = "X",
            };
            generatedPort.contentContainer.Add(deleteButton);

            generatedPort.portName = choicePortName;
            outputContainer.Add(generatedPort);
            RefreshExpandedState();
            RefreshPorts();
        }

        private void RemovePort(GraphView graphView, Port generatedPort)
        {
            var targetEdge = graphView.edges.ToList()
                .Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                graphView.RemoveElement(targetEdge.First());
            }

            outputContainer.Remove(generatedPort);
            RefreshPorts();
            RefreshExpandedState();
        }
    }
}
