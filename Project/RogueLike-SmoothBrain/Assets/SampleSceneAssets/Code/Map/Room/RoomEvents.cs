using Unity.AI.Navigation;
using UnityEngine;

namespace Map
{
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
        private Vector3 enterPos = Vector3.zero;

        private void Awake()
        {
            foreach (Collider coll in GetComponents<Collider>())
            {
                if (coll.isTrigger)
                {
                    triggerCollide = coll;
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
            gameObject.layer = LayerMask.NameToLayer("Default");

            // set bool to true to not call the events in the room
            allEnemiesDeadCalled = (enemies.transform.childCount == 0);

            // create data of the map
            mapData = new RoomData(enemies, transform.parent.GetComponentInChildren<Generation.RoomGenerator>());
            if (mapData.Type == RoomType.Lobby) // because enter not called frame one in game (dumb fix)
            {
                EnterEvents();
            }
        }

        private void EnterEvents()
        {
            Debug.Log("ENTER ROOM");
            enterRoomCalled = true;

            // local events
            gameObject.layer = LayerMask.NameToLayer("Map");
            RoomUtilities.roomData = mapData;
            navMeshSurface.enabled = true;
            enemies.SetActive(true);

            // global events
            RoomUtilities.EnterEvents?.Invoke();
        }

        private void ExitEvents()
        {
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

        private void OnTriggerEnter(Collider other)
        {
            if (!enterRoomCalled && other.gameObject.CompareTag("Player"))
            {
                enterPos = triggerCollide.ClosestPointOnBounds(other.bounds.center);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enterRoomCalled && other.gameObject.CompareTag("Player"))
            {
                Vector3 enterToPlayer = enterPos - other.bounds.center;
                if (enterToPlayer.magnitude >= other.bounds.size.magnitude)
                {
                    EnterEvents();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!exitRoomCalled && other.gameObject.CompareTag("Player"))
            {
                ExitEvents();
            }
        }
    }
}