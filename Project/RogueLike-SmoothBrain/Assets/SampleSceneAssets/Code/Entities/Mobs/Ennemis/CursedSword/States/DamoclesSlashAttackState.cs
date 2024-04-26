using StateMachine; // include all script about stateMachine
using System.Collections;
using System.Linq;
using UnityEngine;

public class DamoclesSlashAttackState : BaseState<DamoclesStateMachine>
{
    public DamoclesSlashAttackState(DamoclesStateMachine currentContext, StateFactory<DamoclesStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private enum State
    {
        Start,
        RunIn,
        Slash,
        BackSlash,
        Backward
    }

    private State curState = State.Start;
    private Coroutine slashRoutine;
    private Vector3 previousPos;
    private Vector3 closeMovement;
    private bool stateEnded = false;
    private float elapsedTimeMovement = 0.0f;
    private float timeBeforeFirstSlash = 4.0f;
    private float timeBetweenSlash = 1.25f;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<DamoclesDeathState>());
        }
        else if (stateEnded)
        {
            SwitchState(Factory.GetState<DamoclesEnGardeState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        curState = State.Start;
        stateEnded = false;
        Context.Stats.SetValue(Stat.SPEED, 6);
        Context.Stats.SetValue(Stat.KNOCKBACK_DISTANCE, 0);
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
        if (slashRoutine != null)
        {
            Context.StopCoroutine(slashRoutine);
            slashRoutine = null;
        }
        //Context.Animator.ResetTrigger(Context.);
        //Context.Animator.SetTrigger(Context.);
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        if (curState == State.Start)
        {
            Vector3 positionToLookAt = new Vector3(Context.Target.position.x, Context.transform.position.y, Context.Target.position.z);
            Context.transform.LookAt(positionToLookAt);
            previousPos = Context.transform.position;
            Vector3 dist = Context.Target.position - Context.transform.position;
            float magnitude = Mathf.Max(0f, dist.magnitude - 2.25f);
            closeMovement = dist.normalized * magnitude;
            curState = State.RunIn;

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        else if (curState == State.RunIn)
        {
            Context.MoveTo(previousPos + closeMovement);
            if (Vector3.Distance(Context.transform.position, previousPos + closeMovement) < 0.1)
            {
                // Delay
                if (Time.time - elapsedTimeMovement < timeBeforeFirstSlash)
                    return;

                elapsedTimeMovement = Time.time;

                curState = State.Slash;
                slashRoutine = Context.StartCoroutine(SlashCoroutine(Context.Attack2Collider));
                elapsedTimeMovement = Time.time;
            }

            //Context.Animator.ResetTrigger(Context.);
            //Context.Animator.SetTrigger(Context.);
        }
        else if (curState == State.Slash)
        {
            // Delay
            if (Time.time - elapsedTimeMovement < timeBetweenSlash)
                return;

            elapsedTimeMovement = Time.time;
            curState = State.BackSlash;
            slashRoutine = Context.StartCoroutine(SlashCoroutine(Context.Attack3Collider)); //collider à changer
        }
        else if (curState == State.BackSlash)
        {
            if (slashRoutine != null)
            {
                curState = State.Backward;
            }
        }
        else if (curState == State.Backward)
        {
            if (Vector3.Distance(Context.Target.position, Context.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE) - 1f)
            {
                Context.MoveTo(previousPos);
            }
            else
            {
                stateEnded = true;
            }
        }
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<DamoclesStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    private IEnumerator SlashCoroutine(BoxCollider attackCollider)
    {
        IDamageable player = PhysicsExtensions.CheckAttackCollideRayCheck(attackCollider, Context.transform.position, "Player", LayerMask.GetMask("Entity"))
                                              .Select(x => x.GetComponent<IDamageable>())
                                              .Where(x => x != null)
                                              .FirstOrDefault();

        if (player != null)
        {
            Context.Attack(player);
        }
        else
        {
            int randSound = Random.Range(0, 3);

            switch (randSound)
            {
                case 0:
                    Context.DamoclesSound.slashSound.Play(Context.transform.position);
                    break;
                case 1:
                    Context.DamoclesSound.slashSound2.Play(Context.transform.position);
                    break;
                case 2:
                    Context.DamoclesSound.slashSound3.Play(Context.transform.position);
                    break;
                default:
                    break;
            }
        }

        slashRoutine = null;
        yield return null;
    }

}
