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
using System.Linq;
using UnityEngine;

public class ErecrosWeaponThrowAttack : BaseState<ErecrosStateMachine>
{
    public ErecrosWeaponThrowAttack(ErecrosStateMachine currentContext, StateFactory<ErecrosStateMachine> currentFactory)
        : base(currentContext, currentFactory) { }

    bool attackEnded = false;
    List<Vector3> targetPos = new();
    Rigidbody[] props;
    List<bool> onBoss = new();
    List<bool> launched = new();

    float delayBetweenLaunch = 0.4f;
    float launchTimer;
    int iterator = 0;

    List<Collider> activeColliders = new();

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
            Context.PropsColliders[i].enabled = false;
            props[i].constraints = RigidbodyConstraints.None;
            props[i].isKinematic = false;

            Vector3 customVector = Context.transform.right * 2f;
            customVector = Quaternion.AngleAxis(180f / (props.Length - 1) * i, Context.transform.forward) * customVector;

            targetPos.Add(Context.transform.position + Context.transform.up * Context.Height + customVector);
            props[i].velocity = (targetPos.Last() - props[i].transform.position).normalized * 20f;

            props[i].GetComponent<ErecrosWeaponBehaviour>().enabled = true;
            props[i].GetComponent<ErecrosWeaponBehaviour>().PlayFlying();

            onBoss.Add(false);
            launched.Add(false);
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
            prop.GetComponent<ErecrosWeaponBehaviour>().Reset();
            prop.GetComponent<ErecrosWeaponBehaviour>().enabled = false;
        }

        Context.AttackCooldown = 1.25f + Random.Range(-0.25f, 0.25f);
    }

    // This method will be called every frame.
    protected override void UpdateState()
    {
        Context.LookAtTarget(Context.Player.transform.position);

        bool allOnBoss = true;
        attackEnded = true;

        for (int i = 0; i < props.Length; i++)
        {
            if (!onBoss[i])
            {
                GetToBoss(i);
                allOnBoss = false;
            }
            else if (!launched[i])
            {
                props[i].transform.position = new Vector3(props[i].transform.position.x, targetPos[i].y + Mathf.Sin(5f * Time.time + i * 5f) * 0.25f, props[i].transform.position.z);
            }
            else if (!props[i].GetComponent<ErecrosWeaponBehaviour>().hitMap)
            {
                if (!Context.PlayerHit)
                {
                    Context.AttackCollide(activeColliders);
                }
                LookAtTarget(props[i].transform, props[i].transform.position + props[i].velocity);
            }
            else
            {
                activeColliders.Remove(Context.PropsColliders[i]);
            }

            // obligé de le faire en 2 conditions séparées
            if (!props[i].GetComponent<ErecrosWeaponBehaviour>().hitMap)
            {
                attackEnded = false;
            }
        }

        if (allOnBoss)
        {
            if (iterator >= props.Length) return;

            launchTimer += Time.deltaTime;

            if (launchTimer >= delayBetweenLaunch)
            {
                //delayBetweenLaunch *= 0.9f;
                launchTimer = 0f;

                props[iterator].transform.parent = Context.PropsParent.transform;
                props[iterator].velocity = (Context.Player.transform.position + Vector3.up - props[iterator].transform.position).normalized * 50f;
                launched[iterator] = true;

                Context.PlayerHit = false;

                Context.Sounds.throwWeapon.Play(Context.transform.position, true);
                props[iterator].GetComponent<ErecrosWeaponBehaviour>().PlayFlying();

                iterator++;
            }
        }
        else
        {
            props[0].GetComponent<ErecrosWeaponBehaviour>().PlayFlying();
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

        _launcher.localRotation = Quaternion.Slerp(_launcher.localRotation, lookRotation, _speed * Time.deltaTime);
    }

    public void LookAtForward(Rigidbody _launcher, float _speed = 5f)
    {
        Quaternion lookRotation = Quaternion.LookRotation(_launcher.transform.forward);

        _launcher.transform.localRotation = Quaternion.Slerp(_launcher.transform.localRotation, lookRotation, _speed * Time.deltaTime);
    }

    void GetToBoss(int i)
    {
        Vector3 customVector = Context.transform.right * 2f;
        customVector = Quaternion.AngleAxis(180f / (props.Length - 1) * i, Context.transform.forward) * customVector;

        targetPos[i] = Context.transform.position + Context.transform.up * Context.Height + customVector;

        props[i].velocity = (targetPos[i] - props[i].transform.position).normalized * 10f;

        LookAtTarget(props[i].transform, targetPos[i]);

        if (Vector3.Distance(props[i].transform.position, targetPos[i]) <= 0.1f)
        {
            props[i].transform.LookAt(props[i].transform.position + Context.transform.forward);
            props[i].transform.parent = Context.transform;
            props[i].velocity = Vector3.zero;
            onBoss[i] = true;

            Context.PropsColliders[i].enabled = true;
            activeColliders.Add(Context.PropsColliders[i]);

            targetPos[i] = props[i].transform.position;
        }
    }


    #endregion
}