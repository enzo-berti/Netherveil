using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    

    [Header("Properties")]
    [SerializeField] protected Stats stats;
    public bool isAlly;
    
    public Stats Stats
    {
        get
        {
            return stats;
        }
    }
}
