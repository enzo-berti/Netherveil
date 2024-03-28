using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SaveBook : MonoBehaviour
{
    [SerializeField] private Transform pointToGo;
    private float speed = 2.0f;
    private Coroutine movementRoutine;
    public UnityEvent onBookArrived;

    public void GoToPoint()
    {
        if (movementRoutine == null)
            movementRoutine = StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        float distance = Vector3.Distance(transform.position, pointToGo.position);
        float time = distance / speed;

        float elapsedTime = 0.0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;

            transform.position = Vector3.Lerp(startPosition, pointToGo.position, t);
            transform.rotation = Quaternion.Slerp(startRotation, pointToGo.rotation, t);

            yield return null;
        }

        onBookArrived?.Invoke();
    }
}
