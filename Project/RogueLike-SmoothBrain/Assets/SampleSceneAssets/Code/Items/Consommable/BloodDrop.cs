using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDrop : MonoBehaviour, IConsommable
{
    public float Price => price;

    public bool CanBeRetreived => canBeRetreived;
    Hero player;
    [SerializeField] private float price = 0;
    private bool canBeRetreived = true;

    [SerializeField] int bloodQuantity = 0;
    public void OnRetreived()
    {
        player.Inventory.Blood += bloodQuantity;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }
}
