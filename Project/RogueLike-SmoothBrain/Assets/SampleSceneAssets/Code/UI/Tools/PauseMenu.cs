using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Selectable firstSelect;

    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject settings;

    public static event Action OnPause;
    public static event Action OnUnpause;

    public void Toggle()
    {
        if (gameObject.activeSelf || settings.gameObject.activeSelf)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (settings.gameObject.activeSelf)
            return;

        Time.timeScale = 0.0f;

        Utilities.PlayerInput.DisableGameplayInputs();
        hud.SetActive(false);
        gameObject.SetActive(true);

        OnPause?.Invoke();

        EventSystem.current.SetSelectedGameObject(firstSelect.gameObject);
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        Utilities.PlayerInput.EnableGameplayInputs();

        hud.SetActive(true);
        gameObject.SetActive(false);
        settings.SetActive(false);

        OnUnpause?.Invoke();
    }

    public void ReloadGame()
    {
        EventSystem.current.gameObject.SetActive(false);
        LevelLoader.current.LoadScene(SceneManager.GetActiveScene().buildIndex, true);
    }

    public void Setting()
    {
        settings.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Menu()
    {
        EventSystem.current.gameObject.SetActive(false);
        Time.timeScale = 1f;
        LevelLoader.current.LoadScene("MainMenu", true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
