// ---[ STATE ] ---
// replace "FinalBossCircularDashAttack_STATEMACHINE" by your state machine class name.
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
using UnityEngine.UIElements;

public class ErecrosPrisonAttack : BaseState<ErecrosStateMachine>
{
    public ErecrosPrisonAttack(ErecrosStateMachine currentContext, StateFactory<ErecrosStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;
    float torusRadius = 0f;
    float dashDistance;

    GameObject torus;

    List<Transform> clones = new();

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
        torus = Object.Instantiate(Context.PrisonTorusPrefab, Context.Player.transform.position + Vector3.up, Quaternion.identity);
        torusRadius = torus.GetComponent<Renderer>().bounds.size.x / 2f;

        Context.Agent.isStopped = true;

        int clonesAmount = 16;

        for (int i = 0; i < clonesAmount; i++)
        {
            Vector3 spawnVector = (Context.transform.position - Context.Player.transform.position).normalized * torusRadius;
            spawnVector = Quaternion.AngleAxis(360f / clonesAmount * i, Vector3.up) * spawnVector;

            GameObject clone = Object.Instantiate(Context.ClonePrefab, Context.Player.transform.position + spawnVector, Context.transform.rotation);

            FacePlayer(clone.transform);
            clones.Add(clone.transform);
        }
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;

        Object.Destroy(torus);

        foreach (Transform clone in clones)
        {
            Object.Destroy(clone.gameObject);
        }
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        if (Input.GetKey(KeyCode.V))
        {
            attackEnded = true;
        }

        if (clones.Count > 0)
        {
            if (dashDistance == 0f)
            {
                FacePlayer(clones[0].transform);
            }

            clones[0].position += clones[0].forward * 15f * Time.deltaTime;
            dashDistance += 15f * Time.deltaTime;

            if (!Context.PlayerHit)
            {
                if (clones[0].GetComponentInChildren<ErecrosCloneBehaviour>().AttackCollide(Context))
                {
                    Context.PlayerHit = true;
                }
            }

            if (dashDistance > torusRadius * 2f)
            {
                Object.Destroy(clones[0].gameObject);
                clones.RemoveAt(0);
                dashDistance = 0f;
                Context.PlayerHit = false;
                return;
            }
        }
        else
        {
            attackEnded = true;
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ErecrosStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    void FacePlayer(Transform _sender)
    {
        Vector3 cloneToPlayer = Context.Player.transform.position - _sender.position;
        cloneToPlayer.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(cloneToPlayer);
        lookRotation.x = 0;
        lookRotation.z = 0;

        _sender.transform.rotation = lookRotation;
    }
}
