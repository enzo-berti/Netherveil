using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour, IAttacker
{
    // Start is called before the first frame update
    public Vector3 direction = Vector3.zero;

    public List<Status> statusToApply = new List<Status>();
    public List<Status> StatusToApply => statusToApply;
    public float FireballSpeed = 2f;

    float radius = 0f;

    public IAttacker.AttackDelegate OnAttack { get; set; }
    public IAttacker.HitDelegate OnAttackHit { get; set; }

    void Start()
    {
        Destroy(gameObject, 3.0f);

        //radius = GetComponentInChildren<Renderer>().bounds.size.y / 2f;
        radius = GetComponent<CapsuleCollider>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction.normalized * Time.deltaTime * FireballSpeed;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, radius);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & LayerMask.GetMask("Map")) != 0)
                {
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Hero hero = (other.GetComponentInParent<Hero>());

        if (hero)
        {
            Attack(hero);
            Destroy(gameObject);
        }
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        damageable.ApplyDamage(10, this);
    }
}
