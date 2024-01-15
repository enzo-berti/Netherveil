using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    Hero hero;
    void Start()
    {
        hero = GetComponent<Hero>();
    }

    void Update()
    {

        Collider[] tab = Physics.OverlapSphere(transform.position, hero.Stats.GetValueStat(Stat.CATCH_RADIUS));
        if (tab.Length > 0)
        {
            foreach (Collider collider in tab) 
            {
                if((collider as IInterractable) != null)
                {
                    (collider as IInterractable).Interract();
                }
            }
        }
    }
}
