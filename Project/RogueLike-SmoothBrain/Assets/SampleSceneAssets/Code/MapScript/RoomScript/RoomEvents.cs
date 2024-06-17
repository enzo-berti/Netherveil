using Map.Generation;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Map.Component
{
    public class RoomEvents : MonoBehaviour
    {
        private RoomData roomData;

        private Room room;
        private GameObject roomPreset;
        private GameObject enemies;
        private GameObject treasures;
        private NavMeshSurface navMeshSurface;

        static private bool hasLeaved = true;
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

            // find room go's
            room = transform.parent.gameObject.GetComponent<Room>();
            room.GetComponentInChildren<RoomPresets>(true).GenerateRandomPreset(); // je détruis tout ici pour me simplifier la vie..
            roomPreset = room.GetComponentInChildren<RoomPresets>(true).transform.GetChild(0).gameObject;
            enemies = roomPreset.transform.Find("Enemies").gameObject;
            treasures = roomPreset.transform.Find("Treasures").gameObject;
            navMeshSurface = room.GetComponentInChildren<NavMeshSurface>(true);

            // create data of the map
            roomData = new RoomData(room, enemies);
        }

        private void Start()
        {
            //if (roomCleared)
            //{
            //    // je suis obligé de faire ça pour que la seed soit correcte
            //    // par rapport à l'ancienne sauvegarde si le joueur tue un boss
            //    if (roomData.Type == RoomType.Boss)
            //    {
            //        //Seed.Iterate(3);
            //    }
            //}

            if (roomData.Type == RoomType.Lobby)
            {
                EnterEvents();
            }

            // set bool to true to not call the events in the room if there is no enemy
            allEnemiesDeadCalled = (enemies.transform.childCount == 0);
            // set bool to true to not call the events in the room if there is no chest
            allChestsOpenCalled = (treasures.GetComponentsInChildren<Item>().Count() == 0);
        }

        private void OpenAllChestsEvent()
        {
            allChestsOpenCalled = true;

            // global events
            MapUtilities.onEarlyAllChestOpen?.Invoke();
            MapUtilities.onAllChestOpen?.Invoke();
        }

        private void LocalEnterEvents()
        {
            enterRoomCalled = true;
            RoomEvents.hasLeaved = false;

            // set all elements to the map layer now that we can see them
            foreach (var c in room.GetComponentsInChildren<MapLayer>(true))
            {
                c.Set();
            }

            // set all neighbor elements has undiscovered
            foreach (Room neighbor in room.neighbors)
            {
                foreach (var c in neighbor.GetComponentsInChildren<MapLayer>(true))
                {
                    c.MarkUndiscovered();
                }

                // activate ui
                if (neighbor.RoomUI)
                {
                    neighbor.RoomUI.gameObject.SetActive(true);
                }
            }

            // activate ui
            if (room.RoomUI)
            {
                room.RoomUI.gameObject.SetActive(true);
            }

            MapUtilities.currentRoomData = roomData;
            MapUtilities.nbEnterRoomByType[MapUtilities.currentRoomData.Type] += 1;

            navMeshSurface.enabled = true;
            enemies.SetActive(true);
        }

        private void EnterEvents()
        {
            LocalEnterEvents();

            // global events
            MapUtilities.onEarlyFirstEnter?.Invoke();
            MapUtilities.onFirstEnter?.Invoke();
        }

        private void LocalExitEvents()
        {
            exitRoomCalled = true;
            RoomEvents.hasLeaved = true;

            navMeshSurface.enabled = false;
            enemies.SetActive(false);
        }

        private void FirstExitEvents()
        {
            LocalExitEvents();

            for (int i = 0; i < transform.parent.parent.childCount; i++)
            {
                if (transform.parent.parent.GetChild(i) == transform.parent)
                {
                    FindObjectOfType<MapGenerator>().roomClearIds.Add(i);
                    break;
                }
            }

            // global events
            MapUtilities.onFirstExit?.Invoke();

            SaveManager.Save();
        }

        private void AllEnemiesDeadEvents()
        {
            // local events
            allEnemiesDeadCalled = true;

            // global events
            MapUtilities.onEarlyAllEnemiesDead?.Invoke();
            MapUtilities.onAllEnemiesDead?.Invoke();
        }

        private void LateUpdate()
        {
            if (!allEnemiesDeadCalled && enemies.transform.childCount == 0)
            {
                AllEnemiesDeadEvents();
            }

            if (!allChestsOpenCalled && treasures.GetComponentsInChildren<Item>().Count() == 0)
            {
                OpenAllChestsEvent();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                enterPos = triggerCollide.ClosestPointOnBounds(other.bounds.center);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enterRoomCalled && hasLeaved && other.gameObject.CompareTag("Player"))
            {
                Vector3 enterToPlayer = enterPos - other.bounds.center;
                if (enterToPlayer.magnitude >= 4f)
                {
                    EnterEvents();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!exitRoomCalled && !hasLeaved)
                {
                    FirstExitEvents();
                }

                MapUtilities.onEarlyExitRoom?.Invoke();
                MapUtilities.onExitRoom?.Invoke();
            }
        }

        public void Unclear()
        {
            enemies.SetActive(false);
            foreach (var c in room.GetComponentsInChildren<MapLayer>(true))
            {
                c.Unset();
            }

            var roomUI = room.GetComponentInChildren<RoomUI>(true);
            if (roomUI)
            {
                roomUI.gameObject.SetActive(false);
            }
        }

        public void Clear()
        {
            LocalEnterEvents();
            LocalExitEvents();
            RoomEvents.hasLeaved = true; // to ensure not being blocked

            if (roomData.Type == RoomType.Boss)
            {
                Seed.Iterate(2);
            }
        }
    }
}