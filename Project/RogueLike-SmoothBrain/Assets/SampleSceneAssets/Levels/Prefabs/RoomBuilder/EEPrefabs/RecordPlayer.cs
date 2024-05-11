using FMOD.Studio;
using FMODUnity;
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
       
        if (Input.GetKeyUp(KeyCode.E) && IsCollide == true && !IsMusicPlaying) 
        {
            playMusic();
        }
        
        eventMusic.getPlaybackState(out PLAYBACK_STATE playbackState);
        IsMusicPlaying = playbackState == PLAYBACK_STATE.PLAYING;
        if (playbackState == PLAYBACK_STATE.STOPPING)
        {
            MusicNote.Stop();
        }
    }

    void playMusic()
    {
        eventMusic = AudioManager.Instance.PlaySound(AllMyTearsMusic);
        MusicNote.Play();
        eventMusic.getPlaybackState(out PLAYBACK_STATE playbackState);
        eventMusic.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        IsMusicPlaying = playbackState == PLAYBACK_STATE.PLAYING;
    }
}
