using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Inventory
{
    public static GameObject ActiveItemGameObject;
    IActiveItem activeItem = null;
    List<IPassiveItem> passiveItems = new List<IPassiveItem>();
    public IActiveItem ActiveItem { get { return activeItem; } }
    public List<IPassiveItem> PassiveItems { get { return passiveItems; } }
    public List<IItem> AllItems
    {
        get
        {
            List<IItem> itemList = new List<IItem>
            {
                activeItem
            };
            itemList.AddRange(passiveItems);
            return itemList;
        }
    }
    public bool HasActiveItem
    {
        get => activeItem != null;
    }
    public int Blood = 0;
    public int Keys = 0;

    private void AddActiveItem(IActiveItem item)
    {
        if (activeItem != null && (activeItem as IPassiveItem) != null)
        {
            (activeItem as IPassiveItem).OnRemove();
        }
        activeItem = item;
    }

    private void AddPassiveItem(IPassiveItem item)
    {
        passiveItems.Add(item);
    }

    public void RemoveItem(IPassiveItem item)
    {
        item.OnRemove();
        passiveItems.Remove(item);
    }
    public void AddItem(Item item)
    {
        ItemEffect itemEffect = item.ItemEffect;
        if ((itemEffect as IActiveItem) != null)
        {
            if (activeItem == null)
            {
                ActiveItemGameObject = GameObject.Instantiate(item.gameObject);
            }
            else
            {
                GameObject.Instantiate(ActiveItemGameObject, item.gameObject.transform.position, Quaternion.identity);
                ActiveItemGameObject = GameObject.Instantiate(item.gameObject);
                ActiveItemGameObject.gameObject.SetActive(false);
            }
            AddActiveItem(itemEffect as IActiveItem);

            if (!itemEffect.HasBeenRetreived)
            {
                itemEffect.CurrentEnergy = (itemEffect as IActiveItem).Cooldown;
            }

        }
        else if (itemEffect as IPassiveItem != null)
        {
            AddPassiveItem(itemEffect as IPassiveItem);
        }
        (itemEffect as IPassiveItem)?.OnRetrieved();
        itemEffect.HasBeenRetreived = true;
    }
}
