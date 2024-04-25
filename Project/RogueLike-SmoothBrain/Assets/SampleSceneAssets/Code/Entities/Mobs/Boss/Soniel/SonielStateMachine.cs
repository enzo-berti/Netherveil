using UnityEngine;
using StateMachine; // include all script about stateMachine
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

public class SonielStateMachine : Mobs, ISoniel
{
    [Serializable]
    public class SonielSounds
    {
        public Sound hit;
        public Sound walk;
        public Sound death;
    }

    [HideInInspector]
    public BaseState<SonielStateMachine> currentState;
    private StateFactory<SonielStateMachine> factory;

    // mob parameters
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    Hero player = null;
    [SerializeField] SonielSounds sounds; 
    [SerializeField] Collider[] attackColliders;
    [SerializeField] float defaultVisionAngle = 100f;

    // animation hash
    int deathHash;

    #region getters/setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public Animator Animator { get => animator; }
    public Hero Player { get => Player; }
    #endregion

    protected override void Start()
    {
        factory = new StateFactory<SonielStateMachine>(this);
        currentState = factory.GetState<SonielTriggeredState>();

        // animation hash
        deathHash = Animator.StringToHash("Death");

        player = Utilities.Hero;
    }

    protected override void Update()
    {
        base.Update();
        currentState.Update();
    }

    #region MOB_METHODS

    public void ApplyDamage(int _value, IAttacker attacker, bool notEffectDamage = true)
    {
        ApplyDamagesMob(_value, sounds.hit, Death, notEffectDamage);
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        damages += additionalDamages;

        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
        ApplyKnockback(damageable, this);

        sounds.hit.Play(transform.position);
    }

    public void Death()
    {
        animator.speed = 1;
        OnDeath?.Invoke(transform.position);
        Hero.OnKill?.Invoke(this);

        sounds.death.Play(transform.position);

        animator.ResetTrigger(deathHash);
        animator.SetTrigger(deathHash);

        Destroy(transform.parent.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);

        currentState = factory.GetState<SonielDeathState>();
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
        sounds.walk.Play(transform.position);
    }

    public void AttackCollide(Collider _collider, bool debugMode = false)
    {
        if (debugMode)
        {
            _collider.gameObject.SetActive(true);
        }

        Collider[] tab = null;

        if (_collider is CapsuleCollider)
            tab = PhysicsExtensions.CapsuleOverlap(_collider as CapsuleCollider, LayerMask.GetMask("Entity"));
        else if (_collider is BoxCollider)
            tab = PhysicsExtensions.BoxOverlap(_collider as BoxCollider, LayerMask.GetMask("Entity"));
        else
            Debug.LogError("Type de collider non reconnu.");

        if (tab != null)
        {
            if (tab.Length > 0)
            {
                foreach (Collider col in tab)
                {
                    if (col.gameObject.GetComponent<IDamageable>() != null && col.gameObject != gameObject)
                    {
                        Attack(col.gameObject.GetComponent<IDamageable>());
                    }
                }
            }
        }
    }
    #endregion
}
