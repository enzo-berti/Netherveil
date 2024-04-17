using StateMachine; // include all scripts about StateMachines
using UnityEngine;

public class DamoclesVulnerableState : BaseState<DamoclesStateMachine>
{
    public DamoclesVulnerableState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }
        
    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            SwitchState(Factory.GetState<DamoclesFollowTargetState>());
        }
        else
        {
            SwitchState(Factory.GetState<DamoclesEnGardeState>());
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
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
