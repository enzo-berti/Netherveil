using System.Collections.Generic;
using UnityEngine;

public class SpearGhosts : ItemEffect , IPassiveItem 
{
    readonly List<GameObject> ghostSpears = new();
    readonly List<GameObject> spearThrowWrappers = new();
    public void OnRetrieved() 
    {
        //used a wrapper instead of the object itself to make it rotate from player's position, not the middle of the collide
        GameObject spearThrowWrapper = GameObject.FindWithTag("Player").GetComponent<PlayerController>().SpearThrowWrapper;

        for(int i = 0; i< 2; ++i)
        {
            spearThrowWrappers.Add(GameObject.Instantiate(spearThrowWrapper, spearThrowWrapper.transform.position,
            spearThrowWrapper.transform.rotation, spearThrowWrapper.transform.parent));
        }

        PlayerInput.OnThrowSpear += ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear += RetrieveSpearGhosts;
        Spear.OnPlacedInHand += DestroySpearGhosts;
    }

    public void OnRemove()
    {
        //delete them in onRemove
        foreach (GameObject wrappers in spearThrowWrappers)
        {
            GameObject.Destroy(wrappers);
        }
        spearThrowWrappers.Clear();

        PlayerInput.OnThrowSpear -= ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear -= RetrieveSpearGhosts;
        Spear.OnPlacedInHand -= DestroySpearGhosts;
    } 
 
    private void ThrowSpearGhosts(Vector3 posToReach)
    {
        //if possible instantiate them with a different material to differentiate them from the original one
        GameObject spear = GameObject.FindWithTag("Player").GetComponent<PlayerController>().Spear.gameObject;
        for (int i = 0; i< 2; ++i)
        {
            ghostSpears.Add(GameObject.Instantiate(spear, spear.transform.position, spear.transform.rotation, spear.transform.parent));
        }

        //need to take into consideration camera angles
        for (int i = 0; i< ghostSpears.Count; ++i)
        {
            ghostSpears[i].GetComponent<Spear>().SpearThrowCollider = spearThrowWrappers[i].GetComponentInChildren<BoxCollider>(includeInactive: true);
            Vector3 newPosToReach = posToReach.RotatePointAroundYAxis(i != 0 ? 15f : -15f);
            spearThrowWrappers[i].transform.LookAt(newPosToReach);
            ghostSpears[i].GetComponent<Spear>().Throw(newPosToReach);
        }
    }

    private void RetrieveSpearGhosts()
    {
        foreach(GameObject spear in ghostSpears)
        {
            spear.GetComponent<Spear>().Return();
        }
    }

    private void DestroySpearGhosts()
    {
        foreach(GameObject spear in ghostSpears)
        {
            GameObject.Destroy(spear);
        }

        ghostSpears.Clear();
    }
} 
