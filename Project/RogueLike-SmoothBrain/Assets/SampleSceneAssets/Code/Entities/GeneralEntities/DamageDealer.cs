using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    //bool canDealDamage; -> J'ai comment� car le warning m'a cass� les couilles mdr ~Dorian
    List<GameObject> hasDealtDamage;
    [SerializeField] Entity damageDealer;
    void Start()
    {
        //canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    void Update()
    {
    }

    public void StartDealDamage()
    {
        //canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        //canDealDamage = false;
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
