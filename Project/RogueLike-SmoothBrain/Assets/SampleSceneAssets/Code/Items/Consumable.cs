using UnityEditor.Rendering;
using UnityEngine;

public abstract class Consumable : MonoBehaviour, IConsumable
{
    public int Price { get; protected set; } = 0;
    public bool CanBeRetrieved { get; protected set; } = true;
    protected Hero player;
    protected GameObject model;

    protected virtual void Start()
    {
         player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        model = GetComponentInChildren<MeshRenderer>().gameObject;
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
        model.transform.Rotate(new Vector3(0, 0.5f, 0));
    }

    private void RetrieveConsumable()
    {
        if (!CanBeRetrieved)
            return;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 tmp = (cameraForward * player.transform.position.z + cameraRight * player.transform.position.x);
        Vector2 playerPos = new Vector2(tmp.x, tmp.z);
        tmp = (cameraForward * transform.position.z + cameraRight * transform.position.x);
        Vector2 itemPos = new Vector2(tmp.x, tmp.z);

        bool isInRange = Vector2.Distance(playerPos, itemPos) <= player.Stats.GetValue(Stat.CATCH_RADIUS);
        if (isInRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 2);
            if (Vector2.Distance(playerPos, itemPos) <= 0.3f)
            {
                OnRetrieved();
            }
        }
    }

    public virtual void OnRetrieved()
    {
        throw new System.NotImplementedException();
    }
}
