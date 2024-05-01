using StateMachine;
using UnityEngine;

public class KlopsWanderingState : BaseState<KlopsStateMachine>
{
    public KlopsWanderingState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory)
        : base(currentContext, currentFactory){ }

    float idleTimer = 0f;

    protected override void CheckSwitchStates()
    {
        if (Context.Player != null)
        {
            SwitchState(Factory.GetState<KlopsTriggeredState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        Context.WanderZoneCenter = Context.transform.position;

        Context.Stats.SetValue(Stat.SPEED, 5);
        idleTimer = Random.Range(-0.5f, 0.5f);
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {

        if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
        {
            idleTimer += Time.deltaTime;
        }

        if (idleTimer >= 1f)
        {
            float minRange = Context.Stats.GetValue(Stat.ATK_RANGE) * 0.5f;
            float maxRange = Context.Stats.GetValue(Stat.ATK_RANGE);

            Context.MoveTo(Context.GetRandomPointOnWanderZone(Context.transform.position, minRange, maxRange));

            idleTimer = Random.Range(-0.5f, 0.5f);
        }
    }

    protected override void SwitchState(BaseState<KlopsStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
