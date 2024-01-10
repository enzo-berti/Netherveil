using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Nodes
{
    public class DialogueNode : Node
    {
        public DialogueNode(GraphView graphView) 
            : base("Assets/Plugins/Dialogs/Editor/Node/DialogueNode/DialogueNodeView.uxml", graphView)
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/Dialogs/Editor/Node/DialogueNode/DialogueNodeView.uss"));

            title = "Dialogue Node";
            dialogueText = "Hello World !";
            AddPort(Direction.Input, Port.Capacity.Multi, "Dialogue");

            var button = this.Q<Button>("button-new-choice");
            button.clickable.clicked += () => AddChoicePort(); 
            
            var textField = this.Q<TextField>("dialogue-field");
            textField.RegisterValueChangedCallback(evt =>
            {
                dialogueText = evt.newValue;
            });
            textField.value = dialogueText;

            RefreshExpandedState();
            RefreshPorts();
        }

        private string dialogueText;
        public string DialogueText
        {
            get => dialogueText;
            set
            {
                dialogueText = value;

                var textField = this.Q<TextField>("dialogue-field");
                textField.value = value;
            }
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
            generatedPort.contentContainer.Add(new Label(" "));
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
