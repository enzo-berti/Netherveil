using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public static int entitySpawn = 0;

    [Header("Properties")]
    [SerializeField] protected Stats stats;
    [SerializeField] List<string> statusNameToApply = new List<string>();
    [SerializeField] List<float> durationStatusToApply = new List<float>();
    public bool isAlly;
    public delegate void DeathDelegate(Vector3 vector);
    public DeathDelegate OnDeath;

    public List<Status> AppliedStatusList = new();
    protected List<Status> statusToApply = new();
    [HideInInspector] public int State;

    protected virtual void Awake()
    {
        for (int i = 0; i < statusNameToApply.Count; i++)
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

        entitySpawn++;
    }

    protected virtual void Update()
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

    public void ApplyKnockback(IDamageable damageable)
    {
        Knockback knockbackable = (damageable as MonoBehaviour).GetComponent<Knockback>();
        if (knockbackable)
        {
            MonoBehaviour damageableGO = damageable as MonoBehaviour;
            Vector3 damageablePos = damageableGO.transform.position;
            Vector3 TargetToMeVec = (transform.position - damageablePos).normalized;

            if (TargetToMeVec != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(new Vector3(TargetToMeVec.x, 0f, TargetToMeVec.z));
                rotation *= Camera.main.transform.rotation;
                float rotationY = rotation.eulerAngles.y;

                if (damageableGO.TryGetComponent(out PlayerController controller))
                {
                    controller.OverridePlayerRotation(rotationY, true);
                }
                //else
                //{
                //    Quaternion appliedRotation = Quaternion.Euler(damageableGO.transform.eulerAngles.x, rotationY, damageableGO.transform.eulerAngles.z);
                //    damageableGO.transform.rotation = appliedRotation;
                //}
            }

            Vector3 temp = damageablePos - transform.position;
            Vector3 direction = new Vector3(temp.x, 0f, temp.z).normalized;
            knockbackable.GetKnockback(direction, 5.0f, stats.GetValue(Stat.KNOCKBACK_COEFF));
            FloatingTextGenerator.CreateActionText(damageableGO.transform.position, "Pushed!");
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
                    Debug.Log("Add Stack to : " + item.GetType().Name);
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

    // Status name list in the entity ( required to instantiate with the reflection )
    SerializedProperty statusNameListProperty;

    // Duration of each status in the entity ( required to instantiate with the reflection )
    SerializedProperty statusDurationListProperty;

    SerializedProperty isAllyProperty;

    // Index of each status ( in the name list )
    List<int> allIndex = new();
    bool isStatusExpended = false;
    int nbStatus = 0;

    // Local lists that we will use to update properties
    List<string> statusNameList = new List<string>();
    List<float> durationList = new();

    List<string> classField = new List<string>();
    private void OnEnable()
    {
        statProperty = serializedObject.FindProperty("stats");
        statusNameListProperty = serializedObject.FindProperty("statusNameToApply");
        statusDurationListProperty = serializedObject.FindProperty("durationStatusToApply");
        isAllyProperty = serializedObject.FindProperty("isAlly");

        // Add existing Status name in a list
        if (statusNameList.Count == 0)
        {
            var typeList = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Status)));
            foreach (Type status in typeList)
            {
                statusNameList.Add(status.Name);
            }
        }

        // Update local lists with the entity lists
        for (int i = 0; i < statusNameListProperty.arraySize; i++)
        {
            nbStatus++;
            allIndex.Add(statusNameList.IndexOf(statusNameListProperty.GetArrayElementAtIndex(i).stringValue));
            durationList.Add(statusDurationListProperty.GetArrayElementAtIndex(i).floatValue);
        }

        // Get all infos
        FieldInfo[] infos = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        // Get entity infos only
        FieldInfo[] entityInfo = typeof(Entity).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in infos)
        {
            if (entityInfo.FirstOrDefault(x => x.Name == field.Name) != null) continue;
            if ((field.IsPublic && field.GetCustomAttribute(typeof(HideInInspector)) == null) || field.GetCustomAttribute(typeof(SerializeField)) != null)
            {
                classField.Add(field.Name);
            }
        }
        // If I don't reverse, last class values will be written in first
        classField.Reverse();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawScript();
        EditorGUILayout.PropertyField(statProperty);

        EditorGUILayout.BeginHorizontal();
        isStatusExpended = EditorGUILayout.Foldout(isStatusExpended, "Status");
        EditorGUILayout.EndHorizontal();

        if (isStatusExpended)
        {
            for (int i = 0; i < nbStatus; i++)
            {
                EditorGUI.indentLevel++;
                // If there is no index for the current status add it
                if (allIndex.Count <= i)
                {
                    allIndex.Add(0);
                }

                // For the current status, display a popup with all the status that exists
                EditorGUILayout.BeginHorizontal();

                // popup to choose the index of the status in the name List
                allIndex[i] = EditorGUILayout.Popup(allIndex[i], statusNameList.ToArray());

                // If there is no duration for the current status add it
                if (durationList.Count <= i)
                {
                    durationList.Add(0);
                }
                // Then, field to choose the duration of the status
                durationList[i] = EditorGUILayout.FloatField(durationList[i]);
                // If there is no value in the entity duration list, for the current status, add it
                if (statusDurationListProperty.arraySize <= i)
                {
                    statusDurationListProperty.InsertArrayElementAtIndex(i);
                }

                // Button to remove a status
                GUI.color = Color.red;
                if (GUILayout.Button("X"))
                {
                    allIndex.RemoveAt(i);
                    durationList.RemoveAt(i);
                    statusNameListProperty.DeleteArrayElementAtIndex(i);
                    statusDurationListProperty.DeleteArrayElementAtIndex(i);
                    nbStatus--;
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();

                // Update properties
                statusNameListProperty.GetArrayElementAtIndex(i).stringValue = statusNameList[allIndex[i]];
                statusDurationListProperty.GetArrayElementAtIndex(i).doubleValue = durationList[i];

                EditorGUI.indentLevel--;

            }

            // Button to add a new status in the list
            GUI.color = Color.green;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                statusNameListProperty.InsertArrayElementAtIndex(nbStatus);
                statusDurationListProperty.InsertArrayElementAtIndex(nbStatus);
                allIndex.Add(0);
                durationList.Add(0);
                nbStatus++;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;


        }
        EditorGUILayout.PropertyField(isAllyProperty);

        foreach (string fieldToDisplay in classField)
        {
            try
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldToDisplay));
            }
            catch
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("No GUI implemented for : " + fieldToDisplay);
                EditorGUILayout.EndHorizontal();
            }

        }

        serializedObject.ApplyModifiedProperties();

    }
    void DrawScript()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(true);
        MonoScript script = MonoScript.FromMonoBehaviour((Entity)target);
        EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }
    public static bool HasAttribute(Type t)
    {
        // Get instance of the attribute.
        SerializeField MyAttribute =
            (SerializeField)Attribute.GetCustomAttribute(t, typeof(SerializeField));

        if (MyAttribute == null)
        {
            Console.WriteLine("The attribute was not found.");
            return false;
        }
        else
        {
            return true;
        }
    }
}
#endif
