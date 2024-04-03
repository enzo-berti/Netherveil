using UnityEngine;

public class TrapProjectile : Projectile
{
    protected override void Update()
    {
        Move(transform.forward);
        transform.Rotate(30f * Time.deltaTime, 0f, 0f);
    }
}