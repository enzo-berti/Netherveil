using PostProcessingEffects;
using UnityEngine;
using UnityEngine.VFX;

public class EzrealAttack : Projectile
{
    VisualEffect effect;

    protected override void Awake()
    {
        base.Awake();
        effect = GetComponentInChildren<VisualEffect>();
    }

    protected override void Update()
    {
        Move(transform.forward);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Map") && !other.isTrigger)
        {
            return;
        }

        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null && other.gameObject.CompareTag("Enemy"))
        {
            damageableObject.ApplyDamage(damage, null);
        }
    }
}
