using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] Stats stats;
    public Stats Stats
    {
        get
        {
            return stats;
        }
    }
}
