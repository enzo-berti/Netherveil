using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public List<Item> ItemsInRange { get; private set; } = new List<Item>();
    Item lastClosestItem = null;

    void Update()
    {
        SelectClosestItem();
    }

    private void SelectClosestItem()
    {
        if (ItemsInRange.Count == 0 || lastClosestItem == ItemsInRange[0])
            return;

        Vector2 playerPos = transform.position.ToCameraOrientedVec2();
        ItemsInRange = ItemsInRange.OrderBy(item =>
        {
            Vector2 itemPos = item.transform.position.ToCameraOrientedVec2();
            return Vector2.Distance(playerPos, itemPos);
        }
        ).ToList();


        foreach (var item in ItemsInRange)
        {
            item.GetComponent<Outline>().DisableOutline();
        }

        ItemsInRange[0].GetComponent<Outline>().EnableOutline();
        lastClosestItem = ItemsInRange[0];
    }
}
