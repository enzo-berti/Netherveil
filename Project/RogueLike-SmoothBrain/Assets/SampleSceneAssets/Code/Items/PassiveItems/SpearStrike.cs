using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SpearStrike : ItemEffect , IPassiveItem 
{
    readonly int AOE_DAMAGES = 10;

    public void OnRetrieved() 
    {
        Spear.OnPlacedInWorld += Thunderstrike;
    }

    public void OnRemove() 
    {
        Spear.OnPlacedInWorld -= Thunderstrike;
    } 
 
    private void Thunderstrike(Vector3 spearPos)
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        GameObject thunderstrikeCollider = GameObject.Instantiate(Resources.Load<GameObject>("ThunderstrikeCollide"));
        GameObject thunderstrikeVFX = GameObject.Instantiate(Resources.Load<GameObject>("VFX_TEMP_Thunderstrike"));
        thunderstrikeCollider.SetActive(false);

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
                    hero.Attack(entity, AOE_DAMAGES);
                }
            }
        }

        GameObject.Destroy(thunderstrikeCollider, 1f);
        //GameObject.Destroy(thunderstrikeVFX, 1f);
    }

    //private IEnumerator PlayVFXLateUpdate(VisualEffect thunderstrikeVFX)
    //{
    //    Debug.Log("eyo");
    //    yield return new WaitForSeconds(0.1f);

    //}
} 
