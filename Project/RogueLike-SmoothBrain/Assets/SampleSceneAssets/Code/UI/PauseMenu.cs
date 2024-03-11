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
        hud.SetActive(true);
        gameObject.SetActive(false);
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
