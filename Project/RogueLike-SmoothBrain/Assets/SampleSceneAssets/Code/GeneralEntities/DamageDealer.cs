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
        damageDeal = dealerWhoGetStats.GetValueStat(Stat.ATK) * dealerWhoGetStats.GetValueStat(Stat.ATK_COEFF);
        hasDealtDamage = new List<GameObject>();
    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;
            int layerMask = 1 << 9;
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLenght,layerMask))
            {
                if (!hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    print("Damage!");
                    hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        bool canDamage;
        if (!collision.gameObject.TryGetComponent<Hero>(out Hero entity) || (entity as IDamageable) == null)
        {
            canDamage = (damageDealer.isAlly && !entity.isAlly || !damageDealer.isAlly && entity.isAlly);
            if(canDamage)
            {
                entity.ApplyDamage((int)damageDealer.Stats.GetValueStat(Stat.ATK));
            }
        }
        else if (!collision.gameObject.TryGetComponent<Mobs>(out Mobs mobs) || (mobs as IDamageable) == null)
        {
            canDamage = (damageDealer.isAlly && !mobs.isAlly || !damageDealer.isAlly && mobs.isAlly);
            if (canDamage)
            {
                entity.ApplyDamage((int)mobs.Stats.GetValueStat(Stat.ATK));
            }
        }
    }
}
