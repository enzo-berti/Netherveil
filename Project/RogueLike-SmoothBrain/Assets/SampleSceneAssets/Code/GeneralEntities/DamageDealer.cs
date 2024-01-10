using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    float damageDeal;
    List<GameObject> hasDealtDamage;

    [SerializeField] float weaponLenght;
    [SerializeField] Stats dealerWhoGetStats;
    [SerializeField] Entity damageDealer;
    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    void Update()
    {
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLenght);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        bool canDamage;
        if (other.gameObject.TryGetComponent<Hero>(out Hero entity))
        {
            canDamage = entity.State == Hero.PlayerState.ATTACK && (damageDealer.isAlly && !entity.isAlly || !damageDealer.isAlly && entity.isAlly);
            if (canDamage)
            {
                entity.ApplyDamage((int)damageDealer.Stats.GetValueStat(Stat.ATK));
            }
        }
        else if (other.gameObject.TryGetComponent<Mobs>(out Mobs mobs))
        {
            canDamage = mobs.State == Mobs.EnemyState.ATTACK && (damageDealer.isAlly && !mobs.isAlly || !damageDealer.isAlly && mobs.isAlly);
            if (canDamage)
            {
                mobs.ApplyDamage((int)damageDealer.Stats.GetValueStat(Stat.ATK));
            }
        }

        
    }
}
