using StateMachine;
using UnityEngine;

public class KlopsAttackState : BaseState<KlopsStateMachine>
{
    bool endState = false;
    float timeBeforeChangeState = 1.5f;
    float currentTime = 0f;
    bool hasShot = false;
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

        hasShot = false;
        endState = false;
    }

    protected override void ExitState()
    {
    }

    protected override void UpdateState()
    {
        currentTime += Time.deltaTime;
        Context.transform.LookAt(Utilities.Player.transform);
        if(!hasShot && currentTime >= 0.2f)
        {
            GameObject fireball = GameObject.Instantiate(Context.FireballPrefab, Context.FireballSpawn.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().direction = Utilities.Player.transform.position - Context.transform.position;
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
