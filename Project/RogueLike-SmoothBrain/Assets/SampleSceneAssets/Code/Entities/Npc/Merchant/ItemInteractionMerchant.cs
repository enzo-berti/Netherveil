using UnityEngine;

public class ItemInteractionMerchant : MonoBehaviour, IInterractable
{
    private Outline outline;
    private ItemDescription itemDescription;
    private Hero hero;
    private PlayerInteractions interactions;
    private Item item;

    private bool isSelect = false;

    private void Start()
    {
        outline = GetComponent<Outline>();
        itemDescription = GetComponent<ItemDescription>();
        item = GetComponent<Item>();
        hero = FindObjectOfType<Hero>();
        interactions = hero.GetComponent<PlayerInteractions>();
    }

    private void Update()
    {
        Interraction();
    }

    public void Select()
    {
        if (isSelect)
            return;

        isSelect = true;
        outline.EnableOutline();
        itemDescription.TogglePanel(true);
        HudHandler.current.ItemBar.Toggle(true);
    }

    public void Deselect()
    {
        if (!isSelect)
            return;

        isSelect = false;
        outline.DisableOutline();
        itemDescription.TogglePanel(false);
        HudHandler.current.ItemBar.Toggle(false, 2.0f);
    }

    private void Interraction()
    {
        bool isInRange = Vector2.Distance(interactions.transform.position.ToCameraOrientedVec2(), transform.position.ToCameraOrientedVec2())
            <= hero.Stats.GetValue(Stat.CATCH_RADIUS);

        if (isInRange && !interactions.InteractablesInRange.Contains(this))
        {
            interactions.InteractablesInRange.Add(this);
        }
        else if (!isInRange && interactions.InteractablesInRange.Contains(this))
        {
            interactions.InteractablesInRange.Remove(this);
            Deselect();
        }
    }

    public void Interract()
    {
        int price = (int)(item.Price * Item.priceCoef);

        if (hero.Inventory.Blood.Value < price)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.NotEnoughtBloodSFX);
            return;
        }
           
        Deselect();

        AudioManager.Instance.PlaySound(AudioManager.Instance.ItemBuySFX);
        hero.Inventory.Blood -= price;
        item.ItemEffect.Name = item.idItemName;

        hero.Inventory.AddItem(item);
        interactions.InteractablesInRange.Remove(this);

        Item.InvokeOnRetrieved(item.ItemEffect);

        Destroy(this.gameObject);
        DeviceManager.Instance.ApplyVibrations(0.1f, 0f, 0.1f);
    }
}
