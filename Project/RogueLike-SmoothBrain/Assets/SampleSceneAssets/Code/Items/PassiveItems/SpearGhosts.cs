using System.Collections.Generic;
using UnityEngine; 
 
public class SpearGhosts : ItemEffect , IPassiveItem 
{ 
    List<Spear> ghostSpears = new List<Spear>();
    public void OnRetrieved() 
    {
        PlayerInput.OnThrowSpear += ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear += RetrieveSpearGhosts;
        Spear.OnPlacedInHand += DestroySpearGhosts;
    }

    public void OnRemove() 
    {
        PlayerInput.OnThrowSpear -= ThrowSpearGhosts;
        PlayerInput.OnRetrieveSpear -= RetrieveSpearGhosts;
        Spear.OnPlacedInHand -= DestroySpearGhosts;
    } 
 
    private void ThrowSpearGhosts(Vector3 posToReach)
    {
        //instantiate 2 spears and stock them into a list, mark them as not original spear by public bool in script, to ensure that they don't get stuck back in player's hand
        //if possible instantiate them with a different material to differentiate them from the original one
        //rotate posToReach by 30° and -30° for spear 1 and 2 (need to take into consideration camera angles)
        //throw them
    }

    private void RetrieveSpearGhosts()
    {
        //retrieve the 2 stocked spears
    }

    private void DestroySpearGhosts()
    {
        //destroy the 2 spear ghosts and clear list
        foreach(Spear spear in ghostSpears)
        {
            GameObject.Destroy(spear.gameObject);
        }

        ghostSpears.Clear();
    }
} 
