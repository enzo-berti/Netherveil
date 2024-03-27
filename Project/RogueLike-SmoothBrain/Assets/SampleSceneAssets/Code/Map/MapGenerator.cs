using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RoomType
{
    None,
    Lobby,
    Normal,
    Treasure,
    Challenge,
    Merchant,
    Secret,
    MiniBoss,
    Boss
}

public struct GenerationParam
{
    public Dictionary<RoomType, int> nbRoom;
    public Dictionary<float, List<Door>> availableDoors;

    public GenerationParam(int nbNormal = 0, int nbTreasure = 0, int nbChallenge = 0, int nbMerchant = 0, int nbSecret = 0, int nbMiniBoss = 0)
    {
        nbRoom = new Dictionary<RoomType, int>
        {
            { RoomType.Normal, nbNormal },
            { RoomType.Treasure, nbTreasure },
            { RoomType.Challenge, nbChallenge },
            { RoomType.Merchant, nbMerchant },
            { RoomType.Secret, nbSecret },
            { RoomType.MiniBoss, nbMiniBoss },
        };

        availableDoors = new Dictionary<float, List<Door>>
        {
            { 0f, new List<Door>() },
            { 90f, new List<Door>() },
            { 180f, new List<Door>() },
            { 270f, new List<Door>() }
        };
    }

    public readonly int TotalRoom
    {
        get 
        { 
            return nbRoom[RoomType.Normal] + nbRoom[RoomType.Treasure] + nbRoom[RoomType.Challenge] + nbRoom[RoomType.Merchant] + nbRoom[RoomType.Secret] + nbRoom[RoomType.MiniBoss]; 
        }
    }

    public int NumRoomAvaibles
    {
        get
        {
            int count = 0;

            foreach (var list in availableDoors.Values)
            {
                count += list.Count;
            }

            if (count == 0)
            {
                Debug.Log(availableDoors.Values.Count);
                foreach (var truc in availableDoors)
                {
                    Debug.Log("LIST : " + truc.Key + " NB : " + truc.Value.Count);
                }
            }

            return count;
        }
    }

    public readonly void AddDoorsGenerator(DoorsGenerator doorsGenerator)
    {
        foreach (var door in doorsGenerator.doors)
        {
            if (availableDoors.ContainsKey(door.Rotation))
            {
                availableDoors[door.Rotation].Add(door);
            }
            else
            {
                Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.Rotation);
            }
        }

