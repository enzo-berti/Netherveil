using System.Collections.Generic;
using UnityEngine;

public class GenerationParameters
{
    public int nbNormal;
    public int nbTreasure;
    public int nbChallenge;
    public int nbMerchant;
    public int nbSecret;
    public int nbMiniBoss;
    public int nbRoomMiniBoss;
    public int nbRoomBoss;

    public int NbRoom
    {
        get { return nbNormal + nbTreasure + nbChallenge + nbMerchant + nbSecret + nbMiniBoss + nbRoomMiniBoss + nbRoomBoss; }
    }
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomNormal = new List<GameObject>();
    [SerializeField] private List<GameObject> roomTreasure = new List<GameObject>();
    [SerializeField] private List<GameObject> roomChallenge = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMerchant = new List<GameObject>();
    [SerializeField] private List<GameObject> roomSecret = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMiniBoss = new List<GameObject>();
    [SerializeField] private List<GameObject> roomBoss = new List<GameObject>();

    private void Start()
    {
        GameManager.Instance.seed.Set(42069);
        GenerationParameters generationParam = new GenerationParameters();
        generationParam.nbNormal = 100;

        GenerateMap(generationParam);
    }

    void GenerateMap(GenerationParameters generationParameters)
    {
        List<GameObject> availableDoors = new List<GameObject>();
        int nbRoom = generationParameters.NbRoom + 1;
        for (int i = 0; i < nbRoom; i++)
        {
            GameObject roomGO;
            if (availableDoors.Count != 0)
            {
                // instantiate room with first availableDoors transform then remove it
                roomGO = Instantiate(roomNormal[0]); // TODO : add random selection

                DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
                doorsGenerator.GenerateSeed(generationParameters);

                int index = 0;
                GameObject entranceDoor = null;
                GameObject exitDoor = null;
                Debug.Log("GENERATE DOOR");
                while(entranceDoor == null)
                {
                    if (index >= availableDoors.Count)
                    {
                        Debug.LogError("NON");
                        return;
                    }

                    foreach (var door in doorsGenerator.doors)
                    {
                        if (door.transform.rotation.eulerAngles.y == (availableDoors[index].transform.rotation.eulerAngles.y + 180f) % 360f)
                        {
                            entranceDoor = door;
                            exitDoor = availableDoors[index];
                            break;
                        }
                    }
                    index++;
                }

                // sortie.pos = entree.pos + (-entree.arrow.pos + sortie.arrow.pos)
                roomGO.transform.position = entranceDoor.transform.parent.parent.parent.position + (-entranceDoor.transform.position + exitDoor.transform.position);
                doorsGenerator.CloseDoor(entranceDoor);

                Destroy(exitDoor);
                availableDoors.Remove(exitDoor);

                availableDoors.AddRange(doorsGenerator.doors);
                generationParameters.nbNormal -= doorsGenerator.doors.Count;
            }
            else // first room
            {
                roomGO = Instantiate(roomNormal[0]);

                DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
                doorsGenerator.GenerateSeed(generationParameters);
                availableDoors.AddRange(doorsGenerator.doors);
                generationParameters.nbNormal -= doorsGenerator.doors.Count;

                Debug.Log("FIRST ROOM INSTANCE", roomGO);
            }

            roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();
            RoomGenerator.RoomGenerated++;
        }
    }
}
