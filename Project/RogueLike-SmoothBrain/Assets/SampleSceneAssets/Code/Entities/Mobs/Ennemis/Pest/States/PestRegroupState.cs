using StateMachine; // include all script about stateMachine
using System.Collections;
using System.Linq;
using UnityEngine;

public class PestRegroupState : BaseState<PestStateMachine>
{
    public PestRegroupState(PestStateMachine currentContext, StateFactory<PestStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private float elapsedTimeMovement = 0.0f;
    private float delayBetweenMovement = 2.0f;

    // This method will be call every Update to check and change a state.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<PestDeathState>());
        }
        else if (Context.Target != null)
        {
            SwitchState(Factory.GetState<PestFollowTargetState>());
        }
        else if (!Context.NearbyEntities.FirstOrDefault(x => x is IPest))
        {
            SwitchState(Factory.GetState<PestPatrolState>());
        }
    }

    // This method will be call only one time before the update.
    protected override void EnterState()
    {
        elapsedTimeMovement = Time.time;
    }

    // This method will be call only one time after the last update.
    protected override void ExitState()
    { }

    // This method will be call every frame.
    protected override void UpdateState()
    {
        // Delay
        if (Time.time - elapsedTimeMovement < delayBetweenMovement)
            return;

        elapsedTimeMovement = Time.time;

        // Movement
        IPest[] pests = Context.NearbyEntities.Where(x => x != null)
                                              .Select(x => x.GetComponent<IPest>())
                                              .Where(x => x != null)
                                              .ToArray();
        
        if (pests.Any())
        {
            Vector3 averagePos = Vector3.zero;
            foreach (IPest pest in pests)
            {
                averagePos += (pest as MonoBehaviour).transform.position * Random.Range(0.5f, 1.5f);
            }
            averagePos /= pests.Count();

            Vector3 avoidPos = Vector3.zero;
            foreach (IPest pest in pests)
            {
                Vector3 direction = ((pest as MonoBehaviour).transform.position - Context.transform.position).normalized;
                float distance = 1 - Vector3.Distance(Context.transform.position, (pest as MonoBehaviour).transform.position);
                avoidPos += direction * distance * Random.Range(0.5f, 1.5f);
            }
            avoidPos /= pests.Count();

            Context.MoveTo(averagePos + avoidPos);
        }
    }

    // This method will be call on state changement.
    // No need to modify this method !
    protected override void SwitchState(BaseState<PestStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.CurrentState = newState;
    }
}
