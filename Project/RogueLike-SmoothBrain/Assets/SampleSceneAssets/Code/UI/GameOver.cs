using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private Camera deathCam;

    [SerializeField] private Image panel;
    [SerializeField] private Image reload;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private Image menu;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private Image quit;
    [SerializeField] private TextMeshProUGUI quitText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private void OnEnable()
    {
        deathCam = GameObject.FindGameObjectWithTag("DeathCam").GetComponent<Camera>();

        deathCam.depth = 1;
        Color clearColor = new Color(1, 1, 1, 0);
        deathCam.backgroundColor = deathCam.backgroundColor * clearColor;
        panel.color *= clearColor;
        reload.color *= clearColor;
        reloadText.color *= clearColor;
        menu.color *= clearColor;
        menuText.color *= clearColor;
        quit.color *= clearColor;
        quitText.color *= clearColor;
        gameOverText.color *= clearColor;

        DisableAllMob();
        StartCoroutine(IncreaseAlpha());
    }

    private void DisableAllMob()
    {
        if (RoomUtilities.roomData.enemies != null)
        {
            foreach (GameObject enemy in RoomUtilities.roomData.enemies)
            {
                if (enemy != null)
                {
                    enemy.SetActive(false);
                }
            }
        }
    }

    IEnumerator IncreaseAlpha()
    {
        float targetAlpha = 1.0f;
        float duration = 2.0f; 
        float elapsedTime = 0f;
        Color initialColor = deathCam.backgroundColor;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            deathCam.backgroundColor = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }

        StartCoroutine(IncreaseElementAlpha(panel));
        StartCoroutine(IncreaseElementAlpha(reload));
        StartCoroutine(IncreaseElementAlpha(reloadText));
        StartCoroutine(IncreaseElementAlpha(menu));
        StartCoroutine(IncreaseElementAlpha(menuText));
        StartCoroutine(IncreaseElementAlpha(quit));
        StartCoroutine(IncreaseElementAlpha(quitText));
        StartCoroutine(IncreaseElementAlpha(gameOverText));
    }

    IEnumerator IncreaseElementAlpha(Graphic element)
    {
        float targetAlpha = 1.0f;
        float duration = 2.0f; 
        float elapsedTime = 0f;
        Color initialColor = element.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            element.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }
    }
}
