using StateMachine; // include all scripts about StateMachines
using Unity.VisualScripting;
using UnityEngine;

public class ZiggoTriggeredState : BaseState<ZiggoStateMachine>
{
    public ZiggoTriggeredState(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (!Context.Player)
        {
            if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
            {
                SwitchState(Factory.GetState<ZiggoWanderingState>());
            }
        }
        else if (Vector3.Distance(Context.Player.transform.position, Context.transform.position) <= Context.Stats.GetValue(Stat.ATK_RANGE))
        {

            if (Context.DashCooldown <= 0f)
            {
                if (Vector3.Angle(Context.Player.transform.forward, Context.Player.transform.position - Context.transform.position) < 45f / 2f)
                {
                    SwitchState(Factory.GetState<ZiggoDashAttack>());
                }
            }
            if (Context.SpitCooldown <= 0f)
            {
                SwitchState(Factory.GetState<ZiggoDashAttack>());
            }
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {
    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        UpdateAttackCooldowns();

        if (Context.Player)
        {
            Vector3 pointToReach;

            Vector3 mobToPlayer = Context.Player.transform.position - Context.transform.position;
            mobToPlayer.y = 0f;
            float distanceToPlayer = mobToPlayer.magnitude;

            if (distanceToPlayer > Context.Stats.GetValue(Stat.ATK_RANGE))
            {
                pointToReach = Context.Player.transform.position;
            }
            else
            {
                pointToReach = Context.Player.transform.position + (new Vector3(-mobToPlayer.z, 0, mobToPlayer.x).normalized - mobToPlayer).normalized * Context.Stats.GetValue(Stat.ATK_RANGE) * 0.75f;
            }

            // rotate
            Quaternion lookRotation = Quaternion.LookRotation(pointToReach, Context.transform.position);
            lookRotation.x = 0;
            lookRotation.z = 0;
            Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, lookRotation, 5f * Time.deltaTime);

            Context.MoveTo(pointToReach);
        }

        if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
        {
            Context.Sounds.moveSound.Stop();
        }
        else
        {
            Context.Sounds.moveSound.Play(Context.transform.position);
        }
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    #region Extra methods

    void UpdateAttackCooldowns()
    {
        if (Context.DashCooldown > 0) Context.DashCooldown -= Time.deltaTime;
        if (Context.SpitCooldown > 0) Context.SpitCooldown -= Time.deltaTime;
    }

    #endregion
}
