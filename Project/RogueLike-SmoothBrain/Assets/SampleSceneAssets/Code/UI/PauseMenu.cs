using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject hud;

    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        hud.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Setting()
    {

    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
