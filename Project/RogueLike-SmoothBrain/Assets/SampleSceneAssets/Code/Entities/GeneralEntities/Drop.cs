using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class Drop
{
    private static readonly float radiusDropRandom = 2.0f; 
    [SerializeField] List<DropInfo> dropList = new();
    public void DropLoot(Vector3 position)
    {
        foreach (DropInfo dropInfo in dropList)
        {
            if (dropInfo.isChanceShared)
            {
                if (UnityEngine.Random.value <= dropInfo.chance)
                {
                    for (int i = 0; i < dropInfo.Quantity; i++)
                    {
                        GameObject go = GameObject.Instantiate(dropInfo.loot, position, Quaternion.identity);
                        Vector3 pos3D;
                        Vector2 pos = MathsExtension.GetPointInCircle(new Vector2(go.transform.position.x, go.transform.position.z), radiusDropRandom);
                        pos3D = new Vector3(pos.x, go.transform.position.y, pos.y);
                        CoroutineManager.Instance.StartCustom(DropMovement(go, pos3D, 1f));
                    }
                }
            }
            else
            {
                for (int i = 0; i < dropInfo.Quantity; i++)
                {
                    if (UnityEngine.Random.value <= dropInfo.chance)
                    {
                        GameObject go = GameObject.Instantiate(dropInfo.loot, position, Quaternion.identity);
                        Vector3 pos3D;
                        Vector2 pos = MathsExtension.GetPointInCircle(new Vector2(go.transform.position.x, go.transform.position.z), radiusDropRandom);
                        pos3D = new Vector3(pos.x, go.transform.position.y, pos.y);
                        CoroutineManager.Instance.StartCustom(DropMovement(go, pos3D, 1f));
                    }
                }
            }

        }
    }

    private IEnumerator DropMovement(GameObject go, Vector3 pos, float throwTime)
    {
        float timer = 0;
        Vector3 basePos = go.transform.position;
        Vector3 position3D = Vector3.zero;
        float a = -16, b = 16;
        float c = go.transform.position.y;
        float timerToReach = MathsExtension.Resolve2ndDegree(a, b, c, 0).Max();
        while (timer < timerToReach)
        {
            yield return null;
            timer = timer > timerToReach ? timerToReach : timer;
            if (timer < 1.0f)
            {
                timer = timer > 1 ? 1 : timer;

                position3D = Vector3.Lerp(basePos, pos, timer);
            }
            position3D.y = MathsExtension.SquareFunction(a, b, c, timer);
            go.transform.position = position3D;
            timer += Time.deltaTime / throwTime;
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Drop))]
public class DropDrawerUIE : PropertyDrawer
{
    SerializedProperty dropProperty;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        dropProperty = property.FindPropertyRelative("dropList");
        EditorGUILayout.PropertyField(dropProperty, label);
    }
}
#endif