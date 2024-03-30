using UnityEngine;

public class BloodDrop : Consumable
{
    [SerializeField] private int price = 0;
    [SerializeField] int bloodQuantity = 0;

    protected override void Start()
    {
        Price = price;
    }

    public override void OnRetrieved()
    {
        player.Inventory.Blood += bloodQuantity;
    }
}
