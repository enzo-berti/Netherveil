using UnityEngine;
using UnityEngine.Playables;

public class BossCinematic : MonoBehaviour
{
    private PlayableDirector director;

    private void Awake()
    {
        director = GetComponentInChildren<PlayableDirector>();
    }

    private void OnEnable()
    {
        Play();
    }

    public void Play()
    {
        director.Play();
    }
}
