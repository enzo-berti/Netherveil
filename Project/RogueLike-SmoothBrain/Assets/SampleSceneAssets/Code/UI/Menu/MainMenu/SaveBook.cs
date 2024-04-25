using System.Collections;
using UnityEngine;

public class SaveBook : MonoBehaviour
{
    [SerializeField] private Transform closeTransform;
    [SerializeField] private Transform openTransform;
    [SerializeField] private GameObject closeObject;
    [SerializeField] private GameObject openObject;
    private float durationMovement = 2.5f;

    private Coroutine routine;

    public void Close()
    {
        if (routine != null)
            return;

        routine = StartCoroutine(CloseRoutine());
    }

    public void Open()
    {
        if (routine != null)
            return;

        routine = StartCoroutine(OpenRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        float elapsed = 0.0f;

        while (elapsed < durationMovement)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, durationMovement);
            float factor = elapsed / durationMovement;
            float ease = EasingFunctions.EaseInOutQuad(factor);
            Vector3 resultPosition = Vector3.Lerp(openTransform.position, closeTransform.position, ease);
            Quaternion resultRotation = Quaternion.Lerp(openTransform.rotation, closeTransform.rotation, ease);

            transform.position = resultPosition;
            transform.rotation = resultRotation;

            yield return null;
        }

        closeObject.SetActive(true);
        openObject.SetActive(false);

        transform.position = closeTransform.position;
        transform.rotation = closeTransform.rotation;

        routine = null;
    }

    private IEnumerator OpenRoutine()
    {
        float elapsed = 0.0f;

        while (elapsed < durationMovement)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, durationMovement);
            float factor = elapsed / durationMovement;
            float ease = EasingFunctions.EaseInOutQuad(factor);
            Vector3 resultPosition = Vector3.Lerp(closeTransform.position, openTransform.position, ease);
            Quaternion resultRotation = Quaternion.Lerp(closeTransform.rotation, openTransform.rotation, ease);

            transform.position = resultPosition;
            transform.rotation = resultRotation;

            yield return null;
        }

        closeObject.SetActive(false);
        openObject.SetActive(true);

        transform.position = openTransform.position;
        transform.rotation = openTransform.rotation;

        routine = null;
    }
}
