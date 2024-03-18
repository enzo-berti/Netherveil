using UnityEngine;

public class Projectile : MonoBehaviour, IProjectile
{
    public DamageState elementalDamage;
    int damage = 5;
    float speed = 20f;
    float lifetime = 10f;
    float startTime;

    public enum DamageState
    {
        NORMAL,
        FIRE,
        ICE,
        ELECTRICITY
    }

    void Start()
    {
        startTime = Time.time;
    }

    public void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void Update()
    {
        Move();

        if (Time.time - startTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Map")) != 0)
        {
            Destroy(gameObject);
            return;
        }

        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.ApplyDamage(damage);

            if (elementalDamage != DamageState.NORMAL && TryGetComponent<Entity>(out var entity))
            {
                switch (elementalDamage)
                {
                    case DamageState.FIRE:
                        entity.ApplyEffect(new Fire(10));
                        break;
                    case DamageState.ICE:
                        entity.ApplyEffect(new Freeze(10));
                        break;
                    case DamageState.ELECTRICITY:
                        entity.ApplyEffect(new Electricity(10));
                        break;
                }
                // apply elementalState
            }

            Destroy(gameObject);
        }
    }
}