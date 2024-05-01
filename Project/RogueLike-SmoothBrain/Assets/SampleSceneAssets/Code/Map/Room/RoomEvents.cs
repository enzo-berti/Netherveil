using Map.Component;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Map
{
    public class RoomEvents : MonoBehaviour
    {
        private RoomData roomData;

        private Room room;
        private GameObject roomPreset;
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
            room = transform.parent.gameObject.GetComponent<Room>();
            roomPreset = room.GetComponentInChildren<RoomPresets>().transform.GetChild(0).gameObject;
            enemies = roomPreset.transform.Find("Enemies").gameObject;
            treasures = roomPreset.transform.Find("Treasures").gameObject;
            //traps = room.transform.Find("Traps").gameObject;
            navMeshSurface = room.GetComponentInChildren<NavMeshSurface>();

            enemies.SetActive(false);
            foreach (var c in room.GetComponentsInChildren<MapLayer>())
            {
                c.Unset();
            }

            var roomUI = room.GetComponentInChildren<RoomUI>(true);
            if (roomUI)
            {
                roomUI.gameObject.SetActive(false);
            }

            // set bool to true to not call the events in the room if there is no enemy
            allEnemiesDeadCalled = (enemies.transform.childCount == 0);
            // set bool to true to not call the events in the room if there is no chest
            allChestsOpenCalled = (treasures.GetComponentsInChildren<Item>().Count() == 0);

            // create data of the map
            roomData = new RoomData(room, enemies);
            if (roomData.Type == RoomType.Lobby) // because enter not called frame one in game (dumb fix)
            {
                EnterEvents();
            }
        }

        private void OpenChestsEvent()
        {
            allChestsOpenCalled = true;

            // global events
            MapUtilities.onEarlyAllChestOpen?.Invoke();
            MapUtilities.onAllChestOpen?.Invoke();
        }

        private void EnterEvents()
        {
            enterRoomCalled = true;

            // local events
            foreach (var c in room.GetComponentsInChildren<MapLayer>())
            {
                c.Set();
            }

            var roomUI = room.GetComponentInChildren<RoomUI>(true);
            if (roomUI)
            {
                roomUI.gameObject.SetActive(true);
            }

            MapUtilities.currentRoomData = roomData;
            MapUtilities.nbEnterRoomByType[MapUtilities.currentRoomData.Type] += 1;
            navMeshSurface.enabled = true;
            enemies.SetActive(true);

            // global events
            MapUtilities.onEarlyEnter?.Invoke();
            MapUtilities.onEnter?.Invoke();
        }

        private void ExitEvents()
        {
            exitRoomCalled = true;

            // local events
            navMeshSurface.enabled = false;
            enemies.SetActive(false);

            // global events
            MapUtilities.onEarlyExit?.Invoke();
            MapUtilities.onExit?.Invoke();
        }

        private void AllEnemiesEvents()
        {
            // global events
            MapUtilities.onEarlyAllEnemiesDead?.Invoke();
            MapUtilities.onAllEnemiesDead?.Invoke();

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