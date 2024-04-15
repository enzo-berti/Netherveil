// ---[ STATE ] ---
// replace "GlorbDeathState_STATEMACHINE" by your state machine class name.
//
// Here you can see an exemple of CheckSwitchStates method:
// protected override void CheckSwitchStates()
// {
//      if (isRunning)
//      {
//          SwitchState(Factory.GetState<RunningState>());
//      }
// }

using StateMachine; // include all script about stateMachine

public class GlorbDeathState : BaseState<GlorbDeathState_STATEMACHINE>
{
    public GlorbDeathState(GlorbDeathState_STATEMACHINE currentContext, StateFactory<GlorbDeathState_STATEMACHINE> currentFactory)
        : base(currentContext, currentFactory) { }
        
    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        throw new System.NotImplementedException();
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
