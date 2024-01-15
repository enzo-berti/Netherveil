using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    Hero hero;
    Vector3 spherePos = Vector3.zero;
    void Start()
    {
        hero = GetComponent<Hero>();
    }

    void Update()
    {
        spherePos = new Vector3(transform.position.x,
            transform.position.y + GetComponent<CharacterController>().bounds.size.y / 2f,
            transform.position.z);

        Collider[] tab = Physics.OverlapSphere(spherePos, hero.Stats.GetValueStat(Stat.CATCH_RADIUS));

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

    private void OnDrawGizmos()
    {
        Color color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.25f);
        Gizmos.color = color;
        Gizmos.DrawSphere(spherePos, hero.Stats.GetValueStat(Stat.CATCH_RADIUS));
    }
}
