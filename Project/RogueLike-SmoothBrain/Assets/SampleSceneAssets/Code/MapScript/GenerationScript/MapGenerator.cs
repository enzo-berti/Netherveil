using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.AI.Navigation;
using UnityEngine;

namespace Map.Generation
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private bool isRandom = true;
        [SerializeField] private string seed; // For debuging purpose
        [SerializeField] private Material miniMapMat;

        [HideInInspector] public bool generate = false; // SUPER BOURRIN OMG
        private int iterationSeedRegister = 0; // BOURRIN 2 

        private Room previousRoomSpawned = null;

        static private readonly RoomType[] priorityType = new RoomType[] { RoomType.Lobby, RoomType.Tutorial, RoomType.Normal, RoomType.Boss, RoomType.Merchant, RoomType.Treasure };
        static private readonly int[] availableRotations = new int[] { 0, 90, 180, 270 };
        const string fileName = "Map.save";

        public List<int> roomClearId = new List<int>();

        private void Awake()
        {
            //if (SaveManager.Instance.HasData)
            //{
            //    LoadSave();
            //}
            //else
            //{
                Seed.RandomizeSeed();

                if (!isRandom)
                {
                    Seed.Set(seed);
                }
            //}

            seed = Seed.seed;
            Generate(new GenerationParameters(nbNormal: 6, nbTreasure: 2, nbMerchant: 1, nbSecret: 0, nbMiniBoss: 0, nbBoss: 1));

            MapUtilities.stage = 0;

            SaveManager.onSave += Save;
        }

        private void LateUpdate()
        {
            if (generate)
            {
                ResetMapDatas();

                generate = false;
                Generate(new GenerationParameters(nbNormal: 6, nbTreasure: 2, nbMerchant: 1, nbSecret: 0, nbMiniBoss: 0, nbBoss: 1));
            }
        }

        private void OnDestroy()
        {
            ResetMapDatas();
        }

        private void ResetMapDatas()
        {
            roomClearId.Clear();
            MapUtilities.onEarlyExit = null;
            MapUtilities.onEarlyEnter = null;
            MapUtilities.onEnter = null;
            MapUtilities.onExit = null;
            MapUtilities.onAllChestOpen = null;
            MapUtilities.onEarlyAllChestOpen = null;
            MapUtilities.onAllEnemiesDead = null;
            MapUtilities.onEarlyAllEnemiesDead = null;
            MapUtilities.onFinishStage = null;
        }

        private void LoadSave()
        {
            string filePath = SaveManager.DirectoryPath + fileName;

            if (!File.Exists(filePath))
            {
                return;
            }

            using (var stream = File.Open(filePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    // seed
                    Seed.seed = reader.ReadString();
                    Seed.Iterate(reader.ReadInt32());
                    // stage
                    MapUtilities.stage = reader.ReadInt32() - 1;
                    // room ids
                    int numberCleared = reader.ReadInt32();
                    for (int i = 0; i < numberCleared; i++)
                    {
                        roomClearId.Add(reader.ReadInt32());
                    }
                }
            }
        }

        private void Save(string directoryPath)
        {
            string filePath = SaveManager.DirectoryPath + fileName;

            using (var stream = File.Open(filePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    // seed
                    writer.Write(Seed.seed);
                    writer.Write(iterationSeedRegister);
                    // stage
                    writer.Write(MapUtilities.stage);
                    // room ids
                    writer.Write(roomClearId.Count);
                    foreach (int id in roomClearId)
                    {
                        writer.Write(id);
                    }
                }

                stream.Close();
            }
        }

        private void ClearRooms()
        {
            foreach (int index in roomClearId)
            {
                Room room = transform.GetChild(index).GetComponent<Room>();

                room.ClearPreset();
            }
        }

        private void ChangeMiniMapColor()
        {
            switch (MapUtilities.stage)
            {
                case 1:
                    miniMapMat.SetColor("_Ground", ColorExtension.Color("8065A4"));
                    miniMapMat.SetColor("_Ceiling", ColorExtension.Color("9276B7"));
                    miniMapMat.SetColor("_Wall", ColorExtension.Color("52406A"));
                    break;
                case 2:
                    miniMapMat.SetColor("_Ground", ColorExtension.Color("69A465"));
                    miniMapMat.SetColor("_Ceiling", ColorExtension.Color("76B77B"));
                    miniMapMat.SetColor("_Wall", ColorExtension.Color("406A40"));
                    break;
                case 3:
                    miniMapMat.SetColor("_Ground", ColorExtension.Color("659CA4"));
                    miniMapMat.SetColor("_Ceiling", ColorExtension.Color("76ABB7"));
                    miniMapMat.SetColor("_Wall", ColorExtension.Color("40636A"));
                    break;
            }
        }

        public void Generate(GenerationParameters genParam)
        {
            MapUtilities.stage++;
            iterationSeedRegister = Seed.Iteration;

            ChangeMiniMapColor();

            if (MapUtilities.stage == 1)
            {
                genParam.nbRoomByType[RoomType.Tutorial] = 1;
            }

            MapUtilities.SetDatas(genParam);

            GenerateRooms(ref genParam);

            GenerateObstructionDoors(ref genParam);

            ClearRooms();
        }

        private bool GenerateRooms(ref GenerationParameters genParam)
        {
            int nbRoom = genParam.NbRoom;

            for (int i = 0; i < nbRoom; i++)
            {
                // Search a type to spawn
                RoomType roomType = RoomType.None;
                foreach (RoomType typeCandidate in priorityType)
                {
                    if (genParam.nbRoomByType[typeCandidate] > 0)
                    {
                        genParam.nbRoomByType[typeCandidate]--;
                        roomType = typeCandidate;
                        break;
                    }
                }

                // Find the right function to spawn the type of room
                switch (roomType)
                {
                    case RoomType.Lobby:
                        GenerateLobbyRoom(ref genParam);
                        break;
                    case RoomType.Tutorial:
                        GenerateTutorialRoom(ref genParam);
                        break;
                    case RoomType.Boss:
                        GenerateBossRoom(ref genParam);
                        break;
                    case RoomType.None:
                        Debug.LogWarning("No type found in room generations");
                        break;
                    default:
                        if (!GenerateRoom(ref genParam, roomType))
                        {
                            Debug.LogError("Can't generate room");
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        private bool GenerateRoom(ref GenerationParameters genParam, RoomType type)
        {
            foreach (Room candidateRoomPrefab in Seed.RandList(MapResources.RoomPrefabs(type)))
            {
                if (type == RoomType.Normal && candidateRoomPrefab == previousRoomSpawned) // temporary
                {
                    continue;
                }

                Room room = Instantiate(candidateRoomPrefab.gameObject).GetComponent<Room>();
                if (!TryPutRoom(room, ref genParam, out Door entranceDoor, out Door exitDoor))
                {
                    continue;
                }

                InitRoom(room, ref genParam, ref entranceDoor, exitDoor);
                if (type == RoomType.Normal) // temporary
                {
                    previousRoomSpawned = room;
                }

                return true;
            }

            Debug.LogError("Can't find any candidate for room");
            return false;
        }

        private void GenerateLobbyRoom(ref GenerationParameters genParam)
        {
            Room room = Instantiate(MapResources.RandRoomPrefab(RoomType.Lobby).gameObject).GetComponent<Room>();

            genParam.AddDoors(room.DoorsGenerator);

            room.transform.parent = gameObject.transform;
        }

        private void GenerateTutorialRoom(ref GenerationParameters genParam)
        {
            Room room = Instantiate(MapResources.RandRoomPrefab(RoomType.Tutorial).gameObject).GetComponent<Room>();

            Door entranceDoor = room.DoorsGenerator.doors[0];
            TrySetEntranceDoorPos(room, ref genParam, entranceDoor, out Door exitDoor);

            InitRoom(room, ref genParam, ref entranceDoor, exitDoor);

            room.transform.parent = gameObject.transform;
        }

        private void GenerateBossRoom(ref GenerationParameters genParam)
        {
            var bossPrefabs = MapResources.RoomPrefabs(RoomType.Boss);
            int bossIndex = MapUtilities.stage % bossPrefabs.Count;

            Room room = Instantiate(bossPrefabs[bossIndex].gameObject).GetComponent<Room>();

            Door entranceDoor = room.DoorsGenerator.doors[0];
            TrySetEntranceBossDoorPos(room, ref genParam, entranceDoor, out Door exitDoor);

            InitRoom(room, ref genParam, ref entranceDoor, exitDoor);

            room.transform.parent = gameObject.transform;
        }

        static private bool TrySetEntranceBossDoorPos(Room room, ref GenerationParameters genParam, Door entranceDoor, out Door exitDoor)
        {
            List<Door> doors = genParam.GetFarestDoors();
            float defaultRot = room.transform.rotation.eulerAngles.y;

            for (int i = 0; i < doors.Count; i++)
            {
                Door candidateExitDoor = doors[(5 + i) % doors.Count]; // start the search at index 5a
                float rotation = candidateExitDoor.Rotation;

                // rotate gameObject entrance door to correspond the exit door
                room.transform.rotation = Quaternion.Euler(0f, (int)(rotation - 180f - entranceDoor.Rotation + defaultRot), 0f);

                // Set position
                room.transform.position = entranceDoor.room.Skeleton.transform.position - entranceDoor.Position + candidateExitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
                Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame

                // Check collision
                if (IsRoomCollidingOtherRoom(room, candidateExitDoor))
                {
                    room.transform.rotation = Quaternion.Euler(0f, defaultRot, 0f); // reset rotation
                    continue; // fail to generate continue to next door candidate
                }

                exitDoor = candidateExitDoor;
                return true;
            }

            exitDoor = new Door();
            return false;
        }

        static private bool TryPutRoom(Room room, ref GenerationParameters genParam, out Door entranceDoor, out Door exitDoor)
        {
            DoorsGenerator doorsGenerator = room.DoorsGenerator;

            foreach (Door entranceDoorCandidate in Seed.RandList(doorsGenerator.doors))
            {
                if (TrySetEntranceDoorPos(room, ref genParam, entranceDoorCandidate, out exitDoor))
                {
                    entranceDoor = entranceDoorCandidate;
                    return true;
                }
            }

            entranceDoor = new Door();
            exitDoor = new Door();
            DestroyImmediate(room.gameObject);
            return false;
        }

        static private bool TrySetEntranceDoorPos(Room room, ref GenerationParameters genParam, Door entranceDoor, out Door exitDoor)
        {
            foreach (int rotation in Seed.RandList(availableRotations))
            {
                if (!genParam.availableDoorsByRotation[rotation].Any())
                {
                    continue;
                }

                float defaultRot = room.transform.rotation.eulerAngles.y;
                foreach (Door candidateExitDoor in Seed.RandList(genParam.availableDoorsByRotation[rotation]))
                {
                    // rotate gameObject entrance door to correspond the exit door
                    room.transform.rotation = Quaternion.Euler(0f, (int)(rotation - 180f - entranceDoor.Rotation + defaultRot), 0f);

                    // Set position
                    room.transform.position = entranceDoor.room.Skeleton.transform.position - entranceDoor.Position + candidateExitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
                    Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame

                    // Check collision
                    if (IsRoomCollidingOtherRoom(room, candidateExitDoor))
                    {
                        room.transform.rotation = Quaternion.Euler(0f, defaultRot, 0f); // reset rotation
                        continue; // fail to generate continue to next door candidate
                    }

                    exitDoor = candidateExitDoor;
                    return true;
                }
            }

            exitDoor = new Door();
            return false;
        }

        public void DestroyMap()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject gameObject = transform.GetChild(i).gameObject;
                gameObject.transform.parent = null;
                DestroyImmediate(gameObject);
            }
        }

        static private bool IsRoomCollidingOtherRoom(Room room, Door exitDoor)
        {
            GameObject skeletonGO = room.Skeleton.gameObject;
            BoxCollider roomColliderEnter = skeletonGO.GetComponent<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.room.Skeleton.GetComponent<BoxCollider>();

            // if we find another trigger with the "map" tag then we collide with another room
            Collider[] colliders = roomColliderEnter.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.UseGlobal).Where(collider => collider != roomColliderEnter && collider != roomColliderExit && collider.isTrigger).ToArray();
            return colliders.Any();
        }

        private void InitRoom(Room room, ref GenerationParameters genParam, ref Door entranceDoor, Door exitDoor)
        {
            // Set parent room
            room.transform.SetParent(gameObject.transform);

            // Add room to exitDoor room neighbours
            Room exitRoom = exitDoor.parentSkeleton.transform.parent.GetComponent<Room>();
            room.neighbor.Add(exitRoom);
            exitRoom.neighbor.Add(room);

            // Removed used door
            room.DoorsGenerator.RemoveDoor(entranceDoor);
            genParam.RemoveDoor(exitDoor);

            // Generate props
            GenerateGates(room, entranceDoor);
            // Stairs
            if (room.type == RoomType.Boss)
            {
                GenerateStairs(room);
            }

            // Add the new doors from the new room into the possible candidates
            genParam.AddDoors(room.DoorsGenerator);

            // Generate one of the seed room and delete the other's
            room.RoomPresets.GenerateRandomPreset();

            // SetActive object's of room
            room.RoomEnemies.gameObject.SetActive(false);
            room.GetComponentInChildren<NavMeshSurface>().enabled = false;
        }

        static private void GenerateGates(Room room, Door entranceDoor)
        {
            GameObject gateGO = Instantiate(MapResources.GatePrefab, entranceDoor.Position, Quaternion.identity);
            gateGO.transform.Rotate(0f, entranceDoor.Rotation, 0f);
            gateGO.transform.parent = room.StaticProps.transform;
        }

        static private void GenerateStairs(Room room)
        {
            Door entranceStairs = Seed.RandList(room.DoorsGenerator.doors)[0];

            GameObject go = Instantiate(Seed.RandList(MapResources.StairsPrefabs)[0], entranceStairs.Position, Quaternion.identity);
            go.transform.Rotate(0f, entranceStairs.Rotation, 0f);
            go.transform.parent = room.StaticProps.transform;

            GenerateGates(room, entranceStairs);
            room.DoorsGenerator.RemoveDoor(entranceStairs);
        }

        static private void GenerateObstructionDoors(ref GenerationParameters genParam)
        {
            foreach (var listDoors in genParam.availableDoorsByRotation)
            {
                foreach (var door in listDoors.Value)
                {
                    GameObject go = Instantiate(Seed.RandList(MapResources.ObstructionDoors)[0], door.Position, Quaternion.identity);
                    go.transform.Rotate(0f, door.Rotation, 0f);
                    go.transform.parent = door.room.StaticProps.transform;
                }
            }

            genParam.availableDoorsByRotation.Clear();
        }
    }
}