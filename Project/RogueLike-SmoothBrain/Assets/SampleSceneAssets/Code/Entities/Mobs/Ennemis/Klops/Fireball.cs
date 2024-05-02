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

    public IAttacker.AttackDelegate OnAttack { get; set; }
    public IAttacker.HitDelegate OnAttackHit { get; set; }

    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += direction.normalized * Time.deltaTime * FireballSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Hero hero = (other.GetComponentInParent<Hero>());

        if (hero)
        {
            Attack(hero);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        damageable.ApplyDamage(10, this);
    }
}
