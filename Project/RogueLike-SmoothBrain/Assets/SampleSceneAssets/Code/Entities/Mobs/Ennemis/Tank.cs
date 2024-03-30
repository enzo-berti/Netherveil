using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Mobs, ITank
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    public List<Status> StatusToApply => statusToApply;
    [SerializeField] CapsuleCollider shockwaveCollider;
    VFXStopper vfxStopper;
    bool cooldownSpeAttack = false;
    float specialAttackTimer = 0f;
    readonly float SPECIAL_ATTACK_TIMER = 2.2f;

    bool cooldownBasicAttack = false;
    float basicAttackTimer = 0f;
    readonly float BASIC_ATTACK_TIMER = 0.75f;
    bool isDying = false;
    Hero player;
    Animator animator;

    
    [SerializeField] EventReference shockwaveSFX;
    [SerializeField] EventReference punchSFX;
    [SerializeField] EventReference hitSFX;
    [Header("SFXs")]
    [SerializeField] EventReference deadSFX;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        vfxStopper = GetComponent<VFXStopper>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Attack(IDamageable damageable)
    {
        if ((damageable as MonoBehaviour).CompareTag("Player"))
        {
            int damages = (int)(stats.GetValue(Stat.ATK) * 3);

            onHit?.Invoke(damageable);
            damageable.ApplyDamage(damages);
        }
        ApplyKnockback(damageable);
    }

    public void BasicAttack(IDamageable damageable)
    {
        int damages = (int)stats.GetValue(Stat.ATK);

        onHit?.Invoke(damageable);
        damageable.ApplyDamage(damages);
    }

    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        if (stats.GetValue(Stat.HP) <= 0)
            return;

        Stats.DecreaseValue(Stat.HP, _value, false);
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));

        if (hasAnimation)
        {
            //add SFX here
            FloatingTextGenerator.CreateDamageText(_value, transform.position, isCrit);
            AudioManager.Instance.PlaySound(hitSFX, transform.position);
            StartCoroutine(HitRoutine());
        }

        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        AudioManager.Instance.PlaySound(deadSFX, transform.position);
        animator.ResetTrigger("Death");
        animator.SetTrigger("Death");
        isDying = true;
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }

    protected override IEnumerator Brain()
    {
        while (!isDying)
        {
            yield return null;

            if (cooldownSpeAttack)
            {
                specialAttackTimer += Time.deltaTime;
                if (specialAttackTimer >= SPECIAL_ATTACK_TIMER)
                {
                    cooldownSpeAttack = false;
                    specialAttackTimer = 0f;
                }
            }

            if (cooldownBasicAttack)
            {
                basicAttackTimer += Time.deltaTime;
                if (basicAttackTimer >= BASIC_ATTACK_TIMER)
                {
                    cooldownBasicAttack = false;
                    basicAttackTimer = 0f;
                }
            }

            Vector2 playerPos2D = player.transform.position.ToCameraOrientedVec2();
            Vector2 tankPos2D = transform.position.ToCameraOrientedVec2();

            bool isInRange = Vector2.Distance(playerPos2D, tankPos2D) <= shockwaveCollider.gameObject.transform.localScale.z/2f;

            if (isInRange && !cooldownSpeAttack)
            {
                animator.ResetTrigger("Shockwave");
                animator.SetTrigger("Shockwave");
                yield return new WaitForSeconds(0.4f);
                AttackCollide();
                cooldownSpeAttack = true;
                vfxStopper.PlayVFX();
                AudioManager.Instance.PlaySound(shockwaveSFX, transform.position);;
            }
            else if (agent.velocity.magnitude == 0f && Vector2.Distance(playerPos2D, tankPos2D) <= agent.stoppingDistance && !cooldownBasicAttack)
            {
                BasicAttack(player);
                cooldownBasicAttack = true;
                AudioManager.Instance.PlaySound(punchSFX, transform.position);
                animator.ResetTrigger("Punch");
                animator.SetTrigger("Punch");
            }
            else
            {
                MoveTo(player.transform.position);
            }

        }
    }

    public void AttackCollide(bool debugMode = true)
    {
        //if (debugMode)
        //{
        //    shockwaveCollider.gameObject.SetActive(true);
        //}

        Collider[] tab = PhysicsExtensions.CapsuleOverlap(shockwaveCollider, LayerMask.GetMask("Entity"));
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

// |--------------|
// |TANK BEHAVIOUR|
// |--------------|
// If Player detect
//  Move to Player
// Else
//  Don't move