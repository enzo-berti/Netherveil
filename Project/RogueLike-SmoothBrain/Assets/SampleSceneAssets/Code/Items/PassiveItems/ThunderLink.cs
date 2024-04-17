using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderLink : ItemEffect, IPassiveItem
{
    readonly List<BoxCollider> thunderLinkColliders = new();
    readonly float THUNDERLINK_WAIT_TIME = 0.5f;

    public void OnRetrieved()
    {
        Spear.OnPlacedInWorld += CreateEletricLinks;
        PlayerInput.OnRetrieveSpear += DeleteEletricLinks;
    }

    public void OnRemove()
    {
        Spear.OnPlacedInWorld -= CreateEletricLinks;
        PlayerInput.OnRetrieveSpear -= DeleteEletricLinks;
    }

    private void DeleteEletricLinks()
    {
        CoroutineManager.Instance.StopAllCoroutines();
        thunderLinkColliders.Clear();
    }

    private void CreateEletricLinks(Spear spear)
    {
        thunderLinkColliders.Add(spear.SpearThrowCollider);
        CoroutineManager.Instance.StartCustom(TriggerElectricLinks(spear));
    }

    private IEnumerator TriggerElectricLinks(Spear spear)
    {
        Hero player = Utilities.Hero;
        while (true)
        {
            yield return new WaitForSeconds(THUNDERLINK_WAIT_TIME / 2f);
            spear.SpearThrowCollider.gameObject.SetActive(false);
            yield return new WaitForSeconds(THUNDERLINK_WAIT_TIME / 2f);

            spear.SpearThrowCollider.gameObject.SetActive(true);
            spear.ScaleColliderToVector((spear.transform.position - Utilities.Player.transform.position));
            spear.SpearThrowCollider.transform.parent.LookAt(spear.transform.position);
            //set vfx size here
            //play vfx

            Collider[] colliders = spear.SpearThrowCollider.BoxOverlap();

            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.TryGetComponent<IDamageable>(out var entity) && collider.gameObject != player.gameObject)
                    {
                        player.Attack(entity);
                    }
                }
            }
        }
    }

}
