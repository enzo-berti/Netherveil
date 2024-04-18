using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    static public GameObject Player { get => GameObject.FindWithTag("Player"); }
    static public Hero Hero { get => Player.GetComponent<Hero>(); }
    static public Inventory Inventory { get => Hero.Inventory; }
}
