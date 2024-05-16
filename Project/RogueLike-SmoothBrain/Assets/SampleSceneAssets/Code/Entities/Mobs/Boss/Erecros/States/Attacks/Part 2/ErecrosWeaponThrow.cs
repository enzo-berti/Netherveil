// ---[ STATE ] ---
// replace "ErecrosWeaponThrow_STATEMACHINE" by your state machine class name.
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
using Unity.VisualScripting;
using UnityEngine;

public class ErecrosWeaponThrow : BaseState<ErecrosStateMachine>
{
    public ErecrosWeaponThrow(ErecrosStateMachine currentContext, StateFactory<ErecrosStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;
    List<Vector3> targetPos = new();
    Rigidbody[] props;

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

        props = Context.PropsRB;

        for (int i = 0; i < props.Length; i++)
        {
            props[i].constraints = RigidbodyConstraints.None;
            props[i].isKinematic = false;
            targetPos.Add(props[i].position);
            targetPos.Add(Context.transform.position + Vector3.up * Context.Height);
            props[i].velocity = (targetPos[i] - props[i].transform.position).normalized * 10f;
        }
    }

    // This method will be called only once after the last update.
    protected override void ExitState()
    {
        Context.Agent.isStopped = false;

        foreach (Rigidbody prop in Context.PropsRB)
        {
            prop.isKinematic = true;
            prop.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {

        for (int i = 0; i < props.Length; i++)
        {
            LookAtTarget(props[i].transform, targetPos[i]);

            if (Vector3.Distance(props[i].transform.position, targetPos[i]) <= 1f)
            {
                props[i].isKinematic = true;
                props[i].constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad9))
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

    #region Extra methods

    public void LookAtTarget(Transform _launcher, Vector3 _target, float _speed = 5f)
    {
        Vector3 mobToPlayer = _target - _launcher.position;

        Quaternion lookRotation = Quaternion.LookRotation(mobToPlayer);

        _launcher.rotation = Quaternion.Slerp(_launcher.rotation, lookRotation, _speed * Time.deltaTime);
    }

    #endregion
}
