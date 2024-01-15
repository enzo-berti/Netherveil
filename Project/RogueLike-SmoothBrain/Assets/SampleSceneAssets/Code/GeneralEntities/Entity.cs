using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] protected Stats stats;
    public bool isAlly;

    public delegate void DeathDelegate(Vector3 vector);
    public DeathDelegate OnDeath;

    
    public Stats Stats
    {
        get
        {
            return stats;
        }
    }

    public enum EntityState : int
    {
        MOVE,
        ATTACK,
        HIT,
        DEAD,
        NB
    }

    public int State;
}