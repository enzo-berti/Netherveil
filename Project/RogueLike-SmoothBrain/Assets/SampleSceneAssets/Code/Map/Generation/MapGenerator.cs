using Map.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.AI.Navigation;
using UnityEngine;

namespace Map.Generation
{
    public struct GenerationParam
    {
        public Dictionary<RoomType, int> nbRoomByType;
        public Dictionary<int, List<Door>> availableDoorsByRotation;

        public GenerationParam(int nbNormal = 0, int nbTreasure = 0, int nbChallenge = 0, int nbMerchant = 0, int nbSecret = 0, int nbMiniBoss = 0, int nbBoss = 0)
        {
            nbRoomByType = new Dictionary<RoomType, int>
            {
                { RoomType.Lobby, 1 },
                { RoomType.Tutorial, 0 },
                { RoomType.Normal, nbNormal },
                { RoomType.Treasure, nbTreasure },
                { RoomType.Challenge, nbChallenge },
                { RoomType.Merchant, nbMerchant },
                { RoomType.Secret, nbSecret },
                { RoomType.MiniBoss, nbMiniBoss },
                { RoomType.Boss, nbBoss },
            };

            availableDoorsByRotation = new Dictionary<int, List<Door>>
            {
                { 0, new List<Door>() },
                { 90, new List<Door>() },
                { 180, new List<Door>() },
                { 270, new List<Door>() }
            };
        }

        public readonly int NbRoom
        {
            get
            {
                int totalCount = 0;
                foreach (int count in nbRoomByType.Values)
                {
                    totalCount += count;
                }

                return totalCount;
            }
        }

        public int AvailableDoorsCount
        {
            get
            {
                int count = 0;

                foreach (var list in availableDoorsByRotation.Values)
                {
                    count += list.Count;
                }

                return count;
            }
        }

        public readonly void AddAvailableDoors(DoorsGenerator doorsGenerator)
        {
            foreach (var door in doorsGenerator.doors)
            {
                int rotation = (int)Math.Round(door.Rotation);
                if (availableDoorsByRotation.ContainsKey(rotation))
                {
                    availableDoorsByRotation[rotation].Add(door);
                }
                else
                {
                    Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.Rotation, doorsGenerator);
                }
            }

            //UnityEngine.Object.Destroy(doorsGenerator); // destroy doorsGenerator
        }

        public readonly List<Door> GetFarestDoors()
        {
            List<Door> result = new List<Door>();
            foreach (var doors in availableDoorsByRotation)
            {
                result.AddRange(doors.Value);
            }

            result.Sort((a, b) => (int)-(b.Position.magnitude - a.Position.magnitude));
            return result;
        }

        public readonly List<Door> GetFarestDoorsByRot(int rot)
        {
            List<Door> result = availableDoorsByRotation[rot];

            result.Sort((a, b) => (int)-(b.Position.magnitude - a.Position.magnitude));
            return result;
        }

        public readonly void RemoveDoor(Door door)
        {
            foreach (var doors in availableDoorsByRotation.Values)
            {
                if (doors.Remove(door))
                {
                    return;
                }
            }
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private bool isRandom = true;
        [SerializeField] private string seed; // For debuging purpose
        [SerializeField] private Material miniMapMat;

        [HideInInspector] public bool generate = false; // SUPER BOURRIN OMG
        [HideInInspector] public int stage = 0; // BOURRIN 2

        static private readonly int[] availableRotations = new int[] { 0, 90, 180, 270 };
        const string fileName = "Map.save";

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
            Generate(new GenerationParam(nbNormal: 6, nbTreasure: 2, nbMerchant: 1, nbSecret: 0, nbMiniBoss: 0, nbBoss: 1));

            SaveManager.Instance.onSave += Save;
        }

