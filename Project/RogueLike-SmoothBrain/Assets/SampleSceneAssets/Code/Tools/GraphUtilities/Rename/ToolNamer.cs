using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolNamer : EditorWindow
{
    private GroupBox replaceBox;
    private GroupBox deleteBox;
    private TextField replaceFromField;
    private TextField replaceToField;
    private TextField deleteField;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Tools/Graphs Utilities/Namer", priority = 10)]
    public static void OpenWindow()
    {
        ToolNamer wnd = GetWindow<ToolNamer>();
        wnd.titleContent = new GUIContent("Tool Nathan Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        root.Q<Button>("Replace-Toolbar-Button").clicked += () => DisplayReplace();
        root.Q<Button>("Delete-Toolbar-Button").clicked += () => DisplayDelete();

        root.Q<Button>("Replace-Button").clicked += () => Replace();
        root.Q<Button>("Delete-Button").clicked += () => Delete();

        replaceBox = root.Q<GroupBox>("ReplaceBox");
        deleteBox = root.Q<GroupBox>("DeleteBox");

        replaceFromField = root.Q<TextField>("Replace-From");
        replaceToField = root.Q<TextField>("Replace-To");
        deleteField = root.Q<TextField>("Delete-Word");
    }

    public void DisplayReplace()
    {
        replaceBox.style.display = DisplayStyle.Flex;
        deleteBox.style.display = DisplayStyle.None;
    }

    public void DisplayDelete()
    {
        replaceBox.style.display = DisplayStyle.None;
        deleteBox.style.display = DisplayStyle.Flex;
    }

    public void Replace()
    {
        GameObject[] objects = Selection.gameObjects;
        foreach (var item in objects)
        {
            item.name = item.name.Replace(replaceFromField.value, replaceToField.value);
        }
    }

    public void Delete()
    {
        GameObject[] objects = Selection.gameObjects;
        foreach (var item in objects)
        {
            item.name = item.name.Replace(deleteField.value, "");
        }
    }
}
