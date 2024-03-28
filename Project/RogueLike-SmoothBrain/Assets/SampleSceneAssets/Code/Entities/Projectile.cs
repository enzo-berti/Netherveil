using UnityEngine;

public abstract class Projectile : MonoBehaviour, IProjectile
{
    public DamageState elementalDamage;
    [SerializeField] protected int damage = 5;
    [SerializeField] protected float speed = 30f;
    [SerializeField] protected float lifeTime = 5f;

    public enum DamageState
    {
        NORMAL,
        FIRE,
        ICE,
        ELECTRICITY
    }

    protected virtual void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Move(Vector3 _direction)
    {
        _direction.Normalize();
        transform.Translate(_direction * speed * Time.deltaTime);
    }

    protected abstract void Update();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Map")) != 0 && !other.isTrigger)
        {
            print("destroy by map");
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
                        entity.ApplyEffect(new Fire(2));
                        break;
                    case DamageState.ICE:
                        entity.ApplyEffect(new Freeze(2));
                        break;
                    case DamageState.ELECTRICITY:
                        entity.ApplyEffect(new Electricity(2));
                        break;
                }
                // apply elementalState
            }

            Destroy(gameObject);
        }
    }
}
