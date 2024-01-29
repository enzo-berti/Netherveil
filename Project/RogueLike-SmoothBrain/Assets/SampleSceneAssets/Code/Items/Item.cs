using System;
using UnityEngine;

public class Item : MonoBehaviour, IInterractable
{
    ItemData data;
    public void Interract()
    {
        GameObject.FindWithTag("Player").GetComponent<Inventory>().AddItem(Activator.CreateInstance(data.effect.GetClass()) as ItemEffect);
    }
}
