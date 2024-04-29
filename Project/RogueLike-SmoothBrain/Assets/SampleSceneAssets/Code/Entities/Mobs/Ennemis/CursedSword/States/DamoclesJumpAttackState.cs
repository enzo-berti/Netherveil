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
    private bool jumpRoutineOn;
    private bool returnToPos = false;
    private Vector3 previousPos;
    private Vector3 previousBladePos;
    private Vector3 jumpTarget;
    Quaternion baseRotation;
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
            Debug.Log("test");
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
        Context.Agent.enabled = false;
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
            curState = State.Jump;
            previousPos = Context.gameObject.transform.position;
            jumpTarget = Context.Target.gameObject.transform.position;
            baseRotation = Context.gameObject.transform.rotation;
        }
        else if (curState == State.RunIn)
        {
            Context.MoveTo(jumpTarget);

            if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) < Context.Stats.GetValue(Stat.ATK_RANGE) / 2)
            {
                Context.Stats.SetValue(Stat.SPEED, 3);
                curState = State.Jump;
            }
        }
        else if (curState == State.Jump)
        {
            if (!jumpRoutineOn)
            {
               Context.StartCoroutine(JumpCoroutine(jumpTarget));
            }
            if (isTargetTouched)
            {
                curState = State.Backward;
                jumpRoutineOn = false;
            }
        }
        else if (curState == State.Backward)
        {
            if (!returnToPos && Context.transform.position != previousPos)
            {
                Context.Animator.SetTrigger("BackToWalk");
                returnToPos = true;
                Context.MoveTo(previousPos);
            }
            else
            {
                returnToPos = false;
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

    private IEnumerator JumpCoroutine(Vector3 posToReach)
    {
        jumpRoutineOn = true;
        float timer = 0f;
        float a = -12;
        float b = -a;
        float c = previousPos.y;

        float timerWanted = MathsExtension.Resolve2ndDegree(a, b, c, -0.25f).Max();
        Context.Animator.SetTrigger("Jump");
        
        while(timer < timerWanted)
        {
            Context.gameObject.transform.rotation = baseRotation;
            timer += Time.deltaTime;
            timer = timer > timerWanted ? timerWanted : timer;
            Vector3 currentPos = Vector3.Lerp(previousPos, posToReach, timer);
            currentPos.y = MathsExtension.SquareFunction(a, b, c, timer);
            Context.transform.position = currentPos;
            yield return null;
        }
        IDamageable player = PhysicsExtensions.CheckAttackCollideRayCheck(Context.Attack1Collider, Context.transform.position, "Player", LayerMask.GetMask("Entity"))
                                              .Select(x => x.GetComponent<IDamageable>())
                                              .Where(x => x != null)
                                              .FirstOrDefault();

        if (player != null)
        {
            Context.Attack(player, (Utilities.Player.transform.position - previousPos).normalized);
            (player as Entity).AddStatus(new Bleeding(5.0f, 1));
            isTargetTouched = true;
            Context.Agent.enabled = true;
        }
        else
        {
            isTargetTouched = false;
            stateEnded = true;
            Context.DamoclesSound.stuckSound.Play(Context.transform.position);
        }

        yield return null;
        
    }

}
