using UnityEngine;

public class ItemInteractionMerchant : MonoBehaviour, IInterractable
{
    private Outline outline;
    private ItemDescription itemDescription;
    private Hero hero;
    private PlayerInteractions interaction;

    private void Start()
    {
        outline = GetComponent<Outline>();
        itemDescription = GetComponent<ItemDescription>();
        hero = FindObjectOfType<Hero>();
        interaction = hero.GetComponent<PlayerInteractions>();
    }

    public void Select()
    {
        outline.EnableOutline();
        itemDescription.TogglePanel(true);
    }

    public void Deselect()
    {
        outline.DisableOutline();
        itemDescription.TogglePanel(false);
    }

    public void Interract()
    {
        //itemToGive.Name = idItemName;
        //hero.Inventory.AddItem(itemToGive);
        //interaction.InteractablesInRange.Remove(this);
        //Item.OnRetrieved?.Invoke(itemToGive);

        //Destroy(this.gameObject);
        //DeviceManager.Instance.ApplyVibrations(0.1f, 0f, 0.1f);
    }
}
