using System.Collections;
using UnityEngine;

public class SaveBook : MonoBehaviour
{
    [SerializeField] private Transform closeTransform;
    [SerializeField] private Transform openTransform;
    [SerializeField] private GameObject closeObject;
    [SerializeField] private GameObject openObject;
    private Animator bookAnimator;
    private float durationMovementIn = 1.0f;
    private float durationMovementOut = 1.0f;

    private Coroutine routine;

    private void Start()
    {
        bookAnimator = openObject.GetComponent<Animator>();
    }

    public void Close()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(CloseRoutine());
    }

    public void Open()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(OpenRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        float elapsed = 0.0f;
        bookAnimator.SetBool("IsOpen", false);

        while (elapsed < durationMovementOut)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, durationMovementOut);
            float factor = elapsed / durationMovementOut;
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

        while (elapsed < durationMovementIn)
        {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, durationMovementIn);
            float factor = elapsed / durationMovementIn;
            float ease = EasingFunctions.EaseInOutQuad(factor);
            Vector3 resultPosition = Vector3.Lerp(closeTransform.position, openTransform.position, ease);
            Quaternion resultRotation = Quaternion.Lerp(closeTransform.rotation, openTransform.rotation, ease);

            transform.position = resultPosition;
            transform.rotation = resultRotation;

            yield return null;
        }

        closeObject.SetActive(false);
        openObject.SetActive(true);
        bookAnimator.SetBool("IsOpen", true);

        transform.position = openTransform.position;
        transform.rotation = openTransform.rotation;

        routine = null;
    }
}
