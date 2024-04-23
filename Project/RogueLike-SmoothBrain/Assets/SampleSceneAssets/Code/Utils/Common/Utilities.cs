using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    static public GameObject Player { get => GameObject.FindWithTag("Player"); }
    static public Hero Hero { get => Player.GetComponent<Hero>(); }
    static public Stats PlayerStat { get => Hero.Stats; }
    static public Inventory Inventory { get => Hero.Inventory; }

    static public bool IsPlayer(Entity entity)
    {
        return entity.CompareTag("Player");
    }
}
