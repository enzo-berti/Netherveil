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
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
#else
        Application.Quit();
#endif
    }
    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject != null)
            Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        else
        {
            Debug.Log("null current");
        }
    }
    public void SetEnableMainMenu(bool enable)
    {
        SetEnableAllMeshButton(enable);
        if(enable)
        {
            Debug.Log("enable");
            DeviceManager.OnChangedToGamepad += SetSelect;
            DeviceManager.OnChangedToKB += SetUnselect;
        }
        else
        {
            Debug.Log("disable");
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
        if(EventSystem.current != null) EventSystem.current.SetSelectedGameObject(selectable.gameObject);
    }
    private void SetUnselect()
    {
        if(EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }
}
