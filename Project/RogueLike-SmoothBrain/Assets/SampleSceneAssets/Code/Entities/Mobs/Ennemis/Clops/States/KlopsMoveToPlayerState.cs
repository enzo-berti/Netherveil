using StateMachine;
using UnityEngine;

public class KlopsMoveToPlayerState : BaseState<KlopsStateMachine>
{
    Vector3 meToPlayerVec { get { return Utilities.Player.transform.position - Context.transform.position; } }
    float minTimeBeforeAttack = 0.5f;
    float currentTimer = 0f;
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
            else if(currentTimer >= minTimeBeforeAttack && !Context.Agent.hasPath)
            {
                SwitchState(Factory.GetState<KlopsAttackState>());
                return;
            }
        }
    }

    protected override void EnterState()
    {
        currentTimer = currentTimer >= minTimeBeforeAttack ? currentTimer : 0f;
    }

    protected override void ExitState()
    {
    }

    protected override void UpdateState()
    {
        if (Context == null) return;
        if(!Context.Agent.hasPath && Vector3.Distance(Context.Player.transform.position, Context.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE) / 1.5f)
        {
            Context.MoveTo(MathsExtension.GetRandomPointInCircle(Context.Player.transform.position, Context.FleeRange * 1.5f, Context.Stats.GetValue(Stat.ATK_RANGE)));
        }
            
        currentTimer += Time.deltaTime;
    }

    protected override void SwitchState(BaseState<KlopsStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
