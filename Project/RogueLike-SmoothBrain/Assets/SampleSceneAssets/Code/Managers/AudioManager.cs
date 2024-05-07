using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomEventTrigger : EventTrigger
{
    public static EventReference buttonSelectSFX;
    public override void OnSelect(BaseEventData data)
    {
        //AudioManager.Instance.buttonSFXInstances.Add(AudioManager.Instance.PlaySound(buttonSelectSFX));
    }
}

public class AudioManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadAudioManager()
    {
        _ = Instance;
    }

    [Header("GlobalSounds")]
    [SerializeField] EventReference questObtainedSFX;
    [SerializeField] EventReference questFinishedSFX;
    [SerializeField] EventReference questLostSFX;
    [SerializeField] EventReference lostLevelSFX;
    [SerializeField] EventReference gainLevelBenedictionSFX;
    [SerializeField] EventReference gainLevelCorruptionSFX;
    [SerializeField] EventReference hornOfBarbatosSFX;
    [SerializeField] EventReference pickUpCollectibleSFX;
    [SerializeField] EventReference pickUpItemSFX;
    [SerializeField] EventReference thunderstrikeSFX;
    [SerializeField] EventReference thunderstrike2SFX;
    [SerializeField] EventReference thunderstrike3SFX;
    [SerializeField] EventReference thunderlinkSFX;
    [SerializeField] EventReference gateOpenSFX;
    [SerializeField] EventReference gateCloseSFX;
    [SerializeField] EventReference allMyTearsMusic;
    [SerializeField] EventReference itemBuySFX;
    [SerializeField] EventReference damnationVeilSFX;
    [SerializeField] EventReference divineShieldSFX;
    [SerializeField] EventReference dashShieldSFX;
    [SerializeField] EventReference thornChestSFX;
    [SerializeField] EventReference runeOfSlothSFX;
    [SerializeField] EventReference runeOfEnvySFX;

    public EventReference QuestObtainedSFX { get => questObtainedSFX; }
    public EventReference QuestFinishedSFX { get => questFinishedSFX; }
    public EventReference QuestLostSFX { get => questLostSFX; }
    public EventReference LostLevelSFX { get => lostLevelSFX; }
    public EventReference GainLevelBenedictionSFX { get => gainLevelBenedictionSFX; }
    public EventReference GainLevelCorruptionSFX { get => gainLevelCorruptionSFX; }
    public EventReference HornOfBarbatosSFX { get => hornOfBarbatosSFX; }
    public EventReference PickUpCollectibleSFX { get => pickUpCollectibleSFX; }
    public EventReference PickUpItemSFX { get => pickUpItemSFX; }
    public EventReference ThunderstrikeSFX { get => thunderstrikeSFX; }
    public EventReference Thunderstrike2SFX { get => thunderstrike2SFX; }
    public EventReference Thunderstrike3SFX { get => thunderstrike3SFX; }
    public EventReference ThunderlinkSFX { get => thunderlinkSFX; }
    public EventReference GateOpenSFX { get => gateOpenSFX; }
    public EventReference GateCloseSFX { get => gateCloseSFX; }
    public EventReference AllMyTearsMusic { get => allMyTearsMusic; }
    public EventReference ItemBuySFX { get => itemBuySFX; }
    public EventReference DamnationVeilSFX { get => damnationVeilSFX; }
    public EventReference DivineShieldSFX { get => divineShieldSFX; }
    public EventReference DashShieldSFX { get => dashShieldSFX; }
    public EventReference ThornChestSFX { get => thornChestSFX; }
    public EventReference RuneOfSlothSFX { get => runeOfSlothSFX; }
    public EventReference RuneOfEnvySFX { get => runeOfEnvySFX; }

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

    public float masterVolumeBarValue
    {
        get => GetBusVolume(masterBus);
        set => SetBusVolume(masterBus, value);
    }
    public float musicVolumeBarValue
    {
        get => GetBusVolume(musicsBus);
        set => SetBusVolume(musicsBus, value);
    }
    public float soundsFXVolumeBarValue
    {
        get => GetBusVolume(soundsFXBus);
        set => SetBusVolume(soundsFXBus, value);
    }
    public float ambiencesVolumeBarValue
    {
        get => GetBusVolume(ambiencesBus);
        set => SetBusVolume(ambiencesBus, value);
    }

    [SerializeField] private EventReference buttonClick;
    [SerializeField] private EventReference buttonSelect;

    private readonly List<EventInstance> audioInstances = new List<EventInstance>();
    public readonly List<EventInstance> buttonSFXInstances = new List<EventInstance>();

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
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadBuses();
    }

    void Start()
    {
        CustomEventTrigger.buttonSelectSFX = buttonSelect;
        UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>(true); // parameter makes it include inactive UI elements with buttons
        foreach (UnityEngine.UI.Button b in buttons)
        {
            b.onClick.AddListener(ButtonClickSFX);
            b.AddComponent<CustomEventTrigger>();
        }
    }

    private void Update()
    {
        for (int i = audioInstances.Count - 1; i >= 0; --i)
        {
            audioInstances[i].getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.STOPPED)
                audioInstances.Remove(audioInstances[i]);
        }
    }

    private void LoadBuses()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicsBus = RuntimeManager.GetBus("bus:/Musics");
        soundsFXBus = RuntimeManager.GetBus("bus:/SoundsFX");
        ambiencesBus = RuntimeManager.GetBus("bus:/Ambiences");

        masterVolumeBarValue = 0.5f;
        musicVolumeBarValue = 0.5f;
        soundsFXVolumeBarValue = 0.5f;
        ambiencesVolumeBarValue = 0.5f;
    }

    private float GetBusVolume(Bus bus)
    {
        if (bus.isValid())
        {
            bus.getVolume(out float result);
            return result;
        }
        else
        {
            throw new Exception("Attempted to set volume on a null bus.");
        }
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

    public void AddSound(ref EventInstance instance)
    {
        audioInstances.Add(instance);
    }

    public bool RemoveSound(ref EventInstance instance)
    {
        return audioInstances.Remove(instance);
    }

    public EventInstance PlaySound(string path)
    {
        EventInstance result = RuntimeManager.CreateInstance(path);
        result.start();
        audioInstances.Add(result);

        return result;
    }

    public EventInstance PlaySound(string path, Vector3 worldPosition)
    {
        EventInstance result = PlaySound(path);
        result.set3DAttributes(worldPosition.To3DAttributes());
        audioInstances.Add(result);

        return result;
    }

    public EventInstance PlaySound(EventReference reference)
    {
        EventInstance result = RuntimeManager.CreateInstance(reference);
        result.start();
        audioInstances.Add(result);

        return result;
    }

    public EventInstance PlaySound(EventReference reference, Vector3 worldPosition)
    {
        EventInstance result = PlaySound(reference);
        result.set3DAttributes(worldPosition.To3DAttributes());
        audioInstances.Add(result);

        return result;
    }

    public EventInstance PlayThunders(Vector3 worldPosition)
    {
        int randomThunder = 0;
        EventReference reference = ThunderstrikeSFX;
        randomThunder = UnityEngine.Random.Range(0, 3);

        switch (randomThunder)
        {
            case 0:
                reference = ThunderstrikeSFX;
                break;
            case 1:
                reference = Thunderstrike2SFX;
                break;
            case 2:
                reference = Thunderstrike3SFX;
                break;
        }

        EventInstance result = PlaySound(reference);
        result.set3DAttributes(worldPosition.To3DAttributes());
        audioInstances.Add(result);

        return result;
    }

    public void StopAllSounds(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.Immediate)
    {
        foreach (var audioInstance in audioInstances)
        {
            audioInstance.stop(stopMode);
        }

        audioInstances.Clear();
    }

    public void StopSound(EventInstance eventInstance, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.Immediate)
    {
        if (!audioInstances.Contains(eventInstance))
            return;

        eventInstance.stop(stopMode);
        audioInstances.Remove(eventInstance);
    }

    public void ButtonClickSFX()
    {
        foreach (var buttonSFXInstance in buttonSFXInstances)
        {
            buttonSFXInstance.stop(FMOD.Studio.STOP_MODE.Immediate);
        }
        buttonSFXInstances.Clear();

        buttonSFXInstances.Add(Instance.PlaySound(buttonClick));
    }
}