        private void LoadSave()
        {
            string filePath = SaveManager.Instance.DirectoryPath + fileName;

            if (!File.Exists(filePath))
            {
                return;
            }

            using (var stream = File.Open(filePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    Seed.seed = reader.ReadString();
                    //stage = reader.ReadInt32();
                }
            }
        }

        private void Save(string directoryPath)
        {
            string filePath = SaveManager.Instance.DirectoryPath + fileName;

            using (var stream = File.Open(filePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(Seed.seed);
                    //writer.Write(stage);
                }

                stream.Close();
            }
        }

        private void LateUpdate()
        {
            if (generate)
            {
                Generate(new GenerationParam(nbNormal: 6, nbTreasure: 2, nbMerchant: 1, nbSecret: 0, nbMiniBoss: 0, nbBoss: 1));
                generate = false;
            }
        }

        private void ChangeMiniMapColor()
        {
            switch (stage)
            {
                case 1:
                    miniMapMat.SetColor("Ground", new Color(128/255, 101 / 255, 164 / 255));
                    miniMapMat.SetColor("Ceiling", new Color(146 / 255, 118 / 255, 183 / 255));
                    miniMapMat.SetColor("Wall", new Color(82 / 255, 64 / 255, 106 / 255));
                    break;
                case 2:
                    miniMapMat.SetColor("Ground", new Color(105 / 255, 164 / 255, 101 / 255));
                    miniMapMat.SetColor("Ceiling", new Color(118 / 255, 183 / 255, 123 / 255));
                    miniMapMat.SetColor("Wall", new Color(64 / 255, 106 / 255, 64 / 255));
                    break;
                case 3:
                    miniMapMat.SetColor("Ground", new Color(101 / 255, 156 / 255, 164 / 255));
                    miniMapMat.SetColor("Ceiling", new Color(118 / 255, 171 / 255, 183 / 255));
                    miniMapMat.SetColor("Wall", new Color(64 / 255, 99 / 255, 106 / 255));
                    break;
            }
        }

        public void Generate(GenerationParam genParam)
        {
            stage++;

            ChangeMiniMapColor();

            if (stage == 1)
            {
                genParam.nbRoomByType[RoomType.Tutorial] = 1;
            }

            MapUtilities.SetDatas(genParam);

            GenerateRooms(ref genParam);

            GenerateObstructionDoors(ref genParam);
        }

        private bool GenerateRooms(ref GenerationParam genParam)
        {
            int nbRoom = genParam.NbRoom;

            for (int i = 0; i < nbRoom; i++)
            {
                for (int indexType = 0; indexType < genParam.nbRoomByType.Count; indexType++)
                {
                    RoomType type = genParam.nbRoomByType.Keys.ElementAt(indexType);

                    if (genParam.nbRoomByType[type] > 0)
                    {
                        switch (type)
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
                            default:
                                if (!GenerateRoom(ref genParam, type))
                                {
                                    Debug.LogError("Can't generate room");
                                    return false;
                                }
                                break;
                        }

                        genParam.nbRoomByType[type]--;
                        break;
                    }
                }
            }

