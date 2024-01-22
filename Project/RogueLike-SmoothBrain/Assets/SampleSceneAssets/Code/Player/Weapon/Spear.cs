using UnityEngine;

public class Spear : MonoBehaviour
{
    bool isTrew = false;
    Transform player;
    Transform parent = null;
    [SerializeField] GameObject trailPf;
    GameObject trail;
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
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            this.gameObject.transform.position = posToReach;
            this.gameObject.transform.rotation = Quaternion.identity * Quaternion.Euler(90,0,0);
        }
        else if(!isTrew && parent != null && (posToReach - this.player.position).magnitude > (parent.position - trail.transform.position).magnitude)
        {
            rb.velocity = Vector3.zero;
            parent = null;
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
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
        parent = this.transform.parent;
        this.transform.parent = null;
        this.transform.LookAt(posToReach);
    }

    public void Return()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, posToReach, Quaternion.identity);
        IsThrew = false;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - trail.transform.position).normalized * 1000, ForceMode.Force);
    }
}
