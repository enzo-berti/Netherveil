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

    private Transform target;

    private State curState = State.Start;
    private float elapsedTimeState = 0.0f;

    private float chargeDuration = 1.0f;
    private float rechargeDuration = 0.25f;

    private float dashDuration = 0.25f;
    private float dashDistance = 10.0f;

    private Coroutine dashRoutine;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        Entity[] entitiesInVision = Context.NearbyEntities;
        if (!entitiesInVision.FirstOrDefault(x => x.GetComponent<PlayerController>()))
        {
            if (entitiesInVision.FirstOrDefault(x => x is IPest))
            {
                SwitchState(Factory.GetState<PestRegroupState>());
            }
            else
            {
                SwitchState(Factory.GetState<PestIdleState>());
            }
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        curState = State.Start;
        elapsedTimeState = 0.0f;

        target = Context.NearbyEntities.FirstOrDefault(x => x.GetComponent<PlayerController>()).transform;
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    { 
        if (dashRoutine != null)
        {
            Context.StopCoroutine(dashRoutine);
            Context.Agent.enabled = true;
            Context.GetComponent<Collider>().enabled = true;
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
            Vector3 positionToLookAt = new Vector3(target.position.x, Context.transform.position.y, target.position.z);
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

                Vector3 dashTarget = Context.transform.position + Context.transform.forward * dashDistance;
                dashRoutine = Context.StartCoroutine(DashCoroutine(dashTarget, dashDuration));

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

    private IEnumerator DashCoroutine(Vector3 targetPosition, float duration)
    {
        Context.Agent.enabled = false;
        Context.GetComponent<Collider>().enabled = false;

        float timeElapsed = 0f;
        Vector3 startPosition = Context.transform.position;

        IDamageable player = PhysicsExtensions.CheckAttackCollideRayCheck(Context.AttackCollider, Context.transform.position, "Player", LayerMask.GetMask("Entity"))
                                              .Select(x => x.GetComponent<IDamageable>())
                                              .Where(x => x != null)
                                              .FirstOrDefault();

        if (player != null)
            player.ApplyDamage((int)Context.Stats.GetValue(Stat.ATK));

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            Context.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        Context.transform.position = targetPosition;
        Context.Agent.enabled = true;
        Context.GetComponent<Collider>().enabled = true;
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
