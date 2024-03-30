using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public List<Item> ItemsInRange { get; private set; } = new List<Item>();

    void Update()
    {
        SelectClosestItem();
    }

    private void SelectClosestItem()
    {
        if (ItemsInRange.Count == 0)
            return;

        Vector2 playerPos = transform.position.ToCameraOrientedVec2();
        ItemsInRange = ItemsInRange.OrderBy(item =>
        {
            Vector2 itemPos = item.transform.position.ToCameraOrientedVec2();
            return Vector2.Distance(playerPos, itemPos);
        }
        ).ToList();


        for (int i = 1; i< ItemsInRange.Count; ++i)
        {
            ItemsInRange[i].GetComponent<Outline>().DisableOutline();
            ItemsInRange[i].GetComponent<ItemDescription>().TogglePanel(false);
        }

        ItemsInRange[0].GetComponent<Outline>().EnableOutline();
        ItemsInRange[0].GetComponent<ItemDescription>().TogglePanel(true);
    }
}
