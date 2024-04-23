using StateMachine; // include all scripts about StateMachines
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZiggoThrowPuddleState : BaseState<ZiggoStateMachine>
{
    public ZiggoThrowPuddleState(ZiggoStateMachine currentContext, StateFactory<ZiggoStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    private bool stateEnded;
    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Context.IsDeath)
        {
            SwitchState(Factory.GetState<ZiggoDeathState>());
        }
        else if (stateEnded)
        {
            SwitchState(Factory.GetState<ZiggoCirclingState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        stateEnded = false;
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Debug.Log("throw");
        //fuite
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<ZiggoStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }

    private IEnumerator LongRangeAttack()
    {
        yield return null;
        //Context.Animator.SetTrigger("Attack");
        //float timeToThrow = 0.8f;

        //Vector2 pointToReach2D = MathsExtension.GetPointOnCircle(new Vector2(Context.Target.transform.position.x, Context.Target.transform.position.z), 1f);
        //Vector3 pointToReach3D = new(pointToReach2D.x, Context.Target.transform.position.y, pointToReach2D.y);
        //if (NavMesh.SamplePosition(pointToReach3D, out var hit, 3, -1))
        //{
        //    pointToReach3D = hit.position;
        //}

        //if (Context.gameObject != null)
        //{
            //GameObject bomb = GameObject.Instantiate(Context.Stats, Context.Hand);
            //yield return new WaitWhile(() => Context.HasLaunchAnim == false);
            //bomb.transform.rotation = Quaternion.identity;
            //bomb.transform.parent = null;

            //ExplodingBomb exploBomb = bomb.GetComponent<ExplodingBomb>();

            //exploBomb.ThrowToPos(Context, pointToReach3D, timeToThrow);
            //exploBomb.SetTimeToExplode(timeToThrow * 1.5f);
            //exploBomb.SetBlastDamages((int)Context.Stats.GetValue(Stat.ATK));
            //exploBomb.Activate();

            //yield return new WaitForSeconds(0.5f);
            //attackFinished = true;
        //}
    }
}
