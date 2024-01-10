using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace DialogueSystem.Editor.Nodes
{
    public abstract class Node : UnityEditor.Experimental.GraphView.Node
    {
        public Node(GraphView graphView)
            : this("UXML/GraphView/Node.uxml", graphView)
        {
            UseDefaultStyling();
        }

        public Node(string uiFile, GraphView graphView)
            : base(uiFile)
        {
            GUID = Guid.NewGuid().ToString();
            this.graphView = graphView;
        }

        public string GUID;
        public GraphView graphView;

        public Port AddPort(Direction portDirection, Port.Capacity capacity = Port.Capacity.Single, string portName = "")
        {
            Port newPort = InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
            newPort.portName = portName;

            switch (portDirection)
            {
                case Direction.Input:
                    inputContainer.Add(newPort);
                    break;
                case Direction.Output:
                    outputContainer.Add(newPort);
                    break;
            }

            return newPort;
        }
    }
}
