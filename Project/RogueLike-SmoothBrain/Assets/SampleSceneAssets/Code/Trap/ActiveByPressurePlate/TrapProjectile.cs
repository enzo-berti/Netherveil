using UnityEngine;

public class TrapProjectile : Projectile
{
    protected override void Update()
    {
        Move(transform.forward);
    }
}