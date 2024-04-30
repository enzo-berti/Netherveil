using StateMachine; // include all scripts about StateMachines
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZiggoSpitAttackState : BaseState<ZiggoStateMachine>
{
    public ZiggoSpitAttackState(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private bool attackEnded;

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (attackEnded)
        {
            SwitchState(Factory.GetState<ZiggoDashAttack>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        attackEnded = false;
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.SpitCooldown = 5f;
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {

    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    private IEnumerator LongRangeAttack()
    {
        Context.Animator.ResetTrigger("Spit");
        Context.Animator.SetTrigger("Spit");
        float timeToThrow = 0.8f;
        Vector2 pointToReach2D = MathsExtension.GetRandomPointOnCircle(new Vector2(Context.Player.transform.position.x, Context.Player.transform.position.z), 1f);
        Vector3 pointToReach3D = new(pointToReach2D.x, Context.Player.transform.position.y, pointToReach2D.y);
        if (NavMesh.SamplePosition(pointToReach3D, out var hit, 3, -1))
        {
            pointToReach3D = hit.position;
        }

        Context.Projectile.transform.rotation = Quaternion.identity;
        Context.Projectile.transform.parent = null;

        ZiggoProjectile exploBomb = Context.Projectile.GetComponent<ZiggoProjectile>();

        exploBomb.ThrowToPos(Context, pointToReach3D, timeToThrow);
        //exploBomb.SetTimeToExplode(timeToThrow * 1.5f);
        //exploBomb.SetBlastDamages((int)Context.Stats.GetValue(Stat.ATK));
        //exploBomb.Activate();

        yield return new WaitForSeconds(timeToThrow);

        // splatter
        attackEnded = true;
    }
}

