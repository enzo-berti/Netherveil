using StateMachine;
using UnityEngine;

public class KlopsFleeState : BaseState<KlopsStateMachine>
{
    public KlopsFleeState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    protected override void CheckSwitchStates()
    {
        if (Context.Player == null) return;
        if(Vector3.Distance(Context.Player.transform.position, Context.transform.position) > Context.FleeRange * 1.5f)
        {
            
            SwitchState(Factory.GetState<KlopsAttackState>());
        }
    }

    protected override void EnterState()
    {
        Debug.Log("In flee state");
    }

    protected override void ExitState()
    {
    }

    protected override void UpdateState()
    {
        if (Context.Player == null) return;
        Vector3 direction = Context.transform.position - Context.Player.transform.position;
        direction.Normalize();

        Context.MoveTo(Context.transform.position + direction);
    }

    protected override void SwitchState(BaseState<KlopsStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
