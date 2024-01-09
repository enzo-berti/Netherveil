using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public enum EntityState
    {
        MOVE,
        DASH,
        ATTACK,
        HIT,
        DEAD
    }

    [Header("Properties")]
    [SerializeField] protected Stats stats;
    public bool isAlly;
    public EntityState state;
    public Stats Stats
    {
        get
        {
            return stats;
        }
    }
}
