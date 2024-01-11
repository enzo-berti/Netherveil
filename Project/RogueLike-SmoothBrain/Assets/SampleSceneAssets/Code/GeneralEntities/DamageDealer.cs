using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    float damageDeal;
    List<GameObject> hasDealtDamage;
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

    private void OnTriggerEnter(Collider other)
    {
        bool canDamage;
        if (other.gameObject.TryGetComponent<Hero>(out Hero entity))
        {
            canDamage = (damageDealer as Mobs) != null && (damageDealer as Mobs).State == Mobs.EnemyState.ATTACK && (damageDealer.isAlly && !entity.isAlly || !damageDealer.isAlly && entity.isAlly);
            if (canDamage)
            {
                entity.ApplyDamage((int)damageDealer.Stats.GetValueStat(Stat.ATK));
            }
        }
        else if (other.gameObject.TryGetComponent<Mobs>(out Mobs mobs))
        {
            canDamage = ((damageDealer as Hero) != null && (damageDealer as Hero).State == Hero.PlayerState.ATTACK || (damageDealer as Mobs) != null && (damageDealer as Mobs).State == Mobs.EnemyState.ATTACK) && (damageDealer.isAlly && !mobs.isAlly || !damageDealer.isAlly && mobs.isAlly);

            if (canDamage)
            {
                mobs.ApplyDamage((int)damageDealer.Stats.GetValueStat(Stat.ATK));
            }
        }
        
    }

}
