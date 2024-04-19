using StateMachine; // include all script about stateMachine
using Unity.VisualScripting;
using UnityEngine;

public class DamoclesEnGardeState : BaseState<DamoclesStateMachine>
{
    public DamoclesEnGardeState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private int nextState = 0;
    private bool stateEnded = false;
    private float elapsedTimeMovement = 0.0f;
    private float guardTime = 4f;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (Vector3.Distance(Context.transform.position, Context.Target.transform.position) > Context.Stats.GetValue(Stat.ATK_RANGE))
        {
            SwitchState(Factory.GetState<DamoclesFollowTargetState>());
        }
        else if (stateEnded)
        {
            stateEnded = false;
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

    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {

    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        Debug.Log("guard");
        Vector3 direction = Context.Target.transform.position - Context.transform.position;
        direction.Normalize();

        Context.Move(new Vector3(-direction.z, 0, direction.x) * Time.deltaTime);

        // Delay
        if (Time.time - elapsedTimeMovement < guardTime)
            return;

        Debug.Log("endguard");
        elapsedTimeMovement = Time.time;

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
