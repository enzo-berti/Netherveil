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
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class ErecrosTriangleDashAttack : BaseState<ErecrosStateMachine>
{
    public ErecrosTriangleDashAttack(ErecrosStateMachine currentContext, StateFactory<ErecrosStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;
    List<ErecrosCloneBehaviour> cloneBehaviours = new();

    float timeBeforeDash = 0.75f;
    bool dashed = false;
    float dashDistance = 12f;

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

        Vector3 mobToPlayer = Context.Player.transform.position - Context.transform.position;

        Context.transform.position = Context.Player.transform.position - mobToPlayer.normalized * 8f;

        Quaternion lookRotation = Quaternion.LookRotation(mobToPlayer);
        Context.transform.rotation = lookRotation;

        int clonesAmount = (Context.CurrentPart > 1 || Context.CurrentPhase > 1) ? 4 : 2;
        for (int i = 0; i < clonesAmount; i++)
        {
            GameObject clone = Object.Instantiate(Context.ClonePrefab, Context.transform.position, Context.transform.rotation);

            Vector3 spawnVector = Context.transform.position - Context.Player.transform.position;
            spawnVector = Quaternion.AngleAxis(360 / (clonesAmount + 1) * (i + 1), Vector3.up) * spawnVector;

            clone.transform.position = Context.Player.transform.position + spawnVector;

            mobToPlayer = Context.Player.transform.position - clone.transform.position;
            mobToPlayer.y = 0f;

            lookRotation = Quaternion.LookRotation(mobToPlayer);
            clone.transform.rotation = lookRotation;

            cloneBehaviours.Add(clone.GetComponentInChildren<ErecrosCloneBehaviour>());
        }
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;

        foreach (ErecrosCloneBehaviour clone in cloneBehaviours)
        {
            Object.Destroy(clone.gameObject);
        }

        cloneBehaviours.Clear();
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        timeBeforeDash -= Time.deltaTime;

        if (!dashed)
        {
            if (timeBeforeDash <= 0)
            {
                Vector3 mobToPlayer = Context.Player.transform.position - Context.transform.position;
                mobToPlayer.y = 0f;

                Context.Stats.IncreaseCoeffValue(Stat.SPEED, 4f);

                dashed = true;
            }
        }
        else
        {
            dashDistance -= Context.Stats.GetValue(Stat.SPEED) * Time.deltaTime;

            Context.transform.position += Context.transform.forward * Context.Stats.GetValue(Stat.SPEED) * Time.deltaTime;

            foreach (ErecrosCloneBehaviour clone in cloneBehaviours)
            {
                clone.transform.position += clone.transform.forward * Context.Stats.GetValue(Stat.SPEED) * Time.deltaTime;
            }

            if (dashDistance <= 0f)
            {
                Context.Stats.DecreaseCoeffValue(Stat.SPEED, 4f);
                attackEnded = true;
            }
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ErecrosStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
