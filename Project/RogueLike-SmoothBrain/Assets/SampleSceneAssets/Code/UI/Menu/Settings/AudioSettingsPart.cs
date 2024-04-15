using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsPart : MenuPart
{
    [Header("Audio Settings")]
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider AmbienceVolumeSlider;
    [SerializeField] private Slider MusicVolumeSlider;

    private void Start()
    {
        mainVolumeSlider.value = AudioManager.Instance.masterVolumeBarValue;
        SFXVolumeSlider.value = AudioManager.Instance.soundsFXVolumeBarValue;
        MusicVolumeSlider.value = AudioManager.Instance.musicVolumeBarValue;
        AmbienceVolumeSlider.value = AudioManager.Instance.ambiencesVolumeBarValue;
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
