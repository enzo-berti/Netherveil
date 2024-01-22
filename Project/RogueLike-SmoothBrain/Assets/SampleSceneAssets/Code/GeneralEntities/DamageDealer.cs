using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealtDamage;
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
        if (other.gameObject.TryGetComponent<Entity>(out Entity entity))
        {
            bool canDamage = damageDealer.State == (int)Entity.EntityState.ATTACK && ((damageDealer.isAlly && !entity.isAlly) || (!damageDealer.isAlly && entity.isAlly));
            if (canDamage)
            {
                (damageDealer as IAttacker).Attack(entity.GetComponent<IDamageable>());
            }
        }
    }

}
