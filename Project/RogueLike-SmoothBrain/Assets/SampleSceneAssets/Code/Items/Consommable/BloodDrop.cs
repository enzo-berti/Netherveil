using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDrop : MonoBehaviour, IConsommable
{
    public float Price => price;

    public bool CanBeRetrieved => canBeRetrieved;
    Hero player;
    [SerializeField] private float price = 0;
    private bool canBeRetrieved = true;

    [SerializeField] int bloodQuantity = 0;
    public void OnRetrieved()
    {
        player.Inventory.Blood += bloodQuantity;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }
}
