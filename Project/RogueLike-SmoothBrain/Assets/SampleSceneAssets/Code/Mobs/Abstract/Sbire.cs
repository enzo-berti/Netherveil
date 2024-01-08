using UnityEngine;

public class Sbire : Mobs
{
    Hero playerScript = null;

    protected bool isTriggered = false;
    protected bool isInAttackRange = false;

    protected void Cervoh()
    {
        if (playerScript != null)
        {
            Vector3 enemyToPlayerVector = playerScript.gameObject.transform.position - transform.position;

            if (enemyToPlayerVector.magnitude <= stats.GetValueStat(Stat.ATK_RANGE))
            {
                isInAttackRange = true;
                AttackPlayer();
            }
            else
            {
                isInAttackRange = false;
                FollowPlayer(enemyToPlayerVector);
            }
        }
    }

    protected void FollowPlayer(Vector3 _distanceToPlayer)
    {
        _distanceToPlayer.Normalize();
        _distanceToPlayer.y = 0;

        // Bon faut le faire tourner mieux ça marche moyen là
        transform.rotation = Quaternion.Euler(_distanceToPlayer);

        transform.Translate(_distanceToPlayer * stats.GetValueStat(Stat.SPEED) * Time.deltaTime);
    }

    // fait sa vie, se balade dans la salle
    protected void Wander()
    {

    }

    protected void AttackPlayer()
    {
        int damage = (int)stats.GetValueStat(Stat.ATK) * (int)stats.GetValueStat(Stat.ATK_COEFF);

        playerScript.ApplyDamage(damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerScript == null)
        {
            if (other.tag == "Player")
            {
                playerScript = other.gameObject.GetComponent<Hero>();
                isTriggered = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerScript != null)
        {
            if (other.tag == "Player")
            {
                playerScript = null;
                isTriggered = false;
            }
        }
    }
}