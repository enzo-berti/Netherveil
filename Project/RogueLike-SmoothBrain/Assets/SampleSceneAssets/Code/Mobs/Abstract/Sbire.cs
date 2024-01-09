using UnityEngine;

public class Sbire : Mobs
{
    Hero playerScript = null;

    private void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
    }
    protected void FollowPlayer()
    {

    }

    // fait sa vie, se balade dans la salle
    protected void Wander()
    {

    }

    protected void AttackPlayer(float _range, float _distanceToPlayer)
    {
        if (_distanceToPlayer < _range && playerScript != null)
        {
            int damage = (int)stats.GetValueStat(Stat.ATK) * (int)stats.GetValueStat(Stat.ATK_COEFF);

            playerScript.ApplyDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerScript == null)
        {
            if (other.tag == "Player")
            {
                playerScript = other.gameObject.GetComponent<Hero>();
            }
        }
    }

}
