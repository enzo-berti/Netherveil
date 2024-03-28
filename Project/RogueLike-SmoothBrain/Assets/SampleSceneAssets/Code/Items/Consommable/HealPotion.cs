using UnityEngine;

public class HealPotion : MonoBehaviour, IConsumable
{
    [SerializeField] int healValue;
    [SerializeField] float price;
    
    public bool canBeRetrieved = true;

    public float Price => price;
    public bool CanBeRetrieved => canBeRetrieved;

    Hero player;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }
    public void OnRetrieved()
    {
        int realHealValue = (int)(healValue * player.Stats.GetValue(Stat.HEAL_COEFF));
        player.Stats.IncreaseValue(Stat.HP, realHealValue, true);
        FloatingTextGenerator.CreateHealText(realHealValue, player.transform.position);
        Destroy(this.gameObject);
    }
}
