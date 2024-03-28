using UnityEngine;

public class GraftedProjectile : Projectile
{
    [HideInInspector]
    public bool onTarget = false;
    Vector3 direction;

    protected override void Awake()
    {
        lifeTime = 5f;
        base.Awake();
    }

    public void Initialize(Vector3 _direction)
    {
        direction = _direction;
    }

    protected override void Update()
    {
        if (!onTarget)
        {
            Move(direction);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Map")) != 0 && !other.isTrigger)
        {
            onTarget = true;
            return;
        }

        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null && !onTarget)
        {
            damageableObject.ApplyDamage(damage);
            direction = -Vector3.up;
        }
    }
}