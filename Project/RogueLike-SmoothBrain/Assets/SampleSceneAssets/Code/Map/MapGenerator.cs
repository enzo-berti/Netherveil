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
        GenerationParameters generationParam = new GenerationParameters();
        generationParam.nbNormal = 2;

        GenerateMap(generationParam);
    }

    void GenerateMap(GenerationParameters generationParameters)
    {
        List<GameObject> availableDoors = new List<GameObject>();
        for (int i = 0; i < generationParameters.NbRoom; i++)
        {
            GameObject roomGO;
            if (availableDoors.Count != 0)
            {
                // instantiate room with first availableDoors transform then remove it
                //roomGO = Instantiate(roomNormal[0], availableDoors[0].transform.position, availableDoors[0].transform.rotation);

                roomGO = Instantiate(roomNormal[0]);

                DoorsGenerator generateTemp = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();

                GameObject doorSelected = generateTemp.GetRandomAvailableDoor();
                generateTemp.SetDoorState(DoorState.OPEN, doorSelected);
                roomGO.transform.position = availableDoors[0].transform.position + (availableDoors[0].transform.position - availableDoors[0].transform.parent.Center());
                //roomGO.transform.rotation = availableDoors[0].transform.rotation;

                generationParameters.nbNormal--;
                availableDoors.Remove(availableDoors[0]);
            }
            else
            {
                roomGO = Instantiate(roomNormal[0]);
            
            }

            roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
            availableDoors.AddRange(doorsGenerator.GenerateDoors(generationParameters));

            RoomGenerator.RoomGenerated++;
        }
    }
}
