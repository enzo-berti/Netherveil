// ---[ STATE MACHINE ] ---
// "factory" is use to get all state possible
// "currentState" can be set in the start with : currentState = factory.GetState<YOUR_STATE>();

using UnityEngine;
using StateMachine; // include all script about stateMachine
using System.Collections.Generic;
using System;
using UnityEngine.VFX;

public class ErecrosStateMachine : Mobs, IFinalBoss
{
    [HideInInspector]
    public BaseState<ErecrosStateMachine> currentState;
    private StateFactory<ErecrosStateMachine> factory;

    [Serializable]
    public class ErecrosSounds
    {
        public Sound intro;
        public Sound miniHit; //
        public Sound miniDeath; //
        public Sound maxiHit; //
        public Sound maxiDeath; //
        public Sound teleport; //
        public Sound dash; //
        public Sound clone; //
        public Sound levitation;
        public Sound prison;
        public Sound shieldHit; //
        public Sound shockwave;
        public Sound invocation; //
        public Sound throwWeapon;
        public Sound weaponHitGround;
        public Sound weaponHitWall;
        public Sound weaponFlying;
    }

    public enum ErecrosColliders
    {
        DASH
    }

    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    Hero player = null;
    [SerializeField] ErecrosSounds sounds;
    [SerializeField] List<NestedList<Collider>> attackColliders;
    bool playerHit = false;
    float attackCooldown = 1f;
    float initialHP;
    CameraUtilities cameraUtilities;

    [SerializeField] int part;
    [SerializeField] int phase;

    [SerializeField] GameObject[] enemiesPrefabs;

    [SerializeField] VisualEffect shieldVFX;
    [SerializeField] GameObject clonePrefab;
    [SerializeField] GameObject prisonTorusPrefab;

    #region Getters/Setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public Animator Animator { get => animator; }
    public List<NestedList<Collider>> Attacks { get => attackColliders; }
    public ErecrosSounds Sounds { get => sounds; }
    public Hero Player { get => player; }
    public bool PlayerHit { get => playerHit; set => playerHit = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public CameraUtilities CameraUtilities { get => cameraUtilities; }
    public GameObject[] EnemiesPrefabs { get => enemiesPrefabs; }
    public int CurrentPart { get => part; }
    public int CurrentPhase { get => phase; }

    public VisualEffect ShieldVFX { get => shieldVFX; }
    public GameObject ClonePrefab { get => clonePrefab; }
    public GameObject PrisonTorusPrefab { get => prisonTorusPrefab; }

    #endregion

    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<ErecrosStateMachine>(this);
        currentState = factory.GetState<ErecrosTriggeredState>();

        player = Utilities.Hero;
        initialHP = stats.GetValue(Stat.HP);
        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();

        part = 1;
        phase = 2;
    }

    protected override void Update()
    {
        if (IsFreeze || IsSpawning)
            return;

        if (IsKnockbackable)
            IsKnockbackable = false;

        base.Update();
        currentState.Update();
    }

    #region Mobs methods
    public void ApplyDamage(int _value, IAttacker attacker, bool notEffectDamage = true)
    {
        if (currentState is not ErecrosSummoningAttack)
        {
            ApplyDamagesMob(_value, part <= 1 ? sounds.miniHit : sounds.maxiHit, Death, notEffectDamage);
        }
        else
        {
            sounds.shieldHit.Play(transform.position, true);
        }
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        damages += additionalDamages;

        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
        //ApplyKnockback(damageable, this);
    }

    public void Death()
    {
        animator.speed = 1;
        OnDeath?.Invoke(transform.position);
        Utilities.Hero.OnKill?.Invoke(this);

        if (part <= 1) sounds.miniDeath.Play(transform.position); else sounds.maxiDeath.Play(transform.position);

        currentState = factory.GetState<ErecrosDeathState>();
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }
    #endregion

    #region Extra methods
    public void DisableHitboxes()
    {
        foreach (NestedList<Collider> attack in attackColliders)
        {
            foreach (Collider attackCollider in attack.data)
            {
                attackCollider.gameObject.SetActive(false);
            }
        }
    }

    public void AttackCollide(List<Collider> colliders, bool _kb = false, bool debugMode = true)
    {
        if (debugMode)
        {
            foreach (Collider collider in colliders)
            {
                collider.gameObject.SetActive(true);
            }
        }

        Vector3 rayOffset = Vector3.up / 2;

        foreach (Collider attackCollider in colliders)
        {
            Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(attackCollider, transform.position + rayOffset, "Player", LayerMask.GetMask("Map"));
            if (tab.Length > 0)
            {
                foreach (Collider col in tab)
                {
                    if (col.gameObject.GetComponent<Hero>() != null)
                    {
                        IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
                        Attack(damageable);

                        if (_kb)
                        {
                            Vector3 knockbackDirection = new Vector3(-transform.forward.z, 0, transform.forward.x);

                            if (Vector3.Cross(transform.forward, player.transform.position - transform.position).y > 0)
                            {
                                knockbackDirection = -knockbackDirection;
                            }

                            ApplyKnockback(damageable, this, knockbackDirection);
                        }

                        playerHit = true;
                        return;
                    }
                }
            }
        }
    }
    #endregion
}
