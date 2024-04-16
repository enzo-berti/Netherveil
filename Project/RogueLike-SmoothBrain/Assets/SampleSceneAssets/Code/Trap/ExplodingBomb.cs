using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class ExplodingBomb : MonoBehaviour
{
    [Header("Gameobjects & Components")]
    [SerializeField] private GameObject graphics;
    [SerializeField] private GameObject VFXObject;
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private Sound bombSFX;
    [Header("Bomb Parameter")]
    [SerializeField] private bool activateOnAwake;
    [SerializeField] private float timerBeforeExplode;
    [SerializeField] private float blastRadius;
    [SerializeField] private int blastDamage;
    [SerializeField] private LayerMask damageLayer;
    //[SerializeField] private float throwHeight = 5f; -> Jamais utilisé, Dorian
    private bool isActive;
    private bool isMoving => throwRoutine != null;
    private float elapsedExplosionTime;
    private Coroutine throwRoutine;
    private Coroutine explosionRoutine;
    IAttacker launcher = null;

    private void Start()
    {
        VFX.SetFloat("ExplosionTime", 1.0f);
        VFX.SetFloat("ExplosionRadius", blastRadius);

        if (activateOnAwake)
            Activate();
    }

    public void SetTimeToExplode(float _timeToExplode)
    {
        timerBeforeExplode = _timeToExplode;
        VFX.SetFloat("TimeToExplode", timerBeforeExplode);
    }

    public void SetBlastDamages(int damages)
    {
        blastDamage = damages;
    }

    void Update()
    {
        if (isActive)
            UpdateTimerExplosion();
    }

    private IEnumerator ThrowToPosCoroutine(Vector3 pos, float throwTime)
    {
        VFXObject.transform.parent = null;
        VFXObject.transform.position = pos;
        VFX.Play();
        
        float timer = 0;
        Vector3 basePos = graphics.transform.position;
        Vector3 position3D = Vector3.zero;
        float a = -16, b = 16;
        float c = graphics.transform.position.y;
        float timerToReach = MathsExtension.Resolve2ndDegree(a, b, c, 0).Max();
        while (timer < timerToReach)
        {
            yield return null;
            timer = timer > timerToReach ? timerToReach : timer;
            if (timer < 1.0f)
            {
                timer = timer > 1 ? 1 : timer;

                position3D = Vector3.Lerp(basePos, pos, timer);
            }
            position3D.y = MathsExtension.SquareFunction(a, b, c, timer);
            graphics.transform.position = position3D;
            timer += Time.deltaTime / throwTime;
        }
    }

    public void ThrowToPos(IAttacker attacker, Vector3 pos, float throwTime)
    {
        launcher = attacker;
        StartCoroutine(ThrowToPosCoroutine(pos, throwTime));
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
        
    }

    public void Explode()
    {
        if (explosionRoutine == null)
            explosionRoutine = StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        Physics.OverlapSphere(graphics.transform.position, blastRadius, damageLayer)
            .Select(entity => entity.GetComponent<IBlastable>())
            .Where(entity => entity != null)
            .ToList()
            .ForEach(currentEntity =>
            {
                currentEntity.ApplyDamage(blastDamage, launcher);
            });

        graphics.SetActive(false);
        bombSFX.Play(graphics.transform.position);
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
    //private void OnDrawGizmos()
    //{
    //    if (isActive || isMoving)
    //    {
    //        Handles.color = new Color(1, 0, 0, 0.25f);
    //        Handles.DrawWireDisc(endPosition, Vector3.up, blastRadius);

    //        Handles.color = Color.white;
    //        Handles.Label(transform.position + Vector3.up,
    //            $"Bomb" +
    //            $"\nActivate : {isActive}" +
    //            $"\nBefore explode : {timerBeforeExplode - Time.time + elapsedExplosionTime}");
    //    }
    //    else
    //    {
    //        Handles.color = Color.white;
    //        Handles.Label(transform.position + Vector3.up,
    //            $"Bomb" +
    //            $"\nActivate : {isActive}");
    //    }
    //}
#endif
}
