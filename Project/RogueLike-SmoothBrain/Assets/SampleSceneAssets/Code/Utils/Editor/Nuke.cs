using UnityEditor;
using UnityEngine;

public class Nuke : EditorWindow
{
    [UnityEditor.MenuItem("Tools/Nuke")]
    public static void CreateRoom()
    {
        EditorWindow.GetWindow(typeof(Nuke));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        if (GUILayout.Button("TU ES SUR ???"))
        {
            OHNON();
        }

        EditorGUILayout.EndVertical();
    }

    void OHNON()
    {
        switch (Random.Range(0, 10))
        {
            case 0:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/");
                break;
            case 1:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Art/");
                break;
            case 2:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/");
                break;
            case 3:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Inputs/");
                break;
            case 4:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Levels/");
                break;
            case 5:
                FileUtil.DeleteFileOrDirectory("Assets/TextMesh Pro/");
                break;
            case 6:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/DialogueSystem");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/Camera");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/Cinematics");
                break;
            case 7:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Art/Meshs");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Art/Animations");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Art/VFX");
                break;
            case 8:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/Trap");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/UI");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Art/Volumes");
                break;
            case 9:
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/Map");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Code/Utils");
                FileUtil.DeleteFileOrDirectory("Assets/SampleSceneAssets/Art/Animations");
                break;
            default:
                FileUtil.DeleteFileOrDirectory("Assets");
                break;
        }
    }
}
