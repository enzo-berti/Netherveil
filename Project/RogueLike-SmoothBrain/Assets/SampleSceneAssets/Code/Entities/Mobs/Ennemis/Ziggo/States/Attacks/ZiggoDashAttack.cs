// ---[ STATE ] ---
// replace "ZiggoCirclingState_STATEMACHINE" by your state machine class name.
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

public class ZiggoDashAttack : BaseState<ZiggoStateMachine>
{
    public ZiggoDashAttack(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;
    Vector3 direction;
    float dashRange;


    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (attackEnded)
        {
            SwitchState(Context.Player ? Factory.GetState<ZiggoTriggeredState>() : Factory.GetState<ZiggoWanderingState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        attackEnded = false;

        Context.Stats.IncreaseCoeffValue(Stat.SPEED, 2);

        direction = Utilities.Hero.transform.position - Context.transform.position;
        dashRange = direction.magnitude;
        direction.y = 0;
        direction.Normalize();

        Context.Animator.ResetTrigger("Dash");
        Context.Animator.SetTrigger("Dash");

        Context.MoveTo(Context.transform.position + direction * (dashRange + 1));
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Stats.DecreaseCoeffValue(Stat.SPEED, 2);
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
        {
            attackEnded = true;
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
