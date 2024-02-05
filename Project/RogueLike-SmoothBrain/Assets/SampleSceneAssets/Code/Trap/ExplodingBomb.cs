using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ExplodingBomb : MonoBehaviour
{
    public enum States
    {
        IN_MAP,
        DROPED_ON_ENTITY_DEATH,
        THROW_BY_ENTITY
    }

    [Header("Bomb Parameter")]
    public States state;
    public float timeBeforExplode;
    public float blastRadius;
    public bool active;
    public bool canDealDamageToThrower;
    public int blastDamage;
    public LayerMask damageLayer;

    private Vector3 startPos;
    private Vector3 endPos;
    private GameObject authorOfTheBombe;
    List<IDamageable> entitiesToDealDamage;

    private void Awake()
    {
        startPos = transform.position;
        entitiesToDealDamage = new List<IDamageable>();
    }

    private void Start()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        switch (state)
        {
            case States.IN_MAP:
                endPos = startPos;
                active = false;
                break;
            case States.DROPED_ON_ENTITY_DEATH:
                endPos = startPos;
                active = true;
                break;
            case States.THROW_BY_ENTITY:
                endPos = player.transform.position;
                active = true;
                break;
            default:
                break;
        }
    }

    public void SetBombDestination(Vector3 _pos)
    {
        endPos = _pos;
    }

    public void SetBombAuthor(GameObject _gameObject)
    {
        authorOfTheBombe = _gameObject;
    }

    void Update()
    {
        if (active)
        {
            UpdateTimerExplotion();
        }
    }

    void UpdateTimerExplotion()
    {
        if (timeBeforExplode > 0)
        {
            timeBeforExplode -= Time.deltaTime;
        }
        else
        {
            ApplyDamage();
            Destroy(gameObject);
        }
    }

    void FillEntitiesList(Collider[] _hitColliders)
    {
        foreach (var hitCollider in _hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (canDealDamageToThrower)
                {
                    entitiesToDealDamage.Add(hitCollider.gameObject.GetComponent<IDamageable>());
                }
                else
                {
                    if (hitCollider.gameObject != authorOfTheBombe)
                    {
                        entitiesToDealDamage.Add(hitCollider.gameObject.GetComponent<IDamageable>());
                    }
                }
            }
        }
    }

    void ApplyDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position,blastRadius, damageLayer);
        FillEntitiesList(hitColliders);
        entitiesToDealDamage.ForEach(actualEntity => { actualEntity.ApplyDamage(blastDamage); });
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (active)
        {
            Handles.color = new Color(1,0,0,0.5f);
            Handles.DrawSolidDisc(endPos, Vector3.up, blastRadius);
        }
    }
#endif

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
