using MeshUI;
using UnityEditor;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MeshButton[] meshButtons;

    public void StartGame()
    {
        LevelLoader.current.LoadScene("InGame");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
#else
        Application.Quit();
#endif
    }

    public void SetEnableMainMenu(bool enable)
    {
        SetEnableAllMeshButton(enable);
    }

    private void SetEnableAllMeshButton(bool enable)
    {
        foreach (MeshButton meshButton in meshButtons)
        {
            meshButton.enabled = enable;
        }
    }
}
