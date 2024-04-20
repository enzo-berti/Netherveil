using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HudHandler : MonoBehaviour
{
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private QuestHUD questHUD;
    [SerializeField] private MapHUD mapHUD;
    [SerializeField] private TMP_Text BloodTestMesh;
    [SerializeField] private MinMaxSlider corruptionSlider;

    private Hero player;

    private void Awake()
    {
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

    public void ToggleMap(InputAction.CallbackContext ctx)
    {
        mapHUD.Toggle();
    }

    public void ToggleQuest(InputAction.CallbackContext ctx)
    {
        questHUD.Toggle();
    }

    public void TogglePause(InputAction.CallbackContext ctx)
    {
        pauseMenu.Toggle();
    }
}
