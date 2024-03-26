using StateMachine; // include all script about stateMachine
using System.Linq;

public class PestIdleState : BaseState<PestStateMachine>
{
    public PestIdleState(PestStateMachine currentContext, StateFactory<PestStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }
        
    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        Entity[] entitiesInVision = Context.NearbyEntities;
        if (entitiesInVision.FirstOrDefault(x => x.GetComponent<PlayerController>()))
        {
            SwitchState(Factory.GetState<PestAttackState>());
        }
        else if (entitiesInVision.FirstOrDefault(x => x is IPest))
        {
            SwitchState(Factory.GetState<PestRegroupState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    { }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    { }

    // This method will be call every frame.
    protected override void UpdateState()
    { }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.CurrentState = newState;
    }
}
