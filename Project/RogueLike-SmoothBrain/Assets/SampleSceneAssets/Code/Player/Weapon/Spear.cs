using UnityEngine;

public class Spear : MonoBehaviour
{
    bool isThrew = false;
    bool isThrowing = false;
    Transform player;
    Transform parent = null;
    [SerializeField] GameObject trailPf;
    GameObject trail;

    Quaternion initLocalRotation;
    Vector3 initLocalPosition;
    Vector3 spearPosition;
    public bool IsThrew
    {
        get
        {
            return (isThrew);
        }
    }
    public bool IsThrowing
    {
        get
        {
            return (isThrowing);
        }
    }
    Vector3 posToReach = new();
    Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        initLocalRotation = transform.localRotation;
        initLocalPosition = transform.localPosition;
    }
    void Update()
    {
        if (trail == null)
        {
            return;
        }
        if (isThrew && (this.player.position - posToReach).magnitude < (this.player.position - trail.transform.position).magnitude)
        {
            Destroy(trail);
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            this.gameObject.transform.position = posToReach;
            this.gameObject.transform.rotation = Quaternion.identity * Quaternion.Euler(90,0,0);
            isThrowing = false;
        }
        else if(!isThrew && parent != null && (spearPosition - posToReach).magnitude < (spearPosition - trail.transform.position).magnitude)
        {
            rb.velocity = Vector3.zero;
            this.gameObject.transform.position = posToReach;
            this.transform.SetParent(parent, true);
            this.gameObject.transform.localPosition = initLocalPosition;
            this.gameObject.transform.localRotation = initLocalRotation;
            parent = null;
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            isThrowing = false;
            Destroy(trail);
        }
    }

    public void Throw(Vector3 _posToReach)
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, this.transform.position, Quaternion.identity);
        posToReach = _posToReach;
        isThrew = true;
        isThrowing = true;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - this.transform.position).normalized * 5000, ForceMode.Force);
        parent = this.transform.parent;
        this.transform.parent = null;
        this.transform.LookAt(posToReach);
    }

    public void Return()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail = Instantiate(trailPf, posToReach, Quaternion.identity);
        spearPosition = posToReach;
        isThrew = false;
        isThrowing = true;
        posToReach = parent.transform.position;
        trail.GetComponent<Rigidbody>().AddForce((posToReach - trail.transform.position).normalized * 5000, ForceMode.Force);
    }
}
