using System.Collections;
using UnityEngine;

public static class UITween
{
    public static IEnumerator UpScaleCoroutine(Transform t, float duration, float maxScale = 1.0f, float minScale = 0.0f)
    {
        float elapsed = 0.0f;
        float startScale = t.localScale.x;
        duration -= (startScale - minScale) / (maxScale - minScale) * duration;

        while (elapsed < duration)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
            float factor = elapsed / duration;
            float lerp = Mathf.Lerp(startScale, maxScale, factor);

            t.localScale = Vector3.one * lerp;

            yield return null;
        }
    }

    public static IEnumerator DownScaleCoroutine(Transform t, float duration, float maxScale = 1.0f, float minScale = 0.0f)
    {
        float elapsed = 0.0f;
        float startScale = t.localScale.x;
        duration -= (startScale - maxScale) / (minScale - maxScale) * duration;

        while (elapsed < duration)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
            float factor = elapsed / duration;
            float lerp = Mathf.Lerp(startScale, minScale, factor);

            t.localScale = Vector3.one * lerp;

            yield return null;
        }
    }
}
