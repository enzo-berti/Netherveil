using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class Entity : MonoBehaviour
{
    
    //[Header("Properties")]
    [SerializeField] protected Stats stats;
    public List<string> statusNameToApply = new List<string>();
    public bool isAlly;
    public delegate void DeathDelegate(Vector3 vector);
    public DeathDelegate OnDeath;

    public List<Status> AppliedStatusList = new();
    [SerializeField] protected List<Status> statusToApply;
    [HideInInspector] public int State;
    private void Awake()
    {
        
    }
    protected virtual void Start()
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
        float chance = UnityEngine.Random.value;
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

#if UNITY_EDITOR
[CustomEditor(typeof(Entity), true), CanEditMultipleObjects]
public class EntityDrawer : Editor
{
    SerializedProperty statProperty;
    SerializedProperty statusNameListProperty;
    SerializedProperty statusToApplyListProperty;
    SerializedProperty ChosenToApplyListProperty;
    int curIndex = 0;
    List<string> statusNameList = new List<string>();
    private void OnEnable()
    {
        statProperty = serializedObject.FindProperty("stats");
        statusToApplyListProperty = serializedObject.FindProperty("statusToApply");
        statusNameListProperty = serializedObject.FindProperty("statusNameToApply");
        
        if (statusNameList.Count == 0)
        {
            var typeList = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Status)));
            foreach (Type status in typeList)
            {
                statusNameList.Add(status.Name);
            }
        }
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(statProperty);
        curIndex = EditorGUILayout.Popup(curIndex, statusNameList.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif