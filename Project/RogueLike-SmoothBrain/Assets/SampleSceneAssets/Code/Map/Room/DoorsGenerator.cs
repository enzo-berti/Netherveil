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
        position = transform.position;
        rotation = transform.rotation.eulerAngles.y;
        parentSkeleton = transform.gameObject.transform.parent.parent.gameObject;
    }

    public Vector3 forward;
    public Vector3 position;
    public float rotation;
    public GameObject parentSkeleton;
}

public class DoorsGenerator : MonoBehaviour
{
    private static int NoiseGenerator = 0;

    // planned for later
    public List<Door> doors = new List<Door>();

    [SerializeField, MinMaxSlider(1, 4)] private Vector2Int minMaxDoors;

#if UNITY_EDITOR
    public void GenerateDoorPosition()
    {
        doors.Clear();

        foreach (Transform child in transform)
        {
            doors.Add(new Door(child));

            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(child.gameObject);
            };
        }
    }
#endif

    /// <summary>
    /// Generate the seed doors (destroy doors between min max range)
    /// </summary>
    /// <param name="generationParameters"></param>
    public void GenerateSeed(GenerationParameters generationParameters)
    {
        // get number of doors that can spawn depending on the number of rooms available by genParams

    }

    public void CloseDoor(int index)
    {
        //GameObject go = doors[index];
        //doors.RemoveAt(index);

        // TODO : Close the room
        //Destroy(go);
    }

    public void CloseDoor(Door door)
    {
        if (doors.Remove(door))
        {
            // TODO : close the room
        }

        Debug.LogWarning("Try to set a door state with the wrong GameObject in ", gameObject);
    }
}