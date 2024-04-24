using UnityEngine;

public abstract class Consumable : MonoBehaviour, IConsumable
{
    public int Price { get; protected set; } = 0;
    public bool CanBeRetrieved { get; protected set; } = true;
    protected Hero player;
    protected GameObject model;
    float lerpTimer = 0f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }

    protected virtual void Start()
    {
        model = GetComponentInChildren<MeshRenderer>().gameObject;
        Destroy(gameObject, 60);
    }

    protected virtual void Update()
    {
        RetrieveConsumable();
        FloatingAnimation();
    }

    private void FloatingAnimation()
    {
        Vector3 updatePos = model.transform.position;
        updatePos.y += Mathf.Sin(Time.time) * 0.0009f;
        model.transform.position = updatePos;
        model.transform.Rotate(new Vector3(0, 50f * Time.deltaTime, 0));
    }

    private void RetrieveConsumable()
    {
        if (!CanBeRetrieved)
            return;

        float distance = Vector2.Distance(player.transform.position.ToCameraOrientedVec2(), transform.position.ToCameraOrientedVec2());
        if (distance <= player.Stats.GetValue(Stat.CATCH_RADIUS))
        {
            lerpTimer += Time.deltaTime / 10f;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, lerpTimer);
            if (distance <= 1f)
            {
                OnRetrieved();
            }
        }
        else
        {
            lerpTimer = 0f;
        }
    }

    public abstract void OnRetrieved();
}
