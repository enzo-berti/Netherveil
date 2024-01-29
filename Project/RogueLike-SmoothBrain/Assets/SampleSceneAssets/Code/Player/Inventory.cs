using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    IActiveItem activeItem = null;
    List<IPassiveItem> passiveItems = new List<IPassiveItem>();

    public IActiveItem ActiveItem { get { return activeItem; } }
    public List<IPassiveItem> PassiveItems { get { return passiveItems; } }

    private void AddActiveItem(IActiveItem item)
    {
        if(activeItem == null)
        {
            activeItem = item;
        }
        else
        {
            activeItem = item;
        }
    }

    private void AddPassiveItem(IPassiveItem item)
    {
        passiveItems.Add(item);
        item.OnRetrieved();
    }

    //public void AddItem(ItemData item)
    //{
    //    if(item.GetComponent<IActiveItem>() != null)
    //    {
    //        AddActiveItem(item.GetComponent<IActiveItem>());
    //    }
    //    else if (item.GetComponent<IPassiveItem>() != null)
    //    {
    //        AddPassiveItem(item.GetComponent<IPassiveItem>());
    //    }

    //    if ((item.GetComponent<IPassiveItem>()) != null)
    //    {
    //        item.GetComponent<IPassiveItem>().OnRetrieved();
    //    }
    //}
}
