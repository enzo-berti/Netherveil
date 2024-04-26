using Map.Component;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Map
{
    public class RoomEvents : MonoBehaviour
    {
        private RoomData roomData;

        private GameObject room;
        private GameObject enemies;
        private GameObject treasures;
        //private GameObject traps;
        private NavMeshSurface navMeshSurface;

        private bool allChestsOpenCalled = false;
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
            GameObject roomPrefab = transform.parent.gameObject;
            room = roomPrefab.GetComponentInChildren<RoomPresets>().transform.GetChild(0).gameObject;
            enemies = room.transform.Find("Enemies").gameObject;
            treasures = room.transform.Find("Treasures").gameObject;
            //traps = room.transform.Find("Traps").gameObject;
            navMeshSurface = roomPrefab.GetComponentInChildren<NavMeshSurface>();

            enemies.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Default");

            // set bool to true to not call the events in the room if there is no enemy
            allEnemiesDeadCalled = (enemies.transform.childCount == 0);
            // set bool to true to not call the events in the room if there is no chest
            allChestsOpenCalled = (treasures.GetComponentsInChildren<Item>().Count() == 0);

            // create data of the map
            roomData = new RoomData(roomPrefab.GetComponent<RoomPrefab>(), enemies);
            if (roomData.Type == RoomType.Lobby) // because enter not called frame one in game (dumb fix)
            {
                EnterEvents();
            }
        }

        private void OpenChestsEvent()
        {
            allChestsOpenCalled = true;

            // global events
            RoomUtilities.allChestOpenEvents?.Invoke();
        }

        private void EnterEvents()
        {
            enterRoomCalled = true;

            // local events
            gameObject.layer = LayerMask.NameToLayer("Map");
            RoomUtilities.roomData = roomData;
            RoomUtilities.nbEnterRoomByType[RoomUtilities.roomData.Type] += 1;
            navMeshSurface.enabled = true;
            enemies.SetActive(true);

            // global events
            RoomUtilities.earlyEnterEvents?.Invoke();
            RoomUtilities.enterEvents?.Invoke();
        }

        private void ExitEvents()
        {
            exitRoomCalled = true;

            // local events
            navMeshSurface.enabled = false;
            enemies.SetActive(false);

            // global events
            RoomUtilities.exitEvents?.Invoke();
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
            if (!allEnemiesDeadCalled && enemies.transform.childCount == 0)
            {
                AllEnemiesEvents();
            }

            if (!allChestsOpenCalled && treasures.GetComponentsInChildren<Item>().Count() == 0)
            {
                OpenChestsEvent();
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