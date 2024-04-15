// ---[ STATE MACHINE ] ---
// "factory" is use to get all state possible
// "currentState" can be set in the start with : currentState = factory.GetState<YOUR_STATE>();

using UnityEngine;
using StateMachine; // include all script about stateMachine

public class GlorbStateMachine : MonoBehaviour
{
    public BaseState<GlorbStateMachine> currentState;
    private StateFactory<GlorbStateMachine> factory;

    void Start()
    {
        factory = new StateFactory<GlorbStateMachine>(this);
        // Set currentState here !
    }

    void Update()
    {
        currentState.Update();
    }
}
