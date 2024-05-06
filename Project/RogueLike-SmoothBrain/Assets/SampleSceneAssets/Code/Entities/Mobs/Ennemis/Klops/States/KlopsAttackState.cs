using StateMachine;
using UnityEngine;

public class KlopsAttackState : BaseState<KlopsStateMachine>
{
    bool endState = false;
    float timeBeforeChangeState = 1.5f;
    float currentTime = 0f;
    bool hasShot = false;
    // Grosse flemme sorry
    bool hasAnim = false;
    public KlopsAttackState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    protected override void CheckSwitchStates()
    {
        if (endState)
        {
            SwitchState(Factory.GetState<KlopsWanderingState>());
        }
    }

    protected override void EnterState()
    {
        currentTime = 0f;

        hasShot = false;
        hasAnim = false;
        endState = false;

        Context.Agent.isStopped = true;
    }

    protected override void ExitState()
    {
        Context.Agent.isStopped = false;
    }

    protected override void UpdateState()
    {
        currentTime += Time.deltaTime;
        Context.transform.LookAt(Utilities.Player.transform);
        if (!hasAnim && currentTime >= 0.2f)
        {
            Context.Animator.ResetTrigger("Attack");
            Context.Animator.SetTrigger("Attack");
            hasAnim = true;
        }
        if (!hasShot && currentTime >= 0.3f)
        {
            GameObject fireball = GameObject.Instantiate(Context.FireballPrefab, Context.FireballSpawn.position, Quaternion.identity);
            Context.KlopsSound.AttackSound.Play(Context.transform.position);
            fireball.GetComponent<Fireball>().Direction = Utilities.Player.transform.position - Context.transform.position;
            fireball.GetComponent<Fireball>().launcher = Context;
            fireball.transform.LookAt(Utilities.Player.transform);
            hasShot = true;
        }
        if (currentTime >= timeBeforeChangeState)
        {
            endState = true;
        }
    }

    protected override void SwitchState(BaseState<KlopsStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
