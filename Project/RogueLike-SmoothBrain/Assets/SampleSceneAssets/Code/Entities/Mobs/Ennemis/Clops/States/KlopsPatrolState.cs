using StateMachine;
using UnityEngine;

public class KlopsPatrolState : BaseState<KlopsStateMachine>
{
    public KlopsPatrolState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory)
        : base(currentContext, currentFactory){ }

    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 2.0f;
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<KlopsDeathState>());
        }
        else if (Context.Target != null)
        {
            SwitchState(Factory.GetState<KlopsMoveToPlayerState>());
        }
    }

    protected override void EnterState()
    {
        Context.Stats.SetValue(Stat.SPEED, 5);
        elapsedTimeMovement = Time.time;
        Context.IsInvincibleCount = 1;
        Context.Animator.SetTrigger("BackToWalk");
    }

    protected override void ExitState()
    {
    }

    protected override void UpdateState()
    {
        // Delay
        if (Time.time - elapsedTimeMovement < delayBetweenMovement)
            return;

        elapsedTimeMovement = Time.time;

        Vector3 pointToReach3D = Context.GetRandomPointOnWanderZone(Context.gameObject.transform.position, 2, 5);

        Context.MoveTo(pointToReach3D * Context.NormalSpeed);
    }

    protected override void SwitchState(BaseState<KlopsStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
