using StateMachine; // include all script about stateMachine
using System.Collections;
using System.Linq;
using UnityEngine;

public class PestAttackState : BaseState<PestStateMachine>
{
    public PestAttackState(PestStateMachine currentContext, StateFactory<PestStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private enum State
    {
        Start,
        Charge,
        Dash,
        Recharge
    }

    private State curState = State.Start;
    private float elapsedTimeState = 0.0f;

    private float chargeDuration = 1.0f;
    private float rechargeDuration = 0.25f;

    private float dashDistance = 0.0f;

    private Coroutine dashRoutine;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            SwitchState(Factory.GetState<PestFollowTargetState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        curState = State.Start;
        elapsedTimeState = 0.0f;
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    { 
        if (dashRoutine != null)
        {
            Context.StopCoroutine(dashRoutine);
            dashRoutine = null;
        }
        Context.Animator.ResetTrigger(Context.ChargeOutHash);
        Context.Animator.SetTrigger(Context.ChargeOutHash);
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        if (curState == State.Start)
        {
            Vector3 positionToLookAt = new Vector3(Context.Target.position.x, Context.transform.position.y, Context.Target.position.z);
            dashDistance = Vector3.Distance(Context.Target.position, Context.transform.position);
            Context.transform.LookAt(positionToLookAt);
            curState = State.Charge;

            Context.Animator.ResetTrigger(Context.ChargeInHash);
            Context.Animator.SetTrigger(Context.ChargeInHash);
        }
        else if (curState == State.Charge)
        {
            elapsedTimeState += Time.deltaTime;
            if (elapsedTimeState >= chargeDuration)
            {
                elapsedTimeState = 0.0f;
                curState = State.Dash;

                Vector3 curScale = Context.AttackCollider.transform.localScale;
                curScale.z = dashDistance;
                Context.AttackCollider.transform.localScale = curScale;

                Vector3 curPos = Context.AttackCollider.transform.localPosition;
                curPos.z = dashDistance / 2.0f;
                Context.AttackCollider.transform.localPosition = curPos;

                dashRoutine = Context.StartCoroutine(DashCoroutine(dashDistance, Context.DashSpeed));

                Context.Animator.ResetTrigger(Context.ChargeOutHash);
                Context.Animator.SetTrigger(Context.ChargeOutHash);
            }
        }
        else if (curState == State.Dash)
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
        }
    }

    private IEnumerator DashCoroutine(float distance, float speed)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = Context.transform.position;
        Vector3 dashTarget = Context.transform.position + Context.transform.forward * distance;

        IDamageable player = PhysicsExtensions.CheckAttackCollideRayCheck(Context.AttackCollider, Context.transform.position, "Player", LayerMask.GetMask("Entity"))
                                              .Select(x => x.GetComponent<IDamageable>())
                                              .Where(x => x != null)
                                              .FirstOrDefault();

        if (player != null)
        {
            Context.Attack(player);
        }

        float duration = distance / speed;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            Context.Agent.Warp(Vector3.Lerp(startPosition, dashTarget, t));
            yield return null;
        }

        Context.Agent.Warp(dashTarget);
        dashRoutine = null;
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.CurrentState = newState;
    }
}
