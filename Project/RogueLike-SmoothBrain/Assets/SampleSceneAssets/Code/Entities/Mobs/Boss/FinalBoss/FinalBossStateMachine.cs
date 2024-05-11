// ---[ STATE MACHINE ] ---
// "factory" is use to get all state possible
// "currentState" can be set in the start with : currentState = factory.GetState<YOUR_STATE>();

using UnityEngine;
using StateMachine; // include all script about stateMachine

public class FinalBossStateMachine : MonoBehaviour
{
    [HideInInspector]
    public BaseState<FinalBossStateMachine> currentState;
    private StateFactory<FinalBossStateMachine> factory;

    void Start()
    {
        factory = new StateFactory<FinalBossStateMachine>(this);
        // Set currentState here !
    }

    void Update()
    {
        currentState.Update();
    }
}
