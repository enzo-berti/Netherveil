using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolNathanEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Tools/Nathan")]
    public static void OpenWindow()
    {
        ToolNathanEditor wnd = GetWindow<ToolNathanEditor>();
        wnd.titleContent = new GUIContent("Tool Nathan Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        root.Q<Button>("ReplaceMesh").clicked += () => Replace();
        root.Q<Button>("DeleteVariant").clicked += () => Delete();
    }

    public void Replace()
    {
        GameObject[] objects = Selection.gameObjects;
        foreach (var item in objects)
        {
            item.name = item.name.Replace("MESH", "PREFAB");
        }
    }

    public void Delete()
    {
        GameObject[] objects = Selection.gameObjects;
        ;
        foreach (var item in objects)
        {
            Debug.Log(item.name);
            item.name = item.name.Replace(" Variant", "");
        }
    }
}
