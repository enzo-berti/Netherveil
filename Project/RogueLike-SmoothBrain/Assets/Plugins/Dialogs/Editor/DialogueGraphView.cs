using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.Runtime;
using DialogueSystem.Editor;
using DialogueSystem.Editor.Nodes;

public class DialogueGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> { }

    private NodeSearchWindow searchWindow;

    //public Blackboard blackboard;
    //public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

    public DialogueGraphView()
    {
        Insert(0, new GridBackground());

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/Dialogs/Editor/DialogueTreeEditor.uss");
        styleSheets.Add(styleSheet);

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        AddElement(new RootNode(this));
    }

    public void AddSearchWindow(EditorWindow editorWindow)
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    public DialogueSystem.Editor.Nodes.Node CreateNode(Type type, Vector2 position)
    {
        DialogueSystem.Editor.Nodes.Node newNode;

        if (type == typeof(DialogueNode))
        {
            newNode = new DialogueNode(this);
            newNode.SetPosition(new Rect(position, defaultNodeSize));
            AddElement(newNode);
            return newNode;
        }

        return null;
    }

    //public void ClearBlackBoardAndExposedProperties()
    //{
    //    exposedProperties.Clear();
    //    blackboard.Clear();
    //}

    //public void AddPropertyToBlackBoard(ExposedProperty exposedProperty)
    //{
    //    var localPropertyName = exposedProperty.propertyName;
    //    var localPropertyValue = exposedProperty.propertyValue;

    //    while (exposedProperties.Any(x => x.propertyName == localPropertyName))
    //        localPropertyName = $"{localPropertyName}(1)";

    //    var property = new ExposedProperty();
    //    property.propertyName = localPropertyName;
    //    property.propertyValue = exposedProperty.propertyValue;
    //    exposedProperties.Add(property);

    //    var container = new VisualElement();
    //    var blackboardField = new BlackboardField { text = property.propertyName, typeText = "string" };
    //    container.Add(blackboardField);

    //    var propertyValueTextField = new TextField("Value:")
    //    {
    //        value = localPropertyValue,
    //    };
    //    propertyValueTextField.RegisterValueChangedCallback(evt =>
    //    {
    //        var changingPropertyIndex = exposedProperties.FindIndex(x => x.propertyName == property.propertyName);
    //        exposedProperties[changingPropertyIndex].propertyValue = evt.newValue;
    //    });
    //    var blackBoardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
    //    container.Add(blackBoardValueRow);

    //    blackboard.Add(container);
    //}
}
