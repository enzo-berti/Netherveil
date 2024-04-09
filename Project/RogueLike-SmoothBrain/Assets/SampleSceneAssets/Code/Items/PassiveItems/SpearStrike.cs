using UnityEngine;
using UnityEngine.VFX;

public class SpearStrike : ItemEffect , IPassiveItem 
{
    public void OnRetrieved() 
    {
        //instantiate the thunderstrike collider and vfx

        Spear.OnPlacedInWorld += Thunderstrike;
    }

    public void OnRemove() 
    {
        Spear.OnPlacedInWorld -= Thunderstrike;
    } 
 
    private void Thunderstrike(Vector3 spearPos)
    {
        GameObject thunderstrikeCollider = GameObject.Instantiate(Resources.Load<GameObject>("ThunderstrikeCollide"));
        thunderstrikeCollider.SetActive(false);
        GameObject thunderstrikeVFX = GameObject.Instantiate(Resources.Load<GameObject>("VFX_TEMP_Thunderstrike"));

        //overlap with the collider and classic damage call
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        thunderstrikeVFX.transform.position = spearPos;
        thunderstrikeCollider.transform.position = spearPos;
        thunderstrikeCollider.SetActive(true);
        thunderstrikeVFX.GetComponent<VisualEffect>().Play();
        Collider[] colliders = thunderstrikeCollider.GetComponent<CapsuleCollider>().CapsuleOverlap();

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<IDamageable>(out var entity) && collider.gameObject != hero.gameObject)
                {
                    hero.Attack(entity, 10);
                }
            }
        }

        GameObject.Destroy(thunderstrikeCollider, 1f);
        GameObject.Destroy(thunderstrikeVFX, 1f);
    }
} 
