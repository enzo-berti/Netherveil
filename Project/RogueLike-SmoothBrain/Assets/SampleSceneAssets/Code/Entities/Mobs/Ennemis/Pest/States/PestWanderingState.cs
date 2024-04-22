using StateMachine; // include all script about stateMachine
using System.Linq;
using UnityEngine;

public class PestWanderingState : BaseState<PestStateMachine>
{
    public PestWanderingState(PestStateMachine currentContext, StateFactory<PestStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private Vector3 randomDirection;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.Player != null)
        {
            SwitchState(Factory.GetState<PestTriggeredState>());
        }
        else if (Context.NearbyEntities.FirstOrDefault(x => x is IPest))
        {
            SwitchState(Factory.GetState<PestRegroupState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        Context.MovementTimer = Context.MovementDelay / 2f;
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    {

    }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        if (Context.CanMove)
        {
            float minRange = Context.Stats.GetValue(Stat.ATK_RANGE) - Context.Stats.GetValue(Stat.ATK_RANGE) / 4f;
            float maxRange = Context.Stats.GetValue(Stat.ATK_RANGE);

            Context.MoveTo(Context.GetRandomPointOnWanderZone(Context.transform.position, minRange, maxRange));
            Context.MovementTimer = 0f;
        }
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.CurrentState = newState;
    }

    #region Extra methods
    void ChoseRandomDirection(float _range)
    {
        bool validDirection = false;

        do
        {
            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);

            randomDirection = new Vector3(randomX, 0, randomZ);

            if (randomDirection == Vector3.zero)
            {
                continue;
            }

            randomDirection.Normalize();

            Debug.DrawRay(Context.transform.position + new Vector3(0, 0.2f, 0), randomDirection * _range, Color.green);

            // aide à éviter les murs
            if (Physics.Raycast(Context.transform.position + new Vector3(0, 1, 0), randomDirection, _range, LayerMask.GetMask("Map")))
            {
                continue;
            }

            validDirection = true;
        } while (!validDirection);
    }

    #endregion
}
