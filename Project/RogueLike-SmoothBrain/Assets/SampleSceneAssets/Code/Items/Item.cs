using UnityEngine;

public abstract class Item : MonoBehaviour, IInterractable
{
    public virtual void Interract()
    {
        Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>().Inventory;
        if((this.GetComponent<IActiveItem>()) != null)
        {
            inventory.AddActiveItem(this.GetComponent<IActiveItem>());
        }
        else if((this.GetComponent<IPassiveItem>()) != null)
        {
            inventory.AddPassiveItem(this.GetComponent<IPassiveItem>());
        }
    }
}
