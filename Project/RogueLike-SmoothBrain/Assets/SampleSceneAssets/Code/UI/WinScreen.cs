using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public static WinScreen instance;
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

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Color clearColor = new Color(1, 1, 1, 0);
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
        StartCoroutine(ModifyElementAlpha(blackPanel, 1f));
                       
        StartCoroutine(ModifyElementAlpha(EndText, 1f));
        if (EndText.color.a == 1f)
        {
            StartCoroutine(ModifyElementAlpha(EndText, 0f));
        }

        StartCoroutine(ModifyElementAlpha(EndText2, 1f));
        if (EndText2.color.a == 1f)
        {
            StartCoroutine(ModifyElementAlpha(EndText2, 0f));
        }

        StartCoroutine(ModifyElementAlpha(EndText3, 1f));
        if (EndText3.color.a == 1f)
        {
            StartCoroutine(ModifyElementAlpha(EndText3, 0f));
        }

        StartCoroutine(ModifyElementAlpha(WinText, 1f));
        if (WinText.color.a == 1f)
        {
            StartCoroutine(ModifyElementAlpha(buttonPanel, 1f));
            StartCoroutine(ModifyElementAlpha(menu, 1f));
            StartCoroutine(ModifyElementAlpha(menuText, 1f));
            StartCoroutine(ModifyElementAlpha(quit, 1f));
            StartCoroutine(ModifyElementAlpha(quitText, 1f));
        }
    }

    IEnumerator ModifyElementAlpha(Graphic element, float targetAlpha)
    {
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
