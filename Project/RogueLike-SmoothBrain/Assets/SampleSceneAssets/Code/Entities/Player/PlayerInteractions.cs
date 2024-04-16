using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private List<IInterractable> interactablesInRange = new List<IInterractable>();
    public List<IInterractable> InteractablesInRange
    {
        get
        {
            SelectClosestItem();
            return interactablesInRange;
        }
        private set
        {
            interactablesInRange = value;
        }
    }

    private void SelectClosestItem()
    {
        if (interactablesInRange.Count == 0)
            return;

        Vector2 playerPos = transform.position.ToCameraOrientedVec2();
        interactablesInRange = interactablesInRange.OrderBy(interactable =>
        {
            Vector2 itemPos = (interactable as MonoBehaviour).transform.position.ToCameraOrientedVec2();
            return Vector2.Distance(playerPos, itemPos);
        }
        ).ToList();


        for (int i = 1; i< interactablesInRange.Count; ++i)
        {
            interactablesInRange[i].Deselect();
        }

        interactablesInRange[0].Select();
    }
}
