using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Entity : MonoBehaviour
{

    //[Header("Properties")]
    [SerializeField] protected Stats stats;
    [SerializeField] List<string> statusNameToApply = new List<string>();
    [SerializeField] List<float> durationStatusToApply = new List<float>();
    public bool isAlly;
    public delegate void DeathDelegate(Vector3 vector);
    public DeathDelegate OnDeath;

    public List<Status> AppliedStatusList = new();
    protected List<Status> statusToApply = new();
    [HideInInspector] public int State;
    private void Awake()
    {
        for(int i = 0;  i < statusNameToApply.Count; i++)
        {
            Type statusType = Assembly.GetExecutingAssembly().GetType(statusNameToApply[i]);
            ConstructorInfo constructor = statusType.GetConstructor(new[] { typeof(float) });
            if (constructor != null)
            {
                statusToApply.Add((Status)constructor.Invoke(new object[] { durationStatusToApply[i] }));
            }
        }
    }
    protected virtual void Start()
    {
        OnDeath += ctx => ClearStatus();
    }

    private void Update()
    {
        if (AppliedStatusList.Count > 0)
        {
            for (int i = AppliedStatusList.Count - 1; i >= 0; i--)
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
        if (chance <= status.statusChance)
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
    SerializedProperty statusDurationListProperty;
    SerializedProperty isAllyProperty;
    List<int> allIndex = new();
    bool isStatusExpended = false;
    int nbStats = 0;
    List<string> statusNameList = new List<string>();
    List<float> durationList = new();
    private void OnEnable()
    {
        statProperty = serializedObject.FindProperty("stats");
        statusNameListProperty = serializedObject.FindProperty("statusNameToApply");
        statusDurationListProperty = serializedObject.FindProperty("durationStatusToApply");
        isAllyProperty = serializedObject.FindProperty("isAlly");


        if (statusNameList.Count == 0)
        {
            var typeList = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Status)));
            foreach (Type status in typeList)
            {
                statusNameList.Add(status.Name);
            }
        }

        for (int i = 0; i < statusNameListProperty.arraySize; i++)
        {
            nbStats++;
            allIndex.Add(statusNameList.IndexOf(statusNameListProperty.GetArrayElementAtIndex(i).stringValue));
            durationList.Add(statusDurationListProperty.GetArrayElementAtIndex(i).floatValue);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(statProperty);
        EditorGUILayout.BeginHorizontal();
        isStatusExpended = EditorGUILayout.Foldout(isStatusExpended, "Status :");
        EditorGUILayout.EndHorizontal();
        if (isStatusExpended)
        {

            for (int i = 0; i < nbStats; i++)
            {
                EditorGUI.indentLevel++;
                if (allIndex.Count <= i)
                {
                    allIndex.Add(0);
                }
                EditorGUILayout.BeginHorizontal();
                allIndex[i] = EditorGUILayout.Popup(allIndex[i], statusNameList.ToArray());
                if(durationList.Count <= i)
                {
                    durationList.Add(0);
                }
                durationList[i] = EditorGUILayout.FloatField(durationList[i]);
                if(statusDurationListProperty.arraySize <= i)
                {
                    statusDurationListProperty.InsertArrayElementAtIndex(i);
                }
                
                GUI.color = Color.red;
                if (GUILayout.Button("X"))
                {
                    allIndex.RemoveAt(i);
                    durationList.RemoveAt(i);
                    statusNameListProperty.DeleteArrayElementAtIndex(i);
                    statusDurationListProperty.DeleteArrayElementAtIndex(i);
                    nbStats--;
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                statusNameListProperty.GetArrayElementAtIndex(i).stringValue = statusNameList[allIndex[i]];
                statusDurationListProperty.GetArrayElementAtIndex(i).doubleValue = durationList[i];
                EditorGUI.indentLevel--;

            }

            GUI.color = Color.green;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                statusNameListProperty.InsertArrayElementAtIndex(nbStats);
                statusDurationListProperty.InsertArrayElementAtIndex(nbStats);
                allIndex.Add(0);
                durationList.Add(0);
                nbStats++;
            }
            EditorGUILayout.EndHorizontal();

            GUI.color = Color.white;


        }
        EditorGUILayout.PropertyField(isAllyProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif