// ---[ STATE ] ---
// replace "ZiggoCirclingState_STATEMACHINE" by your state machine class name.
//
// Here you can see an exemple of the CheckSwitchStates method:
// protected override void CheckSwitchStates()
// {
//      if (isRunning)
//      {
//          SwitchState(Factory.GetState<RunningState>());
//      }
// }

using StateMachine; // include all scripts about StateMachines
using UnityEngine;

public class ZiggoTurnAroundTargetState : BaseState<ZiggoStateMachine>
{
    public ZiggoTurnAroundTargetState(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private int randDir = 0;
    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 2.0f;
    private bool stateEnded;
    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Vector3.Distance(Context.transform.position, Context.Player.transform.position) > Context.Stats.GetValue(Stat.VISION_RANGE))
        {
            SwitchState(Factory.GetState<ZiggoTriggeredState>());
        }
        else if (stateEnded)
        {
            SwitchState(Factory.GetState<ZiggoSpitAttackState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        randDir = Random.Range(0, 2);
        elapsedTimeMovement = Time.time;
        delayBetweenMovement = Random.Range(3, 7);
        stateEnded = false;
        Context.Stats.SetValue(Stat.SPEED, 7);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Vector3 direction = (Context.Player.transform.position - Context.transform.position).normalized;

        if (randDir == 0)
        {
            Context.MoveTo(Context.transform.position + new Vector3(-direction.z, 0, direction.x));
        }
        else
        {
            Context.MoveTo(Context.transform.position + new Vector3(direction.z, 0, -direction.x));
        }


        // Delay
        if (Time.time - elapsedTimeMovement < delayBetweenMovement)
            return;

        elapsedTimeMovement = Time.time;
        stateEnded = true;
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
