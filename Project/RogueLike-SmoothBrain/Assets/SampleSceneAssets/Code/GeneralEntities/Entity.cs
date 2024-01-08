using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] protected Stats stats;
    public Stats Stats
    {
        get
        {
            return stats;
        }
    }
}
