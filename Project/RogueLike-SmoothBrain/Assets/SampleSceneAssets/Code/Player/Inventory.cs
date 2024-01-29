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

    public void AddItem(ItemEffect item)
    {
        if (item as IActiveItem != null)
        {
            AddActiveItem(item as IActiveItem);
        }
        else if (item as IPassiveItem!= null)
        {
            AddPassiveItem(item as IPassiveItem);
        }

        if ((item as IPassiveItem) != null)
        {
            (item as IPassiveItem).OnRetrieved();
        }
    }
}
