using FMODUnity;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class ExplodingBomb : MonoBehaviour, IDamageable
{
    [Header("Gameobjects & Components")]
    [SerializeField] private GameObject graphics;
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private EventReference bombSFX;
    [Header("Bomb Parameter")]
    [SerializeField] private bool activateOnAwake;
    [SerializeField] private float timerBeforeExplode;
    [SerializeField] private float blastRadius;
    [SerializeField] private int blastDamage;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private float throwHeight = 5f;
    private bool isActive;
    private bool isMoving => throwRoutine != null;
    private float elapsedExplosionTime;
    private Coroutine throwRoutine;
    private Coroutine explosionRoutine;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position;

        VFX.SetFloat("ExplosionTime", 1.0f);

        if (activateOnAwake)
            Activate();
    }

    public void SetTimeToExplode(float _timeToExplode)
    {
        timerBeforeExplode = _timeToExplode;
        VFX.SetFloat("TimeToExplode", timerBeforeExplode);
    }

    void Update()
    {
        if (isActive)
            UpdateTimerExplosion();
    }

    public void ThrowTo(Vector3 endPosition, float totalTime = 1f)
    {
        if (throwRoutine != null)
            return;

        VFX.transform.parent = null;
        VFX.transform.position = endPosition;
        VFX.SetFloat("TimeToExplode", totalTime);
        VFX.SetFloat("ExplosionRadius", blastRadius);
        VFX.Play();

        this.endPosition = endPosition;
        throwRoutine = StartCoroutine(LerpPositionUpdate(endPosition, totalTime));
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
        Explode();
    }

    public IEnumerator ThrowToPos(Vector3 pos, float throwTime)
    {
        float timer = 0;

        Vector3 basePos = this.transform.position;
        Vector3 position3D = Vector3.zero;
        float a = -16, b = 16;
        float c = this.transform.position.y;
        float timerToReach = Resolve2ndDegree(a, b, c, 0).Max();
        while (timer < timerToReach)
        {
            yield return null;
            timer = timer > timerToReach ? timerToReach : timer;
            if (timer < 1.0f)
            {
                timer = timer > 1 ? 1 : timer;

                position3D = Vector3.Lerp(basePos, pos, timer);
            }
            position3D.y = SquareFunction(a, b, c, timer);
            this.transform.position = position3D;
            timer += Time.deltaTime / throwTime;
        }
    }

    float[] Resolve2ndDegree(float a, float b, float c, float wantedY)
    {
        c -= wantedY;
        float delta = b * b - 4 * a * c;
        float[] results = new float[2];
        if (delta >= 0)
        {
            results[0] = (float)(-b + Math.Sqrt(delta)) / (2 * a);
            results[1] = (float)(-b - Math.Sqrt(delta)) / (2 * a);
        }
        else
        {
            Debug.LogWarning("No result in Real number");
            return results;
        }
        return results;
    }

    private float SquareFunction(float a, float b, float c, float timer)
    {
        return a * timer * timer + b * timer + c;
    }
    void UpdateTimerExplosion()
    {
        if (elapsedExplosionTime + timerBeforeExplode < Time.time)
            Explode();
    }

    public void Activate()
    {
        isActive = true;
        elapsedExplosionTime = Time.time;
        VFX.Play();
    }

    public void Explode()
    {
        if (explosionRoutine == null)
            explosionRoutine = StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        Physics.OverlapSphere(transform.position, blastRadius, damageLayer)
            .Select(entity => entity.GetComponent<IBlastable>())
            .Where(entity => entity != null)
            .ToList()
            .ForEach(currentEntity =>
            {
                currentEntity.ApplyDamage(blastDamage);
            });

        graphics.SetActive(false);
        AudioManager.Instance.PlaySound(bombSFX, transform.position);
        float timer = VFX.GetFloat("ExplosionTime");

        while (timer > 0f)
        {
            yield return null;
            timer -= Time.deltaTime;
        }

        Destroy(gameObject);
        Destroy(VFX.gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        Activate();
    }

    public void Death()
    {
        throw new System.NotImplementedException();
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
