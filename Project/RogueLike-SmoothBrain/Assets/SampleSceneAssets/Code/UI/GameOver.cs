using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Camera deathCam;

    [SerializeField] private Image panel;
    [SerializeField] private Image resume;
    [SerializeField] private TextMeshProUGUI resumeText;
    [SerializeField] private Image menu;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private Image quit;
    [SerializeField] private TextMeshProUGUI quitText;
    [SerializeField] private TextMeshProUGUI gameOverText;


    private void Awake()
    {
        deathCam.backgroundColor = deathCam.backgroundColor * new Color(1, 1, 1, 0);

        panel.color = panel.color * new Color(1, 1, 1, 0);
        resume.color = resume.color * new Color(1, 1, 1, 0);
        resumeText.color = resumeText.color * new Color(1, 1, 1, 0);
        menu.color = menu.color * new Color(1, 1, 1, 0);
        menuText.color = menuText.color * new Color(1, 1, 1, 0);
        quit.color = quit.color * new Color(1, 1, 1, 0);
        quitText.color = quitText.color * new Color(1, 1, 1, 0);
        gameOverText.color = gameOverText.color * new Color(1, 1, 1, 0);
    }

    private void DisableAllMob()
    {

    }

    IEnumerator IncreasedUIAlpha()
    {
        yield return null;
    }

    private void OnEnable()
    {
        DisableAllMob();
        StartCoroutine(IncreasedUIAlpha());
    }
}
