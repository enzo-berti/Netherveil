using StateMachine; // include all script about stateMachine
using UnityEngine;

public class DamoclesEnGardeState : BaseState<DamoclesStateMachine>
{
    public DamoclesEnGardeState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private int nextState = 0;
    private bool stateEnded = false;
    private float elapsedTimeMovement = 0.0f;
    private float elapsedRotaTime = 0.0f;
    private float guardTime = 4f;
    private float guardRotaTime = 1.5f;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.VISION_RANGE) * 2/3)
        {
            SwitchState(Factory.GetState<DamoclesFollowTargetState>());
        }
        else if (stateEnded)
        {
            nextState = Random.Range(0, 2);

            switch (nextState)
            {
                case 0:
                    SwitchState(Factory.GetState<DamoclesSlashAttackState>());
                    break;
                case 1:
                    SwitchState(Factory.GetState<DamoclesJumpAttackState>());
                    break;
            }
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        elapsedTimeMovement = Time.time;
        elapsedRotaTime = Time.time;
        stateEnded = false;
        Context.Stats.SetValue(Stat.SPEED, 3);
        Context.IsInvincibleCount = 1;
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {

    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        Vector3 direction = Context.Target.transform.position - Context.transform.position;
        direction.y = 0;
        direction.Normalize();

        Context.transform.LookAt(Context.Target.transform.position);

        if (Time.time - elapsedRotaTime < guardRotaTime)
        {
            Context.MoveTo(Context.transform.position + new Vector3(-direction.z, 0, direction.x));
        }
        else
        {
            Context.MoveTo(Context.transform.position + new Vector3(direction.z, 0, -direction.x));
            if (Time.time - elapsedRotaTime > guardRotaTime * 2f)
            {
                elapsedRotaTime = Time.time;
            }
        }

        // Delay
        if (Time.time - elapsedTimeMovement < guardTime)
            return;

        elapsedTimeMovement = Time.time;
        Context.Stats.SetValue(Stat.SPEED, 5);
        stateEnded = true;
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
