using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] protected Stats stats;
    public bool isAlly;

    public delegate void DeathDelegate(Vector3 vector);
    public DeathDelegate OnDeath;

    List<Status> statusList = new();

    private void Update()
    {
        if(statusList.Count > 0)
        {
            for(int i = statusList.Count - 1; i >= 0; i--)
            {
                if (!statusList[i].isFinished)
                {
                    statusList[i].DoEffect();
                }
                else
                {
                    statusList.RemoveAt(i);
                }
            }
        }
        
    }
    public Stats Stats
    {
        get
        {
            return stats;
        }
    }

    public void ApplyEffect(Status status)
    {
        Debug.Log("ApplyEffect Entity");
        status.target = this;
        status.ApplyEffect(this);
    }
    public enum EntityState : int
    {
        MOVE,
        ATTACK,
        HIT,
        DEAD,
        NB
    }

    public void AddStatus(Status status)
    {
        statusList.Add(status);
    }
    [HideInInspector] public int State;
}