using StateMachine; // include all script about stateMachine
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class PestAttackState : BaseState<PestStateMachine>
{
    public PestAttackState(PestStateMachine currentContext, StateFactory<PestStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private float elapsedTimeAttack;

    private float dashDuration = 0.25f;
    private float dashDistance = 8.0f;

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
        elapsedTimeAttack = Time.time;

        Context.Animator.ResetTrigger(Context.ChargeInHash);
        Context.Animator.SetTrigger(Context.ChargeInHash);

        LookAtPlayer();
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
        Context.Animator.ResetTrigger(Context.ChargeOutHash);
        Context.Animator.SetTrigger(Context.ChargeOutHash);
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        // Delay
        if (Time.time - elapsedTimeAttack < Context.DelayToAttack || Context.DashRoutine != null)
            return;

        // Animations
        Context.Animator.ResetTrigger(Context.ChargeOutHash);
        Context.Animator.SetTrigger(Context.ChargeOutHash);

        // Dash
        Vector3 dashTarget = Context.transform.position + Context.transform.forward * dashDistance;

        Context.DashRoutine = Context.StartCoroutine(DashCoroutine(dashTarget, dashDuration));
    }

    private IEnumerator DashCoroutine(Vector3 targetPosition, float duration)
    {
        Context.Agent.enabled = false;
        float timeElapsed = 0f;
        Vector3 startPosition = Context.transform.position;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            Context.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        LookAtPlayer();

        Context.transform.position = targetPosition;
        Context.Agent.enabled = true;
        Context.DashRoutine = null;
        elapsedTimeAttack = Time.time;
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.CurrentState = newState;
    }

    private void LookAtPlayer()
    {
        Transform plyTransform = Context.NearbyEntities.FirstOrDefault(x => x.GetComponent<PlayerController>()).transform;
        Context.transform.LookAt(new Vector3(plyTransform.position.x, Context.transform.position.y, plyTransform.position.z));
    }
}
