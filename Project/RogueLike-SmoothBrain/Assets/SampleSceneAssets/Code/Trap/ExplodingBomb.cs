using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ExplodingBomb : MonoBehaviour
{
    [Header("Bomb Parameter")]
    [SerializeField] private bool activateOnAwake;
    [SerializeField] private float timerBeforeExplode;
    [SerializeField] private float blastRadius;
    [SerializeField] private int blastDamage;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private float throwHeight = 5f;
    private bool isActive;
    private bool isMoving => throwCoroutine != null;
    private float elapsedExplosionTime;
    private Coroutine throwCoroutine;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position;

        if (activateOnAwake)
            Activate();
    }

    void Update()
    {
        if (isActive)
            UpdateTimerExplotion();
    }

    public void ThrowTo(Vector3 endPosition, float totalTime = 1f)
    {
        if (throwCoroutine != null)
            return;

        this.endPosition = endPosition;
        throwCoroutine = StartCoroutine(LerpPositionUpdate(endPosition, totalTime));
    }

    private IEnumerator LerpPositionUpdate(Vector3 endPosition, float totalTime)
    {
        float elapsedTimePosition = 0f;

        while (transform.position != endPosition)
        {
            elapsedTimePosition = Mathf.Clamp(elapsedTimePosition + Time.deltaTime, 0f, totalTime);
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTimePosition / totalTime) + Vector3.up * Mathf.Lerp(0f, throwHeight, elapsedTimePosition / totalTime) * Mathf.Sin(elapsedTimePosition / totalTime * Mathf.PI);
            yield return null;
        }
        transform.position = endPosition;
        Activate();
    }

    void UpdateTimerExplotion()
    {
        if (elapsedExplosionTime + timerBeforeExplode < Time.time)
            Explode();
    }

    public void Activate()
    {
        isActive = true;
        elapsedExplosionTime = Time.time;
    }

    public void Explode()
    {
        Physics.OverlapSphere(transform.position, blastRadius, damageLayer)
            .Select(entity => entity.GetComponent<IBlastable>())
            .Where(entity => entity != null)
            .ToList()
            .ForEach(currentEntity => { 
                currentEntity.ApplyDamage(blastDamage); 
            });

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (isActive || isMoving)
        {
            Handles.color = new Color(1, 0, 0, 0.25f);
            Handles.DrawWireDisc(endPosition, Vector3.up, blastRadius);

            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.up,
                $"Bomb" +
                $"\nActivate : {isActive}" +
                $"\nBefore explode : {timerBeforeExplode - Time.time + elapsedExplosionTime}");
        }
        else
        {
            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.up,
                $"Bomb" +
                $"\nActivate : {isActive}");
        }
    }
#endif
}
