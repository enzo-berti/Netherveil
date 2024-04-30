using StateMachine; // include all script about stateMachine
using UnityEngine;
using UnityEngine.AI;

public class DamoclesEnGardeState : BaseState<DamoclesStateMachine>
{
    public DamoclesEnGardeState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private int nextState = 0;
    private bool stateEnded = false;
    private float elapsedTimeMovement = 0.0f;
    private float elapsedRotaTime = 0.0f;
    private float guardTime = 2.5f;
    private float guardRotaTime = 1.5f;
    private float timerForCircle = Mathf.PI;
    private bool isTimerIncreasing = true;

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
            nextState = Random.Range(1, 2);

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
        Context.Stats.SetValue(Stat.SPEED, 7);
        Context.IsInvincibleCount = 1;
        Context.Animator.SetTrigger("Guard");
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
       
        if(isTimerIncreasing)
        {
            timerForCircle += Time.deltaTime;
            if (timerForCircle > 2 * Mathf.PI) isTimerIncreasing = false;
        }
        else
        {
            timerForCircle -= Time.deltaTime;
            if (timerForCircle < Mathf.PI) isTimerIncreasing = true;
        }
        Context.transform.LookAt(Context.Target.transform.position);
        float posRadiusCircle = Vector3.Distance(Context.transform.position, Context.Target.position);
        posRadiusCircle = Mathf.Clamp(posRadiusCircle, 2f, 5f);
        Context.MoveTo(Context.Target.position.GetPointOnCircle(posRadiusCircle, timerForCircle));
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
