using UnityEngine;
using UnityEngine.InputSystem;

public class HudHandler : MonoBehaviour
{
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject bigMap;
    [SerializeField] private GameObject hud;
    [SerializeField] private PauseMenu pauseMenu;

    public void ToggleMap(InputAction.CallbackContext ctx)
    {
        if (miniMap.activeSelf)
        {
            miniMap.SetActive(false);
            bigMap.SetActive(true);
        }
        else
        {
            miniMap.SetActive(true);
            bigMap.SetActive(false);
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
}
