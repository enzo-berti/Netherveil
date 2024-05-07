using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{

    public EventReference AllMyTearsMusic;
    [SerializeField] ParticleSystem MusicNote;
    bool IsCollide;
    bool IsMusicPlaying = false;
    EventInstance eventMusic;

    void Start()
    {
        MusicNote.Pause();
        EVENT_CALLBACK MusicEndCallbackDelegate = new EVENT_CALLBACK(FMODCallback);
    }

    private void OnTriggerEnter(Collider collide)
    {
        if (collide.tag == "Player")
        {
            IsCollide = true;
        }
    }


    void Update()
    {
        IsMusicPlaying = eventMusic.isValid();

        if (Input.GetKeyUp(KeyCode.E) && IsCollide == true && IsMusicPlaying == false) 
        {
            playMusic();
        }
    }

    void playMusic()
    {
        eventMusic = AudioManager.Instance.PlaySound(AllMyTearsMusic);
        MusicNote.Play();
        IsMusicPlaying = true;
        eventMusic.setCallback(FMODCallback, EVENT_CALLBACK_TYPE.STOPPED);
    }

    private FMOD.RESULT FMODCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        // Cast the instance pointer to EventInstance
        EventInstance instance = new EventInstance(instancePtr);

        if (type == EVENT_CALLBACK_TYPE.STOPPED)
        {
            Debug.Log("Sound finished playing!");
            MusicNote.Stop();
            IsMusicPlaying = false;
        }
        instance.release();
        return FMOD.RESULT.OK;
    }


}
