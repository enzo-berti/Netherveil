using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenu : MenuHandler
{
    public VideoSettingsPart VideoSettingsPart => menuItems.First(x => x.GetComponent<VideoSettingsPart>()) as VideoSettingsPart;
    public AudioSettingsPart AudioSettingsPart => menuItems.First(x => x.GetComponent<AudioSettingsPart>()) as AudioSettingsPart;
    public ControlsSettingsPart ControlsSettingsPart => menuItems.First(x => x.GetComponent<ControlsSettingsPart>()) as ControlsSettingsPart;
    [SerializeField] private Selectable selectable;

    private void Start()
    {
        OpenMenu(0);
    }

    public void EnableSettingsMenu(bool enable)
    {
        if(enable)
        {
            DeviceManager.OnChangedToGamepad += Select;
            DeviceManager.OnChangedToKB += UnSelect;
        }
        else
        {
            DeviceManager.OnChangedToGamepad -= Select;
            DeviceManager.OnChangedToKB -= UnSelect;
        }
    }

    private void Select()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(selectable.gameObject);
    }
    private void UnSelect()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void SetSelectable(Selectable toSet)
    {
        selectable = toSet;
    }
}
