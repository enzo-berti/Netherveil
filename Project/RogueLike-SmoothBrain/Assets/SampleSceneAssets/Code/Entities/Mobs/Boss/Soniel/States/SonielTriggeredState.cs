// ---[ STATE ] ---
// replace "SonielTriggeredState_STATEMACHINE" by your state machine class name.
//
// Here you can see an exemple of the CheckSwitchStates method:
// protected override void CheckSwitchStates()
// {
//      if (isRunning)
//      {
//          SwitchState(Factory.GetState<RunningState>());
//      }
// }

using StateMachine; // include all scripts about StateMachines
using System.Collections.Generic;
using UnityEngine;

public class SonielTriggeredState : BaseState<SonielStateMachine>
{
    public SonielTriggeredState(SonielStateMachine currentContext, StateFactory<SonielStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchState(Factory.GetState<SonielCircularHit>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchState(Factory.GetState<SonielBerserk>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        Context.Animator.SetBool("Walk", true);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Animator.SetBool("Walk", false);
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Context.MoveTo(Context.Player.transform.position - (Context.Player.transform.position - Context.transform.position).normalized * 2f);

        Context.Animator.SetBool("Walk", Context.Agent.remainingDistance > Context.Agent.stoppingDistance);
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<SonielStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
