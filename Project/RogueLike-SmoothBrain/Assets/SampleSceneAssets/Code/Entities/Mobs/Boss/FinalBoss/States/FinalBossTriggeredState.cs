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

public class FinalBossTriggeredState : BaseState<FinalBossStateMachine>
{
    public FinalBossTriggeredState(FinalBossStateMachine currentContext, StateFactory<FinalBossStateMachine> currentFactory)
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

    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {

    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Context.AttackCooldown -= Time.deltaTime;

        Context.MoveTo(Context.Player.transform.position);
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<FinalBossStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    List<Type> GetAvailableAttacks()
    {
        List<Type> availableAttacks = new()
        {
            typeof(FinalBossTriangleDashAttack),
            typeof(FinalBossSummoningAttack),
            typeof(FinalBossTeleportAttack),
            typeof(FinalBossPrisonAttack)
        };

        return availableAttacks;
    }

    #region Extra methods
    void UseDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchState(Factory.GetState<FinalBossTriangleDashAttack>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchState(Factory.GetState<FinalBossSummoningAttack>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchState(Factory.GetState<FinalBossTeleportAttack>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchState(Factory.GetState<FinalBossPrisonAttack>());
        }
    }
    #endregion
}
