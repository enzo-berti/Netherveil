using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomEventTrigger : EventTrigger
{
    public static EventReference buttonSelectSFX;
    public override void OnSelect(BaseEventData data)
    {
        AudioManager.Instance.PlaySound(buttonSelectSFX);
    }
}

public class AudioManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadAudioManager()
    {
        _ = Instance;
    }

    private static AudioManager instance = null;
    public static AudioManager Instance
    {
        get 
        { 
            if (instance == null)
            {
                Instantiate(Resources.Load<GameObject>(nameof(AudioManager)));
            }

            return instance; 
        }
    }

    [Range(0, 1)] public float masterVolumeBarValue = 1f;
    [Range(0, 1)] public float musicVolumeBarValue = 1f;
    [Range(0, 1)] public float soundsFXVolumeBarValue = 1f;
    [Range(0, 1)] public float ambiencesVolumeBarValue = 1f;

    [SerializeField] private EventReference buttonClick;
    [SerializeField] private EventReference buttonSelect;

    private List<EventInstance> audioInstance = new List<EventInstance>();

    private Bus masterBus;
    private Bus musicsBus;
    private Bus soundsFXBus;
    private Bus ambiencesBus;

    public Bus MasterBus { get => masterBus; }
    public Bus MusicsBus { get => musicsBus; }
    public Bus SoundsFXBus { get => soundsFXBus; }
    public Bus AmbiencesBus { get => ambiencesBus; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadBuses();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        CustomEventTrigger.buttonSelectSFX = buttonSelect;
        Button[] buttons = FindObjectsOfType<Button>(true); // parameter makes it include inactive UI elements with buttons
        foreach (Button b in buttons)
        {
            b.onClick.AddListener(ButtonClickSFX);
            b.AddComponent<CustomEventTrigger>();
        }
    }

    private void Update()
    {
        foreach (var audio in audioInstance)
        {
            audio.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.STOPPED)
                audioInstance.Remove(audio);
        }
    }

    private void LoadBuses()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicsBus = RuntimeManager.GetBus("bus:/Musics");
        soundsFXBus = RuntimeManager.GetBus("bus:/SoundsFX");
        ambiencesBus = RuntimeManager.GetBus("bus:/Ambiences");

        SetBusVolumes();
    }

    private void SetBusVolumes()
    {
        SetBusVolume(masterBus, masterVolumeBarValue);
        SetBusVolume(musicsBus, musicVolumeBarValue);
        SetBusVolume(soundsFXBus, soundsFXVolumeBarValue);
        SetBusVolume(ambiencesBus, ambiencesVolumeBarValue);
    }

    private void SetBusVolume(Bus bus, float volume)
    {
        if (bus.isValid())
        {
            bus.setVolume(Mathf.Clamp01(volume));
        }
        else
        {
            Debug.LogWarning("Attempted to set volume on a null bus.");
        }
    }

    public EventInstance PlaySound(string path)
    {
        EventInstance result = RuntimeManager.CreateInstance(path);
        result.start();
        audioInstance.Add(result);

        return result;
    }

    public EventInstance PlaySound(EventReference reference)
    {
        EventInstance result = RuntimeManager.CreateInstance(reference);
        result.start();
        audioInstance.Add(result);

        return result;
    }

    public void StopAllSounds(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.Immediate)
    {
        foreach (var audio in audioInstance)
        {
            audio.stop(stopMode);
            audioInstance.Remove(audio);
        }
    }

    public void StopSound(EventInstance eventInstance, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.Immediate)
    {
        if (!audioInstance.Contains(eventInstance))
            return;

        eventInstance.stop(stopMode);
        audioInstance.Remove(eventInstance);
    }

    public void ButtonClickSFX()
    {
        Instance.PlaySound(buttonClick);
    }
}
