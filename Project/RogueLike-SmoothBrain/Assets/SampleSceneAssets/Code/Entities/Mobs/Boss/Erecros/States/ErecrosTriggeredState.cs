// ---[ STATE ] ---
// replace "FinalBossTriggeredState_STATEMACHINE" by your state machine class name.
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
using System;
using System.Collections.Generic;
using UnityEngine;

public class ErecrosTriggeredState : BaseState<ErecrosStateMachine>
{
    public ErecrosTriggeredState(ErecrosStateMachine currentContext, StateFactory<ErecrosStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    Type lastAttack;

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        UseDebugKeys();

        //if (Context.AttackCooldown <= 0f)
        //{
        //    List<Type> availableAttacks = GetAvailableAttacks();

        //    lastAttack = availableAttacks[UnityEngine.Random.Range(0, availableAttacks.Count)];
        //    SwitchState(Factory.GetState(lastAttack));
        //}
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        Context.Animator.SetBool("Walk", true);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Animator.SetBool("Walk", false);
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Context.AttackCooldown -= Time.deltaTime;

        Context.MoveTo(Context.Player.transform.position);

        Context.Animator.SetBool("Walk", Context.Agent.remainingDistance > Context.Agent.stoppingDistance);

    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ErecrosStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    List<Type> GetAvailableAttacks()
    {
        List<Type> availableAttacks = new()
        {
            typeof(ErecrosTriangleDashAttack),
            typeof(ErecrosSummoningAttack),
            typeof(ErecrosTeleportAttack),
        };

        if (Context.CurrentPhase > 1 && Context.CurrentPart == 1)
        {
            availableAttacks.Add(typeof(ErecrosPrisonAttack));
        }

        return availableAttacks;
    }

    #region Extra methods
    void UseDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchState(Factory.GetState<ErecrosTriangleDashAttack>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchState(Factory.GetState<ErecrosSummoningAttack>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchState(Factory.GetState<ErecrosTeleportAttack>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchState(Factory.GetState<ErecrosPrisonAttack>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchState(Factory.GetState<ErecrosShockwave>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchState(Factory.GetState<ErecrosWeaponThrow>());
        }
    }
    #endregion
}