            return true;
        }

        private bool GenerateRoom(ref GenerationParam genParam, RoomType type)
        {
            foreach (Room candidateRoomPrefab in Seed.RandList(MapResources.RoomPrefabs(type)))
            {
                GameObject roomGO = Instantiate(candidateRoomPrefab.gameObject);
                if (!TryPutRoom(roomGO, ref genParam, out Door entranceDoor, out Door exitDoor))
                {
                    continue;
                }

                InitRoom(roomGO, ref genParam, ref entranceDoor, exitDoor);
                return true;
            }

            Debug.LogError("Can't find any candidate for map room");
            return false;
        }

        private void GenerateLobbyRoom(ref GenerationParam genParam)
        {
            GameObject roomGO = Instantiate(MapResources.RandRoomPrefab(RoomType.Lobby).gameObject);

            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();

            genParam.AddAvailableDoors(doorsGenerator);
            Destroy(doorsGenerator);

            roomGO.transform.parent = gameObject.transform;
        }

        private void GenerateTutorialRoom(ref GenerationParam genParam)
        {
            GameObject roomGO = Instantiate(MapResources.RandRoomPrefab(RoomType.Tutorial).gameObject);
            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();

            Door entranceDoor = doorsGenerator.doors[0];
            TrySetEntranceDoorPos(roomGO, ref genParam, entranceDoor, out Door exitDoor);

            InitRoom(roomGO, ref genParam, ref entranceDoor, exitDoor);

            roomGO.transform.parent = gameObject.transform;
        }

        private void GenerateBossRoom(ref GenerationParam genParam)
        {
            var bossPrefabs = MapResources.RoomPrefabs(RoomType.Boss);
            int bossIndex = stage % bossPrefabs.Count;

            GameObject roomGO = Instantiate(bossPrefabs[bossIndex].gameObject);
            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();

            Door entranceDoor = doorsGenerator.doors[0];
            TrySetEntranceBossDoorPos(roomGO, ref genParam, entranceDoor, out Door exitDoor);

            InitRoom(roomGO, ref genParam, ref entranceDoor, exitDoor);

            roomGO.transform.parent = gameObject.transform;
        }

        static private bool TrySetEntranceBossDoorPos(GameObject roomGO, ref GenerationParam genParam, Door entranceDoor, out Door exitDoor)
        {
            List<Door> doors = genParam.GetFarestDoors();
            float defaultRot = roomGO.transform.rotation.eulerAngles.y;

            for (int i = 0; i < doors.Count; i++)
            {
                Door candidateExitDoor = doors[(5 + i) % doors.Count]; // start the search at index 5a
                float rotation = candidateExitDoor.Rotation;

                // rotate gameObject entrance door to correspond the exit door
                roomGO.transform.rotation = Quaternion.Euler(0f, (int)(rotation - 180f - entranceDoor.Rotation + defaultRot), 0f);

                // Set position
                roomGO.transform.position = entranceDoor.parentSkeleton.transform.position - entranceDoor.Position + candidateExitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
                Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame

                // Check collision
                if (IsRoomCollidingOtherRoom(roomGO, candidateExitDoor))
                {
                    roomGO.transform.rotation = Quaternion.Euler(0f, defaultRot, 0f); // reset rotation
                    continue; // fail to generate continue to next door candidate
                }

                exitDoor = candidateExitDoor;
                return true;
            }

            exitDoor = new Door();
            return false;
        }

        static private bool TryPutRoom(GameObject roomGO, ref GenerationParam genParam, out Door entranceDoor, out Door exitDoor)
        {
            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();

            foreach (Door entranceDoorCandidate in Seed.RandList(doorsGenerator.doors))
            {
                if (TrySetEntranceDoorPos(roomGO, ref genParam, entranceDoorCandidate, out exitDoor))
                {
                    entranceDoor = entranceDoorCandidate;
                    return true;
                }
            }

            entranceDoor = new Door();
            exitDoor = new Door();
            DestroyImmediate(roomGO);
            return false;
        }

        static private bool TrySetEntranceDoorPos(GameObject roomGO, ref GenerationParam genParam, Door entranceDoor, out Door exitDoor)
        {
            foreach (int rotation in Seed.RandList(availableRotations))
            {
                if (!genParam.availableDoorsByRotation[rotation].Any())
                {
                    continue;
                }

                float defaultRot = roomGO.transform.rotation.eulerAngles.y;
                foreach (Door candidateExitDoor in Seed.RandList(genParam.availableDoorsByRotation[rotation]))
                {
                    // rotate gameObject entrance door to correspond the exit door
                    roomGO.transform.rotation = Quaternion.Euler(0f, (int)(rotation - 180f - entranceDoor.Rotation + defaultRot), 0f);

                    // Set position
                    roomGO.transform.position = entranceDoor.parentSkeleton.transform.position - entranceDoor.Position + candidateExitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
                    Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame

                    // Check collision
                    if (IsRoomCollidingOtherRoom(roomGO, candidateExitDoor))
                    {
                        roomGO.transform.rotation = Quaternion.Euler(0f, defaultRot, 0f); // reset rotation
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
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        static private bool IsRoomCollidingOtherRoom(GameObject roomGO, Door exitDoor)
        {
            GameObject skeletonGO = roomGO.GetComponentInChildren<Skeleton>().gameObject;
            BoxCollider roomColliderEnter = skeletonGO.GetComponent<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.parentSkeleton.GetComponent<BoxCollider>();

            // if we find another trigger with the "map" tag then we collide with another room
            Collider[] colliders = roomColliderEnter.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.UseGlobal).Where(collider => collider != roomColliderEnter && collider != roomColliderExit && collider.isTrigger).ToArray();
            return colliders.Any();
        }

        private void InitRoom(GameObject roomGO, ref GenerationParam genParam, ref Door entranceDoor, Door exitDoor)
        {
            // Set parent room
            roomGO.transform.SetParent(gameObject.transform);

            // Add room to exitDoor room neighbours
            Room exitRoom = exitDoor.parentSkeleton.transform.parent.GetComponent<Room>();
            Room room = roomGO.GetComponent<Room>();

            // Removed used door
            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();
            doorsGenerator.RemoveDoor(entranceDoor);
            genParam.RemoveDoor(exitDoor);

            // Generate props
            GenerateGates(roomGO, entranceDoor);
            // Stairs
            if (room.type == RoomType.Boss)
            {
                GenerateStairs(roomGO);
            }

            // Add the new doors from the new room into the possible candidates
            genParam.AddAvailableDoors(doorsGenerator);

            // Generate one of the seed room and delete the other's
            roomGO.GetComponentInChildren<RoomPresets>().GenerateRandomPreset();

            // SetActive object's of room
            roomGO.GetComponentInChildren<NavMeshSurface>().transform.Find("Enemies").gameObject.SetActive(false);
            roomGO.GetComponentInChildren<NavMeshSurface>().enabled = false;
        }

        static private void GenerateGates(GameObject roomGO, Door entranceDoor)
        {
            GameObject gateGO = Instantiate(MapResources.GatePrefab, entranceDoor.Position, Quaternion.identity);
            gateGO.transform.Rotate(0f, entranceDoor.Rotation, 0f);
            gateGO.transform.parent = roomGO.GetComponentInChildren<StaticProps>().transform;
        }

        static private void GenerateStairs(GameObject roomGO)
        {
            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<Skeleton>().transform.Find("Doors").GetComponent<DoorsGenerator>();
            Door entranceStairs = Seed.RandList(doorsGenerator.doors)[0];

            GameObject go = Instantiate(Seed.RandList(MapResources.StairsPrefabs)[0], entranceStairs.Position, Quaternion.identity);
            go.transform.Rotate(0f, entranceStairs.Rotation, 0f);
            go.transform.parent = roomGO.GetComponentInChildren<StaticProps>().transform;

            GenerateGates(roomGO, entranceStairs);
            doorsGenerator.RemoveDoor(entranceStairs);
        }

        static private void GenerateObstructionDoors(ref GenerationParam genParam)
        {
            foreach (var listDoors in genParam.availableDoorsByRotation)
            {
                foreach (var door in listDoors.Value)
                {
                    GameObject go = Instantiate(Seed.RandList(MapResources.ObstructionDoors)[0], door.Position, Quaternion.identity);
                    go.transform.Rotate(0f, door.Rotation, 0f);
                    go.transform.parent = door.parentSkeleton.transform.parent.GetComponentInChildren<StaticProps>().transform;
                }
            }

            genParam.availableDoorsByRotation.Clear();
        }
    }
}