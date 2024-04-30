using StateMachine;

public class KlopsAttackState : BaseState<KlopsStateMachine>
{
    public KlopsAttackState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    protected override void CheckSwitchStates()
    {
    }

    protected override void EnterState()
    {
    }

    protected override void ExitState()
    {
    }

    protected override void UpdateState()
    {
    }

    protected override void SwitchState(BaseState<KlopsStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
