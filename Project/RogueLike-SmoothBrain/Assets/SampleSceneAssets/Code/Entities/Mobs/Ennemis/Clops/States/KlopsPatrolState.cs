using StateMachine;
using UnityEngine;

public class KlopsPatrolState : BaseState<KlopsStateMachine>
{
    public KlopsPatrolState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    bool canMove = false;
    float idleTimer = 0f;
    readonly float MAX_IDLE_COOLDOWN = 2f;

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
    }

    protected override void ExitState()
    {
    }

    protected override void UpdateState()
    {
        if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
        {
            canMove = idleTimer >= MAX_IDLE_COOLDOWN;

            if (!canMove)
            {
                idleTimer += Time.deltaTime;
            }
            else
            {
                float minRange = Context.Stats.GetValue(Stat.ATK_RANGE) / 2f;
                float maxRange = Context.Stats.GetValue(Stat.ATK_RANGE);

                Context.MoveTo(Context.GetRandomPointOnWanderZone(Context.transform.position, minRange, maxRange));
                idleTimer = Random.Range(-0.5f, 0.5f);
            }
        }
    }
}
