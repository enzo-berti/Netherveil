// ---[ STATE ] ---
// replace "SonielBerserkState_STATEMACHINE" by your state machine class name.
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

public class SonielBerserk : BaseState<SonielStateMachine>
{
    public SonielBerserk(SonielStateMachine currentContext, StateFactory<SonielStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    enum BerserkState
    {
        RUSHING,
        STUNNED
    }

    BerserkState currentState;
    float attackDuration;

    Vector3 direction;

    // anim hash
    int rushingHash = Animator.StringToHash("Rushing");
    int stunnedHash = Animator.StringToHash("Stunned");

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (currentState == BerserkState.STUNNED && attackDuration >= Context.Animator.GetCurrentAnimatorClipInfo(0).Length + 0.7f) // +0.7s temps de la transition vers idle
        {
            SwitchState(Factory.GetState<SonielTriggeredState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        currentState = BerserkState.RUSHING;

        direction = Context.Player.transform.position - Context.transform.position;
        direction.y = 0;
        direction.Normalize();

        Context.Animator.ResetTrigger(rushingHash);
        Context.Animator.SetTrigger(rushingHash);

        Context.Stats.SetCoeffValue(Stat.SPEED, 3f);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;

        Context.Stats.SetCoeffValue(Stat.SPEED, 1f);

        Context.PlayerHit = false;

        Context.AttackCooldown = 2f + Random.Range(-0.5f, 0.5f);

        // DEBUG
        Context.DisableHitboxes();
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        if (currentState == BerserkState.STUNNED)
        {
            Context.Agent.isStopped = true;
            attackDuration += Time.deltaTime;
        }
        else
        {
            Context.MoveTo(Context.transform.position + direction * (Context.Agent.stoppingDistance + 0.1f));

            if (!Context.PlayerHit)
            {
                Context.AttackCollide(Context.Attacks[(int)SonielStateMachine.SonielAttacks.BERSERK].data, debugMode: Context.DebugMode);
            }

            if (Physics.Raycast(Context.transform.position + new Vector3(0, 1, 0), Context.transform.forward, 4.5f, LayerMask.GetMask("Map")))
            {
                Context.Animator.ResetTrigger(stunnedHash);
                Context.Animator.SetTrigger(stunnedHash);

                attackDuration = 0;

                // DEBUG
                Context.DisableHitboxes();

                currentState = BerserkState.STUNNED;
            }
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<SonielStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
