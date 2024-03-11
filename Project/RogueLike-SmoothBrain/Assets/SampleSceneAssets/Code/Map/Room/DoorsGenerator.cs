using System;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState : byte
{
    NONE = 0,
    OPEN,
    CLOSE
}

[Serializable]
public struct Door
{
    public Door(Transform transform)
    {
        forward = transform.forward;
        localPosition = transform.position;
        rotation = transform.rotation.eulerAngles.y;
        parentSkeleton = transform.gameObject.transform.parent.parent.gameObject;
    }

    public Vector3 forward;
    [SerializeField] private Vector3 localPosition;
    public float rotation;
    public GameObject parentSkeleton;

    public Vector3 Position 
    {
        get 
        {
            Vector3 pos = localPosition + parentSkeleton.transform.position;
            Vector3 dir = pos - parentSkeleton.transform.position;
            dir = Quaternion.Euler(0, parentSkeleton.transform.eulerAngles.y, 0) * dir;
            pos = dir + parentSkeleton.transform.position;

            return pos;
        }
    }
}

public class DoorsGenerator : MonoBehaviour
{
    // planned for later
    public List<Door> doors = new List<Door>();

    [SerializeField, MinMaxSlider(1, 4)] private Vector2Int minMaxDoors;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var door in doors)
        {
            Gizmos.DrawSphere(door.Position, 0.25f);
        }
    }

#if UNITY_EDITOR
    public void GeneratePrefab()
    {
        doors.Clear();

        foreach (Transform child in transform)
        {
            doors.Add(new Door(child));
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
#endif

    /// <summary>
    /// Generate the seed doors (destroy doors between min max range)
    /// </summary>
    /// <param name="generationParameters"></param>
    public void GenerateSeed(GenerationParam generationParameters)
    {
        // get number of doors that can spawn depending on the number of rooms available by genParams

    }

    public void RemoveDoor(Door door)
    {
        if (!doors.Remove(door))
        {
            Debug.LogWarning("Try to set a door state with the wrong GameObject in ", gameObject);
            return;
        }
    }

    public void EmptyDoors()
    {
        doors.Clear();
    }
}