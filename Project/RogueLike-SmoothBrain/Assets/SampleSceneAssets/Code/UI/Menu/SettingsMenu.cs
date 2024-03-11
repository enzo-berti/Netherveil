using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsMenu : MenuHandler
{
    Resolution[] resolutions;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown displayModeDropdown;

    private void Start()
    {
        SetResolutionDropdown();
        SetDefaultScreenMode();
    }

    private void SetResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio + "hz");

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        //set default resolution
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void SetDefaultScreenMode()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                displayModeDropdown.value = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                displayModeDropdown.value = 1;
                break;
            case FullScreenMode.Windowed:
                displayModeDropdown.value = 2;
                break;
            default:
                break;
        }
        displayModeDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
    }

    public void ToggleVsync(bool toggle)
    {
        QualitySettings.vSyncCount = toggle ? 1 : 0;
    }

    public void ToggleVibrations(bool toggle)
    {
        DeviceManager.Instance.toggleVibrations = toggle;
    }

    public void ToggleScreenShake(bool toggle)
    {
        CameraUtilities.toggleScreenShake = toggle;
    }

    public void SetDisplayMode(int displayIndex)
    {
        if(displayIndex == 2)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            return;
        }

        Screen.fullScreenMode = (FullScreenMode)displayIndex;
    }

    public void ChangeStickDeadzoneMin(float value)
    {
        InputSystem.settings.defaultDeadzoneMin = value;
    }

    public void ChangeStickDeadzoneMax(float value)
    {
        InputSystem.settings.defaultDeadzoneMax = value;
    }

    public void ChangeBrightness(float value)
    {
        if (SettingsManager.Instance.GetComponent<Volume>().profile.TryGet(out LiftGammaGain LFG))
        {
            LFG.gamma.Override(new Vector4(LFG.gamma.value.x, LFG.gamma.value.y, LFG.gamma.value.z, value - 1f));
        }
    }

    public void ChangeMainVolume(float value)
    {
        AudioManager.Instance.masterVolumeBarValue = value;
    }

    public void ChangeSFXVolume(float value)
    {
        AudioManager.Instance.soundsFXVolumeBarValue = value;
    }

    public void ChangeMusicVolume(float value)
    {
        AudioManager.Instance.musicVolumeBarValue = value;
    }

    public void ChangeAmbienceVolume(float value)
    {
        AudioManager.Instance.ambiencesVolumeBarValue = value;
    }
}
