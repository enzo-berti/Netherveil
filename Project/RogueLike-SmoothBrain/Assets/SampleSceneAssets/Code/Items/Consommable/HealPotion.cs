using UnityEngine;

public class HealPotion : MonoBehaviour, IConsommable
{
    [SerializeField] float healValue;
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
        player.Stats.IncreaseValue(Stat.HP, healValue, true);
        Destroy(this.gameObject);
    }
}
