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
    public float timeToExplode;
    public float blastRadius;
    public bool active;
    public bool canDealDamageToThrower;
    public int blastDamage;

    private Vector3 startPos;
    private Vector3 endPos;

    private void Awake()
    {
        startPos = transform.position;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
