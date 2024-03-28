using Unity.AI.Navigation;
using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    private RoomData mapData;

    private GameObject enemies;
    private GameObject traps;
    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        Transform roomGenerator = transform.parent.Find("RoomGenerator");

        // find room go's
        enemies = roomGenerator.transform.GetChild(0).Find("Enemies").gameObject;
        traps = roomGenerator.transform.GetChild(0).Find("Traps").gameObject;
        navMeshSurface = GetComponent<NavMeshSurface>();

        // create data of the map
        mapData = new RoomData();
    }

    private void EnterEvents()
    {
        navMeshSurface.enabled = true;
        enemies.SetActive(true);
    }

    private void ExitEvents()
    {
        navMeshSurface.enabled = false;
        enemies.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RoomUtilities.roomData = mapData;
            RoomUtilities.EnterEvents?.Invoke(ref mapData);
            EnterEvents();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RoomUtilities.ExitEvents?.Invoke(ref mapData);
            ExitEvents();
        }
    }
}