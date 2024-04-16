// ---[ STATE ] ---
// replace "DamoclesJumpAttackState_STATEMACHINE" by your state machine class name.
//
// Here you can see an exemple of CheckSwitchStates method:
// protected override void CheckSwitchStates()
// {
//      if (isRunning)
//      {
//          SwitchState(Factory.GetState<RunningState>());
//      }
// }

using StateMachine; // include all script about stateMachine
using UnityEngine;

public class DamoclesJumpAttackState : BaseState<DamoclesStateMachine>
{
    public DamoclesJumpAttackState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private bool isTargetTouched;
    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (isTargetTouched)
        {
            if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
            {
                SwitchState(Factory.GetState<DamoclesFollowTargetState>());
            }
            else
            {
                SwitchState(Factory.GetState<DamoclesEnGardeState>());
            }
        }
        else
        {
            SwitchState(Factory.GetState<DamoclesVulnerableState>());
        }

    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
