using UnityEngine;

public class HealPotion : MonoBehaviour, IConsommable
{
    [SerializeField] float healValue;
    [SerializeField] float price;
    
    public bool canBeRetreived = true;

    public float Price => price;
    public bool CanBeRetreived => canBeRetreived;

    Hero player;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }
    public void OnRetreived()
    {
        player.Stats.IncreaseValue(Stat.HP, healValue, true);
        Destroy(this.gameObject);
    }
}
