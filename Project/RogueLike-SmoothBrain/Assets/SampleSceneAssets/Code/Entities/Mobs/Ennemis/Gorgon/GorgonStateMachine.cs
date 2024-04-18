// ---[ STATE MACHINE ] ---
// "factory" is use to get all state possible
// "currentState" can be set in the start with : currentState = factory.GetState<YOUR_STATE>();

using UnityEngine;
using StateMachine; // include all script about stateMachine

public class GorgonStateMachine : MonoBehaviour
{
    [HideInInspector]
    public BaseState<GorgonStateMachine> currentState;
    private StateFactory<GorgonStateMachine> factory;

    void Start()
    {
        factory = new StateFactory<GorgonStateMachine>(this);
        currentState = factory.GetState<GorgonWanderingState>();
    }

    void Update()
    {
        currentState.Update();
    }
}
