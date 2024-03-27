using UnityEngine;

public class Projectile : MonoBehaviour, IProjectile
{
    public DamageState elementalDamage;
    [SerializeField] int damage = 5;
    [SerializeField] float speed = 30f;
    [SerializeField] float lifeTime = 5f;
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

    public void Move(Vector3 _direction)
    {
        transform.Translate(_direction.normalized * speed * Time.deltaTime);
    }

    protected virtual void Update()
    {
        Move(Vector3.forward);

        if (Time.time - startTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
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