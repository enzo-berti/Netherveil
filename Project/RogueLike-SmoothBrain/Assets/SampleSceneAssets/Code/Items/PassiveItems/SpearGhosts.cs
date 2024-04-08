using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.VFX;

public class SpearGhosts : ItemEffect , IPassiveItem 
{
    readonly List<GameObject> ghostSpears = new();
    readonly List<GameObject> spearThrowWrappers = new();
    readonly List<GameObject> spearVFXs = new();
    public void OnRetrieved() 
    {
        //used a wrapper instead of the object itself to make it rotate from player's position, not the middle of the collide
        GameObject spearThrowWrapper = GameObject.FindWithTag("Player").GetComponent<PlayerController>().SpearThrowWrapper;
        GameObject spearVFX = GameObject.FindWithTag("Player").GetComponent<PlayerController>().spearLaunchVFX.gameObject;

        for(int i = 0; i< 2; ++i)
        {
            spearThrowWrappers.Add(GameObject.Instantiate(spearThrowWrapper, spearThrowWrapper.transform.position,
            spearThrowWrapper.transform.rotation, spearThrowWrapper.transform.parent));
            spearVFXs.Add(GameObject.Instantiate(spearVFX, spearVFX.transform.position,
            spearVFX.transform.rotation));
        }

        PlayerInput.OnThrowSpear += ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear += RetrieveSpearGhosts;
        Spear.OnPlacedInHand += DestroySpearGhosts;
    }

    public void OnRemove()
    {
        for (int i = 0; i < spearThrowWrappers.Count; ++i)
        {
            GameObject.Destroy(spearThrowWrappers[i]);
            GameObject.Destroy(spearVFXs[i]);
        }
        spearThrowWrappers.Clear();
        spearVFXs.Clear();

        PlayerInput.OnThrowSpear -= ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear -= RetrieveSpearGhosts;
        Spear.OnPlacedInHand -= DestroySpearGhosts;
    } 
 
    private void ThrowSpearGhosts(Vector3 posToReach)
    {
        //if possible instantiate them with a different material to differentiate them from the original one
        GameObject spear = GameObject.FindWithTag("Player").GetComponent<PlayerController>().Spear.gameObject;
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        for (int i = 0; i< spearThrowWrappers.Count; ++i)
        {
            GameObject ghostSpear = GameObject.Instantiate(spear, spear.transform.position, spear.transform.rotation, spear.transform.parent);
            ghostSpears.Add(ghostSpear);

            ghostSpear.GetComponent<Spear>().SpearThrowCollider = spearThrowWrappers[i].GetComponentInChildren<BoxCollider>(includeInactive: true);
            Vector3 newPosToReach = posToReach.RotatePointAroundYAxis(i != 0 ? 15f : -15f);
            spearThrowWrappers[i].transform.LookAt(newPosToReach);
            spearVFXs[i].transform.position = player.transform.position;
            spearVFXs[i].transform.LookAt(newPosToReach);
            spearVFXs[i].GetComponent<VisualEffect>().Play();

            ghostSpear.GetComponent<Spear>().Throw(newPosToReach);
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
