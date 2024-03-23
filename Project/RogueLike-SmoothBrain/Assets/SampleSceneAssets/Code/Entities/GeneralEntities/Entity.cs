using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] protected Stats stats;
    public bool isAlly;

    public delegate void DeathDelegate(Vector3 vector);
    public DeathDelegate OnDeath;

    public List<Status> AppliedStatusList = new();
    [HideInInspector] public int State;

    private void Start()
    {
        OnDeath += ctx => ClearStatus();
    }

    private void Update()
    {
        if(AppliedStatusList.Count > 0)
        {
            for(int i = AppliedStatusList.Count - 1; i >= 0; i--)
            {
                if (!AppliedStatusList[i].isFinished)
                {
                    AppliedStatusList[i].DoEffect();
                }
                else
                {
                    AppliedStatusList.RemoveAt(i);
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
        status.target = this;
        float chance = Random.value;
        if(chance <= status.statusChance)
        {
            foreach (var item in AppliedStatusList)
            {
                if (item.GetType() == status.GetType())
                {
                    item.AddStack(1);
                    return;
                }
            }
            status.ApplyEffect(this);
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

    public void AddStatus(Status status)
    {
        AppliedStatusList.Add(status);
    }

    protected void ClearStatus()
    {
        AppliedStatusList.Clear();
    }
}