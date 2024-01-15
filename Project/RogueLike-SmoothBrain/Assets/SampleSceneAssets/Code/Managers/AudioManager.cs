using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [HideInInspector] public FMOD.Studio.Bus masterBus;
    [HideInInspector] public FMOD.Studio.Bus musicsBus;
    [HideInInspector] public FMOD.Studio.Bus soundsFXBus;
    [HideInInspector] public FMOD.Studio.Bus ambiencesBus;

    [SerializeField, Range(0, 1)]
    public float masterVolumeBarValue;

    [SerializeField, Range(0, 1)]
    public float musicVolumeBarValue;

    [SerializeField, Range(0, 1)]
    public float soundsFXVolumeBarValue;

    [SerializeField, Range(0, 1)]
    public float ambiencesVolumeBarValue;

    [Header("Ambiences Sounds")]

    [SerializeField] EventReference menuPortalOpenEvent;
    [HideInInspector] public FMOD.Studio.EventInstance menuPortalOpenInstance;

    [SerializeField] EventReference menuPortalExplosionEvent;
    [HideInInspector] public FMOD.Studio.EventInstance menuPortalExplosionInstance;

    [SerializeField] EventReference menuPaperEvent;
    [HideInInspector] public FMOD.Studio.EventInstance menuPaperInstance;

    [SerializeField] EventReference menuCandleEvent;
    [HideInInspector] public FMOD.Studio.EventInstance menuCandleInstance;

    [SerializeField] EventReference menuWoodStairsEvent;
    [HideInInspector] public FMOD.Studio.EventInstance menuWoodStairsInstance;

    [Header("SoundFX sounds")]

    [SerializeField] EventReference buttonClickEvent;
    [HideInInspector] public FMOD.Studio.EventInstance buttonClickInstance;

    [SerializeField] EventReference buttonSelectEvent;
    [HideInInspector] public FMOD.Studio.EventInstance buttonSelectInstance;

    [SerializeField] EventReference swordSlash1Event;
    [HideInInspector] public FMOD.Studio.EventInstance swordSlash1Instance;

    [SerializeField] EventReference swordSlash2Event;
    [HideInInspector] public FMOD.Studio.EventInstance swordSlash2Instance;

    [SerializeField] EventReference swordSlash3Event;
    [HideInInspector] public FMOD.Studio.EventInstance swordSlash3Instance;

    [SerializeField] EventReference playerHitEvent;
    [HideInInspector] public FMOD.Studio.EventInstance playerHitInstance;

    [SerializeField] EventReference playerDashEvent;
    [HideInInspector] public FMOD.Studio.EventInstance playerDashInstance;

    [SerializeField] EventReference playerDeathEvent;
    [HideInInspector] public FMOD.Studio.EventInstance playerDeathInstance;

    [SerializeField] EventReference bloodVacuumingEvent;
    [HideInInspector] public FMOD.Studio.EventInstance bloodVacuumingInstance;

    [SerializeField] EventReference enemyDeathEvent;
    [HideInInspector] public FMOD.Studio.EventInstance enemyDeathInstance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadBus();
        LoadAmbienceSounds();
        LoadSoundsFX();
    }

    private void LoadBus()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        musicsBus = FMODUnity.RuntimeManager.GetBus("bus:/Musics");
        soundsFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SoundsFX");
        ambiencesBus = FMODUnity.RuntimeManager.GetBus("bus:/Ambiences");
        masterBus.setVolume(masterVolumeBarValue);
        musicsBus.setVolume(musicVolumeBarValue);
        soundsFXBus.setVolume(soundsFXVolumeBarValue);
        ambiencesBus.setVolume(ambiencesVolumeBarValue);
    }

    private void LoadAmbienceSounds()
    {
        menuPortalOpenInstance = FMODUnity.RuntimeManager.CreateInstance(menuPortalOpenEvent);
        menuPortalOpenInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        menuPortalExplosionInstance = FMODUnity.RuntimeManager.CreateInstance(menuPortalExplosionEvent);
        menuPortalExplosionInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        menuPaperInstance = FMODUnity.RuntimeManager.CreateInstance(menuPaperEvent);
        menuPaperInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        menuCandleInstance = FMODUnity.RuntimeManager.CreateInstance(menuCandleEvent);
        menuCandleInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        menuWoodStairsInstance = FMODUnity.RuntimeManager.CreateInstance(menuWoodStairsEvent);
        menuWoodStairsInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void LoadSoundsFX()
    {
        buttonClickInstance = FMODUnity.RuntimeManager.CreateInstance(buttonClickEvent);
        buttonClickInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        buttonSelectInstance = FMODUnity.RuntimeManager.CreateInstance(buttonSelectEvent);
        buttonSelectInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        swordSlash1Instance = FMODUnity.RuntimeManager.CreateInstance(swordSlash1Event);
        swordSlash1Instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        swordSlash2Instance = FMODUnity.RuntimeManager.CreateInstance(swordSlash2Event);
        swordSlash2Instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        
        swordSlash3Instance = FMODUnity.RuntimeManager.CreateInstance(swordSlash3Event);
        swordSlash3Instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        playerHitInstance = FMODUnity.RuntimeManager.CreateInstance(playerHitEvent);
        playerHitInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        playerDashInstance = FMODUnity.RuntimeManager.CreateInstance(playerDashEvent);
        playerDashInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        playerDeathInstance = FMODUnity.RuntimeManager.CreateInstance(playerDeathEvent);
        playerDeathInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        bloodVacuumingInstance = FMODUnity.RuntimeManager.CreateInstance(bloodVacuumingEvent);
        bloodVacuumingInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        enemyDeathInstance = FMODUnity.RuntimeManager.CreateInstance(enemyDeathEvent);
        enemyDeathInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }
}
