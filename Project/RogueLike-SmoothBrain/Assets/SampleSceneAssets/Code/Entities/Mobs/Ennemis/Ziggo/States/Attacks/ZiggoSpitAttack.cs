using StateMachine; // include all scripts about StateMachines
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZiggoSpitAttack : BaseState<ZiggoStateMachine>
{
    public ZiggoSpitAttack(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private bool attackEnded;
    private float puddleDuration = 1.5f;

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (attackEnded)
        {
            SwitchState(Factory.GetState<ZiggoTriggeredState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        attackEnded = false;
        Context.SpitAttackCoroutine = Context.StartCoroutine(SpitAttack());
        Context.Agent.isStopped = true;

        Context.Sounds.moveSound.Stop();
        Context.Sounds.spitSound.Play(Context.transform.position);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.SpitCooldown = 2f;
        Context.Agent.isStopped = false;
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        if (Context.Player)
        {
            // rotate
            Quaternion lookRotation = Quaternion.LookRotation(Context.Player.transform.position, Context.transform.position);
            lookRotation.x = 0;
            lookRotation.z = 0;
            Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, lookRotation, 10f * Time.deltaTime);
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    private IEnumerator SpitAttack()
    {
        Context.Projectile.SetActive(true);

        ZiggoProjectile projectile = Context.Projectile.GetComponent<ZiggoProjectile>();
        Transform originalParent = Context.Projectile.transform.parent;

        Context.Animator.ResetTrigger("Spit");
        Context.Animator.SetTrigger("Spit");

        float timeToThrow = 1f;
        float maxHeight = 2f;

        Vector2 pointToReach2D = MathsExtension.GetRandomPointOnCircle(new Vector2(Context.Player.transform.position.x, Context.Player.transform.position.z), 1f);
        Vector3 pointToReach3D = new(pointToReach2D.x, Context.Player.transform.position.y, pointToReach2D.y);

        if (NavMesh.SamplePosition(pointToReach3D, out var hit, 3, -1))
        {
            pointToReach3D = hit.position;
        }

        Context.Projectile.transform.rotation = Quaternion.identity;

        Context.Projectile.transform.parent = null;


        projectile.ThrowToPos(pointToReach3D, timeToThrow, maxHeight);

        yield return new WaitForSeconds(timeToThrow);

        projectile.PoisonBallVFX.gameObject.SetActive(false);
        projectile.PoisonPuddleVFX.transform.parent = null;
        projectile.PoisonPuddleVFX.transform.position = new Vector3(projectile.transform.position.x, Utilities.Player.transform.position.y, projectile.transform.position.z);
        projectile.PoisonPuddleVFX.transform.localScale = Vector3.one;

        projectile.PoisonPuddleVFX.SetFloat("Duration", puddleDuration);
        //this is because vfx is based on plane size so 1 plane size equals 5 unity units
        float planeLength = 5f;
        projectile.PoisonPuddleVFX.SetFloat("Diameter", projectile.FlaqueRadius * 2f / planeLength);
        projectile.PoisonPuddleVFX.Play();

        // Splatter
        float maxDiameter = 20f;
        float maxThickness = 0.2f;
        float coeff = 20 / (1 - maxThickness);
        float speed = 3f;

        Context.Sounds.splatterSound.Play(Context.Projectile.transform.position);

        do
        {
            yield return null;
            Vector3 scale = Context.Projectile.transform.localScale;
            scale.x = scale.x >= maxDiameter ? maxDiameter : scale.x + Time.deltaTime * coeff * speed;
            scale.z = scale.z >= maxDiameter ? maxDiameter : scale.z + Time.deltaTime * coeff * speed;
            scale.y = scale.y <= maxThickness ? maxThickness : scale.y - Time.deltaTime * speed;

            Context.Projectile.transform.localScale = scale;

        } while (Context.Projectile.transform.localScale.x != maxDiameter || Context.Projectile.transform.localScale.y != maxThickness);

        attackEnded = true;
        yield return new WaitForSeconds(puddleDuration);

        projectile.PoisonPuddleVFX.Stop();
        projectile.PoisonPuddleVFX.transform.parent = Context.Projectile.transform;
        projectile.PoisonBallVFX.gameObject.SetActive(true);
        Context.Projectile.transform.parent = originalParent;
        Context.Projectile.transform.localPosition = Vector3.zero;
        Context.Projectile.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        Context.Projectile.SetActive(false);
    }
}

