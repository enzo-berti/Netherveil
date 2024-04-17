using DialogueSystem.Runtime;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class QuestDialogueNodeView : DialogueNodeView
    {
        public override Type type => typeof(QuestDialogueNode);

        protected string questTag;
        public string QuestTag
        {
            get => questTag;
            set
            {
                questTag = value;
                eventTagField.value = value;
            }
        }

        private TextField eventTagField;

        public QuestDialogueNodeView(GraphView graphView)
            : base("Assets/Plugins/DialogueSystem/Scripts/Editor/UIBuilder/QuestDialogueNodeView.uxml", graphView)
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/DialogueSystem/Scripts/Editor/UIBuilder/QuestDialogueNodeView.uss"));
            AddPort(Direction.Input, Port.Capacity.Single, "previous dialogue");
            AddPort(Direction.Output, Port.Capacity.Single, "next dialogue");
            title = "Event Dialogue";

            // Querry
            eventTagField = this.Q<TextField>("QuestTag-field");
            eventTagField.RegisterValueChangedCallback(evt => QuestTag = evt.newValue);
        }
    }
}
