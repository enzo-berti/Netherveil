using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class GraftedProjectile : Projectile
{
    [HideInInspector]
    public bool onTarget = false;
    bool ignoreCollisions = false;
    Vector3 direction;
    Grafted grafted;

    public void SetCollisionImmune(bool _state) { ignoreCollisions = _state; }

    protected override void Awake()
    {
        damage = 5;
    }

    public void Initialize(Grafted _grafted)
    {
        grafted = _grafted;
    }
    
    public void SetDirection(Vector3 _direction)
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

    public bool OnLauncher(Vector3 _launcher)
    {
        return Vector3.Distance(transform.position, _launcher) < 1f;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!ignoreCollisions)
        {
            if (((1 << other.gameObject.layer) & LayerMask.GetMask("Map")) != 0 && !other.isTrigger)
            {
                onTarget = true;
                return;
            }
        }

        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null && !onTarget && other.CompareTag("Player"))
        {
            damageableObject.ApplyDamage(damage);
            if (!ignoreCollisions)
            {
                direction = -Vector3.up;
            }
            else
            {
                grafted.ApplyKnockback(damageableObject, new Vector3(-direction.z, 0, direction.x).normalized);
            }
        }
    }
}