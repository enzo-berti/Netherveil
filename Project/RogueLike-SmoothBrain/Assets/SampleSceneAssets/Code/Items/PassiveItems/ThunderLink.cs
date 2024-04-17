using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
 
public class ThunderLink : ItemEffect , IPassiveItem 
{
    readonly List<GameObject> thunderLinkColliders = new();
    readonly List<GameObject> thunderLinkVFXs = new();
    readonly List<Vector3> spearsPos = new();
    readonly float THUNDERLINK_WAIT_TIME = 0.5f;
    readonly int THUNDERLINK_DAMAGES = 5;

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
        for (int i = 0; i< thunderLinkColliders.Count; i++)
        {
            GameObject.Destroy(thunderLinkColliders[i]);
            GameObject.Destroy(thunderLinkVFXs[i]);
            CoroutineManager.Instance.StopCoroutine(TriggerElectricLinks(thunderLinkColliders[i], thunderLinkVFXs[i], spearsPos[i]));
        }
        thunderLinkColliders.Clear();
        thunderLinkVFXs.Clear();
        spearsPos.Clear();
    }

    private void CreateEletricLinks(Vector3 spearPos)
    {
        GameObject thunderlinkCollide = GameObject.Instantiate(Resources.Load<GameObject>("ThunderlinkCollide"));
        GameObject thunderlinkVFX = GameObject.Instantiate(Resources.Load<GameObject>("VFX_Thunderlink"));
        thunderLinkColliders.Add(thunderlinkCollide);
        thunderLinkVFXs.Add(thunderlinkVFX);
        spearsPos.Add(spearPos);
        CoroutineManager.Instance.StartCustom(TriggerElectricLinks(thunderlinkCollide, thunderlinkVFX, spearPos));
    }

    private IEnumerator TriggerElectricLinks(GameObject thunderlinkCollide, GameObject thunderlinkVFX, Vector3 spearPos)
    {
        while(true)
        {
            yield return new WaitForSeconds(THUNDERLINK_WAIT_TIME);

            Hero player = Utilities.Hero;
            thunderlinkCollide.transform.position = player.transform.position;
            thunderlinkVFX.transform.position = player.transform.position;

            Vector3 scale = thunderlinkCollide.transform.localScale;
            scale.z = (spearPos - player.transform.position).magnitude;
            thunderlinkCollide.transform.localScale = scale;
            //set vfx size here

            //ici ce sera la collide en enfant du pivot à changer de position à priori
            thunderlinkCollide.transform.localPosition = new Vector3(0f, 0f, scale.z / 2f);

            thunderlinkCollide.transform.LookAt(spearPos);
            thunderlinkVFX.transform.LookAt(spearPos);

            Collider[] colliders = thunderlinkCollide.GetComponent<BoxCollider>().BoxOverlap();

            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.TryGetComponent<IDamageable>(out var entity) && collider.gameObject != player.gameObject)
                    {
                        player.Attack(entity, THUNDERLINK_DAMAGES);
                    }
                }
            }
        }
    }

} 
