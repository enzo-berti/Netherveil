using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpearGhosts : ItemEffect , IPassiveItem 
{
    readonly List<GameObject> ghostSpears = new();
    readonly List<GameObject> spearThrowWrappers = new();
    public void OnRetrieved() 
    {
        //instantiate as well 2 more spearThrowColliders
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
        //instantiate 2 spears and stock them into a list
        //if possible instantiate them with a different material to differentiate them from the original one

        Transform spearTransform = GameObject.FindWithTag("Player").GetComponent<PlayerController>().Spear.transform;
        for (int i = 0; i< 2; ++i)
        {
            ghostSpears.Add(GameObject.Instantiate(Resources.Load<GameObject>("Spear"), spearTransform.position, spearTransform.rotation, spearTransform.parent));
        }

        //rotate posToReach by 30° and -30° for spear 1 and 2 (need to take into consideration camera angles)
        //assign the spearThrowColliders to the spears
        //throw them

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
