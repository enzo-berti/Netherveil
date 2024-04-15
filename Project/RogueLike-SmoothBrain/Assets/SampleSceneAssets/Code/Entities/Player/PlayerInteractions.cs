using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public List<IInterractable> InteractablesInRange { get; private set; } = new List<IInterractable>();

    void Update()
    {
        SelectClosestItem();
    }

    private void SelectClosestItem()
    {
        if (InteractablesInRange.Count == 0)
            return;

        Vector2 playerPos = transform.position.ToCameraOrientedVec2();
        InteractablesInRange = InteractablesInRange.OrderBy(interactable =>
        {
            Vector2 itemPos = (interactable as MonoBehaviour).transform.position.ToCameraOrientedVec2();
            return Vector2.Distance(playerPos, itemPos);
        }
        ).ToList();


        for (int i = 1; i< InteractablesInRange.Count; ++i)
        {
            if ((InteractablesInRange[i] as MonoBehaviour).TryGetComponent(out Outline outlineItem))
                outlineItem.DisableOutline();
            if ((InteractablesInRange[i] as MonoBehaviour).TryGetComponent(out ItemDescription itemDesc))
                itemDesc.TogglePanel(false);
            if ((InteractablesInRange[i] as MonoBehaviour).TryGetComponent(out Npc npc))
                npc.rangeImage.gameObject.SetActive(false);
        }

        if((InteractablesInRange[0] as MonoBehaviour).TryGetComponent(out Outline outline))
            outline.EnableOutline();

        if ((InteractablesInRange[0] as MonoBehaviour).TryGetComponent(out ItemDescription itemDescription))
            itemDescription.TogglePanel(true);

        if ((InteractablesInRange[0] as MonoBehaviour).TryGetComponent(out Npc npc2))
            npc2.rangeImage.gameObject.SetActive(true);
    }
}
