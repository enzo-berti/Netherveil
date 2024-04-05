using MeshUI;
using UnityEditor;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MeshButton[] meshButtons;
    private FloatingTextMainMenu[] floatingTexts;

    private void Start()
    {
        floatingTexts = FindObjectsOfType<FloatingTextMainMenu>();
    }

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
        SetFadeAllFloatingTexts(enable);
    }

    private void SetEnableAllMeshButton(bool enable)
    {
        foreach (MeshButton meshButton in meshButtons)
        {
            meshButton.enabled = enable;
        }
    }

    private void SetFadeAllFloatingTexts(bool enable)
    {
        foreach (FloatingTextMainMenu ft in floatingTexts)
        {
            ft.FadeFloatingTexts(enable);
        }
    }
}
