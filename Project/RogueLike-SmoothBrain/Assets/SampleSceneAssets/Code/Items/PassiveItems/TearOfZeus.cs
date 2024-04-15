using UnityEngine;
using UnityEngine.VFX;

public class TearOfZeus : ItemEffect, IPassiveItem
{
    readonly int AOE_DAMAGES = 10;
    GameObject thunderstrikeCollider;
    GameObject thunderstrikeVFX;
    public void OnRetrieved()
    {
        thunderstrikeCollider = GameObject.Instantiate(Resources.Load<GameObject>("ThunderstrikeCollide"));
        thunderstrikeVFX = GameObject.Instantiate(Resources.Load<GameObject>("VFX_TEMP_Thunderstrike"));
        thunderstrikeCollider.SetActive(false);

        PlayerInput.OnEndDash += DropTear;
    }

    public void OnRemove()
    {
        GameObject.Destroy(thunderstrikeCollider);
        GameObject.Destroy(thunderstrikeVFX);
        PlayerInput.OnEndDash -= DropTear;
    }

    private void DropTear(Vector3 playerPos)
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();

        thunderstrikeVFX.transform.position = playerPos;
        thunderstrikeCollider.transform.position = playerPos;
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
    }
}
