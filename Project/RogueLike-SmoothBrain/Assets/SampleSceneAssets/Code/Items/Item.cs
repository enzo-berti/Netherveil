using System;
using UnityEngine;

public class Item : MonoBehaviour, IInterractable
{
    [SerializeField]ItemData data;
    private void Update()
    {
        if( Vector2.Distance( GameObject.FindWithTag("Player").transform.position, transform.position ) < 10 )
        {
            Interract();
        }
    }
    public void Interract()
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().Inventory.AddItem(Activator.CreateInstance(data.effect.GetClass()) as ItemEffect);
        Destroy(this.gameObject);
    }
}
