using UnityEngine;

public class HealPotion : Consumable
{
    [SerializeField] int healValue;
    [SerializeField] int price;

    protected override void Start()
    {
        base.Start();
        Price = price;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnRetrieved()
    {
        int realHealValue = (int)(healValue * player.Stats.GetValue(Stat.HEAL_COEFF));
        player.Stats.IncreaseValue(Stat.HP, realHealValue, true);
        FloatingTextGenerator.CreateHealText(realHealValue, player.transform.position);
        Destroy(this.gameObject);
    }
}
