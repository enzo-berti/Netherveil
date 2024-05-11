using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private Camera deathCam;
    [SerializeField] GameObject firstSelect;
    readonly List<Graphic> drawables = new List<Graphic>();

    void Awake()
    {
        FindDrawablesRecursively(transform);
    }

    void FindDrawablesRecursively(Transform current)
    {
        if (current.TryGetComponent(out Graphic graphic))
        {
            drawables.Add(graphic);
        }

        foreach (Transform child in current)
        {
            FindDrawablesRecursively(child);
        }
    }

    private void OnEnable()
    {
        deathCam = GameObject.FindGameObjectWithTag("DeathCam").GetComponent<Camera>();

        deathCam.depth = 1;
        Color clearColor = new Color(1, 1, 1, 0);
        deathCam.backgroundColor *= clearColor;

        foreach(Graphic drawable in drawables)
        {
            drawable.color *= clearColor;
        }

        DisableAllMob();
        StartCoroutine(IncreaseAlpha());
    }

    private void DisableAllMob()
    {
        foreach (GameObject enemy in MapUtilities.currentRoomData.enemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
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

        foreach (Graphic drawable in drawables)
        {
            StartCoroutine(IncreaseElementAlpha(drawable));
        }
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

        if(element.gameObject.TryGetComponent(out Button button))
        {
            button.interactable = true;
            EventSystem.current.SetSelectedGameObject(firstSelect);
        }
        
    }
}
