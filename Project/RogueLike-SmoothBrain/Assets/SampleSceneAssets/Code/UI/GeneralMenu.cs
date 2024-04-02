using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralMenu : MonoBehaviour
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
