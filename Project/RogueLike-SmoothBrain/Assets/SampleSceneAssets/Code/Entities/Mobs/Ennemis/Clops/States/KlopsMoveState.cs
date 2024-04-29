using StateMachine;
using UnityEngine;

public class KlopsMoveState : BaseState<KlopsStateMachine>
{
    public KlopsMoveState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    protected override void CheckSwitchStates()
    {
        Vector3 meToPlayerVec = (Context.Player.transform.position - Context.transform.position);

        if (!Context.Player)
        {
            if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
            {
                SwitchState(Factory.GetState<KlopsPatrolState>());
                return;
            }
        }
        else if (meToPlayerVec.magnitude <= Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            if (meToPlayerVec.magnitude <= Context.FleeRange)
            {
                SwitchState(Factory.GetState<KlopsFleeState>());
                return;
            }
            else
            {
                SwitchState(Factory.GetState<KlopsAttackState>());
                return;
            }
        }
    }

    protected override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}
