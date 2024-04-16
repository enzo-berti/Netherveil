using UnityEngine;
using StateMachine;

public class ClopsStateMachine : MonoBehaviour
{
    [HideInInspector]
    public BaseState<ClopsStateMachine> currentState;
    private StateFactory<ClopsStateMachine> factory;

    void Start()
    {
        factory = new StateFactory<ClopsStateMachine>(this);
        currentState = factory.GetState<ClopsWanderState>();
    }

    void Update()
    {
        currentState.Update();
    }
}
