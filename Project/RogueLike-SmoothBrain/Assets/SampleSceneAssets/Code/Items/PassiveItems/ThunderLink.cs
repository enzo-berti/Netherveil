using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class ThunderLink : ItemEffect, IPassiveItem
{
    readonly List<BoxCollider> thunderLinkColliders = new();
    readonly List<VisualEffect> thunderLinkVFXs = new();
    readonly List<LineRenderer> thunderLinkLineRenderers = new();
    readonly List<Coroutine> thunderLinkRoutines = new();
    readonly float THUNDERLINK_WAIT_TIME = 0.5f;
    readonly float duration = 3f;
    readonly float chance = 0.2f;

    public void OnRetrieved()
    {
        Spear.OnPlacedInWorld += CreateEletricLinks;
        Utilities.PlayerInput.OnRetrieveSpear += DeleteEletricLinks;
    }

    public void OnRemove()
    {
        Spear.OnPlacedInWorld -= CreateEletricLinks;
        Utilities.PlayerInput.OnRetrieveSpear -= DeleteEletricLinks;
    }

    private void DeleteEletricLinks()
    {
        foreach(var coroutine in thunderLinkRoutines)
        {
            CoroutineManager.Instance.StopCoroutine(coroutine);
        }
        thunderLinkRoutines.Clear();
        thunderLinkColliders.Clear();

        for(int i = 0; i< thunderLinkVFXs.Count; i++)
        {
            GameObject.Destroy(thunderLinkVFXs[i].gameObject);
            GameObject.Destroy(thunderLinkLineRenderers[i].gameObject);
        }

        thunderLinkVFXs.Clear();
        thunderLinkLineRenderers.Clear();
    }

    private void CreateEletricLinks(Spear spear)
    {
        thunderLinkColliders.Add(spear.SpearThrowCollider);

        VisualEffect vfx = GameObject.Instantiate(GameResources.Get<GameObject>("VFX_ThunderLink").GetComponent<VisualEffect>(), GameObject.FindWithTag("Player").transform.position, Quaternion.identity);
        vfx.transform.position = GameObject.FindWithTag("Player").transform.position + Vector3.up;
        vfx.SetVector3("Attract Target", spear.transform.position + Vector3.up);
        vfx.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXPositionBinderCustom>().ToArray()[0].Target = spear.transform;
        vfx.Play();
        thunderLinkVFXs.Add(vfx);

        LineRenderer lineRenderer = GameObject.Instantiate(GameResources.Get<GameObject>("VFX_ThunderLinkLine").GetComponent<LineRenderer>());
        lineRenderer.gameObject.SetActive(false);
        thunderLinkLineRenderers.Add(lineRenderer);

        spear.SetThunderLinkVFX(vfx, lineRenderer);
        thunderLinkRoutines.Add(CoroutineManager.Instance.StartCoroutine(TriggerElectricLinks(spear)));
        thunderLinkRoutines.Add(CoroutineManager.Instance.StartCoroutine(MoveThunderLink(spear)));
    }

    private IEnumerator TriggerElectricLinks(Spear spear)
    {
        Hero player = Utilities.Hero;

        while (true)
        {
            yield return new WaitForSeconds(THUNDERLINK_WAIT_TIME / 2f);
            spear.ThunderLinkLineRenderer.gameObject.SetActive(false);
            spear.SpearThrowCollider.gameObject.SetActive(false);
            yield return new WaitForSeconds(THUNDERLINK_WAIT_TIME / 2f);

            AudioManager.Instance.PlaySound(AudioManager.Instance.ThunderlinkSFX, spear.transform.position);
            spear.ThunderLinkLineRenderer.gameObject.SetActive(true);
            //spear.SpearThrowCollider.gameObject.SetActive(true);
            spear.ScaleColliderToVector((spear.transform.position - Utilities.Player.transform.position));
            spear.SpearThrowCollider.transform.parent.LookAt(spear.transform.position);

            Collider[] colliders = spear.SpearThrowCollider.BoxOverlap();

            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.TryGetComponent<IDamageable>(out var entity) && collider.gameObject != player.gameObject)
                    {
                        player.Attack(entity);
                        if((entity as MonoBehaviour).TryGetComponent(out Entity mob))
                        {
                            mob.AddStatus(new Electricity(duration, chance), player);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator MoveThunderLink(Spear spear)
    {
        while(true)
        {
            spear.ThunderLinkVFX.transform.position = Utilities.Player.transform.position + Vector3.up;
            spear.ThunderLinkLineRenderer.SetPosition(0, Utilities.Player.transform.position + Vector3.up);
            spear.ThunderLinkLineRenderer.SetPosition(1, new Vector3(spear.transform.position.x, Utilities.Player.transform.position.y + 1, spear.transform.position.z));
            yield return null;
        }
    }

}
