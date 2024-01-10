using UnityEngine;

public class Sbire : Mobs
{
    public enum EnemyState
    {
        // pour ennemis
        WANDERING,
        TRIGGERED,

        // pour joueur
        MOVE,

        DASH,
        ATTACK,
        HIT,
        DEAD
    }

    protected Transform target = null;
    protected float cooldown = 0;
    protected bool isAttacking = false;
    protected EnemyState state;

    protected void SimpleAI()
    {
        Vector3 enemyToTargetVector = Vector3.zero;

        if (target != null)
        {
            enemyToTargetVector = target.position - transform.position;
            enemyToTargetVector.y = 0;

            if (enemyToTargetVector.magnitude <= stats.GetValueStat(Stat.ATK_RANGE))
                state = EnemyState.ATTACK;
            else
                state = EnemyState.TRIGGERED;
        }

        if (state != EnemyState.ATTACK)
        {
            cooldown = 0;
        }

        // StateMachine
        switch (state)
        {
            case EnemyState.WANDERING:
                break;

            case EnemyState.TRIGGERED:
                FollowPlayer(enemyToTargetVector);
                break;

            case EnemyState.DASH:
                break;

            case EnemyState.ATTACK:
                AttackPlayer();
                break;

            case EnemyState.HIT:
                break;

            case EnemyState.DEAD:
                break;

            default:
                break;
        }
    }

    // sale faut repasser ici
    protected void FollowPlayer(Vector3 _distanceToPlayer)
    {
        _distanceToPlayer.Normalize();

        FaceTarget();

        transform.position += _distanceToPlayer * stats.GetValueStat(Stat.SPEED) * Time.deltaTime;
    }

    // fait sa vie, se balade dans la salle
    protected void Wander()
    {

    }

    protected void AttackPlayer()
    {
        cooldown += Time.deltaTime;

        // tape FIRE_RATE fois par seconde
        if (cooldown >= 1f / stats.GetValueStat(Stat.FIRE_RATE))
        {
            int damage = (int)stats.GetValueStat(Stat.ATK) * (int)stats.GetValueStat(Stat.ATK_COEFF);
            Hero playerScript = target.gameObject.GetComponent<Hero>();

            playerScript.ApplyDamage(-damage);

            isAttacking = true;
            cooldown = 0;
        }
        else
        {
            FaceTarget();
            isAttacking = false;
        }
    }

    void FaceTarget()
    {
        Vector3 enemyToTargetVector = target.position - transform.position;
        enemyToTargetVector.y = 0;

        float angle = Mathf.Atan2(enemyToTargetVector.x, enemyToTargetVector.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            state = EnemyState.TRIGGERED;
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            state = EnemyState.WANDERING;
            target = null;
        }
    }
}