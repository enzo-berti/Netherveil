using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
    [Serializable]
    public class Sound
    {
        public EventReference reference;
        public EventInstance instance;

        public PLAYBACK_STATE GetState()
        {
            PLAYBACK_STATE state;

            instance.getPlaybackState(out state);

            return state;
        }

        public void CreateInstance(bool restart = false)
        {
            if (GetState() == PLAYBACK_STATE.STOPPED || restart)
            {
                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance = RuntimeManager.CreateInstance(reference);
            }
        }
    }

//#if UNITY_EDITOR
//    [CustomPropertyDrawer(typeof(Sound))]
//    public class SoundDrawerUIE : PropertyDrawer
//    {
//        int nbMember = 0;
//        SerializedProperty referenceProperty;
//        SerializedProperty nameProperty;

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            referenceProperty = property.FindPropertyRelative("reference");
//            nameProperty = property.FindPropertyRelative("name");
//            nbMember = 0;
//            string labelText = nameProperty.stringValue;
//            EditorGUI.BeginProperty(position, label, property);

//            DrawMember(position, referenceProperty, labelText);

//            EditorGUI.EndProperty();
//        }
//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            int totalLine = 1;
//            if (property.isExpanded)
//            {
//                totalLine += 1;

//            }
//            return EditorGUIUtility.singleLineHeight * totalLine;
//        }

//        private void DrawMember(Rect position, SerializedProperty propertyToDraw, string labelText)
//        {
//            nbMember++;
//            EditorGUI.indentLevel++;
//            float posX = position.min.x;
//            float posY = position.min.y + EditorGUIUtility.singleLineHeight * nbMember;
//            float width = position.size.x;
//            float height = EditorGUIUtility.singleLineHeight;

//            Rect drawArea = new Rect(posX, posY, width, height);
//            EditorGUI.PropertyField(drawArea, propertyToDraw, new GUIContent(labelText));
//            EditorGUI.indentLevel--;
//        }
//    }
//#endif


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
    public List<EventInstance> buttonSFXInstances = new List<EventInstance>();

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
        UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>(true); // parameter makes it include inactive UI elements with buttons
        foreach (UnityEngine.UI.Button b in buttons)
        {
            b.onClick.AddListener(ButtonClickSFX);
            b.AddComponent<CustomEventTrigger>();
        }
    }

    private void Update()
    {
        for (int i = audioInstance.Count - 1; i >= 0; --i)
        {
            audioInstance[i].getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.STOPPED)
                audioInstance.Remove(audioInstance[i]);
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

    public EventInstance PlaySound(string path, Vector3 worldPosition)
    {
        EventInstance result = PlaySound(path);
        result.set3DAttributes(worldPosition.To3DAttributes());

        return result;
    }

    public EventInstance PlaySound(Sound sound, Vector3 worldPosition, bool restart = false)
    {
        sound.CreateInstance(restart);

        if (sound.GetState() == PLAYBACK_STATE.STOPPED)
        {
            sound.instance.start();
            sound.instance.set3DAttributes(worldPosition.To3DAttributes());
            audioInstance.Add(sound.instance);
        }

        return sound.instance;
    }

    public EventInstance PlaySound(Sound sound, bool restart = false)
    {
        sound.CreateInstance(restart);

        if (sound.GetState() == PLAYBACK_STATE.STOPPED)
        {
            sound.instance.start();
            audioInstance.Add(sound.instance);
        }

        return sound.instance;
    }

    public EventInstance PlaySound(EventReference reference)
    {
        EventInstance result = RuntimeManager.CreateInstance(reference);
        result.start();
        audioInstance.Add(result);

        return result;
    }

    public EventInstance PlaySound(EventReference reference, Vector3 worldPosition)
    {
        EventInstance result = PlaySound(reference);
        result.set3DAttributes(worldPosition.To3DAttributes());
        audioInstance.Add(result);

        return result;
    }

    public void StopAllSounds(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.Immediate)
    {
        for (int i = audioInstance.Count - 1; i >= 0; --i)
        {
            audioInstance[i].stop(stopMode);
            audioInstance.RemoveAt(i);
        }
    }

    public void StopSound(EventInstance eventInstance, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.Immediate)
    {
        if (!audioInstance.Contains(eventInstance))
            return;

        eventInstance.stop(stopMode);
        audioInstance.Remove(eventInstance);
    }
    public void StopSound(Sound sound, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.Immediate)
    {
        if (!audioInstance.Contains(sound.instance))
            return;

        sound.instance.stop(stopMode);
        audioInstance.Remove(sound.instance);
    }

    public void ButtonClickSFX()
    {
        for (int i = buttonSFXInstances.Count - 1; i >= 0; --i)
        {
            buttonSFXInstances[i].stop(FMOD.Studio.STOP_MODE.Immediate);
            buttonSFXInstances.RemoveAt(i);
        }

        buttonSFXInstances.Add(Instance.PlaySound(buttonClick));
    }
}