using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    IActiveItem activeItem = null;
    List<IPassiveItem> passiveItems = new List<IPassiveItem>();

    public IActiveItem ActiveItem { get { return activeItem; } }
    public List<IPassiveItem> PassiveItems { get { return passiveItems; } }

    public void AddActiveItem(IActiveItem item)
    {
        if(activeItem == null)
        {
            activeItem = item;
        }
        else
        {
            // TODO : drop l'objet qu'on poss�de d�j� pour le remplacer par le nouveau.
            activeItem = item;
        }
    }

    public void AddPassiveItem(IPassiveItem item)
    {
        passiveItems.Add(item);
    }
}
