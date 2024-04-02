using StateMachine; // include all script about stateMachine
using System.Linq;
using UnityEngine;

public class PestFollowTargetState : BaseState<PestStateMachine>
{
    public PestFollowTargetState(PestStateMachine currentContext, StateFactory<PestStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 1.5f;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<PestDeathState>());
        }
        else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            SwitchState(Factory.GetState<PestAttackState>());
        }
        else if (Context.Target == null)
        {
            if (Context.NearbyEntities.FirstOrDefault(x => x is IPest))
            {
                SwitchState(Factory.GetState<PestRegroupState>());
            }
            else
            {
                SwitchState(Factory.GetState<PestPatrolState>());
            }
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
    {
        // Delay
        if (Time.time - elapsedTimeMovement < delayBetweenMovement)
            return;

        elapsedTimeMovement = Time.time;

        Vector3 direction = (Context.Target.position - Context.transform.position).normalized;

        Context.MoveTo(Context.transform.position + direction * Context.NormalSpeed);
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.CurrentState = newState;
    }
}
