using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Graphic textMesh1;
    [SerializeField] private Graphic textMesh2;
    [SerializeField] private Graphic textMesh3;
    [SerializeField] private Graphic textMesh4;
    [SerializeField] private GameObject buttonsObject;

    private IEnumerator Start()
    {
        Color baseColorText1 = textMesh1.color;
        baseColorText1.a = 1.0f;
        Color baseTransparencyText1 = baseColorText1;
        baseTransparencyText1.a = 0.0f;
        textMesh1.color = baseTransparencyText1;

        Color baseColorText2 = textMesh2.color;
        baseColorText2.a = 1.0f;
        Color baseTransparencyText2 = baseColorText2;
        baseTransparencyText2.a = 0.0f;
        textMesh2.color = baseTransparencyText2;

        Color baseColorText3 = textMesh3.color;
        baseColorText3.a = 1.0f;
        Color baseTransparencyText3 = baseColorText3;
        baseTransparencyText3.a = 0.0f;
        textMesh3.color = baseTransparencyText3;

        Color baseColorText4 = textMesh4.color;
        baseColorText4.a = 1.0f;
        Color baseTransparencyText4 = baseColorText4;
        baseTransparencyText4.a = 0.0f;
        textMesh4.color = baseTransparencyText4;

        buttonsObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        float elapsed = 0.0f;
        float duration = 3.14f;

        while (elapsed < duration)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
            float factor = elapsed / duration;
            float ease = Mathf.Sin(factor * Mathf.PI);

            textMesh1.color = Color.Lerp(baseTransparencyText1, baseColorText1, ease);

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        elapsed = 0.0f;
        duration = 3.14f;

        while (elapsed < duration)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
            float factor = elapsed / duration;
            float ease = Mathf.Sin(factor * Mathf.PI);

            textMesh2.color = Color.Lerp(baseTransparencyText2, baseColorText2, ease);

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        elapsed = 0.0f;
        duration = 3.14f;

        while (elapsed < duration)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
            float factor = elapsed / duration;
            float ease = Mathf.Sin(factor * Mathf.PI);

            textMesh3.color = Color.Lerp(baseTransparencyText3, baseColorText3, ease);

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        elapsed = 0.0f;
        duration = 1.2f;

        while (elapsed < duration)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
            float factor = elapsed / duration;
            float ease = EasingFunctions.EaseOutSin(factor);

            textMesh4.color = Color.Lerp(baseTransparencyText4, baseColorText4, ease);

            yield return null;
        }

        buttonsObject.SetActive(true);
    }

    public void LoadMenu()
    {
        LevelLoader.current.LoadScene("MainMenu", true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
#else
        Application.Quit();
#endif
    }
}
