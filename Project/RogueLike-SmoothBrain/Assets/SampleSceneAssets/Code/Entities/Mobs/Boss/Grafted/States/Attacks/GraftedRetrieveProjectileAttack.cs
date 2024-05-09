// ---[ STATE ] ---
// replace "GraftedRetrieveProjectileAttack_STATEMACHINE" by your state machine class name.
//
// Here you can see an exemple of the CheckSwitchStates method:
// protected override void CheckSwitchStates()
// {
//      if (isRunning)
//      {
//          SwitchState(Factory.GetState<RunningState>());
//      }
// }

using StateMachine;
using UnityEngine; // include all scripts about StateMachines

public class GraftedRetrieveProjectileAttack : BaseState<GraftedStateMachine>
{
    public GraftedRetrieveProjectileAttack(GraftedStateMachine currentContext, StateFactory<GraftedStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;
    float rangeSecurity = 0f;

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (attackEnded)
        {
            SwitchState(Factory.GetState<GraftedTriggeredState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        attackEnded = false;

        Context.Agent.isStopped = true;
        Context.FreezeRotation = true;
        rangeSecurity = 0f;

        Context.Animator.ResetTrigger("Retrieve");
        Context.Animator.SetTrigger("Retrieve");
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;
        Context.FreezeRotation = false;
        Context.Sounds.retrievingProjectileSound.Stop();

        Context.HasProjectile = true;
        Context.Cooldown = 2f + Random.Range(-0.25f, 0.25f);
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        rangeSecurity += Time.deltaTime;

        Context.Projectile.SetTempSpeed(Context.Projectile.Speed * 0.25f);
        Context.Sounds.retrievingProjectileSound.Play(Context.transform.position);

        if (Context.Projectile.onTarget)
        {
            Context.Projectile.SetDirection(Context.transform.position + new Vector3(0, Context.Height / 6f, 0) - Context.Projectile.transform.position);
            Context.Projectile.SetCollisionImmune(true);
            Context.Projectile.onTarget = false;
        }
        else if (Context.Projectile.OnLauncher(Context.transform.position + new Vector3(0, Context.Height / 6f, 0)) || rangeSecurity > 5f)
        {
            Object.Destroy(Context.Projectile.gameObject);

            Context.Animator.ResetTrigger("Retrieved");
            Context.Animator.SetTrigger("Retrieved");

            attackEnded = true;
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<GraftedStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
