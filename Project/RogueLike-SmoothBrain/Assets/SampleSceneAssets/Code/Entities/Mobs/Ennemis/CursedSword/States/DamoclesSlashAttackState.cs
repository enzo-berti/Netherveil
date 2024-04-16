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
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
    private Coroutine slashRoutine;
    private Vector3 previousPos;

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
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
        if (slashRoutine != null)
        {
            Context.StopCoroutine(slashRoutine);
            slashRoutine = null;
        }
        //Context.Animator.ResetTrigger(Context.);
        //Context.Animator.SetTrigger(Context.);
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

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        else if (curState == State.RunIn)
        {
            if (Vector3.Distance(Context.Target.position, Context.transform.position) > 1)
            {
                Context.MoveTo(Context.Target.position);
            }
            else
            {
                curState = State.Slash;
            }

            slashRoutine = Context.StartCoroutine(SlashCoroutine(Context.Attack1Collider));

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        else if (curState == State.Slash)
        {
            if (slashRoutine != null)
            {
                curState = State.BackSlash;
                slashRoutine = Context.StartCoroutine(SlashCoroutine(Context.Attack1Collider)); //collider à changer
            }
        }
        else if (curState == State.BackSlash)
        {
            if (slashRoutine != null)
            {
                curState = State.Backward;
            }
        }
        else if (curState == State.Backward)
        {
            if (Vector3.Distance(Context.Target.position, Context.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
            {
                Context.MoveTo(previousPos);
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

    private IEnumerator SlashCoroutine(BoxCollider attackCollider)
    {
        IDamageable player = PhysicsExtensions.CheckAttackCollideRayCheck(attackCollider, Context.transform.position, "Player", LayerMask.GetMask("Entity"))
                                              .Select(x => x.GetComponent<IDamageable>())
                                              .Where(x => x != null)
                                              .FirstOrDefault();

        if (player != null)
        {
            Context.Attack(player);
        }

        slashRoutine = null;
        yield return null;
    }

}
