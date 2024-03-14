using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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

    public void PlaySound(string path)
    {
        RuntimeManager.PlayOneShot(path);
    }

    public void PlaySound(EventReference reference)
    {
        RuntimeManager.PlayOneShot(reference);
    }
}
