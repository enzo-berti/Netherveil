// ---[ STATE ] ---
// replace "FinalBossSummoningAttack_STATEMACHINE" by your state machine class name.
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

public class ErecrosSummoningAttack : BaseState<ErecrosStateMachine>
{
    public ErecrosSummoningAttack(ErecrosStateMachine currentContext, StateFactory<ErecrosStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;

    List<Mobs> enemies = new();
    float spawnRadius = 4f;

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

        int iterations = (Context.CurrentPhase > 1 || Context.CurrentPart > 1) ? 5 : 3; 

        for (int i = 0; i < iterations; i++)
        {
            Vector3 spawnVector = (Context.Player.transform.position - Context.transform.position).normalized * spawnRadius;
            spawnVector = Quaternion.AngleAxis(360 / iterations * i, Vector3.up) * spawnVector;

            Vector3 mobPos = Context.transform.position + spawnVector;

            enemies.Add(Object.Instantiate(Context.EnemiesPrefabs[Random.Range(0, Context.EnemiesPrefabs.Length)], mobPos, Quaternion.identity).GetComponentInChildren<Mobs>());
        }

        Context.ShieldVFX.Reinit();
        Context.ShieldVFX.Play();
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;
        Context.AttackCooldown = 2f + Random.Range(-0.25f, 0.25f);

        Context.ShieldVFX.Stop();
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Context.LookAtPlayer();

        foreach (Mobs enemy in enemies)
        {
            if (enemy.GetComponentInChildren<Mobs>().Stats.GetValue(Stat.HP) <= 0)
            {
                enemies.Remove(enemy);
            }
        }

        attackEnded = enemies.Count <= 0f;
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ErecrosStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
