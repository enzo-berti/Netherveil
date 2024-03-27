using UnityEngine;

public class GraftedProjectile : Projectile
{
    [HideInInspector]
    public bool onTarget = false;
    Vector3 direction;

    public void Initialize(Vector3 _direction)
    {
        direction = _direction;
        direction.Normalize();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Move(direction);
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Map")) != 0)
        {
            onTarget = true;
            return;
        }

        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.ApplyDamage(damage);
            //direction = transform.position - Vector3.up;
        }
    }
}