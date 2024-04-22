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
using System.Collections;
using System.Linq;
using UnityEngine;

public class DamoclesJumpAttackState : BaseState<DamoclesStateMachine>
{
    public DamoclesJumpAttackState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private bool isTargetTouched = false;
    private bool stateEnded = false;

    private enum State
    {
        Start,
        RunIn,
        Jump,
        Backward
    }

    private State curState = State.Start;
    private Coroutine jumpRoutine;
    private Vector3 previousPos;
    private Vector3 jumpTarget;
    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (isTargetTouched && stateEnded)
        {
            SwitchState(Factory.GetState<DamoclesEnGardeState>());
        }
        else if (!isTargetTouched && stateEnded)
        {
            stateEnded = false;
            SwitchState(Factory.GetState<DamoclesVulnerableState>());
        }

    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        curState = State.Start;
        stateEnded = false;
        isTargetTouched = false;
        Context.Stats.SetValue(Stat.SPEED, 5);
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {

    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        if (curState == State.Start)
        {
            Vector3 positionToLookAt = new Vector3(Context.Target.position.x, Context.transform.position.y, Context.Target.position.z);
            Context.transform.LookAt(positionToLookAt);
            curState = State.RunIn;
            previousPos = Context.transform.position;
            Vector3 direction = (Context.Target.position - Context.transform.position).normalized;
            jumpTarget = Context.Target.position;
            Context.MoveTo(Context.transform.position + direction * Context.NormalSpeed);

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        else if (curState == State.RunIn)
        {
            Context.MoveTo(jumpTarget);

            if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) < Context.Stats.GetValue(Stat.ATK_RANGE) / 2)
            {
                Context.Stats.SetValue(Stat.SPEED, 3);
                //Context.Animator.ResetTrigger(Context.);
                //Context.Animator.SetTrigger(Context.);

                curState = State.Jump;
            }
        }
        else if (curState == State.Jump)
        {

            if (Vector3.Distance(Context.transform.position, jumpTarget) < 0.1f)
            {
                jumpRoutine = Context.StartCoroutine(JumpCoroutine());
            }

            if (isTargetTouched)
            {
                curState = State.Backward;
            }

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        else if (curState == State.Backward)
        {
            if (Vector3.Distance(Context.Target.position, Context.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
            {
                Context.MoveTo(previousPos);
            }
            else
            {
                stateEnded = true;
            }
        }
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    private IEnumerator JumpCoroutine()
    {
        IDamageable player = PhysicsExtensions.CheckAttackCollideRayCheck(Context.Attack1Collider, Context.transform.position, "Player", LayerMask.GetMask("Entity"))
                                              .Select(x => x.GetComponent<IDamageable>())
                                              .Where(x => x != null)
                                              .FirstOrDefault();

        if (player != null)
        {
            Context.Attack(player);
            isTargetTouched = true;
        }
        else
        {
            isTargetTouched = false;
            stateEnded = true;
        }

        yield return null;
        jumpRoutine = null;
    }
}
