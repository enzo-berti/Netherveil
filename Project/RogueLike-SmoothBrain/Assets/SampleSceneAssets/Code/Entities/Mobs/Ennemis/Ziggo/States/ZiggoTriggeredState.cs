using StateMachine; // include all scripts about StateMachines
using Unity.VisualScripting;
using UnityEngine;

public class ZiggoTriggeredState : BaseState<ZiggoStateMachine>
{
    public ZiggoTriggeredState(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 0.5f;
    int randDir = 0;
    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        //if (Context.IsDeath)
        //{
        //    SwitchState(Factory.GetState<ZiggoDeathState>());
        //}
        //else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
        //{
        //    SwitchState(Factory.GetState<ZiggoTurnAroundTargetState>());
        //}
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
        Vector3 pointToReach;

        if (Vector3.Distance(Context.Player.transform.position, Context.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            //pointToReach = Context.Player.transform.position + (Context.transform.position - Context.Player.transform.position).normalized * Context.Stats.GetValue(Stat.ATK_RANGE);
            pointToReach = Context.Player.transform.position;
        }
        else
        {
            Vector3 mobToPlayer = Context.Player.transform.position - Context.transform.position;
            mobToPlayer.y = 0;
            mobToPlayer.Normalize();
            pointToReach = Context.transform.position + new Vector3(-mobToPlayer.z, 0, mobToPlayer.x);
        }

        Context.MoveTo(pointToReach);

        //if (Time.time - elapsedTimeMovement > delayBetweenMovement)
        //{
        //    if (randDir == 0)
        //    {
        //        Context.MoveTo(Context.transform.position + new Vector3(-direction.z, 0, direction.x));
        //    }
        //    else
        //    {
        //        Context.MoveTo(Context.transform.position + new Vector3(direction.z, 0, -direction.x));
        //    }

        //    if (Time.time - elapsedTimeMovement > delayBetweenMovement * 3)
        //        elapsedTimeMovement = Time.time;
        //}
        //else
        //{
        //    randDir = Random.Range(0, 2);

        //    if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
        //        Context.MoveTo(Context.transform.position + direction * Context.NormalSpeed);
        //}

        Context.MoveTo(Context.Player.transform.position);
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
