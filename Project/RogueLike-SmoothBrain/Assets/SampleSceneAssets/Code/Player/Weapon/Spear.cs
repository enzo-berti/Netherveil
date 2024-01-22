using UnityEngine;

public class Spear : MonoBehaviour
{
    bool isTrew = false;
    Transform player;
    Transform parent = null;
    [SerializeField] GameObject trailPf;
    [SerializeField] GameObject trail;
    public bool IsThrew
    {
        get
        {
            return (isTrew);
        }
        set
        {
            isTrew = value;
        }
    }
    Vector3 posToReach = new();
    Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (trail == null)
        {
            return;
        }
        if (isTrew && (this.player.position - posToReach).magnitude < (this.player.position - trail.transform.position).magnitude)
        {
            Destroy(trail);
        }
        else if(!isTrew && parent != null && (posToReach - this.player.position).magnitude > (parent.position - trail.transform.position).magnitude)
        {
            rb.velocity = Vector3.zero;
            parent = null;
            Destroy(trail);
        }
    }

    public void Throw(Vector3 _posToReach)
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        posToReach = _posToReach;
        isTrew = true;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - this.transform.position).normalized * 1000, ForceMode.Force);
        this.transform.LookAt(posToReach);
    }

    public void Return()
    {
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        IsThrew = false;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - this.transform.position).normalized * 1000, ForceMode.Force);
    }
}
