using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HudHandler : MonoBehaviour
{
    private static HudHandler instance;
    public static HudHandler current
    {
        get
        {
            if (instance == null)
                throw new System.Exception("No HUD Handler in the scene");

            return instance;
        }
    }

    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private TMP_Text BloodTestMesh;
    [SerializeField] private MinMaxSlider corruptionSlider;

    [Header("HUD parts")]
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private QuestHUD questHUD;
    [SerializeField] private MapHUD mapHUD;
    [SerializeField] private DescriptionTabHUD descriptionTab;
    [SerializeField] private MessageInfoHUD messageInfoHUD;

    public PauseMenu PauseMenu => pauseMenu;
    public QuestHUD QuestHUD => questHUD;
    public MapHUD MapHUD => mapHUD;
    public DescriptionTabHUD DescriptionTab => descriptionTab;
    public MessageInfoHUD MessageInfoHUD => messageInfoHUD;

    private Hero player;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Hero>();
    }

    private void Update()
    {
        BloodTestMesh.text = player.Inventory.Blood.ToString();
        corruptionSlider.value = player.Stats.GetValue(Stat.CORRUPTION);
    }

    private void OnEnable()
    {
        player.OnDeath += ActiveGameOver;
    }

    private void OnDisable()
    {
        player.OnDeath -= ActiveGameOver;
    }


    public void ActiveGameOver(Vector3 _)
    {
        GameOver.SetActive(true);
        hud.SetActive(false);
    }
}
