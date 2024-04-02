using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour
{
    [SerializeField] private GameObject minimapCam;
    [SerializeField] private GameObject bigmapCam;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject bigMap;
    [SerializeField] private GameObject miniMapFrame;
    [SerializeField] private GameObject bigMapFrame;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private Slider lifeJauge;
    [SerializeField] private TextMeshProUGUI lifeRatioText;
    [SerializeField] private GeneralMenu pauseMenu;
    
    Hero player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        player.OnDeath += ActiveGameOver;
    }

    public void ToggleMap(InputAction.CallbackContext ctx)
    {
        if (miniMap.activeSelf)
        {
            minimapCam.SetActive(false);
            bigmapCam.SetActive(true);
            miniMap.SetActive(false);
            bigMap.SetActive(true);
            miniMapFrame.SetActive(false);
            bigMapFrame.SetActive(true);
        }
        else
        {
            minimapCam.SetActive(true);
            bigmapCam.SetActive(false);
            miniMap.SetActive(true);
            bigMap.SetActive(false);
            miniMapFrame.SetActive(true);
            bigMapFrame.SetActive(false);
        }
    }

    public void TogglePause(InputAction.CallbackContext ctx)
    {
        if (hud.activeSelf)
        {
            Time.timeScale = 0f;
            hud.SetActive(false);
            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            hud.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
        }
    }

    public void ActiveGameOver(Vector3 _playerPosDeath)
    {
        GameOver.SetActive(true);
        hud.SetActive(false);
    }

    private void Update()
    {
        if(lifeJauge.value != player.Stats.GetValue(Stat.HP))
        {
            lifeJauge.value = player.Stats.GetValue(Stat.HP);
            lifeJauge.maxValue = player.Stats.GetMaxValue(Stat.HP);
            lifeRatioText.text = lifeJauge.value.ToString() + " / " + player.Stats.GetMaxValue(Stat.HP);
        }
    }
    private void OnDestroy()
    {
        player.OnDeath -= ActiveGameOver;
    }
}
