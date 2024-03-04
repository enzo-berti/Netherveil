using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public GenerationParam(int nbNormal = 0, int nbTreasure = 0, int nbChallenge = 0, int nbMerchant = 0, int nbSecret = 0, int nbMiniBoss = 0, int nbBoss = 0)
    {
        nbRoom = new Dictionary<RoomType, int>
        {
            { RoomType.Normal, nbNormal },
            { RoomType.Treasure, nbTreasure },
            { RoomType.Challenge, nbChallenge },
            { RoomType.Merchant, nbMerchant },
            { RoomType.Secret, nbSecret },
            { RoomType.MiniBoss, nbMiniBoss },
            { RoomType.Boss, nbBoss },
        };


        availableDoors = new Dictionary<float, List<Door>>
        {
            { 0f, new List<Door>() },
            { 90f, new List<Door>() },
            { 180f, new List<Door>() },
            { 270f, new List<Door>() }
        };
    }

    public int TotalRoom
    {
        get { return nbRoom[RoomType.Normal] + nbRoom[RoomType.Treasure] + nbRoom[RoomType.Challenge] + nbRoom[RoomType.Merchant] + nbRoom[RoomType.Secret] + nbRoom[RoomType.MiniBoss] + nbRoom[RoomType.Boss]; }
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

    private void Awake()
    {
        GameManager.Instance.seed.Set(341304);

        GenerationParam genParam = new GenerationParam(nbNormal: 5);

        GenerateMap(ref genParam);
    }

    bool GetDoorCandidates(ref GenerationParam genParam, DoorsGenerator doorsGenerator, out Door entranceDoor, out Door exitDoor)
    {
        entranceDoor = new Door();
        exitDoor = new Door();

        int loopCount = 0;
        for (int startIndex = GameManager.Instance.seed.Range(0, doorsGenerator.doors.Count, ref NoiseGenerator); loopCount < doorsGenerator.doors.Count; loopCount++)
        {
            Door door = doorsGenerator.doors[(startIndex + loopCount) % doorsGenerator.doors.Count];
            float neededRotation = (door.rotation + 180f) % 360f;

            if (genParam.availableDoors.ContainsKey(neededRotation) && genParam.availableDoors[neededRotation].Count != 0)
            {
                int randIndex = GameManager.Instance.seed.Range(0, genParam.availableDoors[neededRotation].Count, ref NoiseGenerator);

                entranceDoor = door;
                exitDoor = genParam.availableDoors[neededRotation][randIndex];
                break;
            }
        }

        if (loopCount == doorsGenerator.doors.Count) // couldn't find a candidate
        {
            return false;
        }

        return true;
    }

    void GenerateMap(ref GenerationParam genParam)
    {
        int nbRoom = genParam.TotalRoom;
        InstantiateLobby(out GameObject obj, ref genParam);

        for (int i = 0; i < nbRoom - 1; i++)
        {
            GenerateRoom(ref genParam);
        }

        // TODO : spawn things to hides the holes
    }

    void GenerateRoom(ref GenerationParam genParam)
    {
        bool hasGenerated = false;
        while (!hasGenerated)
        {
            // instantiate room with first availableDoors transform then remove it
            GameObject roomGO = Instantiate(roomNormal[GameManager.Instance.seed.Range(0, roomNormal.Count, ref NoiseGenerator)]); // TODO : add random selection

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(genParam);

            if (!GetDoorCandidates(ref genParam, doorsGenerator, out Door entranceDoor, out Door exitDoor))
            {
                DestroyImmediate(roomGO);
                continue;
            }

            // sortie.pos = entree.pos + (-entree.arrow.pos + sortie.arrow.pos) + forward * 0.1 (forward = pour avoir un offset)
            roomGO.transform.position = entranceDoor.parentSkeleton.transform.parent.transform.position - entranceDoor.Position + exitDoor.Position + (-exitDoor.forward * 1f);
            Physics.SyncTransforms(); // need to update physics before doing testing in the same frame (bad)

            // bon sinon j'évite la collide de la salle et la salle exit (forcément que les deux collides putaig)
            BoxCollider roomCollider = roomGO.transform.Find("Skeleton").GetComponent<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.parentSkeleton.GetComponent<BoxCollider>();

            Collider[] colliders = roomCollider.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.Collide).Where(collider => collider != roomCollider && collider != roomColliderExit).ToArray();
            if (colliders.Length > 2) // more than the two meshCollider
            {
                genParam.availableDoors[exitDoor.rotation].Remove(exitDoor);
                DestroyImmediate(roomGO);

                // TODO : spawn a little cellule or something like this to hide the hole in the wall
                continue;
            }

            // Close door for the next generation
            doorsGenerator.RemoveDoor(entranceDoor);

            // Destroy used door
            genParam.availableDoors[exitDoor.rotation].Remove(exitDoor);

            // Add the new doors from the new room into the possible candidates
            foreach (var door in doorsGenerator.doors)
            {
                if (genParam.availableDoors.ContainsKey(door.rotation))
                {
                    genParam.availableDoors[door.rotation].Add(door);
                    //doorsGenerator.RemoveDoor(door);
                }
                else
                {
                    Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.rotation);
                }
            }

            genParam.nbRoom[RoomType.Normal] -= doorsGenerator.doors.Count;

            roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();
            roomGO.transform.parent = gameObject.transform;
            hasGenerated = true;
        }
    }

    private void InstantiateLobby(out GameObject roomGO, ref GenerationParam genParam)
    {
        roomGO = Instantiate(roomLobby[GameManager.Instance.seed.Range(0, roomLobby.Count, ref NoiseGenerator)]);
    
        DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
        doorsGenerator.GenerateSeed(genParam);
    
        foreach (var door in doorsGenerator.doors)
        {
            if (genParam.availableDoors.ContainsKey(door.rotation))
            {
                genParam.availableDoors[door.rotation].Add(door);
            }
            else
            {
                Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.rotation);
            }
        }
    
        genParam.nbRoom[RoomType.Normal] -= doorsGenerator.doors.Count;
        roomGO.transform.parent = gameObject.transform;
    }
}
