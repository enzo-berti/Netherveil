using StateMachine; // include all script about stateMachine
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DamoclesFollowTargetState : BaseState<DamoclesStateMachine>
{
    public DamoclesFollowTargetState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            SwitchState(Factory.GetState<DamoclesEnGardeState>());
        }
        else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.VISION_RANGE))
        {
            SwitchState(Factory.GetState<DamoclesIdle>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        Vector3 direction = (Context.Target.position - Context.transform.position).normalized;

        if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
            Context.MoveTo(Context.transform.position + direction * Context.NormalSpeed);
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
