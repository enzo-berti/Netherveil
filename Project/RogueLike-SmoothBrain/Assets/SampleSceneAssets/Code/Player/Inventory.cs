using System.Collections.Generic;

public class Inventory
{
    IActiveItem activeItem = null;
    List<IPassiveItem> passiveItems;

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
            // TODO : drop l'objet qu'on possède déjà pour le remplacer par le nouveau.
            activeItem = item;
        }
    }

    public void AddPassiveItem(IPassiveItem item)
    {
        passiveItems.Add(item);
    }
}
