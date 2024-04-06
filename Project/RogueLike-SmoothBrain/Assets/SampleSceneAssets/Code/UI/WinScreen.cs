using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Image blackPanel;
    [SerializeField] private Image buttonPanel;
    [SerializeField] private Image menu;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private Image quit;
    [SerializeField] private TextMeshProUGUI quitText;
    [SerializeField] private TextMeshProUGUI WinText;
    [SerializeField] private TextMeshProUGUI EndText;
    [SerializeField] private TextMeshProUGUI EndText2;
    [SerializeField] private TextMeshProUGUI EndText3;

    private void OnEnable()
    {
        Color clearColor = new Color(1, 1, 1, 0);
        buttonPanel.color *= clearColor;
        blackPanel.color *= clearColor;
        menu.color *= clearColor;
        menuText.color *= clearColor;
        quit.color *= clearColor;
        quitText.color *= clearColor;
        WinText.color *= clearColor;
        EndText.color *= clearColor;
        EndText2.color *= clearColor;
        EndText3.color *= clearColor;

        DisableAllMob();
        IncreaseAlpha();
    }

    private void DisableAllMob()
    {
        foreach (GameObject enemy in RoomUtilities.roomData.enemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }
    }

    void IncreaseAlpha()
    {
        StartCoroutine(ModifyElementAlpha(blackPanel,0f, 1f));
        StartCoroutine(ModifyElementAlpha(EndText, 1.5f, 1f));
        StartCoroutine(ModifyElementAlpha(EndText, 3.5f, 0f));
        StartCoroutine(ModifyElementAlpha(EndText2, 2.5f, 1f));
        StartCoroutine(ModifyElementAlpha(EndText2, 4.5f, 0f));
        StartCoroutine(ModifyElementAlpha(EndText3, 3.5f, 1f));
        StartCoroutine(ModifyElementAlpha(EndText3, 5.5f, 0f));
        StartCoroutine(ModifyElementAlpha(WinText, 6.5f, 1f));
        StartCoroutine(ModifyElementAlpha(menu, 7.5f, 1f));
        StartCoroutine(ModifyElementAlpha(menuText, 7.5f, 1f));
        StartCoroutine(ModifyElementAlpha(quit, 7.5f, 1f));
        StartCoroutine(ModifyElementAlpha(quitText, 7.5f, 1f));
        StartCoroutine(ModifyElementAlpha(buttonPanel, 7.5f, 1f));
    }

    IEnumerator ModifyElementAlpha(Graphic element, float timeToWait, float targetAlpha)
    {
        yield return new WaitForSeconds(timeToWait);

        float duration = 2.0f;
        float elapsedTime = 0f;
        Color initialColor = element.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * 1.5f;
            float t = Mathf.Clamp01(elapsedTime / duration);
            element.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }
    }
}
