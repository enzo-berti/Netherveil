// ---[ STATE ] ---
// replace "SonielCircularHit_STATEMACHINE" by your state machine class name.
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
using UnityEngine;

public class SonielCircularHit : BaseState<SonielStateMachine>
{
    public SonielCircularHit(SonielStateMachine currentContext, StateFactory<SonielStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    int circularHash = Animator.StringToHash("Circular");
    float attackDuration = 0f;

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (attackDuration >= Context.Animator.GetCurrentAnimatorClipInfo(0).Length)
        {
            SwitchState(Factory.GetState<SonielTriggeredState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        Context.PlayerHit = false;
        Context.Agent.isStopped = true;
        attackDuration = 0f;

        Context.Animator.ResetTrigger(circularHash);
        Context.Animator.SetTrigger(circularHash);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.PlayerHit = false;
        Context.Agent.isStopped = false;

        Context.DisableHitboxes();
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        if (!Context.PlayerHit)
        {
            Context.AttackCollide(Context.Attacks[(int)SonielStateMachine.SonielAttacks.CIRCULAR].data, debugMode: true);
        }

        attackDuration += Time.deltaTime;
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<SonielStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
