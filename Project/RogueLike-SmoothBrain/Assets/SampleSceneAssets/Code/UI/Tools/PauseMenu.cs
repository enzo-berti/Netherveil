using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject hud;

    public static event Action OnPause;
    public static event Action OnUnpause;

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;

        hud.SetActive(false);
        gameObject.SetActive(true);

        OnPause?.Invoke();
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;

        hud.SetActive(true);
        gameObject.SetActive(false);

        OnUnpause?.Invoke();
    }

    public void ReloadGame()
    {
        FindObjectOfType<LevelLoader>().LoadScene(SceneManager.GetActiveScene().buildIndex, "Fade");
    }

    public void Setting()
    {

    }

    public void Menu()
    {
        Time.timeScale = 1f;
        FindObjectOfType<LevelLoader>().LoadScene("MainMenu", "Fade");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
