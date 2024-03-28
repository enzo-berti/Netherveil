using UnityEngine;

public class TrapProjectile : Projectile
{
    protected override void Update()
    {
       Move(Vector3.forward);
    }
}