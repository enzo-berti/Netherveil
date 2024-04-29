using StateMachine; // include all scripts about StateMachines
using UnityEngine;

public class ZiggoWandering : BaseState<ZiggoStateMachine>
{
    public ZiggoWandering(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 2.0f;
    Vector3 direction;
    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<ZiggoDeathState>());
        }
        else if (Context.Target != null)
        {
            SwitchState(Factory.GetState<ZiggoDashAttackState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        elapsedTimeMovement = Time.time;
        direction = Random.insideUnitCircle.normalized;
        Context.Stats.SetValue(Stat.SPEED, 5);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Context.MoveTo(Context.transform.position + direction * 10 * Context.NormalSpeed);

        // Delay
        if (Time.time - elapsedTimeMovement < delayBetweenMovement)
            return;

        elapsedTimeMovement = Time.time;
        direction = Random.insideUnitCircle.normalized;
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
