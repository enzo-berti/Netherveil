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
        else if (Context.Player != null)
        {
            Debug.Log("Go to klops move");
            SwitchState(Factory.GetState<KlopsMoveToPlayerState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        Context.WanderZoneCenter = Context.transform.position;

        Context.Stats.SetValue(Stat.SPEED, 5);
        elapsedTimeMovement = Time.time;
        Debug.Log("Enter PatrolState");
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
    }

    // This method will be call every frame.
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
