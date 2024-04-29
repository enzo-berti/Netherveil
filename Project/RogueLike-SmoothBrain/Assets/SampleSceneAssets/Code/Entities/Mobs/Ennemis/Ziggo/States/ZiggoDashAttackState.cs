using StateMachine; // include all scripts about StateMachines
using UnityEngine;

public class ZiggoDashAttackState : BaseState<ZiggoStateMachine>
{
    public ZiggoDashAttackState(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 0.5f;
    int randDir = 0;
    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<ZiggoDeathState>());
        }
        else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            SwitchState(Factory.GetState<ZiggoTurnAroundTargetState>());
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
        Vector3 direction = (Context.Target.position - Context.transform.position).normalized;

        if (Time.time - elapsedTimeMovement > delayBetweenMovement)
        {
            if (randDir == 0)
            {
                Context.MoveTo(Context.transform.position + new Vector3(-direction.z, 0, direction.x));
            }
            else
            {
                Context.MoveTo(Context.transform.position + new Vector3(direction.z, 0, -direction.x));
            }

            if (Time.time - elapsedTimeMovement > delayBetweenMovement * 3)
                elapsedTimeMovement = Time.time;
        }
        else
        {
            randDir = Random.Range(0, 2);

            if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
                Context.MoveTo(Context.transform.position + direction * Context.NormalSpeed);
        }

        ///// marche pas
        /*Vector3 M = Context.transform.position;
        Vector3 O = Context.Target.transform.position;
        Vector3 nextPos = new Vector3(0,0,0);
        float xM, yM;
        float angle = Mathf.PI / 180;
        xM = M.x - O.x;
        yM = M.y - O.y;
        nextPos.x = Mathf.Round(xM * Mathf.Cos(angle) + yM * Mathf.Sin(angle) + O.x);
        nextPos.y = Mathf.Round(-xM * Mathf.Sin(angle) + yM * Mathf.Cos(angle) + O.y);


        if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
            Context.MoveTo(Context.transform.position + nextPos * Context.NormalSpeed);*/
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
