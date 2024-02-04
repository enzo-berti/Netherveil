using System.Collections.Generic;
using UnityEngine;

public enum DoorState : byte
{
    NONE = 0,
    OPEN,
    CLOSE
}

public class DoorsGenerator : MonoBehaviour
{

    private static int NoiseGenerator = 0;

    // planned for later
    [SerializeField] private List<Vector3> doorsPosition = new List<Vector3>();

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
        foreach (Transform child in transform)
        {
            doorsPosition.Add(child.position);

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
        //int numDoorsToClose = GameManager.Instance.seed.Range(doors.Count - minMaxDoors.x, minMaxDoors.y - doors.Count, ref NoiseGenerator) - minMaxDoors.x;
        //
        //for (int i = 0; i < numDoorsToClose; i++)
        //{
        //    int index = GameManager.Instance.seed.Range(0, doors.Count - 1, ref NoiseGenerator);
        //
        //    //CloseDoor(index);
        //}
    }

    public void CloseDoor(int index)
    {
        GameObject go = doors[index];
        doors.RemoveAt(index);

        // TODO : Close the room (h word)
        Destroy(go);
    }

    public void CloseDoor(GameObject door)
    {
        if (doors.Remove(door))
        {
            // TODO : close the room (h word)
            Destroy(door);
        }

        //Debug.LogWarning("Try to set a door state with the wrong GameObject in " + gameObject, door);
    }
}