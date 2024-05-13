using Cinemachine;
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
        foreach (var output in director.playableAsset.outputs)
        {
            CinemachineTrack cinemachineTrack = output.sourceObject as CinemachineTrack;
            if (cinemachineTrack != null)
            {
                CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
                director.SetGenericBinding(cinemachineTrack, brain);
            }
        }
    }

    public void Play()
    {
        director.Play();
    }
}
