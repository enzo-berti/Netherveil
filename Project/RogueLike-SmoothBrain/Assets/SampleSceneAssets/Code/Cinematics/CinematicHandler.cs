using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CinematicHandler : MonoBehaviour
{
    private PlayableDirector director;
    [SerializeField] private bool skipable = false;

    private void Start()
    {
        director = GetComponent<PlayableDirector>();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (skipable)
                Skip();
        }
    }

    private void Skip()
    {
        director.time = director.playableAsset.duration;
        director.Evaluate();
        director.Stop();
    }
}
