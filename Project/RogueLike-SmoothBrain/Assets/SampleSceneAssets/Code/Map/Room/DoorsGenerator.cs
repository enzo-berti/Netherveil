using System.Collections.Generic;
using UnityEngine;

public enum DoorState : byte
{
    NONE = 0,
    OPEN,
    CLOSE
}

public struct Door
{
    public Door(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation.eulerAngles.y;
    }

    public Vector3 position;
    public float rotation;
}

public class DoorsGenerator : MonoBehaviour
{
    private static int NoiseGenerator = 0;

    // planned for later
    [SerializeField] public List<Door> doorsPosition = new List<Door>();

    public List<GameObject> doors { get; private set; } = new List<GameObject>();
    [SerializeField, MinMaxSlider(1, 4)] private Vector2Int minMaxDoors;

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            doors.Add(child.gameObject);
        }
    }

#if UNITY_EDITOR
    public void GenerateDoorPosition()
    {
        doorsPosition.Clear();

        foreach (Transform child in transform)
        {
            doorsPosition.Add(new Door(child));

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
        GameObject go = doors[index];
        doors.RemoveAt(index);

        // TODO : Close the room
        Destroy(go);
    }

    public void CloseDoor(ref Door door)
    {
        if (doorsPosition.Remove(door))
        {
            // TODO : close the room
        }

        Debug.LogWarning("Try to set a door state with the wrong GameObject in ", gameObject);
    }
}