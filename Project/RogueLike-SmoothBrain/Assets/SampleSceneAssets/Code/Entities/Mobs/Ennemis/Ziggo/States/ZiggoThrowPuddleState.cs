using StateMachine; // include all scripts about StateMachines
using UnityEngine;
public class ZiggoThrowPuddleState : BaseState<ZiggoStateMachine>
{
    public ZiggoThrowPuddleState(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private bool stateEnded;
    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<ZiggoDeathState>());
        }
        else if (stateEnded)
        {
            SwitchState(Factory.GetState<ZiggoCirclingState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        stateEnded = false;
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Debug.Log("throw");
        //fuite
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
