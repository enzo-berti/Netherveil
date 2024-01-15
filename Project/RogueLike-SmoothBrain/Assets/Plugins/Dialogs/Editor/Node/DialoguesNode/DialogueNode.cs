using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Nodes
{
    public abstract class DialogueNode : Node
    {
        public DialogueNode(string uiFile, GraphView graphView)
            : base(uiFile, graphView)
        {
            nameText = "Joe";
            dialogueText = "Hello World !";

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
    }
}
