using MeshUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MeshButton[] meshButtons;
    [SerializeField] private Selectable selectable;
    public void StartGame()
    {
        LevelLoader.current.LoadScene("InGame");
    }
    private void Start()
    {
        DeviceManager.OnChangedToGamepad += SetSelect;
        DeviceManager.OnChangedToKB += SetUnselect;
    }
    public void Quit()
    {
        DeviceManager.OnChangedToGamepad -= SetSelect;
        DeviceManager.OnChangedToKB -= SetUnselect;
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
        if(enable)
        {
            DeviceManager.OnChangedToGamepad += SetSelect;
            DeviceManager.OnChangedToKB += SetUnselect;
        }
        else
        {
            DeviceManager.OnChangedToGamepad -= SetSelect;
            DeviceManager.OnChangedToKB -= SetUnselect;
        }
    }

    private void SetEnableAllMeshButton(bool enable)
    {
        foreach (MeshButton meshButton in meshButtons)
        {
            meshButton.enabled = enable;
        }
    }

    private void SetSelect()
    {
        EventSystem.current.SetSelectedGameObject(selectable.gameObject);
    }
    private void SetUnselect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
