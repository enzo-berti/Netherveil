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
    public class FinalBossSounds
    {
        public Sound hit;
    }

    public enum FinalBossColliders
    {

    }

    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    Hero player = null;
    [SerializeField] FinalBossSounds sounds;
    [SerializeField] List<NestedList<Collider>> attackColliders;
    bool playerHit = false;
    float attackCooldown = 1f;
    float initialHP;
    CameraUtilities cameraUtilities;

    int part;
    int phase;

    [SerializeField] GameObject[] enemiesPrefabs;

    [SerializeField] VisualEffect shieldVFX;
    [SerializeField] GameObject clonePrefab;

    #region Getters/Setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public Animator Animator { get => animator; }
    public List<NestedList<Collider>> Attacks { get => attackColliders; }
    public Hero Player { get => player; }
    public bool PlayerHit { get => playerHit; set => playerHit = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public CameraUtilities CameraUtilities { get => cameraUtilities; }
    public GameObject[] EnemiesPrefabs { get => enemiesPrefabs; }
    public int CurrentPart { get => part; }
    public int CurrentPhase { get => phase; }

    public VisualEffect ShieldVFX { get => shieldVFX; }
    public GameObject ClonePrefab { get => clonePrefab; }

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
        if (isFreeze || IsSpawning)
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
            ApplyDamagesMob(_value, sounds.hit, Death, notEffectDamage);
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

        currentState = factory.GetState<SonielDeathState>();
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }
    #endregion

    #region Extra methods

    public void LookAtPlayer()
    {
        Vector3 mobToPlayer = player.transform.position - transform.position;
        mobToPlayer.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(mobToPlayer);
        lookRotation.x = 0;
        lookRotation.z = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
    }

    #endregion
}
