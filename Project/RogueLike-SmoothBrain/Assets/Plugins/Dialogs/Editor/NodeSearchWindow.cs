using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView graphView;
        private EditorWindow window;
        private Texture2D indentationIcon;

        public void Init(EditorWindow window, DialogueGraphView graphView)
        {
            this.window = window;
            this.graphView = graphView;

            // Intendation hack for search window as a transparent icon
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),

                new SearchTreeEntry(new GUIContent("Dialogue Node", indentationIcon))
                {
                    userData = new Nodes.DialogueNode(graphView),
                    level = 2
                },
            };
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, 
                context.screenMousePosition - window.position.position);
            var localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);

            switch (SearchTreeEntry.userData)
            {
                case Nodes.DialogueNode dialogueNode:
                    graphView.CreateNode(typeof(Nodes.DialogueNode), localMousePosition);
                    return true;
                default:
                    return false;
            }
        }
    }
}
