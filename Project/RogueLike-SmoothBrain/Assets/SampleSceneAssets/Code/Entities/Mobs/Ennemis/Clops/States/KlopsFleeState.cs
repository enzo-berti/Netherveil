using StateMachine;

public class KlopsFleeState : BaseState<KlopsStateMachine>
{
    public KlopsFleeState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    protected override void CheckSwitchStates()
    {
        throw new System.NotImplementedException();
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
