// ---[ STATE ] ---
// replace "DamoclesSlashAttackState_STATEMACHINE" by your state machine class name.
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

public class DamoclesSlashAttackState : BaseState<DamoclesStateMachine>
{
    public DamoclesSlashAttackState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private enum State
    {
        Start,
        RunIn,
        Slash,
        BackSlash,
        Backward
    }

    private State curState = State.Start;
    private float walkDistance = 0.0f;

    // This method will be call every Update to check and change a state.
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
        if (curState == State.Start)
        {
            Vector3 positionToLookAt = new Vector3(Context.Target.position.x, Context.transform.position.y, Context.Target.position.z);
            walkDistance = Vector3.Distance(Context.Target.position, Context.transform.position) - 1;
            Context.transform.LookAt(positionToLookAt);
            curState = State.RunIn;

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        else if (curState == State.RunIn)
        {
            Context.MoveTo(Context.Target.position);

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        /*else if (curState == State.Dash)
        {
            if (dashRoutine != null)
            {
                curState = State.Recharge;
            }
        }
        else if (curState == State.Recharge)
        {
            elapsedTimeState += Time.deltaTime;
            if (elapsedTimeState >= rechargeDuration)
            {
                elapsedTimeState = 0.0f;
                curState = State.Start;
            }
        }*/
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
