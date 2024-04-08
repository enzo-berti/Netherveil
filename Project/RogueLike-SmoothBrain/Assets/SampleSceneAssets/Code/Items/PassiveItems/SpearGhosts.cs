using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpearGhosts : ItemEffect , IPassiveItem 
{
    readonly List<GameObject> ghostSpears = new();
    readonly List<GameObject> spearThrowColliders = new();
    public void OnRetrieved() 
    {
        //instantiate as well 2 more spearThrowColliders
        BoxCollider spearThrowCollider = GameObject.FindWithTag("Player").GetComponent<PlayerController>().GetSpearThrowCollider();

        for(int i = 0; i< 2; ++i)
        {
            spearThrowColliders.Add(GameObject.Instantiate(spearThrowCollider.gameObject, spearThrowCollider.gameObject.transform.position,
            spearThrowCollider.gameObject.transform.rotation, spearThrowCollider.transform.parent));
        }

        PlayerInput.OnThrowSpear += ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear += RetrieveSpearGhosts;
        Spear.OnPlacedInHand += DestroySpearGhosts;
    }

    public void OnRemove()
    {
        //delete them in onRemove
        foreach (GameObject colliders in spearThrowColliders)
        {
            GameObject.Destroy(colliders);
        }
        spearThrowColliders.Clear();

        PlayerInput.OnThrowSpear -= ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear -= RetrieveSpearGhosts;
        Spear.OnPlacedInHand -= DestroySpearGhosts;
    } 
 
    private void ThrowSpearGhosts(Vector3 posToReach)
    {
        //instantiate 2 spears and stock them into a list
        //if possible instantiate them with a different material to differentiate them from the original one
        //assign the spearThrowColliders to the spears

        Transform spearTransform = GameObject.FindWithTag("Player").GetComponent<PlayerController>().Spear.transform;
        for (int i = 0; i< 2; ++i)
        {
            ghostSpears.Add(GameObject.Instantiate(Resources.Load<GameObject>("Spear"), spearTransform.position, spearTransform.rotation, spearTransform.parent));
        }


        //rotate posToReach by 30° and -30° for spear 1 and 2 (need to take into consideration camera angles)
        //throw them

        foreach(GameObject spear in ghostSpears)
        {
            spear.GetComponent<Spear>().Throw(posToReach + new Vector3(UnityEngine.Random.Range(0.5f, 2), 0f, UnityEngine.Random.Range(0.5f, 2)));
        }
    }

    private void RetrieveSpearGhosts()
    {
        //retrieve the 2 stocked spears
        foreach(GameObject spear in ghostSpears)
        {
            spear.GetComponent<Spear>().Return();
        }
    }

    private void DestroySpearGhosts()
    {
        //destroy the 2 spear ghosts and clear list
        foreach(GameObject spear in ghostSpears)
        {
            GameObject.Destroy(spear);
        }

        ghostSpears.Clear();
    }
} 
