using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FloatingTextMainMenu : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.2f;
    private TMP_Text m_Text;
    private Coroutine routine;

    private void Start()
    {
        m_Text = GetComponent<TMP_Text>();
    }

    public void FadeFloatingTexts(bool toggle)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FadeFloatingText(toggle));
    }

    private IEnumerator FadeFloatingText(bool toggle)
    {
        float elapsed = 0;
        float fadeFrom = toggle ? 0f : 1f;
        float fadeTo = toggle ? 1f : 0f;

        m_Text.alpha = fadeFrom;
        while (elapsed < fadeTime)
        {
            yield return null;
            elapsed = Mathf.Min(elapsed + Time.deltaTime, fadeTime);
            m_Text.alpha = Mathf.Lerp(fadeFrom, fadeTo, elapsed / fadeTime);
        }
        m_Text.alpha = fadeTo;
    }
}
