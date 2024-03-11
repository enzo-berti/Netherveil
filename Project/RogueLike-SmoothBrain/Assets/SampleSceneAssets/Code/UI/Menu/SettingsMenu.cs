using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MenuHandler
{
    public void ToggleVsync()
    {
        bool isVSyncEnabled = QualitySettings.vSyncCount > 0;
        QualitySettings.vSyncCount = isVSyncEnabled ? 0 : 1;
    }

    public void ToggleVibrations()
    {
        DeviceManager.Instance.toggleVibrations = !DeviceManager.Instance.toggleVibrations;
    }

    public void Borderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void Windowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    public void Fullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    public void ChangeStickDeadzoneMin(float value)
    {
        InputSystem.settings.defaultDeadzoneMin = value;
    }

    public void ChangeStickDeadzoneMax(float value)
    {
        InputSystem.settings.defaultDeadzoneMax = value;
    }
}
