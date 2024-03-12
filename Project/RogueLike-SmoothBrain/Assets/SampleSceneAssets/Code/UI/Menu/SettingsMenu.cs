using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsMenu : MenuHandler
{
    Resolution[] resolutions;

    [Header("Video Settings")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown displayModeDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Toggle vSyncToggle;
    [SerializeField] Toggle screenShakeToggle;

    [Header("Audio Settings")]
    [SerializeField] Slider mainVolumeSlider;
    [SerializeField] Slider SFXVolumeSlider;
    [SerializeField] Slider AmbienceVolumeSlider;
    [SerializeField] Slider MusicVolumeSlider;

    [Header("Control Settings")]
    [SerializeField] Slider deadzoneMin;
    [SerializeField] Slider deadzoneMax;
    [SerializeField] Toggle vibrationsToggle;

    private void Start()
    {
        DefaultVideoSettings();
        DefaultControlSettings();
        DefaultAudioSettings();
        DeviceManager.OnChangedToKB += ToggleGamepadSettings;
        DeviceManager.OnChangedToGamepad += ToggleGamepadSettings;
    }

    private void OnDestroy()
    {
        DeviceManager.OnChangedToKB -= ToggleGamepadSettings;
        DeviceManager.OnChangedToGamepad -= ToggleGamepadSettings;
    }

    private void ToggleGamepadSettings()
    {
        bool toggle = !DeviceManager.Instance.IsPlayingKB();

        deadzoneMin.gameObject.SetActive(toggle);
        deadzoneMax.gameObject.SetActive(toggle);
        vibrationsToggle.gameObject.SetActive(toggle);
    }

    private void DefaultAudioSettings()
    {
        mainVolumeSlider.value = AudioManager.Instance.masterVolumeBarValue;
        SFXVolumeSlider.value = AudioManager.Instance.soundsFXVolumeBarValue;
        MusicVolumeSlider.value = AudioManager.Instance.musicVolumeBarValue;
        AmbienceVolumeSlider.value = AudioManager.Instance.ambiencesVolumeBarValue;
    }

    private void DefaultControlSettings()
    {
        deadzoneMin.value = InputSystem.settings.defaultDeadzoneMin;
        deadzoneMax.value = InputSystem.settings.defaultDeadzoneMax;
        vibrationsToggle.isOn = DeviceManager.Instance.toggleVibrations;
    }

    private void DefaultVideoSettings()
    {
        SetResolutionDropdown();
        SetDefaultScreenMode();

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        if (SettingsManager.Instance.GetComponent<Volume>().profile.TryGet(out LiftGammaGain LFG))
        {
            brightnessSlider.value = LFG.gamma.value.w - 1f;
        }

        screenShakeToggle.isOn = CameraUtilities.toggleScreenShake;
    }

    private void SetResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new();
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

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
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
        if (displayIndex == 2)
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
