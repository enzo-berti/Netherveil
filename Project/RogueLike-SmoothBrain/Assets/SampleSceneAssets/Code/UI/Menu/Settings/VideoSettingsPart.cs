using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Collections.Generic;

public class VideoSettingsPart : MenuPart
{
    private Resolution[] resolutions;

    [Header("Video Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Toggle screenShakeToggle;

    #region INITIALISATION
    private void Start()
    {
        InitDropdownResolution();
        InitDropdownScreenMode();
        InitDropdownQuality();
        InitToggleVsync();
        InitToggleCameraShaking();
        InitSliderBrightness();
    }

    private void InitDropdownResolution()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        // List all resolution available
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

    private void InitDropdownScreenMode()
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

    private void InitDropdownQuality()
    {
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    private void InitToggleVsync()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
    }

    private void InitToggleCameraShaking()
    {
        screenShakeToggle.isOn = CameraUtilities.toggleScreenShake;
    }

    private void InitSliderBrightness()
    {
        if (SettingsManager.Instance.GetComponent<Volume>().profile.TryGet(out LiftGammaGain LFG))
        {
            brightnessSlider.value = LFG.gamma.value.w;
        }
    }
    #endregion

    #region UI_EVENTS
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
    }

    public void SetDisplayMode(int displayIndex)
    {
        switch (displayIndex)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            default:
                Debug.LogWarning("This window mode don't exist !");
                break;
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void ToggleVsync(bool toggle)
    {
        QualitySettings.vSyncCount = toggle ? 1 : 0;
    }

    public void ToggleScreenShake(bool toggle)
    {
        CameraUtilities.toggleScreenShake = toggle;
    }

    public void ChangeBrightness(float value)
    {
        if (SettingsManager.Instance.GetComponent<Volume>().profile.TryGet(out LiftGammaGain LFG))
        {
            LFG.gamma.Override(new Vector4(1f, 1f, 1f, value));
        }
    }
    #endregion
}
