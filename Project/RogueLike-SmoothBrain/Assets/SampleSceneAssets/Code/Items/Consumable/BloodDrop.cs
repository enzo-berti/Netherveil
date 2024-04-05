using UnityEngine;

public class BloodDrop : Consumable
{
    public static float BloodDropCoeff = 1.0f;
    [SerializeField] private int price = 0;
    [SerializeField] int bloodQuantity = 0;

    protected override void Start()
    {
        Price = price;
    }

    public override void OnRetrieved()
    {
        player.Inventory.Blood += (int)(bloodQuantity * BloodDropCoeff);
    }
}
