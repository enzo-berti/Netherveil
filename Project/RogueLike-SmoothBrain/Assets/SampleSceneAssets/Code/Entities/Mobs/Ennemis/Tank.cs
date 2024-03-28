using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Tank : Mobs, ITank
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    public List<Status> StatusToApply => statusToApply;
    [SerializeField] CapsuleCollider shockwaveCollider;
    bool cooldownSpeAttack = false;
    float specialAttackTimer = 0f;
    readonly float SPECIAL_ATTACK_TIMER = 4f;

    bool cooldownBasicAttack = false;
    float basicAttackTimer = 0f;
    readonly float BASIC_ATTACK_TIMER = 0.75f;

    protected override void Start()
    {
        base.Start();

    }

    public void Attack(IDamageable damageable)
    {
        int damages = (int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF) * 3);

        onHit?.Invoke(damageable);
        damageable.ApplyDamage(damages);
        ApplyKnockback(damageable);
    }

    public void BasicAttack(IDamageable damageable)
    {
        int damages = (int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF));

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
            StartCoroutine(HitRoutine());
        }

        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
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

            Hero player = PhysicsExtensions.OverlapVisionCone(transform.position, 360, 10f, transform.forward)
                .ToList()
                .Select(x => x.GetComponent<Hero>())
                .Where(x => x != null)
                .FirstOrDefault();

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

            if (player != null)
            {
                // Player detect
                if (Vector3.Distance(transform.position, player.transform.position) <= shockwaveCollider.gameObject.transform.localScale.z && !cooldownSpeAttack)
                {
                    agent.isStopped = true;
                    yield return new WaitForSeconds(0.4f);
                    AttackCollide();
                    agent.isStopped = false;
                    cooldownSpeAttack = true;
                }
                else if (agent.velocity.magnitude == 0f && Vector3.Distance(transform.position, player.transform.position) <= agent.stoppingDistance && !cooldownBasicAttack)
                {
                    BasicAttack(player);
                    cooldownBasicAttack = true;
                }
                else
                {
                    MoveTo(player.transform.position);
                }
            }
        }
    }

    public void AttackCollide(bool debugMode = true)
    {
        if (debugMode)
        {
            shockwaveCollider.gameObject.SetActive(true);
        }

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