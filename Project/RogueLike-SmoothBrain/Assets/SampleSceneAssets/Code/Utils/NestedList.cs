using System.Collections.Generic;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
#endif

//used to serialize list of lists, so that you can have List<NestedList<T>> and be serialized in inspector.
[System.Serializable]
public class NestedList<T>
{
    public List<T> data;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NestedList<>))]
public class NestedListDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var dataField = new PropertyField(property.FindPropertyRelative("data"));
        container.Add(dataField);

        return container;
    }
}
#endif
