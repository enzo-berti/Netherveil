using StateMachine; // include all script about stateMachine
using UnityEngine;

public class DamoclesIdle : BaseState<DamoclesStateMachine>
{
    public DamoclesIdle(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 1.0f;
    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (Context.Target != null)
        {
            SwitchState(Factory.GetState<DamoclesFollowTargetState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        Context.Stats.SetValue(Stat.SPEED, 5);
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        // Delay
        if (Time.time - elapsedTimeMovement < delayBetweenMovement)
            return;

        elapsedTimeMovement = Time.time;

        Vector3 direction = Random.insideUnitCircle.normalized;

        Context.MoveTo(Context.transform.position + direction * Context.NormalSpeed);
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
