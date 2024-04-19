// ---[ STATE ] ---
// replace "GlorbWanderingState_STATEMACHINE" by your state machine class name.
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
using UnityEngine;

public class GlorbWanderingState : BaseState<GlorbStateMachine>
{
    public GlorbWanderingState(GlorbStateMachine currentContext, StateFactory<GlorbStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    Vector3 randomDirection;
    float idleTimer = 0f;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.Player != null)
        {
            SwitchState(Factory.GetState<GlorbTriggeredState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {

    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {

    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
        {
            idleTimer += Time.deltaTime;
        }

        if (idleTimer > 1f)
        {
            float range = Context.Stats.GetValue(Stat.VISION_RANGE) / 2f;
            range += Random.Range(0, range);

            ChoseRandomDirection(range);
            Context.MoveTo(Context.transform.position + randomDirection * range);
            idleTimer = 0f;
        }
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<GlorbStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }



    // Extra methods
    void ChoseRandomDirection(float _range)
    {
        bool validDirection = false;

        do
        {
            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);

            randomDirection = new Vector3(randomX, 0, randomZ);

            if (randomDirection == Vector3.zero)
            {
                continue;
            }

            randomDirection.Normalize();

            // aide à éviter les murs
            if (Physics.Raycast(Context.transform.position + new Vector3(0, 1, 0), randomDirection, _range, LayerMask.GetMask("Map")))
            {
                continue;
            }

            validDirection = true;
        } while (!validDirection);
    }
}