        UnityEngine.Object.Destroy(doorsGenerator); // destroy doorsGenerator
    }

    public readonly Door GetFarestDoor()
    {
        Tuple<int, float> farestDoor = new Tuple<int, float>(-1, 0);
        float key = 0f;

        foreach (var doors in availableDoors)
        {
            for (int i = 0; i < doors.Value.Count; i++)
            {
                float distance = doors.Value[i].Position.magnitude;
                if (distance > farestDoor.Item2)
                {
                    key = doors.Key;
                    farestDoor = new Tuple<int, float>(i, distance);
                }
            }
        }

        return availableDoors[key][farestDoor.Item1];
    }

    public readonly void RemoveDoor(Door door)
    {
        foreach (var doors in availableDoors.Values)
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
    public static int NoiseGenerator = 0;

    [SerializeField] private List<GameObject> roomLobby = new List<GameObject>();
    [SerializeField] private List<GameObject> roomNormal = new List<GameObject>();
    [SerializeField] private List<GameObject> roomTreasure = new List<GameObject>();
    [SerializeField] private List<GameObject> roomChallenge = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMerchant = new List<GameObject>();
    [SerializeField] private List<GameObject> roomSecret = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMiniBoss = new List<GameObject>();
    [SerializeField] private List<GameObject> roomBoss = new List<GameObject>();

    [SerializeField] private List<GameObject> obstructionsDoor;

    private void Awake()
    {
        GenerateMap(new GenerationParam(nbNormal: 20));
    }

    bool GetDoorCandidates(ref GenerationParam genParam, DoorsGenerator doorsGenerator, out Door entranceDoor, out Door exitDoor)
    {
        entranceDoor = new Door();
        exitDoor = new Door();

        int noiseIndex = GameAssets.Instance.seed.Range(0, doorsGenerator.doors.Count, ref NoiseGenerator);
        for (int i = 0; noiseIndex < doorsGenerator.doors.Count; i++)
        {
            int index = (i + noiseIndex) % doorsGenerator.doors.Count;
            Door door = doorsGenerator.doors[index];

            for (int j = 0; j < 4; j++)
            {
                float indexRotation = (noiseIndex + j) % 4;
                float doorRotation = door.Rotation + 90f * indexRotation;
                float neededRotation = (doorRotation + 180f) % 360f;

                if (genParam.availableDoors.ContainsKey(neededRotation) && genParam.availableDoors[neededRotation].Count != 0)
                {
                    int randIndex = GameAssets.Instance.seed.Range(0, genParam.availableDoors[neededRotation].Count, ref NoiseGenerator);

                    entranceDoor = door;
                    exitDoor = genParam.availableDoors[neededRotation][randIndex];

                    doorsGenerator.transform.parent.parent.Rotate(0, 90f * indexRotation, 0); // rotate gameObject entrance to correspond the neededRotation

                    return true;
                }
            }
        }

        return false;
    }

    private void GenerateBossRoom(ref GenerationParam genParam)
    {
        Door door = genParam.GetFarestDoor();

        genParam.RemoveDoor(door);
    }

    private void GenerateMap(GenerationParam genParam)
    {
        int nbRoom = genParam.TotalRoom;
        InstantiateLobby(ref genParam);

        for (int i = 0; i < nbRoom - 1; i++)
        {
            GenerateRoom(ref genParam);
        }

        GenerateBossRoom(ref genParam);

        // TODO : spawn things to hides the holes
        foreach (var listDoors in genParam.availableDoors)
        {
            foreach (var door in listDoors.Value)
            {
                GameObject go = Instantiate(obstructionsDoor[UnityEngine.Random.Range(0, obstructionsDoor.Count)], door.Position, Quaternion.identity);
                go.transform.Rotate(0, door.Rotation % 360, 0);
                go.transform.parent = gameObject.transform;
            }
        }
    }

    void GenerateRoom(ref GenerationParam genParam)
    {
        bool hasGenerated = false;
        while (!hasGenerated)
        {
            // instantiate room with first availableDoors transform then remove it
            int prefabIndex = GameAssets.Instance.seed.Range(0, roomNormal.Count, ref NoiseGenerator);
            GameObject roomGO = Instantiate(roomNormal[prefabIndex]); // TODO : add random selection

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(genParam);

            if (!GetDoorCandidates(ref genParam, doorsGenerator, out Door entranceDoor, out Door exitDoor))
            {
                DestroyImmediate(roomGO);
                continue;
            }

            // sortie.pos = entree.pos + (-entree.arrow.pos + sortie.arrow.pos) + forward * 0.1 (forward = pour avoir un offset)
            roomGO.transform.position = entranceDoor.parentSkeleton.transform.parent.transform.position - entranceDoor.Position + exitDoor.Position;
            Physics.SyncTransforms(); // need to update physics before doing testing in the same frame (bad)

            // bon sinon j'évite la collide de la salle et la salle exit (forcément que les deux collides putaig)
            BoxCollider roomCollider = roomGO.transform.Find("Skeleton").GetComponent<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.parentSkeleton.GetComponent<BoxCollider>();

            Collider[] colliders = roomCollider.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.Collide).Where(collider => collider != roomCollider && collider != roomColliderExit).ToArray();
            if (colliders.Length > 2) // more than the two meshCollider
            {
                DestroyImmediate(roomGO);

                // TODO : spawn a little cellule or something like this to hide the hole in the wall
                continue;
            }

            // Destroy used door
            genParam.availableDoors[exitDoor.Rotation].Remove(exitDoor);
            // removed door
            doorsGenerator.RemoveDoor(entranceDoor);

            // Add the new doors from the new room into the possible candidates
            genParam.AddDoorsGenerator(doorsGenerator);

            genParam.nbRoom[RoomType.Normal] -= doorsGenerator.doors.Count;

            roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();
            roomGO.transform.Find("RoomGenerator").gameObject.SetActive(false);
            roomGO.GetComponentInChildren<NavMeshSurface>().enabled = false;
            roomGO.transform.parent = gameObject.transform;
            hasGenerated = true;
        }
    }

    private void InstantiateLobby(ref GenerationParam genParam)
    {
        GameObject roomGO = Instantiate(roomLobby[GameAssets.Instance.seed.Range(0, roomLobby.Count, ref NoiseGenerator)]);

        DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
        doorsGenerator.GenerateSeed(genParam);

        genParam.AddDoorsGenerator(doorsGenerator);
        Destroy(doorsGenerator);

        genParam.nbRoom[RoomType.Normal] -= doorsGenerator.doors.Count;
        roomGO.transform.parent = gameObject.transform;
    }
}
