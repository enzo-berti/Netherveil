using StateMachine;
using UnityEngine;

public class KlopsMoveToPlayerState : BaseState<KlopsStateMachine>
{
    Vector3 meToPlayerVec { get { return Context.Player.transform.position - Context.transform.position; } }
    public KlopsMoveToPlayerState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    protected override void CheckSwitchStates()
    {
        if (meToPlayerVec.magnitude <= Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            if (meToPlayerVec.magnitude <= Context.FleeRange)
            {
                SwitchState(Factory.GetState<KlopsFleeState>());
                return;
            }
            else if(meToPlayerVec.magnitude <= Context.Stats.GetValue(Stat.ATK_RANGE))
            {
                SwitchState(Factory.GetState<KlopsAttackState>());
                return;
            }
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
        Context.MoveTo(Context.transform.position + meToPlayerVec.normalized);
    }
}
