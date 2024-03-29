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
    Hero player;

    
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
    }

    public void Attack(IDamageable damageable)
    {
        int damages = (int)(stats.GetValue(Stat.ATK) * 3);

        onHit?.Invoke(damageable);
        damageable.ApplyDamage(damages);
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
        AudioManager.Instance.PlaySound(deadSFX, transform.position);
        Destroy(gameObject);
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }

    protected override IEnumerator Brain()
    {
        while (true)
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


            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            Vector3 tmp = (cameraForward * player.transform.position.z + cameraRight * player.transform.position.x);
            Vector2 playerPos = new Vector2(tmp.x, tmp.z);
            tmp = (cameraForward * transform.position.z + cameraRight * transform.position.x);
            Vector2 tankPos = new Vector2(tmp.x, tmp.z);

            bool isInRange = Vector2.Distance(playerPos, tankPos) <= shockwaveCollider.gameObject.transform.localScale.z/2f;

            // Player detect
            if (isInRange && !cooldownSpeAttack)
            {
                Debug.Log("AYA");
                AttackCollide();
                cooldownSpeAttack = true;
                vfxStopper.PlayVFX();
                AudioManager.Instance.PlaySound(shockwaveSFX, transform.position);
            }
            else if (agent.velocity.magnitude == 0f && Vector2.Distance(playerPos, tankPos) <= agent.stoppingDistance && !cooldownBasicAttack)
            {
                BasicAttack(player);
                cooldownBasicAttack = true;
                AudioManager.Instance.PlaySound(punchSFX, transform.position);
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