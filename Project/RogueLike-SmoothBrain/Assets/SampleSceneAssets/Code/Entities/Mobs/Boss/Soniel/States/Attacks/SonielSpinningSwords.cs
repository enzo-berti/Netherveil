// ---[ STATE ] ---
// replace "SonielSpinningSwords_STATEMACHINE" by your state machine class name.
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
using UnityEngine;

public class SonielSpinningSwords : BaseState<SonielStateMachine>
{
    public SonielSpinningSwords(SonielStateMachine currentContext, StateFactory<SonielStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    // anim hash
    int throwLeftHash = Animator.StringToHash("ThrowLeft");
    int throwRightHash = Animator.StringToHash("ThrowRight");
    int getLeftHash = Animator.StringToHash("GetLeft");
    int getRightHash = Animator.StringToHash("GetRight");
    int throwToIdleHash = Animator.StringToHash("ThrowToIdle");

    float attackDuration = 0f;

    // This method will be called every Update to check whether or not to switch states.
    protected override void CheckSwitchStates()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchState(Factory.GetState<SonielTriggeredState>());
        }
    }

    // This method will be called only once before the update.
    protected override void EnterState()
    {
        Context.Agent.isStopped = true;

        if (Context.HasArms)
        {
            if (Context.HasLeftArm)
            {
                Context.Animator.ResetTrigger(throwLeftHash);
                Context.Animator.SetTrigger(throwLeftHash);

            }
            else if (Context.HasRightArm)
            {
                Context.Animator.ResetTrigger(throwRightHash);
                Context.Animator.SetTrigger(throwRightHash);
            }
        }



        //Context.Animator.SetBool(getLeftHash, true);
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;
        Context.Animator.SetBool(getLeftHash, false);
        Context.Animator.SetBool(getRightHash, false);
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        attackDuration += Time.deltaTime;
        if (attackDuration >= Context.Animator.GetCurrentAnimatorClipInfo(0).Length)
        {
            attackDuration = 0f;
            Context.Animator.ResetTrigger(throwToIdleHash);
            Context.Animator.SetTrigger(throwToIdleHash);
        }
    }

    // This method will be called on state switch.
    // No need to modify this method !
    protected override void SwitchState(BaseState<SonielStateMachine> newState)
    {
        base.SwitchState(newState);
        Context.currentState = newState;
    }
}
