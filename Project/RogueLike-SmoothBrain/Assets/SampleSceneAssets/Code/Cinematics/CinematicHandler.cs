using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CinematicHandler : MonoBehaviour
{
    [SerializeField] private bool skipable = false;
    private PlayableDirector director;

    [Header("Events")]
    [SerializeField] private UnityEvent onSkip;

    private void Start()
    {
        director = GetComponent<PlayableDirector>();
    }

    void Update()
    {
        bool gamepadButtonPressed = !DeviceManager.Instance.IsPlayingKB() && Gamepad.current.allControls.Any(x => x is ButtonControl && x.IsPressed() && !x.synthetic);
        if (Input.anyKeyDown || gamepadButtonPressed)
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

        onSkip?.Invoke();
    }
}
