using StateMachine;
using UnityEngine;

public class KlopsAttackState : BaseState<KlopsStateMachine>
{
    bool endState = false;
    float timeBeforeChangeState = 1.5f;
    float currentTime = 0f;
    public KlopsAttackState(KlopsStateMachine currentContext, StateFactory<KlopsStateMachine> currentFactory) : base(currentContext, currentFactory)
    {
    }

    protected override void CheckSwitchStates()
    {
        if(endState)
        {
            SwitchState(Factory.GetState<KlopsPatrolState>());
        }
    }

    protected override void EnterState()
    {
        currentTime = 0f;
        Context.transform.LookAt(Context.Player.transform.position);
        GameObject fireball = GameObject.Instantiate(Context.FireballPrefab, Context.FireballSpawn.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().direction = Context.Player.transform.position - Context.transform.position;
        
        endState = false;
    }

    protected override void ExitState()
    {
    }

    protected override void UpdateState()
    {
        currentTime += Time.deltaTime;
        Context.transform.LookAt(Context.Player.transform);
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
