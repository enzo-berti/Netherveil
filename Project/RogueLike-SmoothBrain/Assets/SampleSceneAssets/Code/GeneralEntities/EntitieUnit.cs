using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntitieUnit : MonoBehaviour
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
