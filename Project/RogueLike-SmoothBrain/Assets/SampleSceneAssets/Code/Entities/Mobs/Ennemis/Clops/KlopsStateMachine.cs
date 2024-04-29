using UnityEngine;
using StateMachine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class KlopsStateMachine : Mobs, IKlops
{
    [System.Serializable]
    public class KlopsSounds
    {
        public Sound deathSound;
        public Sound takeDamageSound;
        public Sound hitSound;
    }

    [HideInInspector]
    // state machine variables
    public BaseState<KlopsStateMachine> currentState;
    private StateFactory<KlopsStateMachine> factory;

    // mobs variables
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    [SerializeField] private KlopsSounds klopsSounds;
    [SerializeField] float defaultVisionAngle = 360f;
    [SerializeField] GameObject fireballPrefab;
    //[SerializeField, Range(0f, 360f)] private float angle = 180.0f;
    //[SerializeField] private BoxCollider attack1Collider;
    //[SerializeField] private BoxCollider attack2Collider;
    //[SerializeField] private BoxCollider attack3Collider;
    private Transform target;
    private bool isDeath = false;
    Hero player = null;

    // animation hash
    private int deathHash;

    // getters and setters
    public GameObject FireballPrefab { get => fireballPrefab; }
    public float VisionAngle { get => defaultVisionAngle; }
    public float FleeRange { get => stats.GetValue(Stat.ATK_RANGE) * 0.5f; }
    public Hero Player { get => player; }
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public BaseState<KlopsStateMachine> CurrentState { get => currentState; set => currentState = value; }
    public Entity[] NearbyEntities { get => nearbyEntities; }
    public Animator Animator { get => animator; }
    public Transform Target { get => target; set => target = value; }
    public bool IsDeath { get => isDeath; }
    public KlopsSounds KlopsSound { get => klopsSounds; }

    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<KlopsStateMachine>(this);
        // Set currentState here !
        currentState = factory.GetState<KlopsPatrolState>();

        // common initialization


        // hashing animation
        deathHash = Animator.StringToHash("Death");

        // opti variables
        maxFrameUpdate = 10;
        frameToUpdate = entitySpawn % maxFrameUpdate;
    }

    protected override void Update()
    {
        if (isFreeze || IsSpawning)
            return;

        base.Update();

        currentState.Update();
    }

    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            if (!agent.enabled)
            {
                yield return null;
                continue;
            }

            nearbyEntities = PhysicsExtensions.OverlapVisionCone(transform.position, VisionAngle, stats.GetValue(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
                    .Select(x => x.GetComponent<Hero>())
                    .Where(x => x != null && x != this)
                    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                    .ToArray();

            Entity playerEntity = nearbyEntities.FirstOrDefault(x => x.GetComponent<Hero>());
            player = playerEntity != null ? playerEntity.GetComponent<Hero>() : null;

            yield return new WaitUntil(() => Time.frameCount % maxFrameUpdate == frameToUpdate);
        }
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        damages += additionalDamages;

        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
        ApplyKnockback(damageable, this);

        //clopsSounds.attackSound.Play(transform.position);
    }

    public void MoveTo(Vector3 posToMove)
    {
        if (!agent.enabled)
            return;

        agent.SetDestination(posToMove);
    }

    public void ApplyDamage(int _value, IAttacker attacker, bool notEffectDamage = true)
    {
        ApplyDamagesMob(_value, klopsSounds.hitSound, Death, notEffectDamage);
    }

    public void Death()
    {
        animator.speed = 1;
        OnDeath?.Invoke(transform.position);
        Hero.OnKill?.Invoke(this);
        klopsSounds.deathSound.Play(transform.position);
        animator.ResetTrigger(deathHash);
        animator.SetTrigger(deathHash);
        isDeath = true;

        Destroy(transform.parent.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
