using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Nodes
{
    public class DialogueNode : Node
    {
        public DialogueNode(GraphView graphView)
            : base("Assets/Plugins/Dialogs/Editor/Node/DialoguesNode/DialogueNodeView.uxml", graphView)
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/Dialogs/Editor/Node/DialoguesNode/DialogueNodeView.uss"));

            title = "Choices Node";
            nameText = "Joe";
            dialogueText = "Hello World !";
            AddPort(Direction.Input, Port.Capacity.Multi, "previous dialogue");

            // Name field
            var nameTextField = this.Q<TextField>("name-field");
            nameTextField.RegisterValueChangedCallback(evt =>
            {
                nameText = evt.newValue;
            });
            nameTextField.value = nameText;

            // Dialogue field
            var dialogueTextField = this.Q<TextField>("dialogue-field");
            dialogueTextField.RegisterValueChangedCallback(evt =>
            {
                dialogueText = evt.newValue;
            });
            dialogueTextField.value = dialogueText;

            // Illustration field
            var illustrationTextField = this.Q<ObjectField>("illustration-field");
            illustrationTextField.RegisterValueChangedCallback(evt =>
            {
                illustrationSprite = (Sprite)evt.newValue;
            });

            // Type dropdown
            var typeDropdown = this.Q<DropdownField>("type-dropdown");
            typeDropdown.RegisterValueChangedCallback(evt =>
            {
                typeDialogue = evt.newValue;

                outputContainer.Clear();
                switch (typeDialogue)
                {
                    case "Text":
                        this.Q<Button>("button-new-choice").SetEnabled(false);
                        AddPort(Direction.Output, Port.Capacity.Single, "next dialogue");
                        break;
                    case "Choice":
                        this.Q<Button>("button-new-choice").SetEnabled(true);
                        break;
                    default:
                        break;
                }
            });

            var button = this.Q<Button>("button-new-choice");
            button.clickable.clicked += () => AddChoicePort();

            RefreshExpandedState();
            RefreshPorts();
        }

        protected string nameText;
        public string NameText
        {
            get => nameText;
            set
            {
                nameText = value;

                var textField = this.Q<TextField>("name-field");
                textField.value = value;
            }
        }

        protected string dialogueText;
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

        protected Sprite illustrationSprite;
        public Sprite IllustrationSprite
        {
            get => illustrationSprite;
            set
            {
                illustrationSprite = value;

                var objectField = this.Q<ObjectField>("illustration-field");
                objectField.value = value;
            }
        }

        protected string typeDialogue;
        public string TypeDialogue
        {
            get => typeDialogue;
            set
            {
                typeDialogue = value;

                var dropdown = this.Q<DropdownField>("type-dropdown");
                dropdown.value = value;
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
