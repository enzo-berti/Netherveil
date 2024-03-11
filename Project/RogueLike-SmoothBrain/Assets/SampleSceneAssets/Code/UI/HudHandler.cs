using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HudHandler : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject bigMap;

    [Header("PAUSE")]
    [SerializeField] private GameObject pause;

    public void OpenClosePause()
    {
        if (hud.activeSelf)
        {
            pause.SetActive(true);
            hud.SetActive(false);
        }
        else
        {
            Resume();
        }
    }

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

    public void Resume()
    {
        pause.SetActive(false);
        hud.SetActive(true);
    }

    public void Setting()
    {

    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
