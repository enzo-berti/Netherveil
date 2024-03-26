using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour
{
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject bigMap;
    [SerializeField] private GameObject miniMapFrame;
    [SerializeField] private GameObject bigMapFrame;
    [SerializeField] private GameObject hud;
    [SerializeField] private Slider lifeJauge;
    [SerializeField] private TextMeshProUGUI lifeRatioText;
    [SerializeField] private PauseMenu pauseMenu;

    public void ToggleMap(InputAction.CallbackContext ctx)
    {
        if (miniMap.activeSelf)
        {
            miniMap.SetActive(false);
            bigMap.SetActive(true);
            miniMapFrame.SetActive(false);
            bigMapFrame.SetActive(true);
        }
        else
        {
            miniMap.SetActive(true);
            bigMap.SetActive(false);
            miniMapFrame.SetActive(true);
            bigMapFrame.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (hud.activeSelf)
        {
            hud.SetActive(false);
            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            hud.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        lifeRatioText.text = lifeJauge.value.ToString() + "\n----\n100";
    }
}
