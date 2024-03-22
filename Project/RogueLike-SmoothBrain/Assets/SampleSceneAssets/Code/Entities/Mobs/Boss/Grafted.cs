using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Grafted : Mobs, IAttacker, IDamageable, IMovable, IBlastable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    private List<Status> statusToApply = new List<Status>();
    public List<Status> StatusToApply => statusToApply;

    [SerializeField, Range(0f, 360f)] private float visionAngle = 360f;

    [Header("Boss parameters")]
    Hero player = null;

    [Header("Boss Attack Hitboxes")]
    [SerializeField] List<NestedList<Collider>> attacks;

    [Header("Thrust")]
    [SerializeField] float thrustCooldown = 0.5f;
    float thrustCDTimer;
    [SerializeField] float thrustCharge = 1f;
    float thrustChargeTimer;
    [SerializeField] float thrustDuration = 1f;
    float thrustDurationTimer;

    [Header("Dash")]
    [SerializeField] float maxDashRange;
    [SerializeField] Transform dashPivot;
    [SerializeField] float dashSpeed = 2f;
    float dashTimer = 0f;
    float minDashSize = 2;

    int thrustCounter = 0;

    enum Attacks
    {
        THRUST,
        DASH,
        AOE,
        RANGE,
        NONE
    }

    enum AttackState
    {
        CHARGING,
        ATTACKING,
        RECOVERING,
        IDLE
    }

    Attacks currentAttack = Attacks.NONE;
    AttackState attackState = AttackState.IDLE;
    float attackCooldown = 0;
    bool hasProjectile = false;

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;


            if (!player)
            {
                player = FindObjectOfType<Hero>();
            }
            else
            {
                // Face player
                if (attackState != AttackState.ATTACKING)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                    lookRotation.x = 0;
                    lookRotation.z = 0;

                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
                }

                // Move towards player

                MoveTo(attackState == AttackState.IDLE ? player.transform.position - (player.transform.position - transform.position).normalized * 2f : transform.position);


                // Attacks
                if (attackCooldown > 0)
                {
                    attackState = AttackState.IDLE;
                    attackCooldown -= Time.deltaTime;
                    if (attackCooldown < 0) attackCooldown = 0;
                }
                else if (attackCooldown == 0)
                {
                    //currentAttack = (Attacks)Random.Range(0, 3);
                    currentAttack = Attacks.DASH;
                }

                switch (currentAttack)
                {
                    case Attacks.RANGE:
                        if (hasProjectile) ThrowProjectile(); else RetrieveProjectile();
                        break;

                    case Attacks.THRUST:
                        TripleThrust();
                        break;

                    case Attacks.DASH:
                        Dash();
                        break;
                }
            }
        }
    }

    public void Attack(IDamageable _damageable)
    {
        int damages = (int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF));
        onHit?.Invoke(_damageable);
        _damageable.ApplyDamage(damages);
        FloatingTextGenerator.CreateDamageText(damages, (_damageable as MonoBehaviour).transform.position);
    }

    public void ApplyDamage(int _value, bool hasAnimation = true)
    {
        Stats.DecreaseValue(Stat.HP, _value, false);
        if (hasAnimation)
        {
            //add SFX here
        }
        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
    }

    public void MoveTo(Vector3 _pos)
    {
        agent.SetDestination(_pos);
    }

    public void AttackCollide(List<Collider> colliders, bool debugMode = true)
    {
        if (debugMode)
        {
            foreach (Collider collider in colliders)
            {
                collider.gameObject.SetActive(true);
            }
        }

        Vector3 rayOffset = Vector3.up / 2;

        foreach (Collider spearCollider in colliders)
        {
            Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(spearCollider, transform.position + rayOffset, "Player", LayerMask.GetMask("Map"));
            if (tab.Length > 0)
            {
                foreach (Collider col in tab)
                {
                    if (col.gameObject.GetComponent<Hero>() != null)
                    {
                        Attack(col.gameObject.GetComponent<IDamageable>());
                        break;
                    }
                }
            }
        }
    }

    ///////////////////////////////// ATTACKS

    void ThrowProjectile()
    {
        hasProjectile = false;
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
    }

    void RetrieveProjectile()
    {
        hasProjectile = true;
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
    }

    void TripleThrust()
    {
        Debug.Log(attackState);

        switch (attackState)
        {
            case AttackState.IDLE:

                attackState = AttackState.CHARGING;
                break;

            case AttackState.CHARGING:

                if (thrustChargeTimer < thrustCharge)
                {
                    thrustChargeTimer += Time.deltaTime;
                }
                else
                {
                    AttackCollide(attacks[(int)Attacks.THRUST].data);
                    thrustChargeTimer = 0;
                    attackState = AttackState.ATTACKING;
                }
                break;

            case AttackState.ATTACKING:

                if (thrustDurationTimer < thrustDuration)
                {
                    thrustDurationTimer += Time.deltaTime;
                }
                else
                {
                    DisableHitboxes();
                    thrustDurationTimer = 0;
                    attackState = AttackState.RECOVERING;
                }
                break;

            case AttackState.RECOVERING:

                thrustCounter++;

                if (thrustCounter < 3)
                {
                    attackState = AttackState.IDLE;
                }
                else if (thrustCDTimer < thrustCooldown)
                {
                    thrustCDTimer += Time.deltaTime;
                }
                else
                {
                    thrustCDTimer = 0;
                    thrustCounter = 0;
                    currentAttack = Attacks.NONE;
                    attackState = AttackState.IDLE;
                    attackCooldown = 2f;
                }
                break;
        }
    }

    void Dash()
    {
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;

        dashTimer += Time.deltaTime * dashSpeed;
        if (minDashSize + dashTimer < maxDashRange)
        {
            dashPivot.localScale = new Vector3(1, 1, minDashSize + dashTimer);
        }
    }

    void DisableHitboxes()
    {
        foreach (NestedList<Collider> attackColliders in attacks)
        {
            foreach (Collider attackCollider in attackColliders.data)
            {
                attackCollider.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject))
            return;

        DisplayVisionRange(visionAngle);
        DisplayAttackRange(visionAngle);
        DisplayInfos();
    }
#endif
}