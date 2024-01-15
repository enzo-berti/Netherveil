using UnityEngine;

public abstract class Item : MonoBehaviour, IInterractable
{
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }

    public Rarity RarityTier { get; private set; } = 0;
    public virtual void Interract()
    {
        Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>().Inventory;
        if((this.GetComponent<IActiveItem>()) != null)
        {
            inventory.AddActiveItem(this.GetComponent<IActiveItem>());
        }
        else if((this.GetComponent<IPassiveItem>()) != null)
        {
            Debug.Log(inventory == null);
            Debug.Log(this.GetComponent<IPassiveItem>() == null);
            inventory.AddPassiveItem(this.GetComponent<IPassiveItem>());
        }

        if((this.GetComponent<IPassiveItem>()) != null)
        {
            this.GetComponent<IPassiveItem>().OnRetrieved();
        }

        Destroy(this.gameObject);
    }
}
