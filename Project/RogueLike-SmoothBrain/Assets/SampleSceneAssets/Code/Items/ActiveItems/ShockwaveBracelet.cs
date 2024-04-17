using UnityEngine;

public class ShockwaveBracelet : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 20f;
    readonly int AOE_DAMAGES = 15;

    public void Activate()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        GameObject shockwaveCollider = GameObject.Instantiate(Resources.Load<GameObject>("ShockwaveBraceletCollide"));
        GameObject shockwaveVFX = GameObject.Instantiate(Resources.Load<GameObject>("VFX_ShockWaveTank"));
        shockwaveCollider.SetActive(false);

        shockwaveVFX.transform.position = hero.transform.position;
        shockwaveCollider.transform.position = hero.transform.position;
        shockwaveCollider.SetActive(true);

        shockwaveVFX.GetComponent<VFXStopper>().PlayVFX();

        Collider[] colliders = shockwaveCollider.GetComponent<CapsuleCollider>().CapsuleOverlap();

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<IDamageable>(out var entity) && collider.gameObject != hero.gameObject)
                {
                    hero.ApplyKnockback(entity, hero);
                    hero.Attack(entity, AOE_DAMAGES);
                }
            }
        }

        GameObject.Destroy(shockwaveCollider, 3f);
        GameObject.Destroy(shockwaveVFX, 3f);
    }
} 
