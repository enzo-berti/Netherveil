using Unity.AI.Navigation;
using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    private RoomData mapData;

    private GameObject room;
    private GameObject enemies;
    private GameObject traps;
    private NavMeshSurface navMeshSurface;

    private bool allEnemiesDeadCalled = false;
    private bool enterRoomCalled = false;
    private bool exitRoomCalled = false;

    private Collider triggerCollide = null;

    private void Awake()
    {
        foreach (Collider collider in GetComponents<Collider>())
        {
            if (collider.isTrigger)
            {
                triggerCollide = collider;
                break;
            }
        }
    }

    private void Start()
    {
        // find room go's
        room = transform.parent.Find("RoomGenerator").GetChild(0).gameObject;
        enemies = room.transform.Find("Enemies").gameObject;
        traps = room.transform.Find("Traps").gameObject;
        navMeshSurface = transform.parent.GetComponentInChildren<NavMeshSurface>();

        enemies.SetActive(false);

        // set bool to true to not call the events in the room
        allEnemiesDeadCalled = (enemies.transform.childCount == 0);

        // create data of the map
        mapData = new RoomData(enemies, transform.parent.GetComponentInChildren<RoomGenerator>());
    }

    private void EnterEvents()
    {
        if (enterRoomCalled)
        {
            return;
        }
        Debug.Log("ENTER ROOM");
        enterRoomCalled = true;

        // global events
        RoomUtilities.roomData = mapData;
        RoomUtilities.EnterEvents?.Invoke();

        // local events
        navMeshSurface.enabled = true;
        enemies.SetActive(true);
    }

    private void ExitEvents()
    {
        if (exitRoomCalled)
        {
            return;
        }
        Debug.Log("EXIT ROOM");
        exitRoomCalled = true;

        // local events
        navMeshSurface.enabled = false;
        enemies.SetActive(false);

        // global events
        RoomUtilities.ExitEvents?.Invoke();
    }

    private void AllEnemiesEvents()
    {
        // global events
        RoomUtilities.allEnemiesDeadEvents?.Invoke();

        // local events
        allEnemiesDeadCalled = true;
    }

    private void FixedUpdate()
    {
        if (enemies.transform.childCount == 0 && !allEnemiesDeadCalled)
        {
            AllEnemiesEvents();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (triggerCollide.bounds.Contains(other.bounds.max) && triggerCollide.bounds.Contains(other.bounds.min))
            {
                EnterEvents();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ExitEvents();
        }
    }
}