using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Transition : MonoBehaviour
{
    [SerializeField] private bool playOnAwake = true;

    private event Action onTransitionEnd;

    private Animator animator;
    private bool enable = false;
    private bool transitionEnd = true;

    public Action OnTransitionEnd => onTransitionEnd;
    public bool TransitionEnd => transitionEnd;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (playOnAwake)
        {
            animator.SetBool("Transit", false);
        }
    }

    private void OnEnable()
    {
        onTransitionEnd += () => transitionEnd = true;
    }

    private void OnDisable()
    {
        onTransitionEnd -= () => transitionEnd = true;
    }

    public void Toggle()
    {
        if (enable)
            Disable();
        else
            Enable();
    }

    public void Enable()
    {
        if (enable)
        {
            Debug.LogWarning("Transition already enable");
            return;
        }

        enable = true;
        transitionEnd = false;
        animator.SetBool("Transit", true);
    }

    public void Disable()
    {
        if (enable)
        {
            Debug.LogWarning("Transition already disable");
            return;
        }

        enable = false;
        transitionEnd = false;
        animator.SetBool("Transit", false);
    }
}
