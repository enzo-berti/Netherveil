// ---[ STATE ] ---
// replace "FinalBossTriangleDashAttack_STATEMACHINE" by your state machine class name.
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
using System.Collections.Generic;
using UnityEngine;

public class ErecrosTriangleDashAttack : BaseState<ErecrosStateMachine>
{
    public ErecrosTriangleDashAttack(ErecrosStateMachine currentContext, StateFactory<ErecrosStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;
    List<ErecrosCloneBehaviour> cloneBehaviours = new();

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (attackEnded)
        {
            SwitchState(Factory.GetState<ErecrosTriggeredState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        Context.Agent.isStopped = true;

        int clonesAmount = (Context.CurrentPart > 1 || Context.CurrentPhase > 1) ? 2 : 4;
        for (int i = 0; i < clonesAmount; i++)
        {
            GameObject clone = Object.Instantiate(Context.ClonePrefab, Context.transform.position, Context.transform.rotation);

            Vector3 spawnVector = Context.transform.position - Context.Player.transform.position;
            spawnVector = Quaternion.AngleAxis(360 / (clonesAmount + 1) * (i + 1), Vector3.up) * spawnVector;
            clone.transform.position = Context.Player.transform.position + spawnVector;

            cloneBehaviours.Add(clone.GetComponentInChildren<ErecrosCloneBehaviour>());
        }

    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ErecrosStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
