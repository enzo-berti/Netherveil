using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    static public Hero Hero { get => GameObject.FindWithTag("Player").GetComponent<Hero>(); }
    static public GameObject Player { get => GameObject.FindWithTag("Player"); }
}
