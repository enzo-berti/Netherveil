using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.Runtime;

namespace DialogueSystem.Editor
{
    public class DialogueTreeEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private DialogueGraphView graphView;

        private ToolbarMenu assetMenu;

        [MenuItem("Tools/Dialogues/Editor")]
        public static void OpenWindow()
        {
            DialogueTreeEditor wnd = GetWindow<DialogueTreeEditor>();
            wnd.titleContent = new GUIContent("Dialogue Editor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/Dialogs/Editor/DialogueTreeEditor.uxml");
            visualTree.CloneTree(root);

            // Instantiate UXML
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/Dialogs/Editor/DialogueTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            // Querry
            graphView = root.Q<DialogueGraphView>();
            graphView.AddSearchWindow(this);

            assetMenu = root.Q<ToolbarMenu>("toolbar-assets");

            assetMenu.menu.AppendAction("Save file", action => Save());
            assetMenu.menu.AppendAction("Load file", action => Load());

            // Generate MiniMap
            var miniMap = new MiniMap { anchored = false };
            var cords = new Vector2(position.width - 210, 0);
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            graphView.Add(miniMap);

            // Generate BlackBoard
            var blackboard = new Blackboard(graphView);
            blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
            blackboard.addItemRequested = _blackboard => { graphView.AddPropertyToBlackBoard(new ExposedProperty()); };
            blackboard.editTextRequested = (blackboard1, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField)element).text;
                if (graphView.exposedProperties.Any(x => x.propertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one!", "OK");
                    return;
                }

                var propertyIndex = graphView.exposedProperties.FindIndex(x => x.propertyName == oldPropertyName);
                graphView.exposedProperties[propertyIndex].propertyValue = newValue;

                ((BlackboardField)element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            graphView.Add(blackboard);
            graphView.blackboard = blackboard;
        }

        private void Save()
        {
            var saveUtility = GraphSaveUtility.GetInstance(graphView);
            saveUtility.SaveGraph();
        }

        private void Load()
        {
            var saveUtility = GraphSaveUtility.GetInstance(graphView);
            saveUtility.LoadGraph();
        }
    }
}
