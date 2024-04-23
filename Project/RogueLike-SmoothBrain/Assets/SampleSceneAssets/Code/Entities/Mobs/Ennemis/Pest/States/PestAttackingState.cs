using StateMachine; // include all script about stateMachine
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PestAttackingState : BaseState<PestStateMachine>
{
    public PestAttackingState(PestStateMachine currentContext, StateFactory<PestStateMachine> currentFactory)
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

    private float chargeDuration = 0.5f;
    private float rechargeDuration = 0.25f;

    private float dashDistance = 0.0f;

    private Coroutine dashRoutine;

    bool attackEnded = false;

    Vector3 playerPos;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (attackEnded)
        {
            if (!Context.Player)
            {
                SwitchState(Factory.GetState<PestWanderingState>());
            }
            else if (Vector3.Distance(Context.transform.position, Context.Player.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
            {
                SwitchState(Factory.GetState<PestTriggeredState>());
            }
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        curState = State.Start;
        elapsedTimeState = 0.0f;
        playerPos = Context.Player.position;
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
        if (dashRoutine != null)
        {
            Context.StopCoroutine(dashRoutine);
            dashRoutine = null;
        }

        Context.Animator.ResetTrigger("Cancel");
        Context.Animator.SetTrigger("Cancel");
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        if (Context == null) return;

        if (Context.Player) playerPos = Context.Player.position;

        if (curState == State.Start)
        {
            attackEnded = false;
            Vector3 positionToLookAt = new Vector3(playerPos.x, Context.transform.position.y, playerPos.z);
            LookAt(positionToLookAt, 10f);

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

                dashDistance = Vector3.Distance(playerPos, Context.transform.position);
                dashDistance = Mathf.Clamp(dashDistance, 0f, Context.Stats.GetValue(Stat.ATK_RANGE));

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
            else if (elapsedTimeState <= chargeDuration - 0.2f)
            {
                Vector3 positionToLookAt = new Vector3(playerPos.x, Context.transform.position.y, playerPos.z);
                LookAt(positionToLookAt, 10f);
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
                attackEnded = true;
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
        bool isOnNavMesh = true;

        while (timeElapsed < duration && isOnNavMesh)
        {
            yield return null;

            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            Vector3 warpPosition = Vector3.Lerp(startPosition, dashTarget, t);

            if (isOnNavMesh = NavMesh.SamplePosition(warpPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                Context.Agent.Warp(hit.position);
            }
        }

        dashRoutine = null;
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.CurrentState = newState;
    }

    #region Extra methods
    void LookAt(Vector3 _target, float _speed)
    {
        Quaternion lookRotation = Quaternion.LookRotation(_target - Context.transform.position);
        lookRotation.x = 0;
        lookRotation.z = 0;

        Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, lookRotation, _speed * Time.deltaTime);
    }
    #endregion
}
