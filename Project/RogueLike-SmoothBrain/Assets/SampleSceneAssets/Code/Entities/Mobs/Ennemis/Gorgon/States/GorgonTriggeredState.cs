// ---[ STATE ] ---
// replace "GorgonTriggeredState_STATEMACHINE" by your state machine class name.
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

public class GorgonTriggeredState : BaseState<GorgonStateMachine>
{
    public GorgonTriggeredState(GorgonStateMachine currentContext, StateFactory<GorgonStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (!Context.Player)
        {
            if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
            {
                SwitchState(Factory.GetState<GorgonWanderingState>());
                return;
            }
        }
        else if (Vector3.SqrMagnitude(Context.Player.transform.position - Context.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE) * Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            if (Context.IsAttackAvailable)
            {
                SwitchState(Factory.GetState<GorgonAttackingState>());
                return;
            }
            else if (Context.IsFleeAvailable)
            {
                SwitchState(Factory.GetState<GorgonFleeingState>());
                return;
            }
        }
        else if (Context.IsAttackAvailable && Context.IsDashAvailable)
        {
            SwitchState(Factory.GetState<GorgonDashingState>());
            return;
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        if (Context.Player)
        {
            Context.MoveTo(Context.Player.transform.position);
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<GorgonStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
