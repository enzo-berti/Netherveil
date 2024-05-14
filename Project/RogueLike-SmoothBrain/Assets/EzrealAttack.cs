using PostProcessingEffects;
using UnityEngine;
using UnityEngine.VFX;

public class EzrealAttack : Projectile
{

    [SerializeField] VisualEffect effect;

    protected override void Awake()
    {
        base.Awake();
        AudioManager.Instance.PlaySound(AudioManager.Instance.EzrealUltSFX);
    }

    protected override void Update()
    {
        Move(transform.forward);
        effect.transform.position = transform.position;
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

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
