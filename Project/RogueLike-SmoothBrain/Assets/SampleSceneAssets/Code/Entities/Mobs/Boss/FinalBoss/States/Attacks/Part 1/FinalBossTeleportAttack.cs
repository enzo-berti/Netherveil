// ---[ STATE ] ---
// replace "FinalBossTeleportatAttack_STATEMACHINE" by your state machine class name.
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
using UnityEngine.AI;

public class FinalBossTeleportAttack : BaseState<FinalBossStateMachine>
{
    public FinalBossTeleportAttack(FinalBossStateMachine currentContext, StateFactory<FinalBossStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        SwitchState(Factory.GetState<FinalBossTriggeredState>());
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        Vector3 newRandomPos = Context.transform.position;
        NavMeshHit hit;

        do
        {
            Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
            Vector3 randomDirection3D = new Vector3(randomDirection2D.x, 0, randomDirection2D.y);

            newRandomPos = Context.Player.transform.position + randomDirection3D * Random.Range(8f, 12f);

        } while (!NavMesh.SamplePosition(newRandomPos, out hit, 0.1f, NavMesh.AllAreas) || newRandomPos == Context.Player.transform.position);

        GameObject clone = Object.Instantiate(Context.ClonePrefab, Context.transform.position, Context.transform.rotation);
        clone.GetComponentInChildren<FinalBossCloneBehaviour>().Explode(Context);
        Context.transform.position = newRandomPos;
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.AttackCooldown = 2f + Random.Range(-0.25f, 0.25f);
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {

    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<FinalBossStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
